using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cvte.Net.Http;
using Newtonsoft.Json;

namespace Cvte.Net.WebServices.Feedback
{
	
	
	public class FeedbackWebService : IFeedbackWebService
	{
		private const string AuthorizationKey = "Authorization";

		private string _appId;

		private string _appName;

		private string _appVersion;

		private readonly WebService _webService = new WebService();

		public FeedbackWebService(string host, int productId, string authId, string authKey)
			: this(host, productId.ToString(), authId, authKey)
		{
		}

		public FeedbackWebService(string host, string productId, string authId, string authKey)
		{
			_webService.Host = host;
			_webService.AddRequestFilter(new BasicHttpRequestFilter<FeedbackReqeustBase>((FeedbackReqeustBase request) =>
			{
				request.AuthId = authId;
				string requestUrl = request.GetRequestUrl();
				string valueToEncrypt = (requestUrl.Remove(requestUrl.LastIndexOf("?", StringComparison.Ordinal)).Remove(0, requestUrl.LastIndexOf(host, StringComparison.Ordinal) + host.Length) + request.Timestamp + authKey).ToUpper();
				valueToEncrypt = SecurityUtility.Encrypt32(valueToEncrypt).ToLower();
				request.HeaderCollection = new WebHeaderCollection { { "Authorization", valueToEncrypt } };
			}));
			_webService.AddRequestFilter(new BasicHttpRequestFilter<CommitFeedbackRequest>((CommitFeedbackRequest request) =>
			{
				request.ProductId = productId;
			}));
		}

		public void Init(string appId, string appName, string appVersion)
		{
			_appId = appId;
			_appName = appName;
			_appVersion = appVersion;
		}

		public async Task<bool> Commit(UserInfo user, string title, string content, IList<string> pictures, IList<string> attachments, IList<CustomInfo> softwareInfos)
		{
			if (user == null || !user.Verify())
			{
				throw new ArgumentException("user");
			}
			if (string.IsNullOrEmpty(title))
			{
				throw new ArgumentException("title");
			}
			if (string.IsNullOrEmpty(content))
			{
				throw new ArgumentException("content");
			}
			List<FileInfo> pictureInfos = new List<FileInfo>();
			foreach (string picture in pictures)
			{
				UploadPictueRequest uploadPictueRequest = new UploadPictueRequest
				{
					FilePath = picture
				};
				FeedbackResponse<UploadFileResponseBody> feedbackResponse = await _webService.UploadFileAsync<FeedbackResponse<UploadFileResponseBody>>(uploadPictueRequest.FilePath, uploadPictueRequest, uploadPictueRequest.FileKey);
				if (!feedbackResponse.Success)
				{
					return false;
				}
				pictureInfos.Add(new FileInfo(Path.GetFileName(picture), feedbackResponse.Data.Url));
			}
			List<FileInfo> attachmentInfos = new List<FileInfo>();
			foreach (string picture in attachments)
			{
				UploadAttachmentRequest uploadAttachmentRequest = new UploadAttachmentRequest
				{
					FilePath = picture
				};
				FeedbackResponse<UploadFileResponseBody> feedbackResponse2 = await _webService.UploadFileAsync<FeedbackResponse<UploadFileResponseBody>>(uploadAttachmentRequest.FilePath, uploadAttachmentRequest, uploadAttachmentRequest.FileKey);
				if (!feedbackResponse2.Success)
				{
					return false;
				}
				attachmentInfos.Add(new FileInfo(Path.GetFileName(picture), feedbackResponse2.Data.Url));
			}
			return (await _webService.PostAsync<FeedbackResponse<CommitResponseBody>>(new CommitFeedbackRequest(_appId, title, content, FeedbackType.Probliem, FeedbackChannel.Pc)
			{
				AppName = _appName,
				UserId = user.UserId,
				Mobile = user.Mobile,
				Email = user.Email,
				Wenxin = user.Wenxin,
				Device = user.Device,
				Version = _appVersion,
				Platform = Environment.OSVersion.VersionString,
				Pictures = JsonConvert.SerializeObject(pictureInfos),
				Attachments = JsonConvert.SerializeObject(attachmentInfos),
				EnvironmentInfo = JsonConvert.SerializeObject(new List<CustomInfo>
				{
					new CustomInfo("OSBit", "系统位数", Environment.Is64BitOperatingSystem ? "64" : "32")
				}),
				SoftwareInfo = JsonConvert.SerializeObject(softwareInfos)
			})).Success;
		}
	}
}
