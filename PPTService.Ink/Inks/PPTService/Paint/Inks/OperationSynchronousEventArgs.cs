using System;
using PPTService.Paint.Framework;

namespace PPTService.Paint.Inks
{
	public class OperationSynchronousEventArgs : EventArgs
	{
		public Slide AttachedSlide { get; set; }

		[Obsolete("耦合EasiNet业务")]
		public Action<object> LocalOperation { get; set; }
	}
}
