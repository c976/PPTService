using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Cvte.Net.Http;
using Cvte.Net.Http.HostBackup;
using Cvte.Net.WebServices.Files.Upload;

namespace Cvte.Net.WebServices.BlockFile
{
	
	
	public class BlockUploadTask
	{
		private readonly string _host;

		
		private readonly IHttpRequestHostBackupFilter _requestHostBackupFilter;

		private readonly FileStream _fileStream;

		private readonly CancellationToken _cancellationToken;

		private readonly WebService _webService;

		public BlockFileInfo BlockFileInfo { get; }

		public string UploadId { get; set; }

		public string UploadUrl { get; set; }

		public BlockUploadTask(string host,  IHttpRequestHostBackupFilter requestHostBackupFilter, BlockFileInfo blockFileInfo, FileStream fileStream, CancellationToken cancellationToken)
		{
			_webService = new WebService(new WebServiceSettings
			{
				HostBackupFilter = requestHostBackupFilter
			})
			{
				Host = host
			};
			_host = host;
			_requestHostBackupFilter = requestHostBackupFilter;
			_fileStream = fileStream;
			_cancellationToken = cancellationToken;
			BlockFileInfo = blockFileInfo;
		}

		
		public async Task<ValueTuple<bool, HttpStatusCode>> StartTask()
		{
			PreBlockFileResponse preBlockFileResponse = await _webService.GetAsync<PreBlockFileResponse>(new PreBlockFileRequest
			{
				Part = BlockFileInfo.Index,
				UploadId = UploadId
			});
			if (!preBlockFileResponse.Success)
			{
				return new ValueTuple<bool, HttpStatusCode>(preBlockFileResponse.Success, preBlockFileResponse.StatusCode);
			}
			BlockFileRequest blockFileRequest = new BlockFileRequest
			{
				Url = UploadUrl.Replace("<blockSize>", $"{BlockFileInfo.BlockSize}")
			};
			if (preBlockFileResponse.HeaderFileInfo?.Authorization == null)
			{
				throw new ArgumentNullException("/api/v1/upload/getSinglePartUploadToken 返回的 response 中 Header 缺少 Authorization 信息，无法上传文件！");
			}
			blockFileRequest.HeaderCollection["Authorization"] = preBlockFileResponse.HeaderFileInfo.Authorization;
			BlockFileResponse blockFileResponse = await UploadFileBlockAsync<BlockFileResponse>(_fileStream, BlockFileInfo.StartOffset, BlockFileInfo.BlockSize, blockFileRequest, _cancellationToken);
			if (blockFileResponse.Success)
			{
				BlockFileInfo.Ctx = blockFileResponse.Ctx;
			}
			return new ValueTuple<bool, HttpStatusCode>(blockFileResponse.Success, blockFileResponse.StatusCode);
		}

		private async Task<TResponse> UploadFileBlockAsync< TResponse>(Stream stream, long startOffset, long blockSize, JsonRequest request, CancellationToken cancellationToken, string fileKey = "file") where TResponse : HttpResponse, new()
		{
			request.Method = "POST";
			WebServiceRequestHandler webServiceRequestHandler = new WebServiceRequestHandler(_host);
			UnitFileUploadUtilityProxy uploadUtilityProxy = new UnitFileUploadUtilityProxy(_requestHostBackupFilter);
			return await webServiceRequestHandler.HandleRequestAsync<TResponse>(request, (HttpRequest req) => uploadUtilityProxy.UploadBlockFile(request.GetRequestUrl(), stream, startOffset, blockSize, request.HeaderCollection, cancellationToken, fileKey)).ConfigureAwait(false);
		}
	}
}
