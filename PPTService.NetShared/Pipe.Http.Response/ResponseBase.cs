using System.Runtime.Serialization;

namespace PPTService.Core.Pipe.Http.Response
{
	[DataContract]
	public abstract class ResponseBase
	{
		public ErrorCode ErrorCode { get; set; }

		public bool IsBadRequest
		{
			get
			{
				if (ErrorCode == ErrorCode.Ok)
				{
					return Html == null;
				}
				return true;
			}
		}

		public abstract bool Succeed { get; }

		public string Html { get; protected set; }

		public virtual int? MonitorCode => null;

		public abstract void ParseHtml(string html);
	}
}
