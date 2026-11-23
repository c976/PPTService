using System.Collections.Generic;
using System.Windows;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils.Paste
{
	internal class DataObjectContext
	{
		private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();

		public RichTextProperty RichTextProperty { get; set; }

		public DataObjectAdapter DataObject { get; }

		public string Html => TryGetString(DataFormats.Html);

		public string Rtf => TryGetString(DataFormats.Rtf);

		public string UnicodeText => TryGetString(DataFormats.UnicodeText);

		public string Text => TryGetString(DataFormats.Text);

		public string SaveInfoString => TryGetString("CoreLibWindowsTextEditor");

		public DataObjectContext(DataObjectAdapter dataObject)
		{
			DataObject = dataObject;
		}

		private string TryGetString(string dataFormat)
		{
			if (_cache.ContainsKey(dataFormat))
			{
				return _cache[dataFormat];
			}
			DataObjectAdapter dataObject = DataObject;
			string text = null;
			if (dataObject.GetDataPresent(dataFormat))
			{
				text = (string)dataObject.GetData(dataFormat);
			}
			_cache[dataFormat] = text;
			return text;
		}
	}
}
