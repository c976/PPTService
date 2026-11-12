using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using PPTService.Core.Pipe.Http.Request;
using PPTService.Core.Pipe.Http.Response;
using PPTService.Core.Pipe.Http.Utils;

namespace PPTService.Core.Pipe.Http
{
	public abstract class HttpPipe : IHttpPipe
	{
		private sealed class Class0<T, U> where T : RequestBase where U : ResponseBase, new()
		{
			public T gparam_0;

			internal string method_0()
			{
				return HttpUtility.smethod_0(gparam_0.GetRequestUrl(), gparam_0.GetRequestMethod(), gparam_0.ContentType, gparam_0.UserAgent, gparam_0.HeaderCollection, gparam_0.CookieCollection, gparam_0.TimeOut);
			}
		}

		private sealed class Class1<T, U> where T : RequestBase where U : ResponseBase, new()
		{
			public T gparam_0;

			internal string method_0()
			{
				return HttpUtility.smethod_3(gparam_0.GetRequestUrl(), gparam_0.GetRequestMethod(), gparam_0.ContentType, gparam_0.UserAgent, gparam_0.GetRequestData(), gparam_0.HeaderCollection, gparam_0.CookieCollection, gparam_0.TimeOut);
			}
		}

		private sealed class Class2<T, U> where T : UploadFileRequest where U : ResponseBase, new()
		{
			public T gparam_0;

			internal string method_0()
			{
				return HttpUtility.smethod_5(gparam_0.GetRequestUrl(), gparam_0.FileKey, gparam_0.FilePath, gparam_0.GetParams(), gparam_0.HeaderCollection, gparam_0.UpdateProgress);
			}
		}

		private sealed class Class3<T, U> where T : UploadFilesRequest where U : ResponseBase, new()
		{
			public T gparam_0;

			internal string method_0()
			{
				return HttpUtility.smethod_6(gparam_0.GetRequestUrl(), gparam_0.KeyFilePathMapping, gparam_0.GetParams(), gparam_0.HeaderCollection, gparam_0.UpdateProgress);
			}
		}

		private sealed class Class4<T, U> where T : DownloadFileRequest where U : ResponseBase, new()
		{
			public T gparam_0;

			internal string method_0()
			{
				HttpUtility.smethod_7(gparam_0.GetRequestUrl(), gparam_0.SavePath, gparam_0.GetRequestMethod(), gparam_0.ContentType, gparam_0.GetRequestData(), gparam_0.UpdateProgress, gparam_0.HandleError, gparam_0.TimeOut);
				return null;
			}
		}

		private bool bool_0;

		public bool DebugMode
		{
			get
			{
				return bool_0;
			}
			set
			{
				bool_0 = value;
			}
		}

		public TResponse Get<TRequest, TResponse>(TRequest request) where TRequest : RequestBase where TResponse : ResponseBase, new()
		{
			PreRequest(request);
			return method_0<TRequest, TResponse>(request, () => HttpUtility.smethod_0(request.GetRequestUrl(), request.GetRequestMethod(), request.ContentType, request.UserAgent, request.HeaderCollection, request.CookieCollection, request.TimeOut));
		}

		public TResponse Post<TRequest, TResponse>(TRequest request) where TRequest : RequestBase where TResponse : ResponseBase, new()
		{
			PreRequest(request);
			return method_0<TRequest, TResponse>(request, () => HttpUtility.smethod_3(request.GetRequestUrl(), request.GetRequestMethod(), request.ContentType, request.UserAgent, request.GetRequestData(), request.HeaderCollection, request.CookieCollection, request.TimeOut));
		}

		public TResponse UploadFile<TRequest, TResponse>(TRequest request) where TRequest : UploadFileRequest where TResponse : ResponseBase, new()
		{
			PreRequest(request);
			return method_0<TRequest, TResponse>(request, () => HttpUtility.smethod_5(request.GetRequestUrl(), request.FileKey, request.FilePath, request.GetParams(), request.HeaderCollection, request.UpdateProgress));
		}

		public TResponse UploadFiles<TRequest, TResponse>(TRequest request) where TRequest : UploadFilesRequest where TResponse : ResponseBase, new()
		{
			PreRequest(request);
			return method_0<TRequest, TResponse>(request, () => HttpUtility.smethod_6(request.GetRequestUrl(), request.KeyFilePathMapping, request.GetParams(), request.HeaderCollection, request.UpdateProgress));
		}

		public TResponse DownloadFile<TRequest, TResponse>(TRequest request) where TRequest : DownloadFileRequest where TResponse : ResponseBase, new()
		{
			PreRequest(request);
			return method_0<TRequest, TResponse>(request, () =>
			{
				HttpUtility.smethod_7(request.GetRequestUrl(), request.SavePath, request.GetRequestMethod(), request.ContentType, request.GetRequestData(), request.UpdateProgress, request.HandleError, request.TimeOut);
				return (string)null;
			});
		}

		private U method_0<T, U>(T gparam_0, Func<string> func_0) where T : RequestBase where U : ResponseBase, new()
		{
			U val;
			try
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				string html = func_0();
				stopwatch.Stop();
				val = new U
				{
					ErrorCode = ErrorCode.Ok
				};
				val.ParseHtml(html);
				if (!(gparam_0 is UploadFileRequest) && !(gparam_0 is DownloadFileRequest) && val.MonitorCode.HasValue)
				{
					string text = gparam_0.GetRequestUrl();
					int num = text.IndexOf('?');
					if (num > 0)
					{
						text = text.Substring(0, num);
					}
					HandleMonitor(text, gparam_0.GetRequestMethod(), val.MonitorCode.Value, stopwatch.ElapsedMilliseconds);
				}
			}
			catch (TimeoutException ex)
			{
				HandleException(ex);
				val = new U
				{
					ErrorCode = ErrorCode.TimeOut
				};
			}
			catch (WebException ex2)
			{
				HandleException(ex2);
				val = new U
				{
					ErrorCode = ErrorCode.WebError
				};
			}
			catch (UriFormatException ex3)
			{
				HandleException(ex3);
				val = new U
				{
					ErrorCode = ErrorCode.InvalidUri
				};
			}
			catch (Exception ex4)
			{
				HandleException(ex4);
				val = new U
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
