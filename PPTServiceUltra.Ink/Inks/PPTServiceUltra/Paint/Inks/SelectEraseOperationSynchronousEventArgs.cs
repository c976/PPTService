using System.Collections.Generic;

namespace PPTServiceUltra.Paint.Inks
{
	public class SelectEraseOperationSynchronousEventArgs : OperationSynchronousEventArgs
	{
		public List<string> DeletedInkIdList { get; set; }
	}
}
