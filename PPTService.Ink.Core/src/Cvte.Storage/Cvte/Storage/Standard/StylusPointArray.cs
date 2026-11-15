using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public class StylusPointArray : ArrayType<StylusPoint>, IStorableType
	{
		public StylusPointArray()
		{
		}

		public StylusPointArray(IEnumerable<StylusPoint> items)
			: base(items)
		{
		}

		public static implicit operator StylusPointArray(StylusPointCollection points)
		{
			return new StylusPointArray(((IEnumerable<System.Windows.Input.StylusPoint>)points).Select((Func<System.Windows.Input.StylusPoint, StylusPoint>)( (System.Windows.Input.StylusPoint p) => p)));
		}

		public static implicit operator StylusPointCollection(StylusPointArray points)
		{
			if (points != null)
			{
				return new StylusPointCollection(((IEnumerable<StylusPoint>)points.Items).Select((Func<StylusPoint, System.Windows.Input.StylusPoint>)( (StylusPoint p) => p)));
			}
			return new StylusPointCollection();
		}
	}
}
