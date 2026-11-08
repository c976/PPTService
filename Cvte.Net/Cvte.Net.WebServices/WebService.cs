using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Cvte.Net.Http;
using Cvte.Net.Http.HostBackup;
using Cvte.Net.WebServices.Files.Download;
using Cvte.Net.WebServices.Files.Upload;
using Cvte.Net.WebServices.Files.Utils;
using Cvte.Net.WebServices.Filters;

namespace Cvte.Net.WebServices
{
	
	
	public class WebService : IWebService
	{
		private readonly IList<IHttpRequestFilter> _requestFilters = new List<IHttpRequestFilter>();

		private readonly IList<IHttpRequestAsyncFilter> _requestAsyncFilters = new List<IHttpRequestAsyncFilter>();

		private readonly IList<IHttpResponseFilter> _responseFilters = new List<IHttpResponseFilter>();

		private readonly HttpUtilityProxy _httpUtility;

		public string Host { get; set; } = "";

		public WebServiceSettings Settings { get; }

		public async Task<TResponse> UploadFileAsync< TResponse>(string filePath, HttpRequest request, IProgress<UploadProgress> uploadProgress, CancellationToken cancellationToken) where TResponse : HttpResponse, new()
		{
			request.Method = GetMethod(request, "POST");
			return await HandleRequestAsync<TResponse>(request, (HttpRequest req) => HttpUtility.UploadFile(req.GetRequestUrl(), filePath, request.Method, req.GetParams(), req.HeaderCollection, req.CookieCollection, uploadProgress, cancellationToken));
		}

		public async Task<TResponse> UploadFileAsync< TResponse>(IDictionary<string, string> dicFile, HttpRequest request, IProgress<UploadProgress> uploadProgress, CancellationToken cancellationToken) where TResponse : HttpResponse, new()
		{
			request.Method = GetMethod(request, "POST");
			return await HandleRequestAsync<TResponse>(request, (HttpRequest req) => HttpUtility.UploadFile(req.GetRequestUrl(), dicFile, request.Method, req.GetParams(), req.HeaderCollection, req.CookieCollection, uploadProgress, cancellationToken));
		}

		public async Task<TResponse> RequestAsync< TResponse>(HttpRequest request, RequestMethods method) where TResponse : HttpResponse, new()
		{
			request.Method = method.ToString().ToUpper();
			Tuple<bool, TResponse> tuple = await PreProccess<TResponse>(request);
			if (!tuple.Item1)
			{
				return tuple.Item2;
			}
			return await HandleRequestAsync<TResponse>(request, (HttpRequest req) => _httpUtility.PostAsync(req.GetRequestUrl(), req.Method, req.ContentType, req.UserAgent, req.GetRequestData, req.HeaderCollection, req.CookieCollection, req.TimeOut)).ConfigureAwait(false);
		}

		public WebService()
			: this(new WebServiceSettings())
		{
		}

		public WebService(WebServiceSettings settings)
		{
			Settings = settings;
			_httpUtility = new HttpUtilityProxy( () => Settings.HostBackupFilter);
			AddRequestFilter(new RequestTraceFilter());
		}

		public void AddRequestFilter(IHttpRequestFilter httpRequestFilter)
		{
			_requestFilters.Add(httpRequestFilter);
		}

		public void AddRequestAsyncFilter(IHttpRequestAsyncFilter httpRequestFilter)
		{
			_requestAsyncFilters.Add(httpRequestFilter);
		}

		public void AddResponseFilter(IHttpResponseFilter httpResponseFilter)
		{
			_responseFilters.Add(httpResponseFilter);
		}

		public async Task<TResponse> GetAsync< TResponse>(HttpRequest request) where TResponse : HttpResponse, new()
		{
			request.Method = GetMethod(request, "GET");
			Tuple<bool, TResponse> tuple = await PreProccess<TResponse>(request).ConfigureAwait(false);
			if (!tuple.Item1)
			{
				return tuple.Item2;
			}
			return await HandleRequestAsync<TResponse>(request, (HttpRequest req) => _httpUtility.GetAsync(req.GetRequestUrl(), req.Method, req.UserAgent, req.HeaderCollection, req.CookieCollection, req.TimeOut, req.Referer)).ConfigureAwait(false);
		}

