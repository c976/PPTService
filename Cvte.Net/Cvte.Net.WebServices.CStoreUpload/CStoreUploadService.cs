using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Cvte.Net.Http;
using Cvte.Net.Http.HostBackup;

namespace Cvte.Net.WebServices.CStoreUpload
{
	
	
	public class CStoreUploadService
	{
		private readonly WebService _webService;

		public CStoreUploadService()
			: this(new WebService())
		{
		}

		public CStoreUploadService(WebService webService)
		{
			_webService = webService;
		}

		
		public CStoreUploadService(IHttpRequestHostBackupFilter hostBackupFilter)
		{
			_webService = new WebService(new WebServiceSettings
			{
				HostBackupFilter = hostBackupFilter
			});
		}

		public async Task<CStoreUploadFileResponse> UploadFileAsync(string filePath, CStoreUploadFileRequest uploadFileRequest)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			foreach (FieldEntry headerField in uploadFileRequest.HeaderFields)
			{
				uploadFileRequest.HeaderCollection.Add(headerField.Key, headerField.Value);
			}
			return await _webService.UploadFileAsync<CStoreUploadFileResponse>(filePath, uploadFileRequest, null, CancellationToken.None);
		}

		public async Task<CStoreUploadFileResponse> UploadFileAsync(string filePath, CStoreUploadFileRequest uploadFileRequest, IProgress<UploadProgress> uploadProgress, CancellationToken cancellationToken)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			foreach (FieldEntry headerField in uploadFileRequest.HeaderFields)
			{
				uploadFileRequest.HeaderCollection.Add(headerField.Key, headerField.Value);
			}
			return await _webService.UploadFileAsync<CStoreUploadFileResponse>(filePath, uploadFileRequest, uploadProgress, cancellationToken);
		}

		public async Task<CStoreUploadFileResponse> UploadFileAsync(string filePath, CStoreUploadFileRequest uploadFileRequest, IProgress<UploadProgress> uploadProgress, CancellationToken cancellationToken, string type)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			foreach (FieldEntry headerField in uploadFileRequest.HeaderFields)
			{
				uploadFileRequest.HeaderCollection.Add(headerField.Key, headerField.Value);
			}
			if (ServiceProviderExtension.GetServiceProvider(type) == ServiceProvider.QINIU)
			{
				List<FieldEntry> list = uploadFileRequest.FormFields.ToList();
				uint num = Crc32Provider.CheckSumFile(filePath);
				list.Add(new FieldEntry
				{
					Key = "crc32",
					Value = num.ToString()
				});
				uploadFileRequest.FormFields = list;
			}
			return await _webService.UploadFileAsync<CStoreUploadFileResponse>(filePath, uploadFileRequest, uploadProgress, cancellationToken);
		}

		public async Task<CStoreUploadFileResponse> UploadFileAsync(string filePruneName, Stream fileStream, CStoreUploadFileRequest uploadFileRequest, IProgress<UploadProgress> uploadProgress, CancellationToken cancellationToken, string type)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			foreach (FieldEntry headerField in uploadFileRequest.HeaderFields)
			{
				uploadFileRequest.HeaderCollection.Add(headerField.Key, headerField.Value);
			}
			if (ServiceProviderExtension.GetServiceProvider(type) == ServiceProvider.QINIU)
			{
				List<FieldEntry> list = uploadFileRequest.FormFields.ToList();
				uint num = Crc32Provider.CheckSumFileWithStream(fileStream);
				list.Add(new FieldEntry
				{
					Key = "crc32",
					Value = num.ToString()
				});
				uploadFileRequest.FormFields = list;
			}
			return await _webService.UploadFileAsync<CStoreUploadFileResponse>(filePruneName, fileStream, uploadFileRequest, uploadProgress, cancellationToken).ConfigureAwait(false);
		}
	}
}
