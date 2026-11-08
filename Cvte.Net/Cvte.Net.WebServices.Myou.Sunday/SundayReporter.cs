using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Cvte.Net.WebServices.Myou.Sunday
{
	
	
	public class SundayReporter : WebService, ISundayReporter
	{
		private readonly SundayEnvironment _sundayEnvironment;

		public SundayContext SundayContext { get; }

		public SundayReporter(SundayContext sundayContext)
		{
			SundayContext = sundayContext;
			_sundayEnvironment = new SundayEnvironment();
		}

		public async Task ReportErrorWithAttachmentAsync(string businessKey, string subKey, string errorType, string extraMessage, Exception exception, IDictionary<string, string> otherMessage, string attachmentPath)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}
			if (string.IsNullOrWhiteSpace(businessKey))
			{
				businessKey = exception.GetType().Name;
			}
			if (string.IsNullOrWhiteSpace(subKey))
			{
				subKey = exception.GetType().Name;
			}
			if (otherMessage == null)
			{
				otherMessage = new Dictionary<string, string>();
			}
			otherMessage["#ExceptionMessage"] = exception.Message;
			if (exception.InnerException != null)
			{
				otherMessage["#InnerExceptionType"] = exception.InnerException.GetType().Name;
				otherMessage["#InnerExceptionMessage"] = exception.InnerException.Message;
				otherMessage["#InnerExceptionStackTrace"] = exception.InnerException.StackTrace;
			}
			await ReportErrorWithAttachmentAsync(businessKey, subKey, errorType, extraMessage, exception.StackTrace, otherMessage, attachmentPath);
		}

		public async Task ReportErrorWithAttachmentAsync(string businessKey, string subKey, string errorType, string extraMessage, StackTrace stackTrace, IDictionary<string, string> otherMessage, string attachmentPath)
		{
			if (string.IsNullOrWhiteSpace(businessKey))
			{
				throw new ArgumentNullException("businessKey");
			}
			if (stackTrace == null)
			{
				throw new ArgumentNullException("stackTrace");
			}
			await ReportErrorWithAttachmentAsync(businessKey, subKey, errorType, extraMessage, stackTrace.ToString(), otherMessage, attachmentPath);
		}

		private async Task ReportErrorWithAttachmentAsync(string businessKey, string subKey, string errorType, string extraMessage, string stackTrace, IDictionary<string, string> otherMessage, string attachmentPath)
		{
			if (string.IsNullOrWhiteSpace(businessKey))
			{
				throw new ArgumentNullException("businessKey");
			}
			if (string.IsNullOrWhiteSpace(subKey))
			{
				subKey = businessKey;
			}
			bool flag = !string.IsNullOrWhiteSpace(attachmentPath);
			PostErrorRequest request = BuildPostErrorRequest(businessKey, subKey, errorType, stackTrace, CombineExtraMessage(extraMessage, otherMessage), flag);
			if (flag)
			{
				await UploadFileAsync<PostErrorResponse>(attachmentPath, request);
			}
			else
			{
				await PostAsync<PostErrorResponse>(request);
			}
		}

		private IDictionary<string, string> CombineExtraMessage(string extraMessage, IDictionary<string, string> otherMessage)
		{
			if (otherMessage == null)
			{
				otherMessage = new Dictionary<string, string>();
			}
			if (string.IsNullOrWhiteSpace(extraMessage))
			{
				return otherMessage;
			}
			if (otherMessage.ContainsKey("ExtraMessage"))
			{
				otherMessage["ExtraMessage-" + Guid.NewGuid().ToString()] = extraMessage;
			}
			else
			{
				otherMessage["ExtraMessage"] = extraMessage;
			}
			return otherMessage;
		}

		private PostErrorRequest BuildPostErrorRequest(string businessKey, string subKey, string errorType, string stackTrace, object context, bool needUploadLog)
		{
			PostErrorRequest postErrorRequest = new PostErrorRequest();
			postErrorRequest.SessionId = SessionIdProvider.SessionId;
			postErrorRequest.User = SundayContext.UserId;
			postErrorRequest.AppVersion = SundayContext.AppVersion;
			postErrorRequest.AppName = SundayContext.AppName;
			postErrorRequest.AppSource = SundayContext.AppSource;
			postErrorRequest.ComponentName = SundayContext.ComponentName;
			postErrorRequest.ComponentVersion = SundayContext.ComponentVersion;
			postErrorRequest.OSName = _sundayEnvironment.OSName;
			postErrorRequest.OSVersion = _sundayEnvironment.OSVersion;
			postErrorRequest.OSLang = _sundayEnvironment.OSLang;
			postErrorRequest.ScreenResolution = _sundayEnvironment.ScreenResolution;
			postErrorRequest.Cpu = _sundayEnvironment.Cpu;
			postErrorRequest.Ram = _sundayEnvironment.Ram;
			postErrorRequest.Mac = _sundayEnvironment.Mac;
			postErrorRequest.DeviceGraph = _sundayEnvironment.DeviceGraph;
			postErrorRequest.DeviceId = (string.IsNullOrWhiteSpace(SundayContext.DeviceId) ? _sundayEnvironment.DeviceId : SundayContext.DeviceId);
			postErrorRequest.DeviceName = SundayContext.DeviceName;
			postErrorRequest.ErrorKey = businessKey;
			postErrorRequest.ErrorSubKey = subKey;
			postErrorRequest.Stack = stackTrace;
			postErrorRequest.Context = context;
			postErrorRequest.Type = errorType;
			postErrorRequest.Init(SundayContext.SundayAppId, SundayContext.SundayAppSecret, needUploadLog ? ("https://sunday.cvte.com/bugly/api/v1/log?appId=" + SundayContext.SundayAppId) : ("https://sunday.cvte.com/bugly/api/v1/bug?appId=" + SundayContext.SundayAppSecret));
			return postErrorRequest;
		}
	}
}
