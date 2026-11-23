using System;
using System.Collections.Generic;
using System.Linq;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public class CharChangeEventArgs : EventArgs
	{
		public bool IsFromUndoRedo { get; set; }

		public int Offset { get; }

		public int RemoveLength { get; }

		public int InsertLength { get; }

		[Obsolete("拼写不对，请使用 RemovedChars 代替")]
		public List<ICharObject> RemoveChars => RemovedChars.ToList();

		public IReadOnlyList<ICharObject> RemovedChars { get; }

		[Obsolete("拼写不对，请使用 InsertedChars 代替")]
		public List<ICharObject> InsertChars => InsertedChars.ToList();

		public IReadOnlyList<ICharObject> InsertedChars { get; }

		internal CharChangeEventArgs(int offset, IReadOnlyCollection<ICharObject> removeChars, IReadOnlyList<ICharObject> insertChars)
		{
			Offset = offset;
			RemovedChars = removeChars.ToList();
			InsertedChars = insertChars;
			RemoveLength = removeChars.Count;
			InsertLength = insertChars.Count;
		}
	}
}
