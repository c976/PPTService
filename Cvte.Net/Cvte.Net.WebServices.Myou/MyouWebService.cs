using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Cvte.Net.WebServices.EventReport.Utils;
using Newtonsoft.Json;

namespace Cvte.Net.WebServices.Myou
{
	
	
	public class MyouWebService : WebService, IErrorReporter
	{
		private readonly IList<string> _distinctStackTrace = new List<string>();

		private static readonly int MaxStackTraceLength = 8192;

		public Context Context { get; private set; }

		public MyouWebService()
		{
			Context = new Context();
		}

		[Obsolete("上报Sunday异常，请使用 SundayReporter；初始化 MyouWebService 请使用其它的构造函数。", true)]
		public MyouWebService(string sundayAppId, string sundayAppSecret)
		{
			Context = new Context();
		}

		public MyouWebService(Context context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			Context = context;
		}

		[Obsolete("上报Sunday异常，请使用 SundayReporter；初始化 MyouWebService 请使用其它的构造函数。", true)]
		public MyouWebService(string sundayAppId, string sundayAppSecret, Context context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			Context = context;
		}

		public void ConfigContext(Context context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			Context = context;
		}

		[Obsolete("请使用Context")]
		public void ConfigError(string appKey, string appVersion)
		{
			Context.AppKey = appKey;
			Context.AppVersion = appVersion;
		}

		public async Task ReportErrorAsync(Exception exception)
		{
			await ReportErrorAsync(exception, string.Empty, ErrorPriority.Normal, string.Empty);
		}

		public async Task ReportErrorAsync(Exception exception, string type)
		{
			await ReportErrorAsync(exception, type, ErrorPriority.Normal, string.Empty);
		}

		public async Task ReportErrorAsync(Exception exception, string type, ErrorPriority priority)
		{
			await ReportErrorAsync(exception, type, priority, string.Empty);
		}

		public async Task ReportErrorAsync(Exception exception, string type, ErrorPriority priority, string context)
		{
			string text = exception.StackTrace;
			if (!_distinctStackTrace.Contains(text))
			{
				_distinctStackTrace.Add(text);
				if (text?.Length > MaxStackTraceLength)
				{
					text = text.Substring(0, MaxStackTraceLength);
				}
				await ReportErrorAsync(new ExceptionEntry
				{
					SessionId = PostEventHelper.SessionId,
					Time = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds,
					ExceptionProperties = new Dictionary<string, string>(StringComparer.Ordinal)
					{
						{
							"name",
							exception.GetType().Name
						},
						{ "msg", exception.Message },
						{ "st", text },
						{ "ct", context }
					},
					CommonProperties = new Dictionary<string, string>(StringComparer.Ordinal)
					{
						{
							"tid",
							SessionIdProvider.SessionId
						},
						{ "tips", type },
						{
							"pri",
							priority.ToString()
						},
						{
							"os",
							Environment.OSVersion.VersionString
						},
						{
							"ps",
							$"{SystemParameters.PrimaryScreenWidth}*{SystemParameters.PrimaryScreenHeight}"
						}
					}
				}).ConfigureAwait(false);
			}
		}

		private async Task ReportErrorAsync(ExceptionEntry exception)
		{
			string value = JsonConvert.SerializeObject(exception);
			PostExceptionReqeust postExceptionReqeust = new PostExceptionReqeust
			{
				Headers = new Dictionary<string, string>(StringComparer.Ordinal)
				{
					{ "appkey", Context.AppKey },
					{ "app_version", Context.AppVersion },
					{
						"os",
						Environment.OSVersion.VersionString
					},
					{
						"cpu",
						EnvironmentUtility.CPU
					},
					{
						"ram",
						EnvironmentUtility.RAM
					},
					{
						"zone",
						EnvironmentUtility.TimeZone
					},
					{
						"deviceid_md5",
						SecurityUtility.Encrypt32(EnvironmentUtility.MacAddressId)
					}
				},
				Exceptions = new List<IDictionary<string, string>>
				{
					new Dictionary<string, string>(StringComparer.Ordinal)
					{
						{ "context", value },
						{
							"datetime",
							DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
						}
					}
				}
			};
			postExceptionReqeust.HeaderCollection.Add("X-MengYou-API", "windows");
			await PostAsync<PostExceptionResponse>(postExceptionReqeust);
		}

		[Obsolete("此方法使用可能造成误导，请使用 SundayReporter。", true)]
		public async Task ReportErrorWithAttachmentAsync(Exception exception, string type, ErrorPriority priority, string context, string attachmentPath)
		{
			await Task.FromResult(0);
		}

		[Obsolete("此方法使用可能造成误导，请使用 SundayReporter。", true)]
		public async Task ReportErrorWithAttachmentAsync(Exception exception, string type, ErrorPriority priority, IDictionary<string, string> properties, string attachmentPath)
		{
			await Task.FromResult(0);
		}
	}
}
