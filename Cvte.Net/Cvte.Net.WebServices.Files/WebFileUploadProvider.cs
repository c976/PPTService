using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Cvte.Net.Http;
using Cvte.Net.Http.HostBackup;
using Cvte.Net.WebServices.BlockFile;
using Cvte.Net.WebServices.Files.Upload;

namespace Cvte.Net.WebServices.Files
{
	
	
	public class WebFileUploadProvider
	{
		private string Host { get; }

		
		
		private IHttpRequestHostBackupFilter RequestHostBackupFilter
		{
			
			get;
		}

		public WebFileUploadProvider(string host,  IHttpRequestHostBackupFilter requestHostBackupFilter)
		{
			Host = host;
			RequestHostBackupFilter = requestHostBackupFilter;
		}

		public Task<UploadFileByBlockResponse> UploadFileBlockAsync(UploadFileInfo uploadFileInfo, IProgress<UploadProgress> uploadProgress, CancellationToken cancellationToken, string fileKey = "file")
		{
			return new UploadFileBlockHandler(Host, RequestHostBackupFilter).UploadFileBlockAsync(uploadFileInfo, uploadProgress, cancellationToken, fileKey);
		}
	}
}
