using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Cvte.Net.WebServices.CloudDisk
{
	
	public interface ICloudDiskWebService
	{
		Task<CreateDirectoryResponse> CreateDirectoryAsync(string name, string parentId);

		Task<UpdateDirectoryResponse> UpdateDirectoryNameAsync(string directoryId, string name);

		Task<MoveFileOrDirectoryResponse> MoveFileOrDirectoryAsync(IList<string> sources, string targetId);

		Task<UploadFileResponse> UploadFileAsync(string filePath, IProgress<UploadFileProgress> progress, CancellationToken cancellationToken);

		Task<UploadFileResponse> UploadFileAsync(string filePath, string directoryId, IProgress<UploadFileProgress> progress, CancellationToken cancellationToken);

		Task<GetFilesResponse> GetFilesAsync(string directoryId, int pageIndex, int pageSize);

		Task<GetFileDetailsResponse> GetFileDetailsAsync(string fileId);

		Task<DeleteFileOrDirectoryResponse> DeleteFileOrDirectoryAsync(string uid);

		Task<UpdateFileResponse> UpdateFileAsync(string uid, string name);
	}
}
