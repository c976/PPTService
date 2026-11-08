using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cvte.Net.WebServices.CloudDisk
{
	
	
	public class CloudDiskWebService : WebService, ICloudDiskWebService
	{
		public CloudDiskWebService(WebServiceSettings settings)
			: base(settings)
		{
		}

		public async Task<CreateDirectoryResponse> CreateDirectoryAsync(string name, string parentId)
		{
			return await PostAsync<CreateDirectoryResponse>(new CreateDirectoryRequest
			{
				Name = name,
				ParentId = parentId
			});
		}

		public async Task<UpdateDirectoryResponse> UpdateDirectoryNameAsync(string directoryId, string name)
		{
			return await PostAsync<UpdateDirectoryResponse>(new UpdateDirectoryRequest(directoryId)
			{
				Name = name
			});
		}

		public async Task<MoveFileOrDirectoryResponse> MoveFileOrDirectoryAsync(IList<string> sources, string targetId)
		{
			return await PostAsync<MoveFileOrDirectoryResponse>(new MoveFileOrDirectoryRequest
			{
				Target = targetId,
				Sources = sources
			});
		}

		public async Task<UploadFileResponse> UploadFileAsync(string filePath, IProgress<UploadFileProgress> progress, CancellationToken cancellationToken)
		{
			return await UploadFileAsync(filePath, string.Empty, progress, cancellationToken);
		}

		public async Task<UploadFileResponse> UploadFileAsync(string filePath, string directoryId, IProgress<UploadFileProgress> progress, CancellationToken cancellationToken)
		{
			int num = 2;
			try
			{
				if (!File.Exists(filePath))
				{
					return new UploadFileResponse
					{
						Error = new FileNotFoundException("file not found", filePath)
					};
				}
				ApplyUploadFileRequest request = new ApplyUploadFileRequest
				{
					DirectoryId = directoryId,
					Files = new List<FileItem>
					{
						new FileItem(filePath)
					}
				};
				ApplyUploadFileResponse applyUploadFileResponse = await PostAsync<ApplyUploadFileResponse>(request);
				if (cancellationToken.IsCancellationRequested)
				{
					return new UploadFileResponse
					{
						Error = new OperationCanceledException()
					};
				}
				if (!applyUploadFileResponse.Success)
				{
					return new UploadFileResponse
					{
						StatusCode = applyUploadFileResponse.StatusCode,
						Code = applyUploadFileResponse.Code,
						Error = applyUploadFileResponse.Error
					};
				}
				FileItem fileInfo = applyUploadFileResponse.Files.FirstOrDefault();
				if (fileInfo == null)
				{
					return new UploadFileResponse
					{
						StatusCode = applyUploadFileResponse.StatusCode,
						Code = applyUploadFileResponse.Code,
						Error = applyUploadFileResponse.Error
					};
				}
				switch (fileInfo.TypeCode)
				{
				case 0:
					return new UploadFileResponse
					{
						Error = new Exception(fileInfo.Message)
					};
				case 1:
					return new UploadFileResponse
					{
						Code = 200,
						Uid = fileInfo.Uid,
						Name = fileInfo.FileName,
						Hash = fileInfo.Hash,
						Size = fileInfo.Size
					};
				default:
				{
					if (!string.IsNullOrEmpty(applyUploadFileResponse.KeyName))
					{
						applyUploadFileResponse.UploadParams.Add(applyUploadFileResponse.KeyName, fileInfo.KeyValue);
					}
					HttpResponseMessage httpResponseMessage = await UploadFileAsync(filePath, applyUploadFileResponse.UploadUrl, applyUploadFileResponse.UploadParams, applyUploadFileResponse.FileKey, cancellationToken);
					if (!httpResponseMessage.IsSuccessStatusCode)
					{
						return new UploadFileResponse
						{
							StatusCode = httpResponseMessage.StatusCode
						};
					}
					string value = await httpResponseMessage.Content.ReadAsStringAsync();
					var anonymousTypeObject = new
					{
						ret = ""
					};
					anonymousTypeObject = JsonConvert.DeserializeAnonymousType(value, anonymousTypeObject);
					if (!string.IsNullOrEmpty(anonymousTypeObject.ret) && anonymousTypeObject.ret != "ok")
					{
						return new UploadFileResponse
						{
							Code = 500
						};
					}
					return new UploadFileResponse
					{
						Code = 200,
						Uid = fileInfo.Uid,
						Name = fileInfo.FileName,
						Hash = fileInfo.Hash,
						Size = fileInfo.Size
					};
				}
				}
			}
			catch (Exception error)
			{
				return new UploadFileResponse
				{
					Error = error
				};
			}
		}

		public async Task<GetFilesResponse> GetFilesAsync(string directoryId, int pageIndex, int pageSize)
		{
			return await GetAsync<GetFilesResponse>(new GetFilesRequest
			{
				DirectoryId = directoryId,
				PageIndex = pageIndex,
				PageSize = pageSize
			});
		}

		public async Task<GetFileDetailsResponse> GetFileDetailsAsync(string fileId)
		{
			return await GetAsync<GetFileDetailsResponse>(new GetFileDetailRequest(fileId));
		}

		public async Task<DeleteFileOrDirectoryResponse> DeleteFileOrDirectoryAsync(string uid)
		{
			return await PostAsync<DeleteFileOrDirectoryResponse>(new DeleteFileOrDirectoryRequest(uid));
		}

		public async Task<UpdateFileResponse> UpdateFileAsync(string uid, string name)
		{
			return await PostAsync<UpdateFileResponse>(new UpdateFileRequest(uid)
			{
				Name = name
			});
		}

		private static async Task<HttpResponseMessage> UploadFileAsync(string filePath, string uploadUrl, IDictionary<string, string> uploadParams, string fileKey, CancellationToken cancellationToken)
		{
			using (HttpClient client = new HttpClient())
			{
				client.Timeout = TimeSpan.FromSeconds(Math.Max((double)new FileInfo(filePath).Length / 10240.0, 100.0));
				MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
				foreach (KeyValuePair<string, string> uploadParam in uploadParams)
				{
					multipartFormDataContent.Add(new StringContent(uploadParam.Value, Encoding.UTF8), uploadParam.Key);
				}
				using (FileStream fileStream = File.OpenRead(filePath))
				{
					StreamContent streamContent = new StreamContent(fileStream);
					string text = Path.GetFileName(filePath);
					if (text != null)
					{
						text = Encoding.GetEncoding("ISO8859-1").GetString(Encoding.UTF8.GetBytes(text));
					}
					streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
					streamContent.Headers.TryAddWithoutValidation("Content-Disposition", "form-data; name=\"" + fileKey + "\"; filename=\"" + text + "\"");
					multipartFormDataContent.Add(streamContent);
					HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uploadUrl)
					{
						Content = multipartFormDataContent
					};
					httpRequestMessage.Headers.TryAddWithoutValidation("Charsert", "UTF-8");
					httpRequestMessage.Headers.TryAddWithoutValidation("Cache-Control", "no-cache");
					httpRequestMessage.Headers.TryAddWithoutValidation("Pragma", "no-cache");
					return await client.SendAsync(httpRequestMessage, cancellationToken);
				}
			}
		}
	}
}
