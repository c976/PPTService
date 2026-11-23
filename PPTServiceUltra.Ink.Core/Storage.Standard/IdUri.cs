using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public class IdUri : Uri
	{
		public IdUri()
		{
		}

		public IdUri(string value)
		{
			Value = value.Replace("id://", "");
		}

		public override string Encode()
		{
			return "id://" + Value;
		}
	}
}
