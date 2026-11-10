using System;
using System.Runtime.CompilerServices;

namespace Cvte.Pipe.Http.Request
{
	public class DownloadProgressChangedEventArgs : EventArgs
	{
		private long _0001;

		private long _0002;

		public long CurrentProgress
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

		public long TotalProgress
		{
			get
			{
				return _0002;
			}
			set
			{
				_0002 = value;
			}
		}
	}
}
