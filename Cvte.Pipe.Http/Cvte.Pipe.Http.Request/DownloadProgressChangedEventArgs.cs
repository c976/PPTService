using System;
using System.Runtime.CompilerServices;

namespace Cvte.Pipe.Http.Request
{
	public class DownloadProgressChangedEventArgs : EventArgs
	{
		[CompilerGenerated]
		private long _0001;

		[CompilerGenerated]
		private long _0002;

		public long CurrentProgress
		{
			[CompilerGenerated]
			get
			{
				return _0001;
			}
			[CompilerGenerated]
			set
			{
				_0001 = value;
			}
		}

		public long TotalProgress
		{
			[CompilerGenerated]
			get
			{
				return _0002;
			}
			[CompilerGenerated]
			set
			{
				_0002 = value;
			}
		}
	}
}
