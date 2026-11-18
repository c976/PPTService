using System;
using System.Runtime.CompilerServices;

namespace PPTService.Core.Pipe.Http.Request
{
	public class DownloadErrorEventArgs : EventArgs
	{
		private Exception exception_0;

		public Exception Error
		{
			get
			{
				return exception_0;
			}
			set
			{
				exception_0 = value;
			}
		}
	}
}
