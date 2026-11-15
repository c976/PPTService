using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public class TimeSpanArray : ArrayType<TimeSpan>, IStorableType
	{
		public TimeSpanArray()
		{
		}

		public TimeSpanArray(IEnumerable<TimeSpan> items)
			: base(items)
		{
		}

		public static implicit operator TimeSpanArray(List<System.TimeSpan> timeSpans)
		{
			return new TimeSpanArray(((IEnumerable<System.TimeSpan>)timeSpans).Select((Func<System.TimeSpan, TimeSpan>)( (System.TimeSpan p) => p)));
		}

		public static implicit operator List<System.TimeSpan>(TimeSpanArray timeSpans)
		{
			return ((IEnumerable<TimeSpan>)timeSpans.Items).Select((Func<TimeSpan, System.TimeSpan>)( (TimeSpan p) => p)).ToList();
		}
	}
}
