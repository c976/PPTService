using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using Cvte.Pipe.Http;
using Cvte.Pipe.Http.Request;
using Newtonsoft.Json;
using Cvte.WebService.Internal;

namespace Cvte.WebService.Feedback
{
	public class FeedbackService
	{
		private sealed class Class21
		{
			public FeedbackService feedbackService_0;

			public string string_0;

			public int int_0;

			internal void method_0(RequestBase requestBase_0)
			{
				requestBase_0.Host = feedbackService_0.string_0;
			}

			internal void method_1(RequestBase requestBase_0)
			{
				FeedbackReqeustBase feedbackReqeustBase = (FeedbackReqeustBase)requestBase_0;
				feedbackReqeustBase.AuthId = feedbackService_0.string_1;
				string requestUrl = requestBase_0.GetRequestUrl();
				string text = (requestUrl.Remove(requestUrl.LastIndexOf("?", StringComparison.Ordinal)).Remove(0, requestUrl.LastIndexOf(string_0, StringComparison.Ordinal) + string_0.Length) + feedbackReqeustBase.Timestamp + feedbackService_0.string_2).ToUpper();
				text = Md5Hash.smethod_0(text).ToLower();
				feedbackReqeustBase.HeaderCollection = new WebHeaderCollection { { "Authorization", text } };
			}

			internal void method_2(RequestBase requestBase_0)
			{
				((CommitFeedbackRequest)requestBase_0).ProductId = int_0;
			}
		}

		private readonly BasicHttpPipe basicHttpPipe_0 = new BasicHttpPipe();

		private readonly string string_0;

		private readonly string string_1;

		private readonly string string_2;

		private string string_3;

		private string string_4;

		private string string_5;

		public FeedbackService(string host, int productId, string authId, string authKey)
		{
			FeedbackService feedbackService_0 = this;
			string_0 = host;
			string_1 = authId;
			string_2 = authKey;
			basicHttpPipe_0.AddRequestFilter((RequestBase requestBase_0) => true, (RequestBase requestBase_0) =>
			{
				requestBase_0.Host = feedbackService_0.string_0;
			});
			basicHttpPipe_0.AddRequestFilter((RequestBase requestBase_0) => requestBase_0 is FeedbackReqeustBase, (RequestBase requestBase_0) =>
			{
				FeedbackReqeustBase feedbackReqeustBase = (FeedbackReqeustBase)requestBase_0;
				feedbackReqeustBase.AuthId = feedbackService_0.string_1;
				string requestUrl = requestBase_0.GetRequestUrl();
				string text = (requestUrl.Remove(requestUrl.LastIndexOf("?", StringComparison.Ordinal)).Remove(0, requestUrl.LastIndexOf(host, StringComparison.Ordinal) + host.Length) + feedbackReqeustBase.Timestamp + feedbackService_0.string_2).ToUpper();
				text = Md5Hash.smethod_0(text).ToLower();
				feedbackReqeustBase.HeaderCollection = new WebHeaderCollection { { "Authorization", text } };
			});
			basicHttpPipe_0.AddRequestFilter((RequestBase requestBase_0) => requestBase_0 is CommitFeedbackRequest, (RequestBase requestBase_0) =>
			{
				((CommitFeedbackRequest)requestBase_0).ProductId = productId;
			});
		}

		public void Init(string appId, string appName, string appVersion)
		{
			string_3 = appId;
			string_4 = appName;
			string_5 = appVersion;
		}

		public bool Commit(UserInfo user, string title, string content, IList<string> pictures, IList<string> attachments, IList<CustomInfo> softwareInfos)
		{
			if (user != null && user.Verify())
			{
				if (string.IsNullOrEmpty(title))
				{
					throw new ArgumentException("title");
				}
				if (string.IsNullOrEmpty(content))
				{
					throw new ArgumentException("content");
				}
				List<FileInfo> list = new List<FileInfo>();
				foreach (string picture in pictures)
				{
					FeedbackResponse<UploadFileResponseBody> feedbackResponse = basicHttpPipe_0.UploadFile<PictureUploadRequest, FeedbackResponse<UploadFileResponseBody>>(new PictureUploadRequest
					{
						FilePath = picture
					});
					if (!feedbackResponse.IsBadRequest && feedbackResponse.Succeed)
					{
						list.Add(new FileInfo(Path.GetFileName(picture), feedbackResponse.Data.Url));
						continue;
					}
					return false;
				}
				List<FileInfo> list2 = new List<FileInfo>();
				foreach (string attachment in attachments)
				{
					FeedbackResponse<UploadFileResponseBody> feedbackResponse2 = basicHttpPipe_0.UploadFile<AttachmentUploadRequest, FeedbackResponse<UploadFileResponseBody>>(new AttachmentUploadRequest
					{
						FilePath = attachment
					});
					if (!feedbackResponse2.IsBadRequest && feedbackResponse2.Succeed)
					{
						list2.Add(new FileInfo(Path.GetFileName(attachment), feedbackResponse2.Data.Url));
						continue;
					}
					return false;
				}
				return basicHttpPipe_0.Post<CommitFeedbackRequest, FeedbackResponse<CommitResponseBody>>(new CommitFeedbackRequest(string_3, title, content, FeedbackType.Probliem, FeedbackChannel.Pc)
				{
					AppName = string_4,
					UserId = user.UserId,
					Mobile = user.Mobile,
					Email = user.Email,
					Wenxin = user.Wenxin,
					Device = user.Device,
					Version = string_5,
					Platform = Environment.OSVersion.VersionString,
					Pictures = JsonConvert.SerializeObject((object)list),
					Attachments = JsonConvert.SerializeObject((object)list2),
					EnvironmentInfo = JsonConvert.SerializeObject((object)new List<CustomInfo>
					{
						new CustomInfo("OSBit", "系统位数", Environment.Is64BitOperatingSystem ? "64" : "32")
					}),
					SoftwareInfo = JsonConvert.SerializeObject((object)softwareInfos)
				}).Succeed;
			}
			throw new ArgumentException("user");
		}
	}
}
