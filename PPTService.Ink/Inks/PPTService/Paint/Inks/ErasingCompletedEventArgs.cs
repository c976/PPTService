using System;

namespace PPTService.Paint.Inks
{
	public class ErasingCompletedEventArgs : EventArgs
	{
		public bool IsCanceled { get; private set; }

		public ErasingCompletedEventArgs(bool isCanceled)
		{
			IsCanceled = isCanceled;
		}
	}
}
