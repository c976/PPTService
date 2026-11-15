using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class CharChangeRoutedEventArgs : RoutedEventArgs
	{
		public bool IsFromUndoRedo { get; set; }

		public int Offset { get; }

		[Obsolete("拼写错误的属性，请使用RemovedLength代替")]
		public int RemoveLength
		{
			get
			{
				return RemovedLength;
			}
			set
			{
				RemovedLength = value;
			}
		}

		public int RemovedLength { get; set; }

		[Obsolete("拼写错误的属性，请使用InsertedLength代替")]
		public int InsertLength
		{
			get
			{
				return InsertedLength;
			}
			set
			{
				InsertedLength = value;
			}
		}

		public int InsertedLength { get; set; }

		public IReadOnlyList<ICharObject> RemoveChars { get; }

		[Obsolete("属性拼写错误，请使用 InsertedChars 代替")]
		public List<ICharObject> InsertChars => InsertedChars.ToList();

		public IReadOnlyList<ICharObject> InsertedChars { get; }

		public CharChangeRoutedEventArgs(RoutedEvent routedEvent, CharChangeEventArgs args)
			: base(routedEvent)
		{
			Offset = args.Offset;
			RemoveChars = args.RemovedChars;
			InsertedChars = args.InsertedChars;
			RemovedLength = RemoveChars.Count;
			InsertedLength = InsertedChars.Count;
			IsFromUndoRedo = args.IsFromUndoRedo;
		}

		public CharChangeRoutedEventArgs(RoutedEvent routedEvent, CharChangeEventArgs args, object source)
			: base(routedEvent, source)
		{
			Offset = args.Offset;
			RemoveChars = args.RemovedChars;
			InsertedChars = args.InsertedChars;
			RemovedLength = RemoveChars.Count;
			InsertedLength = InsertedChars.Count;
			IsFromUndoRedo = args.IsFromUndoRedo;
		}
	}
}
