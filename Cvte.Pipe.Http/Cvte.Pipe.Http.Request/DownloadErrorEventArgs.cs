using System;
using System.Runtime.CompilerServices;

namespace Cvte.Pipe.Http.Request
{
	public class DownloadErrorEventArgs : EventArgs
	{
		[CompilerGenerated]
		private Exception _0001;

		public Exception Error
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
	}
}
