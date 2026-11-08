using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Feedback
{
	
	
	[Request("/issue?authId={authId}&timestamp={timestamp}")]
	[DataContract]
	internal class CommitFeedbackRequest : FeedbackReqeustBase
	{
		[DataMember(Name = "appId")]
		public string AppId { get; private set; }

		[DataMember(Name = "productId")]
		public string ProductId { get; set; }

		[DataMember(Name = "title")]
		public string Title { get; private set; }

		[DataMember(Name = "type")]
		public int Type { get; private set; }

		[DataMember(Name = "channel")]
		public int Channel { get; private set; }

		[DataMember(Name = "appName")]
		public string AppName { get; set; }

		[DataMember(Name = "userId")]
		public string UserId { get; set; }

		[DataMember(Name = "email")]
		public string Email { get; set; }

		[DataMember(Name = "wx")]
		public string Wenxin { get; set; }

		[DataMember(Name = "mobile")]
		public string Mobile { get; set; }

		[DataMember(Name = "device")]
		public string Device { get; set; }

		[DataMember(Name = "content")]
		public string Content { get; private set; }

		[DataMember(Name = "pics")]
		public string Pictures { get; set; }

		[DataMember(Name = "attachments")]
		public string Attachments { get; set; }

		[DataMember(Name = "version")]
		public string Version { get; set; }

		[DataMember(Name = "platform")]
		public string Platform { get; set; }

		[DataMember(Name = "softwareInfo")]
		public string SoftwareInfo { get; set; }

		[DataMember(Name = "environmentInfo")]
		public string EnvironmentInfo { get; set; }

		public CommitFeedbackRequest(string appId, string title, string content, FeedbackType type, FeedbackChannel channel)
		{
			AppId = appId;
			Title = title;
			Type = (int)type;
			Channel = (int)channel;
			Content = content;
		}
	}
}
