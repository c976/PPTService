using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public class UnknowDocumentVersionException : DocumentException
	{
		public string Version { get; set; }
	}
}
