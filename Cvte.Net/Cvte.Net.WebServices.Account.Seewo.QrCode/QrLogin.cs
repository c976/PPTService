using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Cvte.Net.Http;
using Cvte.Net.Http.HostBackup;
using Cvte.Net.WebServices.Account.Seewo.QrCode.Models;
using Cvte.Net.WebServices.Account.Seewo.UserCenter.Models;

namespace Cvte.Net.WebServices.Account.Seewo.QrCode
{
	
	
	public class QrLogin : WebService, IQRLogin
	{
		private const string AppCookieName = "x-auth-app";

		private const string AppBrandCookieName = "x-auth-brand";

		private const string QrCookieKey = "qrkey";

		private readonly string _getQrLoginImageApi;

		public int Interval { get; set; } = 1000;

		public string CachePath { get; set; }

		
		
		public IHttpRequestHostBackupFilter RequestHostBackupFilter
		{
			
			get;
			
			set;
		}

		public QrLogin(string appName, WebServiceSettings settings)
			: this(appName, "", settings)
		{
		}

		public QrLogin(string appName, string appBrand, WebServiceSettings settings)
			: base(settings)
		{
			Host = "http://id.seewo.com/";
			CachePath = Path.GetTempPath();
			if (string.IsNullOrWhiteSpace(appName))
			{
				throw new ArgumentNullException("appName");
			}
			if (string.IsNullOrWhiteSpace(appBrand))
			{
				_getQrLoginImageApi = "scan/qrcode?oriSys=" + appName + "&t=1484722930223.3638";
			}
			else
			{
				_getQrLoginImageApi = "scan/qrcode?oriSys=" + appName + "&brand=" + appBrand + "&t=1484722930223.3638";
			}
			AddRequestFilter(new BasicHttpRequestFilter((HttpRequest request) =>
			{
				CookieCollection cookieCollection = new CookieCollection
				{
					new Cookie
					{
						Name = "x-auth-app",
						Value = appName
					},
					new Cookie
					{
						Name = "x-auth-brand",
						Value = appBrand
					}
				};
				CheckQrCodeRequest checkQrCodeRequest = request as CheckQrCodeRequest;
				if (checkQrCodeRequest != null)
				{
					cookieCollection.Add(new Cookie
					{
						Name = "qrkey",
						Value = checkQrCodeRequest.BarCode
					});
				}
				request.CookieCollection = cookieCollection;
			}));
		}

		
		public async Task<QrCodeImageObject> GetLoginQrAsync()
		{
			try
			{
				if (!Directory.Exists(CachePath))
				{
					Directory.CreateDirectory(CachePath);
				}
			}
			catch (IOException)
			{
				CachePath = Path.GetTempPath();
			}
			string filePath = Path.Combine(CachePath, Path.GetRandomFileName() + ".png");
			HttpResponse httpResponse = await DownloadFileAsync<HttpResponse>(new DownloadFileRequest
			{
				Url = Host + _getQrLoginImageApi,
				FilePath = filePath
			}, new Progress<DownloadProgress>((DownloadProgress d) =>
			{
			}), default(CancellationToken)).ConfigureAwait(false);
			if (!httpResponse.Success)
			{
				return null;
			}
			string text = "";
			foreach (Cookie cookie in httpResponse.Cookies)
			{
				if (string.Equals(cookie.Name, "qrkey", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(cookie.Value))
				{
					text = cookie.Value;
					break;
				}
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				return null;
			}
			return new QrCodeImageObject
			{
				BarCode = text,
				FilePath = filePath
			};
		}

		public void WaitingLoginAsync(Action<QrLoginResult> callback, string barCode, CancellationTokenSource cancellationTokenSource, ManualResetEvent manualResetEvent)
		{
			if (string.IsNullOrWhiteSpace(barCode))
			{
				throw new ArgumentException("Please call TryGetLoginQrImage method first.", "barCode");
			}
			Task.Run( async () =>
			{
				while (true)
				{
					manualResetEvent.WaitOne();
					if (cancellationTokenSource.IsCancellationRequested)
					{
						break;
					}
					QrLoginResult qrLoginResult = await CheckAsync(barCode).ConfigureAwait(false);
					callback(qrLoginResult);
					if (qrLoginResult.State != QrState.Ok && qrLoginResult.State != QrState.Scaned)
					{
						break;
					}
					await Task.Delay(Interval, cancellationTokenSource.Token).ConfigureAwait(false);
				}
			}, cancellationTokenSource.Token);
		}

		private async Task<QrLoginResult> CheckAsync(string barCode)
		{
			CheckUserCenterResponse checkUserCenterResponse = await GetAsync<CheckUserCenterResponse>(new CheckQrCodeRequest(barCode)).ConfigureAwait(false);
			if (checkUserCenterResponse.IsSuccessStatusCode)
			{
				QrAccountResponseCodes qrAccountResponseCode = checkUserCenterResponse.QrAccountResponseCode;
				switch (qrAccountResponseCode)
				{
				case QrAccountResponseCodes.Ok:
				case QrAccountResponseCodes.QrCodeScaned:
				case QrAccountResponseCodes.QrCodeDisabled:
				case QrAccountResponseCodes.QrCodeServiceError:
					return new QrLoginResult((QrState)qrAccountResponseCode);
				case QrAccountResponseCodes.QrLogined:
				{
					CheckTokenResponse checkTokenResponse = await PostAsync<CheckTokenResponse>(new CheckTokenRequest(checkUserCenterResponse.Token)).ConfigureAwait(false);
					return checkTokenResponse.Success ? new QrLoginResult(QrState.Logined, checkTokenResponse.UserInfo) : new QrLoginResult(QrState.Unknow);
				}
				default:
					return new QrLoginResult(QrState.Unknow);
				}
			}
			return new QrLoginResult(QrState.NetworkError);
		}
	}
}
