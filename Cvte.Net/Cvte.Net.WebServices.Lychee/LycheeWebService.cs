using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Cvte.Net.WebServices.Lychee
{
	
	
	public class LycheeWebService : WebService, ILycheeWebService, IWebService
	{
		public LycheeWebService(string appKey, string signKey)
		{
			Host = "http://lizhi.im";
			AddRequestFilter(new LycheeRequestSignFilter(appKey, signKey));
		}

		public async Task<ParsingPptLycheeHttpResponse> ParsingPptAsync(string filePath)
		{
			if (!File.Exists(filePath))
			{
				return new ParsingPptLycheeHttpResponse
				{
					Error = new FileNotFoundException(filePath + " not found.", filePath)
				};
			}
			LycheeHttpResponse<string> uploadResponse = await UploadFileAsync<LycheeHttpResponse<string>>(filePath, new UploadPptFileRequest
			{
				Url = "http://doc.lizhi.im/upload"
			});
			if (uploadResponse.Success)
			{
				return await Task.Run( () =>
				{
					int num = 8;
					int num2 = 0;
					while (true)
					{
						LycheeHttpResponse<string> result = PostAsync<LycheeHttpResponse<string>>(new CheckStateRequest(uploadResponse.Content)).Result;
						if (result.Success)
						{
							return new ParsingPptLycheeHttpResponse
							{
								Code = "ok",
								DownloadUrls = new List<string> { result.Content },
								ResultType = PptParseResultType.ZipPackage
							};
						}
						if (!result.IsSuccessStatusCode)
						{
							return new ParsingPptLycheeHttpResponse
							{
								StatusCode = result.StatusCode
							};
						}
						num2++;
						if (num2 >= num)
						{
							break;
						}
						Thread.Sleep(2000);
					}
					return new ParsingPptLycheeHttpResponse
					{
						Error = new Exception("ppt parsing timeout.")
					};
				});
			}
			return new ParsingPptLycheeHttpResponse
			{
				StatusCode = uploadResponse.StatusCode,
				Error = (uploadResponse.Error ?? (string.IsNullOrEmpty(uploadResponse.ErrorMsg) ? null : new Exception(uploadResponse.ErrorMsg)))
			};
		}

		public async Task<PublishFileLycheeHttpResponse> PublishFileAsync(string filePath)
		{
			if (!File.Exists(filePath))
			{
				return new PublishFileLycheeHttpResponse
				{
					Error = new FileNotFoundException(filePath + " not found.", filePath)
				};
			}
			ApplyTokenResponse applyTokenResponse = await PostAsync<ApplyTokenResponse>(new ApplyTokenRequest
			{
				Count = 1
			});
			if (applyTokenResponse.Success)
			{
				UploadFileLycheeRequest request = new UploadFileLycheeRequest
				{
					Url = applyTokenResponse.Content.UpUrl,
					Key = applyTokenResponse.Content.Token,
					AppId = applyTokenResponse.Content.AppId,
					Token = applyTokenResponse.Content.Token,
					PageName = applyTokenResponse.Content.Pages[0].Name
				};
				UploadFileLycheeResponse uploadFileLycheeResponse = await UploadFileAsync<UploadFileLycheeResponse>(filePath, request);
				if (uploadFileLycheeResponse.Success)
				{
					return new PublishFileLycheeHttpResponse
					{
						Code = "ok",
						DownloadUrl = uploadFileLycheeResponse.Url
					};
				}
				return new PublishFileLycheeHttpResponse
				{
					StatusCode = uploadFileLycheeResponse.StatusCode
				};
			}
			return new PublishFileLycheeHttpResponse
			{
				StatusCode = applyTokenResponse.StatusCode,
				Error = new Exception(applyTokenResponse.ErrorMsg)
			};
		}

		public Task<PublishFileLycheeHttpResponse> PublishFilesAsync(IList<string> files, PublicInfo info)
		{
			return PublishFilesAsync(files, info, default(CancellationToken), null);
		}

		public Task<PublishFileLycheeHttpResponse> PublishFilesAsync(IList<string> files, PublicInfo info, CancellationToken cancellationToken)
		{
			return PublishFilesAsync(files, info, cancellationToken, null);
		}

		public async Task<PublishFileLycheeHttpResponse> PublishFilesAsync(IList<string> files, PublicInfo info, CancellationToken cancellationToken, Action<double> progressChanged)
		{
			if (files.Count == 0)
			{
				return new PublishFileLycheeHttpResponse
				{
					Error = new Exception("no files to publish")
				};
			}
			try
			{
				progressChanged?.Invoke(0.05);
				ApplyTokenResponse tokenResponse = await PostAsync<ApplyTokenResponse>(new ApplyTokenRequest
				{
					Pid = string.Empty,
					Title = info.Title,
					Author = info.Author,
					Scale = info.Scale,
					Count = files.Count
				});
				if (tokenResponse.Success)
				{
					cancellationToken.ThrowIfCancellationRequested();
					progressChanged?.Invoke(0.1);
					LycheeHttpResponse lycheeHttpResponse = await PostAsync<LycheeHttpResponse>(new StartPublishHttpRequest
					{
						Pid = tokenResponse.Content.Pid
					});
					if (!lycheeHttpResponse.Success)
					{
						return new PublishFileLycheeHttpResponse
						{
							Error = lycheeHttpResponse.Error
						};
					}
					cancellationToken.ThrowIfCancellationRequested();
					progressChanged?.Invoke(0.2);
					ApplyTokenContent content = tokenResponse.Content;
					int count = 0;
					Parallel.ForEach(content.Pages,  (ApplyTokenContent.Page page) =>
					{
						cancellationToken.ThrowIfCancellationRequested();
						string filePath = files[int.Parse(page.SortNo) - 1];
						UploadFileLycheeRequest request = new UploadFileLycheeRequest
						{
							Url = content.UpUrl,
							Key = content.Token,
							AppId = content.AppId,
							Token = content.Token,
							PageName = page.Name
						};
						count++;
						double num = (double)count / ((double)files.Count * 1.0);
						progressChanged?.Invoke(0.7 * num + 0.2);
						UploadFileAsync<UploadFileLycheeResponse>(filePath, request).Wait();
					});
					cancellationToken.ThrowIfCancellationRequested();
					progressChanged?.Invoke(0.95);
					CompletePublishResponse completePublishResponse = await PostAsync<CompletePublishResponse>(new CompletePublishRequest
					{
						Pid = content.Pid
					});
					if (completePublishResponse.Success)
					{
						cancellationToken.ThrowIfCancellationRequested();
						progressChanged?.Invoke(1.0);
						return new PublishFileLycheeHttpResponse
						{
							StatusCode = completePublishResponse.StatusCode,
							Code = "ok",
							DownloadUrl = content.PUrlQr
						};
					}
					return new PublishFileLycheeHttpResponse
					{
						Error = completePublishResponse.Error
					};
				}
				return new PublishFileLycheeHttpResponse
				{
					StatusCode = tokenResponse.StatusCode,
					Error = tokenResponse.Error
				};
			}
			catch (AggregateException error)
			{
				return new PublishFileLycheeHttpResponse
				{
					Error = error
				};
			}
			catch (Exception error2)
			{
				return new PublishFileLycheeHttpResponse
				{
					Error = error2
				};
			}
		}
	}
}
