using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using Cvte.Pipe.Http.Request;
using Cvte.Pipe.Http.Response;
using Cvte.Pipe.Http.Utils;

namespace Cvte.Pipe.Http
{
	public abstract class HttpPipe : IHttpPipe
	{
		// SA_COMPAT: unused compiler-generated display classes preserved below.
#if SA_ORIGINAL_DISPLAY_CLASSES
		[CompilerGenerated]
		private sealed class SA_C1<_0001, _0002> where _0001 : RequestBase where _0002 : ResponseBase, new()
		{
			public dynamic _0001;

			internal string _0001_M2()
			{
				return HttpUtility._0001(this._0001.GetRequestUrl(), this._0001.GetRequestMethod(), this._0001.ContentType, this._0001.UserAgent, this._0001.HeaderCollection, this._0001.CookieCollection, this._0001.TimeOut);
			}
		}

		[CompilerGenerated]
		private sealed class _0002<_0001, _0002> where _0001 : RequestBase where _0002 : ResponseBase, new()
		{
			public dynamic _0001;

			internal string _0001_M2()
			{
				return HttpUtility._0001(this._0001.GetRequestUrl(), this._0001.GetRequestMethod(), this._0001.ContentType, this._0001.UserAgent, this._0001.GetRequestData(), this._0001.HeaderCollection, this._0001.CookieCollection, this._0001.TimeOut);
			}
		}

		[CompilerGenerated]
		private sealed class _0003<_0001, _0002> where _0001 : UploadFileRequest where _0002 : ResponseBase, new()
		{
			public dynamic _0001;

			internal string _0001_M2()
			{
				return HttpUtility._0001(this._0001.GetRequestUrl(), this._0001.FileKey, this._0001.FilePath, this._0001.GetParams(), this._0001.HeaderCollection, this._0001.UpdateProgress);
			}
		}

		[CompilerGenerated]
		private sealed class _0004<_0001, _0002> where _0001 : UploadFilesRequest where _0002 : ResponseBase, new()
		{
			public dynamic _0001;

			internal string _0001_M2()
			{
				return HttpUtility._0001(this._0001.GetRequestUrl(), this._0001.KeyFilePathMapping, this._0001.GetParams(), this._0001.HeaderCollection, this._0001.UpdateProgress);
			}
		}

		[CompilerGenerated]
		private sealed class _0005<_0001, _0002> where _0001 : DownloadFileRequest where _0002 : ResponseBase, new()
		{
			public dynamic _0001;

			internal string _0001_M2()
			{
				HttpUtility._0001(this._0001.GetRequestUrl(), this._0001.SavePath, this._0001.GetRequestMethod(), this._0001.ContentType, this._0001.GetRequestData(), this._0001.UpdateProgress, this._0001.HandleError, this._0001.TimeOut);
				return null;
			}
		}
#endif
		[CompilerGenerated]
		private bool m__0001;

		public bool DebugMode
		{
			[CompilerGenerated]
			get
			{
				return this.m__0001;
			}
			[CompilerGenerated]
			set
			{
				this.m__0001 = value;
			}
		}

		public TResponse Get<TRequest, TResponse>(TRequest request) where TRequest : RequestBase where TResponse : ResponseBase, new()
		{
			PreRequest(request);
			return SA_Execute<TRequest, TResponse>(request, () => HttpUtility._0001(request.GetRequestUrl(), request.GetRequestMethod(), request.ContentType, request.UserAgent, request.HeaderCollection, request.CookieCollection, request.TimeOut));
		}

		public TResponse Post<TRequest, TResponse>(TRequest request) where TRequest : RequestBase where TResponse : ResponseBase, new()
		{
			PreRequest(request);
			return SA_Execute<TRequest, TResponse>(request, () => HttpUtility._0001(request.GetRequestUrl(), request.GetRequestMethod(), request.ContentType, request.UserAgent, request.GetRequestData(), request.HeaderCollection, request.CookieCollection, request.TimeOut));
		}

		public TResponse UploadFile<TRequest, TResponse>(TRequest request) where TRequest : UploadFileRequest where TResponse : ResponseBase, new()
		{
			PreRequest(request);
			return SA_Execute<TRequest, TResponse>(request, () => HttpUtility._0001(request.GetRequestUrl(), request.FileKey, request.FilePath, request.GetParams(), request.HeaderCollection, request.UpdateProgress));
		}

		public TResponse UploadFiles<TRequest, TResponse>(TRequest request) where TRequest : UploadFilesRequest where TResponse : ResponseBase, new()
		{
			PreRequest(request);
			return SA_Execute<TRequest, TResponse>(request, () => HttpUtility._0001(request.GetRequestUrl(), request.KeyFilePathMapping, request.GetParams(), request.HeaderCollection, request.UpdateProgress));
		}

		public TResponse DownloadFile<TRequest, TResponse>(TRequest request) where TRequest : DownloadFileRequest where TResponse : ResponseBase, new()
		{
			PreRequest(request);
			return SA_Execute<TRequest, TResponse>(request, () =>
			{
				HttpUtility._0001(request.GetRequestUrl(), request.SavePath, request.GetRequestMethod(), request.ContentType, request.GetRequestData(), request.UpdateProgress, request.HandleError, request.TimeOut);
				return (string)null;
			});
		}

		private SA_TRes SA_Execute<SA_TReq, SA_TRes>(SA_TReq SA_request, Func<string> SA_getHtml) where SA_TReq : RequestBase where SA_TRes : ResponseBase, new()
		{
			SA_TRes val;
			try
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string html = SA_getHtml();
				stopwatch.Stop();
				val = new SA_TRes
				{
					ErrorCode = ErrorCode.Ok
				};
				val.ParseHtml(html);
				if (!(SA_request is UploadFileRequest) && !(SA_request is DownloadFileRequest) && val.MonitorCode.HasValue)
				{
					string text = SA_request.GetRequestUrl();
					int num = text.IndexOf('?');
					if (num > 0)
					{
						text = text.Substring(0, num);
					}
					HandleMonitor(text, SA_request.GetRequestMethod(), val.MonitorCode.Value, stopwatch.ElapsedMilliseconds);
				}
			}
			catch (TimeoutException ex)
			{
				HandleException(ex);
				val = new SA_TRes
				{
					ErrorCode = ErrorCode.TimeOut
				};
			}
			catch (WebException ex2)
			{
				HandleException(ex2);
				val = new SA_TRes
				{
					ErrorCode = ErrorCode.WebError
				};
			}
			catch (UriFormatException ex3)
			{
				HandleException(ex3);
				val = new SA_TRes
				{
					ErrorCode = ErrorCode.InvalidUri
				};
			}
			catch (Exception ex4)
			{
				HandleException(ex4);
				val = new SA_TRes
				{
					ErrorCode = ErrorCode.Unknow
				};
			}
			return val;
		}

		protected virtual void PreRequest(RequestBase request)
		{
		}

		protected virtual void HandleException(Exception ex)
		{
			if (DebugMode)
			{
				Console.WriteLine(ex);
			}
		}

		protected virtual void HandleMonitor(string url, string method, int responseCode, long elapsedTime)
		{
		}
	}
}
