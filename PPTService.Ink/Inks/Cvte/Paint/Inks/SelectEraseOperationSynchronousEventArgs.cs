using System.Collections.Generic;

namespace Cvte.Paint.Inks
{
	public class SelectEraseOperationSynchronousEventArgs : OperationSynchronousEventArgs
	{
		public List<string> DeletedInkIdList { get; set; }
	}
}
