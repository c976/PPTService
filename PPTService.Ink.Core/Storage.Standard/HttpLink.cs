using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	[ActionType("Http")]
	public class HttpLink : Hyperlink
	{
		[ActionParameter("url")]
		public string EscapedUrl
		{
			get
			{
				return System.Uri.EscapeDataString(Url);
			}
			set
			{
				Url = System.Uri.UnescapeDataString(value);
			}
		}

		public string Url { get; set; }

		public HttpLink()
		{
		}

		public HttpLink(string url)
		{
			Url = url;
		}
	}
}
