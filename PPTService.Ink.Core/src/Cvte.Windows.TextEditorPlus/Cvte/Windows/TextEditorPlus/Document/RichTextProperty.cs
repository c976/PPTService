using System.Collections.Generic;
using System.Windows;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class RichTextProperty
	{
		public Dictionary<int, List<LineProperty>> ParagraphLineInfo = new Dictionary<int, List<LineProperty>>();

		public SizeToContent SizeToContent { get; set; }

		public ArrangingType ArrangingType { get; set; }

		public VerticalAlignment VerticalTextAlignment { get; set; }

		public List<ParagraphProperty> ParagraphProperties { get; set; } = new List<ParagraphProperty>();

		public List<ICharObject> CharObjects { get; set; } = new List<ICharObject>();

		public List<RunProperty> RunProperties { get; set; } = new List<RunProperty>();
	}
}
