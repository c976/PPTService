using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public class TextDecorationCollection : SplitStringEncodedType<string>
	{
		private const string Baseline = "Baseline";

		private const string OverLine = "OverLine";

		private const string Underline = "Underline";

		private const string Strikethrough = "Strikethrough";

		private List<string> _innerCollection;

		public static implicit operator TextDecorationCollection(System.Windows.TextDecorationCollection value)
		{
			return new TextDecorationCollection
			{
				_innerCollection = ToTextDecorationCollection(value)
			};
		}

		public static implicit operator System.Windows.TextDecorationCollection(TextDecorationCollection value)
		{
			return ToSystemTextDecorationCollection(value._innerCollection);
		}

		private static List<string> ToTextDecorationCollection(System.Windows.TextDecorationCollection value)
		{
			if (value == null || value.Count < 1)
			{
				return new List<string>();
			}
			List<string> list = new List<string>();
			foreach (TextDecoration item in value)
			{
				switch (item.Location)
				{
				case TextDecorationLocation.Baseline:
					list.Add("Baseline");
					break;
				case TextDecorationLocation.OverLine:
					list.Add("OverLine");
					break;
				case TextDecorationLocation.Underline:
					list.Add("Underline");
					break;
				case TextDecorationLocation.Strikethrough:
					list.Add("Strikethrough");
					break;
				}
			}
			return list;
		}

		private static System.Windows.TextDecorationCollection ToSystemTextDecorationCollection(List<string> value)
		{
			System.Windows.TextDecorationCollection textDecorationCollection = new System.Windows.TextDecorationCollection();
			if (value == null)
			{
				return textDecorationCollection;
			}
			foreach (string item in value)
			{
				switch (item)
				{
				case "Baseline":
					textDecorationCollection.Add(TextDecorations.Baseline);
					break;
				case "OverLine":
					textDecorationCollection.Add(TextDecorations.OverLine);
					break;
				case "Underline":
					textDecorationCollection.Add(TextDecorations.Underline);
					break;
				case "Strikethrough":
					textDecorationCollection.Add(TextDecorations.Strikethrough);
					break;
				}
			}
			return textDecorationCollection;
		}

		public bool IsNullOrEmpty()
		{
			if (_innerCollection != null)
			{
				return _innerCollection.Count < 1;
			}
			return true;
		}

		protected override string[] GetParams()
		{
			if (_innerCollection == null || _innerCollection.Count < 1)
			{
				return new string[0];
			}
			return _innerCollection.ToArray();
		}

		protected override void SetParams(params string[] values)
		{
			_innerCollection = values?.ToList() ?? new List<string>();
		}
	}
}
