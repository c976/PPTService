using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Account.CVTouch
{
	
	
	[DataContract]
	public class GenerateCheckCodeHttpResponse : AccountHttpResponse
	{
		internal string SessionId
		{
			get
			{
				if (Cookies == null)
				{
					return "";
				}
				return Cookies["sid"]?.Value ?? "";
			}
		}

		public string ImageUrl { get; internal set; }
	}
}
