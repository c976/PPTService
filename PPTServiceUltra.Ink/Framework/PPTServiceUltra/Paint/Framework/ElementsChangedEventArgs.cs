using System;
using System.Collections.Generic;
using System.Linq;
using PPTServiceUltra.Paint.Framework.Operations;

namespace PPTServiceUltra.Paint.Framework
{
	public class ElementsChangedEventArgs : EventArgs
	{
		public ICollection<ElementDelta> DeltaCollection { get; private set; }

		public Slide Slide { get; private set; }

		public bool IsFinalState { get; private set; }

		public ElementsChangedEventArgs(IEnumerable<ElementDelta> deltaCollection, Slide slide, bool isFinalState = true)
		{
			DeltaCollection = deltaCollection.ToList();
			Slide = slide;
			IsFinalState = isFinalState;
		}
	}
}
