using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public class AppUri : Uri
	{
		public AppUri()
		{
		}

		public AppUri(string value)
		{
			Value = UriUtils.RemovePrefix(value, "app://");
		}

		public override string Encode()
		{
			return "app://" + Value;
		}
	}
}
