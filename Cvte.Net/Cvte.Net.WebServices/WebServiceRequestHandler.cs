using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Cvte.Net.Http;
using Cvte.Net.WebServices.Exceptions;
using Cvte.Net.WebServices.Monitors;

namespace Cvte.Net.WebServices
{
	
	
	internal class WebServiceRequestHandler
	{
		private readonly IList<IHttpRequestFilter> _requestFilters;

		private readonly IList<IHttpRequestAsyncFilter> _requestAsyncFilters;

		private readonly IList<IHttpResponseFilter> _responseFilters;

		private string Host { get; }

		private WebServiceSettings Settings { get; }

		public WebServiceRequestHandler(string host)
		{
			Host = host;
			Settings = new WebServiceSettings();
			_requestFilters = new List<IHttpRequestFilter>(0);
			_requestAsyncFilters = new List<IHttpRequestAsyncFilter>(0);
			_responseFilters = new List<IHttpResponseFilter>(0);
		}

		public WebServiceRequestHandler(string host, WebServiceSettings settings, IList<IHttpRequestFilter> requestFilters, IList<IHttpRequestAsyncFilter> requestAsyncFilters, IList<IHttpResponseFilter> responseFilters)
		{
			Host = host;
			Settings = settings;
			_requestFilters = requestFilters;
			_requestAsyncFilters = requestAsyncFilters;
			_responseFilters = responseFilters;
		}

		public async Task<TResponse> HandleRequestAsync< TResponse>(HttpRequest request, Func<HttpRequest, Task<HttpWebResponseWrapper>> requestFunc) where TResponse : HttpResponse, new()
		{
			request.Host = Host;
			foreach (IHttpRequestFilter requestFilter in _requestFilters)
			{
				if (requestFilter.Match(request))
				{
					requestFilter.Handle(request);
				}
			}
			foreach (IHttpRequestAsyncFilter requestAsyncFilter in _requestAsyncFilters)
			{
				if (requestAsyncFilter.Match(request))
				{
					await requestAsyncFilter.HandleAsync(request);
				}
			}
			Stopwatch stopwatch = new Stopwatch();
			HttpWebResponseWrapper httpWebResponseWrapper;
			try
			{
				stopwatch.Start();
				httpWebResponseWrapper = await requestFunc(request).ConfigureAwait(false);
				if (httpWebResponseWrapper == null)
				{
					throw new InvalidOperationException("Invalid requestFunc ;return null responseWrapper");
				}
				stopwatch.Stop();
			}
			catch (AggregateException ex)
			{
				httpWebResponseWrapper = new HttpWebResponseWrapper(null, ex.InnerException);
			}
			catch (Exception exception)
			{
				httpWebResponseWrapper = new HttpWebResponseWrapper(null, exception);
			}
			TResponse response;
			if (httpWebResponseWrapper.Success)
			{
				response = new TResponse
				{
					StatusCode = (httpWebResponseWrapper.Response?.StatusCode ?? ((HttpStatusCode)0)),
					Error = httpWebResponseWrapper.Exception
				};
				try
				{
					response.Cookies = httpWebResponseWrapper.Response?.Cookies ?? new CookieCollection();
					response.Html = httpWebResponseWrapper.Html ?? string.Empty;
				}
				catch (Exception innerException)
				{
					response.Error = new ParseException("Parsing html error", innerException);
				}
			}
			else
			{
				response = HandleWebException<TResponse>(httpWebResponseWrapper);
			}
			if (response is IApiMonitorResponse)
			{
				Task.Run( async () =>
				{
					await ApiMonitorAsync(request, response, stopwatch.ElapsedMilliseconds).ConfigureAwait(false);
				}).ConfigureAwait(false);
			}
			response.Url = (string.IsNullOrWhiteSpace(request.RuntimeUrl) ? request.GetRequestUrl() : request.RuntimeUrl);
			response.Method = request.Method;
			response.Host = request.Host;
			response.Headers = httpWebResponseWrapper.Response?.Headers ?? new WebHeaderCollection();
			foreach (IHttpResponseFilter responseFilter in _responseFilters)
			{
				if (responseFilter.Match(response))
				{
					responseFilter.Handle(request, response);
				}
			}
			return response;
		}

		private static TResponse HandleWebException< TResponse>(HttpWebResponseWrapper responseWrapper) where TResponse : HttpResponse, new()
		{
			TResponse val = new TResponse
			{
				Error = responseWrapper.Exception
			};
			if (responseWrapper.Response != null)
			{
				val.StatusCode = responseWrapper.Response.StatusCode;
			}
			try
			{
				WebException ex = responseWrapper.Exception as WebException;
				if (ex != null)
				{
					if (ex.Status == WebExceptionStatus.ProtocolError)
					{
						HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
						if (httpWebResponse != null)
						{
							val.StatusCode = httpWebResponse.StatusCode;
						}
					}
					if (ex.Response != null)
					{
						using (Stream stream = ex.Response.GetResponseStream())
						{
							if (stream != null)
							{
								using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
								{
									val.Html = streamReader.ReadToEnd();
								}
							}
						}
					}
				}
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
			return val;
		}

		private async Task ApiMonitorAsync(HttpRequest request, HttpResponse response, long elapsedTime)
		{
			if (Settings.ApiMonitorSetting.Enable)
			{
				IApiMonitor apiMonitor = Settings.ApiMonitorSetting.ApiMonitor;
				if (apiMonitor != null && response is IApiMonitorResponse)
				{
					await apiMonitor.Report(request, response, elapsedTime).ConfigureAwait(false);
				}
			}
		}
	}
}
