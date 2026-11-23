using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public class Base64Uri : Uri
	{
		public Base64Uri()
		{
		}

		public Base64Uri(string value)
		{
			Value = value;
		}

		public override string Encode()
		{
			return Value;
		}
	}
}
