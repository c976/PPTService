using System;
using System.Runtime.CompilerServices;

namespace Cvte.Pipe.Http.Request
{
	public class DownloadProgressChangedEventArgs : EventArgs
	{
		private long long_0;

		private long long_1;

		public long CurrentProgress
		{
			get
			{
				return long_0;
			}
			set
			{
				long_0 = value;
			}
		}

		public long TotalProgress
		{
			get
			{
				return long_1;
			}
			set
			{
				long_1 = value;
			}
		}
	}
}
