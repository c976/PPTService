using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Cvte.Paint.Inks
{
	public class StrokePointCollection : ReadOnlyCollection<StrokePointDetails>
	{
		public StrokePointCollection()
			: base((IList<StrokePointDetails>)new List<StrokePointDetails>())
		{
		}

		public void Add(StylusPoint point)
		{
			Items.Add(new StrokePointDetails(point));
		}

		internal void Add(StrokePointDetails point)
		{
			Items.Add(point);
		}
	}
}
