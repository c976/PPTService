using System;
using System.Collections.Generic;
using System.Linq;

namespace Cvte.Paint.Framework.Controls.Texts
{
	public class InternalTextEditorCollectionChangedEventArgs : EventArgs
	{
		public IReadOnlyList<PaintTextEditor> AddedEditors { get; }

		public IReadOnlyList<PaintTextEditor> RemovedEditors { get; }

		public InternalTextEditorCollectionChangedEventArgs(IEnumerable<PaintTextEditor> addedEditors, IEnumerable<PaintTextEditor> removedEditors)
		{
			if (addedEditors == null)
			{
				throw new ArgumentNullException("addedEditors");
			}
			if (removedEditors == null)
			{
				throw new ArgumentNullException("removedEditors");
			}
			AddedEditors = addedEditors.ToList();
			RemovedEditors = removedEditors.ToList();
		}
	}
}
