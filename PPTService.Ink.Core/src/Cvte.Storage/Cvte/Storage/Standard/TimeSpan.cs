using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public class TimeSpan : SplitStringEncodedType<long>
	{
		public long Ticks { get; set; }

		public static implicit operator TimeSpan(System.TimeSpan timeSpan)
		{
			return new TimeSpan
			{
				Ticks = timeSpan.Ticks
			};
		}

		public static implicit operator System.TimeSpan(TimeSpan timeSpan)
		{
			if (timeSpan == null)
			{
				return System.TimeSpan.Zero;
			}
			return new System.TimeSpan(timeSpan.Ticks);
		}

		protected override long[] GetParams()
		{
			return new long[1] { Ticks };
		}

		protected override void SetParams(params long[] values)
		{
			Ticks = values[0];
		}
	}
}
