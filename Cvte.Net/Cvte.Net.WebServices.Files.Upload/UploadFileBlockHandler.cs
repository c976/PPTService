using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Cvte.Net.Http;
using Cvte.Net.Http.HostBackup;
using Cvte.Net.WebServices.BlockFile;

namespace Cvte.Net.WebServices.Files.Upload
{
	
	
	internal class UploadFileBlockHandler
	{
		
		private readonly IHttpRequestHostBackupFilter _requestHostBackupFilter;

		private readonly ConcurrentDictionary<string, BlockTaskManager> _blockTaskManagers = new ConcurrentDictionary<string, BlockTaskManager>();

		private string Host { get; }

		public UploadFileBlockHandler(string host,  IHttpRequestHostBackupFilter requestHostBackupFilter)
		{
			_requestHostBackupFilter = requestHostBackupFilter;
			Host = host;
		}

		public async Task<UploadFileByBlockResponse> UploadFileBlockAsync(UploadFileInfo uploadFileInfo, IProgress<UploadProgress> uploadProgress, CancellationToken cancellationToken, string fileKey = "file")
		{
			UploadFileByBlockResponse response = new UploadFileByBlockResponse();
			string taskId = uploadFileInfo.TaskId;
			BlockTaskManager blockTaskManager;
			if (_blockTaskManagers.ContainsKey(taskId))
			{
				blockTaskManager = _blockTaskManagers[taskId];
			}
			else
			{
				blockTaskManager = new BlockTaskManager(Host, _requestHostBackupFilter, uploadFileInfo, uploadProgress, cancellationToken);
				if (!_blockTaskManagers.TryAdd(taskId, blockTaskManager))
				{
					return response;
				}
				blockTaskManager.BlockFileCancel +=  (object sender,  BlockFileArgs args) =>
				{
					BlockTaskManager value2;
					_blockTaskManagers.TryRemove(args.FileHash, out value2);
				};
			}
			UploadFileByBlockResponse uploadFileByBlockResponse = await blockTaskManager.DoBlockFileTransfer().ConfigureAwait(false);
			BlockTaskManager value;
			if (uploadFileByBlockResponse.Success && !_blockTaskManagers.TryRemove(taskId, out value))
			{
				return response;
			}
			return uploadFileByBlockResponse;
		}
	}
}
