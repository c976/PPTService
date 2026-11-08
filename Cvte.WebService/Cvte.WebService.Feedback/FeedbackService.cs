using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using _0006;
using _0008;
using _000F;
using Cvte.Pipe.Http;
using Cvte.Pipe.Http.Request;
using Newtonsoft.Json;

namespace Cvte.WebService.Feedback
{
	public class FeedbackService
	{
		[CompilerGenerated]
		private sealed class SA_C4
		{
			public dynamic _0001;



			internal void _0001_M4(RequestBase P_0)
			{
				SA_Proxy._007E_0092___0082(P_0, this._0001.m__0001);
			}

			internal void _0002(RequestBase P_0)
			{
				FeedbackReqeustBase feedbackReqeustBase = (FeedbackReqeustBase)P_0;
				feedbackReqeustBase.AuthId = this._0001._0002;
				string text = SA_Proxy._007E_0082___0018(P_0);
				string text2 = SA_Proxy._007E_0080___0018(SA_Proxy._008F___0080(SA_Proxy._007E_001A___0014(SA_Proxy._007E_0006_0002___0091(text, SA_Proxy._007E_0005_0002___0090(text, "?", StringComparison.Ordinal)), 0, SA_Proxy._007E_0005_0002___0090(text, this._0001, StringComparison.Ordinal) + SA_Proxy._007E_0018___0013(this._0001)), feedbackReqeustBase.Timestamp, this._0001._0003));
				text2 = SA_Proxy._007E_0084___0018(SA_Helpers.Str(text2));
				SA_Proxy._007E_0008_0002___0093(feedbackReqeustBase, new WebHeaderCollection { { "Authorization", text2 } });
			}

			internal void _0003(RequestBase P_0)
			{
				((CommitFeedbackRequest)P_0).ProductId = this._0001;
			}
		}

		public dynamic m__0001;


		private readonly string _0002;

		private readonly string _0003;

		private string _0004;

		private string _0005;

		private string _0006;

		public FeedbackService(string host, int productId, string authId, string authKey)
		{
			FeedbackService feedbackService = this;
			this.m__0001 = host;
			_0002 = authId;
			_0003 = authKey;
			this.m__0001.AddRequestFilter(
				(Func<bool>)(delegate { return true; }),
				(Action<RequestBase>)((RequestBase P_0) =>
				{
					SA_Proxy._007E_0092___0082(P_0, feedbackService.m__0001);
				}));
			this.m__0001.AddRequestFilter(
				(Func<RequestBase, bool>)((RequestBase P_0) => P_0 is FeedbackReqeustBase),
				(Action<RequestBase>)((RequestBase P_0) =>
				{
					FeedbackReqeustBase feedbackReqeustBase = (FeedbackReqeustBase)P_0;
					feedbackReqeustBase.AuthId = feedbackService._0002;
					string text = SA_Proxy._007E_0082___0018(P_0);
					string text2 = SA_Proxy._007E_0080___0018(SA_Proxy._008F___0080(SA_Proxy._007E_001A___0014(SA_Proxy._007E_0006_0002___0091(text, SA_Proxy._007E_0005_0002___0090(text, "?", StringComparison.Ordinal)), 0, SA_Proxy._007E_0005_0002___0090(text, host, StringComparison.Ordinal) + SA_Proxy._007E_0018___0013(host)), feedbackReqeustBase.Timestamp, feedbackService._0003));
					text2 = SA_Proxy._007E_0084___0018(SA_Helpers.Str(text2));
					SA_Proxy._007E_0008_0002___0093(feedbackReqeustBase, new WebHeaderCollection { { "Authorization", text2 } });
				}));
			this.m__0001.AddRequestFilter(
				(Func<RequestBase, bool>)((RequestBase P_0) => P_0 is CommitFeedbackRequest),
				(Action<RequestBase>)((RequestBase P_0) =>
				{
					((CommitFeedbackRequest)P_0).ProductId = productId;
				}));
		}



		public void Init(string appId, string appName, string appVersion)
		{
			_0004 = appId;
			_0005 = appName;
			_0006 = appVersion;
		}

		public bool Commit(UserInfo user, string title, string content, IList<string> pictures, IList<string> attachments, IList<CustomInfo> softwareInfos)
		{
			if (user != null)
			{
				bool flag = user.Verify();
				List<FileInfo> list = default(List<FileInfo>);
				while (flag && 5 != 0)
				{
					if (string.IsNullOrEmpty(title))
					{
						throw new ArgumentException("title");
					}
					bool flag2;
					while (true)
					{
						if (string.IsNullOrEmpty(content))
						{
							throw new ArgumentException("content");
						}
						if (8 != 0)
						{
							list = new List<FileInfo>();
						}
						IEnumerator<string> enumerator = pictures.GetEnumerator();
						try
						{
							if (8 == 0)
							{
								goto IL_0065;
							}
							goto IL_00bc;
							IL_00bc:
							if (enumerator.MoveNext())
							{
								goto IL_0065;
							}
							goto end_IL_0060;
							IL_0065:
							string current = enumerator.Current;
							FeedbackResponse<UploadFileResponseBody> feedbackResponse = this.m__0001.UploadFile<_000F._0001, FeedbackResponse<UploadFileResponseBody>>(new _000F._0001
							{
								FilePath = current
							});
							if (feedbackResponse.IsBadRequest || !feedbackResponse.Succeed)
							{
								flag2 = false;
								break;
							}
							list.Add(new FileInfo(Path.GetFileName(current), feedbackResponse.Data.Url));
							goto IL_00bc;
							end_IL_0060:;
						}
						finally
						{
							if (enumerator != null && 0 == 0)
							{
								enumerator.Dispose();
							}
						}
						if (false)
						{
							continue;
						}
						List<FileInfo> list2 = new List<FileInfo>();
						foreach (string attachment in attachments)
						{
							FeedbackResponse<UploadFileResponseBody> feedbackResponse2 = this.m__0001.UploadFile<_0008._0002, FeedbackResponse<UploadFileResponseBody>>(new _0008._0002
							{
								FilePath = attachment
							});
							if (feedbackResponse2.IsBadRequest || !feedbackResponse2.Succeed)
							{
								flag2 = false;
								goto end_IL_027b;
							}
							list2.Add(new FileInfo(Path.GetFileName(attachment), feedbackResponse2.Data.Url));
						}
						return this.m__0001.Post<CommitFeedbackRequest, FeedbackResponse<CommitResponseBody>>(new CommitFeedbackRequest(_0004, title, content, FeedbackType.Probliem, FeedbackChannel.Pc)
						{
							AppName = _0005,
							UserId = user.UserId,
							Mobile = user.Mobile,
							Email = user.Email,
							Wenxin = user.Wenxin,
							Device = user.Device,
							Version = _0006,
							Platform = Environment.OSVersion.VersionString,
							Pictures = JsonConvert.SerializeObject(list),
							Attachments = JsonConvert.SerializeObject(list2),
							EnvironmentInfo = JsonConvert.SerializeObject(new List<CustomInfo>
							{
								new CustomInfo("OSBit", "系统位数", Environment.Is64BitOperatingSystem ? "64" : "32")
							}),
							SoftwareInfo = JsonConvert.SerializeObject(softwareInfos)
						}).Succeed;
						continue;
						end_IL_027b:
						break;
					}
					flag = flag2;
					if (true)
					{
						return flag;
					}
				}
			}
			throw new ArgumentException("user");
		}
	}
}
