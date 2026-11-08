using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Cvte.Net.Http;
using Cvte.Net.Http.HostBackup;

namespace Cvte.Net.WebServices.BlockFile
{
	
	
	public class BlockTaskManager
	{
		private readonly WebService _webService;

		private FileStream _fileStream;

		private const long MaxBlockSize = 4194304L;

		private long _uploadedSize;

		private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(5);

		
		private readonly IHttpRequestHostBackupFilter _requestHostBackupFilter;

		private readonly UploadFileInfo _uploadFileInfo;

		private readonly IProgress<UploadProgress> _uploadProgress;

		private readonly List<BlockFileInfo> _currentBlockFileInfo = new List<BlockFileInfo>();

		private readonly ConcurrentQueue<BlockUploadTask> _blockTasksQueue = new ConcurrentQueue<BlockUploadTask>();

		private readonly BlockingCollection<BlockUploadTask> _failedTasks = new BlockingCollection<BlockUploadTask>();

		private readonly CancellationToken _cancellationToken;

		public event EventHandler<BlockFileArgs> BlockFileCancel;

		public BlockTaskManager(string host,  IHttpRequestHostBackupFilter requestHostBackupFilter, UploadFileInfo uploadFileInfo, IProgress<UploadProgress> uploadProgress, CancellationToken cancellationToken)
		{
			_webService = new WebService(new WebServiceSettings
			{
				HostBackupFilter = requestHostBackupFilter
			})
			{
				Host = host
			};
			_requestHostBackupFilter = requestHostBackupFilter;
			_uploadFileInfo = uploadFileInfo;
			_cancellationToken = cancellationToken;
			_uploadProgress = uploadProgress;
			_uploadedSize = 0L;
		}

		private async Task<bool> PreHandleBeforeStartTask()
		{
			_fileStream = new FileStream(_uploadFileInfo.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
			return await Task.Run(() =>
			{
				long startOffset = 0L;
				long num = 0L;
				long fileSize = _uploadFileInfo.FileSize;
				long num2 = 1L;
				while (num < fileSize)
				{
					long num3 = 4194304L;
					if (fileSize - num < 4194304)
					{
						num3 = fileSize - num;
					}
					num += num3;
					BlockFileInfo blockFileInfo = new BlockFileInfo
					{
						StartOffset = startOffset,
						EndOffset = num,
						BlockSize = num3,
						Index = num2
					};
					_currentBlockFileInfo.Add(blockFileInfo);
					BlockUploadTask item = new BlockUploadTask(_webService.Host, _requestHostBackupFilter, blockFileInfo, _fileStream, _cancellationToken)
					{
						UploadId = _uploadFileInfo.UploadId,
						UploadUrl = _uploadFileInfo.UploadUrl
					};
					_blockTasksQueue.Enqueue(item);
					startOffset = num;
					num2++;
				}
				return true;
			});
		}

		public async Task<UploadFileByBlockResponse> DoBlockFileTransfer()
		{
			List<HttpStatusCode> responseResults = new List<HttpStatusCode>();
			UploadFileByBlockResponse response = new UploadFileByBlockResponse();
			if (!_currentBlockFileInfo.Any())
			{
				if (!(await PreHandleBeforeStartTask()))
				{
					return response;
				}
			}
			else if (_failedTasks.Any())
			{
				BlockUploadTask item;
				while (_failedTasks.TryTake(out item))
				{
					_blockTasksQueue.Enqueue(item);
				}
			}
			while (_blockTasksQueue.Any())
			{
				if (_cancellationToken.IsCancellationRequested)
				{
					BlockFileCancel?.Invoke(this, new BlockFileArgs(_uploadFileInfo.TaskId));
					response.Code = 0;
					_fileStream.Close();
					return response;
				}
				_semaphoreSlim.Wait();
				BlockUploadTask blockTask;
				if (_blockTasksQueue.TryDequeue(out blockTask))
				{
					ValueTuple<bool, HttpStatusCode> valueTuple = await blockTask.StartTask();
					bool item2 = valueTuple.Item1;
					HttpStatusCode item3 = valueTuple.Item2;
					if (item2)
					{
						_uploadedSize += blockTask.BlockFileInfo.BlockSize;
						_uploadProgress?.Report(new UploadProgress(_uploadedSize, _uploadFileInfo.FileSize));
					}
					else
					{
						if (!responseResults.Contains(item3))
						{
							responseResults.Add(item3);
						}
						_failedTasks.Add(blockTask);
					}
				}
				_semaphoreSlim.Release();
				blockTask = null;
			}
			if (_blockTasksQueue.Count == 0 && _failedTasks.Count == 0)
			{
				_fileStream.Close();
				PreMergeFileResponse preMergeFileResponse = await _webService.GetAsync<PreMergeFileResponse>(new PreMergeFileRequest
				{
					UploadId = _uploadFileInfo.UploadId
				});
				if (preMergeFileResponse.Success)
				{
					string data = string.Join(",", _currentBlockFileInfo.Select( (BlockFileInfo x) => x.Ctx).ToList());
					MergeBlockFileRequest mergeBlockFileRequest = new MergeBlockFileRequest
					{
						Url = _uploadFileInfo.CompleteUrl.Replace("<fileSize>", _uploadFileInfo.FileSize.ToString()),
						Data = data
					};
					mergeBlockFileRequest.HeaderCollection["Authorization"] = preMergeFileResponse.HeaderFileInfo.Authorization;
					if ((await _webService.PostAsync<MergeBlockFileResponse>(mergeBlockFileRequest)).Success)
					{
						response.Code = 0;
						response.Success = true;
						return response;
					}
				}
			}
			if (responseResults.Count == 1)
			{
				response.Code = (int)responseResults[0];
			}
			return response;
		}
	}
}
