using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public class HttpUri : Uri
	{
		public HttpUri()
		{
		}

		public HttpUri(string value)
		{
			Value = value;
		}

		public override string Encode()
		{
			return Value;
		}
	}
}
