using System.Collections.Generic;

namespace PPTService.Paint.Inks
{
	public class SelectEraseOperationSynchronousEventArgs : OperationSynchronousEventArgs
	{
		public List<string> DeletedInkIdList { get; set; }
	}
}
