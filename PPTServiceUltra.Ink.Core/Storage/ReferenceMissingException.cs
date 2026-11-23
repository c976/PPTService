using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public class ReferenceMissingException : ReferenceException
	{
		private readonly string _referenceId;

		public override string Message => "无法找到Id=" + _referenceId + "的资源。" + ExtendMessage;

		public string ExtendMessage { get; set; } = string.Empty;

		public ReferenceMissingException(string referenceId)
		{
			_referenceId = referenceId;
		}
	}
}