		public async Task<TResponse> PostAsync< TResponse>(HttpRequest request) where TResponse : HttpResponse, new()
		{
			request.Method = GetMethod(request, "POST");
			Tuple<bool, TResponse> tuple = await PreProccess<TResponse>(request).ConfigureAwait(false);
			if (!tuple.Item1)
			{
				return tuple.Item2;
			}
			return await HandleRequestAsync<TResponse>(request, (HttpRequest req) => _httpUtility.PostAsync(req.GetRequestUrl(), req.Method, req.ContentType, req.UserAgent, req.GetRequestData, req.HeaderCollection, req.CookieCollection, req.TimeOut, req.Referer)).ConfigureAwait(false);
		}

		public async Task<TResponse> DownloadFileWithResumeAsync< TResponse>(DownloadFileRequest request, IProgress<DownloadProgress> progress, CancellationToken cancellationToken) where TResponse : HttpResponse, new()
		{
			request.Method = HttpMethod.Get.Method;
			string filePath = request.FilePath;
			DirectoryInfo directoryInfo = CommonFileHelper.GetTempPathDirectory(filePath);
			long finishedSize = await SpliceFileHelper.GetDownloadedSize(directoryInfo, filePath);
			string expectedFilePath = filePath + Path.GetRandomFileName() + ".tmp";
			UnitFileDownloadUtilityProxy downloadUtilityProxy = new UnitFileDownloadUtilityProxy(Settings.HostBackupFilter);
			TResponse response = await HandleRequestAsync<TResponse>(request, (HttpRequest req) => downloadUtilityProxy.DownloadFile(request.Url, expectedFilePath, finishedSize, progress, cancellationToken, request.TimeOut)).ConfigureAwait(false);
			if (response.Success)
			{
				CommonFileHelper.FileCheck(await SpliceFileHelper.SpliceFiles(directoryInfo, filePath).ConfigureAwait(false), filePath, request.Hash);
			}
			else if (response.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
			{
				File.Delete(await SpliceFileHelper.SpliceFiles(directoryInfo, filePath).ConfigureAwait(false));
				response = await DownloadFileWithResumeAsync<TResponse>(request, progress, cancellationToken).ConfigureAwait(false);
			}
			return response;
		}

		public async Task<TResponse> DownloadFileAsync< TResponse>(DownloadFileRequest request, IProgress<DownloadProgress> progress, CancellationToken cancellationToken) where TResponse : HttpResponse, new()
		{
			UnitFileDownloadUtilityProxy downloadUtilityProxy = new UnitFileDownloadUtilityProxy(Settings.HostBackupFilter);
			return await HandleRequestAsync<TResponse>(request, (HttpRequest req) => downloadUtilityProxy.DownloadFile(request.Url, request.FilePath, 0L, progress, cancellationToken, request.TimeOut)).ConfigureAwait(false);
		}

		public async Task<TResponse> UploadFileAsync< TResponse>(string filePath, HttpRequest request, string fileKey = "file") where TResponse : HttpResponse, new()
		{
			return await UploadFileAsync<TResponse>(filePath, request, null, fileKey).ConfigureAwait(false);
		}

		public async Task<TResponse> UploadFileAsync< TResponse>(string filePath, HttpRequest request, IProgress<UploadProgress> uploadProgress, string fileKey = "file") where TResponse : HttpResponse, new()
		{
			UnitFileUploadUtilityProxy uploadUtilityProxy = new UnitFileUploadUtilityProxy(Settings.HostBackupFilter);
			request.Method = "POST";
			return await HandleRequestAsync<TResponse>(request, (HttpRequest req) => uploadUtilityProxy.UploadFile(req.GetRequestUrl(), filePath, req.GetParams(), req.HeaderCollection, uploadProgress, fileKey)).ConfigureAwait(false);
		}

		public async Task<TResponse> UploadFileAsync< TResponse>(string filePath, UploadFileRequest request, IProgress<UploadProgress> uploadProgress, CancellationToken cancellationToken, string fileKey = "file") where TResponse : HttpResponse, new()
		{
			UnitFileUploadUtilityProxy uploadUtilityProxy = new UnitFileUploadUtilityProxy(Settings.HostBackupFilter);
			request.Method = "POST";
			return await HandleRequestAsync<TResponse>(request, (HttpRequest req) => uploadUtilityProxy.UploadFile(request.GetRequestUrl(), filePath, request.GetFormDataParams(), request.HeaderCollection, uploadProgress, cancellationToken, fileKey)).ConfigureAwait(false);
		}

		public async Task<TResponse> UploadFileAsync< TResponse>(string filePureName, Stream fileStream, UploadFileRequest request, IProgress<UploadProgress> uploadProgress, CancellationToken cancellationToken, string fileKey = "file") where TResponse : HttpResponse, new()
		{
			UnitFileUploadUtilityProxy uploadUtilityProxy = new UnitFileUploadUtilityProxy(Settings.HostBackupFilter);
			request.Method = "POST";
			return await HandleRequestAsync<TResponse>(request, (HttpRequest req) => uploadUtilityProxy.UploadFile(request.GetRequestUrl(), filePureName, fileStream, request.GetFormDataParams(), request.HeaderCollection, uploadProgress, cancellationToken, fileKey)).ConfigureAwait(false);
		}

		public async Task<TResponse> UploadFileAsync< TResponse>(string filePath, UploadFileRequest request, IProgress<UploadProgress> uploadProgress, CancellationToken cancellationToken, int getResponseTimeout) where TResponse : HttpResponse, new()
		{
			UnitFileUploadUtilityProxy uploadUtilityProxy = new UnitFileUploadUtilityProxy(Settings.HostBackupFilter);
			request.Method = "POST";
			return await HandleRequestAsync<TResponse>(request, (HttpRequest req) => uploadUtilityProxy.UploadFile(request.GetRequestUrl(), filePath, request.GetFormDataParams(), request.HeaderCollection, uploadProgress, cancellationToken, "file", getResponseTimeout)).ConfigureAwait(false);
		}

		public async Task<long> GetResponseContentLength(HttpRequest request)
		{
			HttpWebResponseWrapper httpWebResponseWrapper = await _httpUtility.GetAsync(request.GetRequestUrl(), HttpMethod.Head.Method, request.UserAgent, request.HeaderCollection, request.CookieCollection).ConfigureAwait(false);
			if (httpWebResponseWrapper.Success)
			{
				return httpWebResponseWrapper.Response?.ContentLength ?? 0;
			}
			return 0L;
		}

		protected virtual async Task<Tuple<bool, TResponse>> PreProccess< TResponse>(HttpRequest request) where TResponse : HttpResponse, new()
		{
			return await Task.FromResult(new Tuple<bool, TResponse>(true, null)).ConfigureAwait(false);
		}

		private Task<TResponse> HandleRequestAsync< TResponse>(HttpRequest request, Func<HttpRequest, Task<HttpWebResponseWrapper>> requestFunc) where TResponse : HttpResponse, new()
		{
			return new WebServiceRequestHandler(Host, Settings, _requestFilters, _requestAsyncFilters, _responseFilters).HandleRequestAsync<TResponse>(request, requestFunc);
		}

		private static string GetMethod(HttpRequest request, string defaultValue)
		{
			RequestAttribute requestAttribute = request.GetType().GetCustomAttributes<RequestAttribute>().FirstOrDefault();
			if (requestAttribute == null || requestAttribute.RequestMethod == RequestMethods.Default)
			{
				return defaultValue;
			}
			return requestAttribute.RequestMethod.ToString().ToUpper();
		}
	}
}
