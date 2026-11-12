using System;
using Cvte.Paint.Framework;

namespace Cvte.Paint.Inks
{
	public class OperationSynchronousEventArgs : EventArgs
	{
		public Slide AttachedSlide { get; set; }

		[Obsolete("耦合EasiNet业务")]
		public Action<object> LocalOperation { get; set; }
	}
}
