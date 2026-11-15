using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	public class UnknowDocumentVersionException : DocumentException
	{
		public string Version { get; set; }
	}
}
