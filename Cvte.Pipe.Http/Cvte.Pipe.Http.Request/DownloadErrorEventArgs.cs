using System;
using System.Runtime.CompilerServices;

namespace Cvte.Pipe.Http.Request
{
	public class DownloadErrorEventArgs : EventArgs
	{
		private Exception _0001;

		public Exception Error
		{
			get
			{
				return _0001;
			}
			set
			{
				_0001 = value;
			}
		}
	}
}
