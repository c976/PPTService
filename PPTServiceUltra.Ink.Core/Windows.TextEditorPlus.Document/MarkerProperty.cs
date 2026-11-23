using System.Runtime.CompilerServices;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Ink.Core.Storage.Standard;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public class MarkerProperty
	{
		public AutoNumberProperty AutoNumber { get; set; }

		public ICharObject MarkerChar { get; set; }

		public System.Windows.Media.FontFamily FontFamily { get; set; }

		[CanBeNull]
		public System.Windows.Media.Brush Foreground { get; set; }

		public bool IsEmpty()
		{
			if (MarkerChar == null)
			{
				if (AutoNumber != null)
				{
					return AutoNumber.AutoNumberType == AutoNumberType.Nil;
				}
				return true;
			}
			return false;
		}

		public void Clear()
		{
			MarkerChar = null;
			AutoNumber = null;
		}

		public MarkerProperty Clone()
		{
			MarkerProperty markerProperty = new MarkerProperty
			{
				AutoNumber = AutoNumber?.Clone(),
				MarkerChar = (ICharObject)(MarkerChar?.Clone())
			};
			if (FontFamily != null)
			{
				markerProperty.FontFamily = (PPTServiceUltra.Ink.Core.Storage.Standard.FontFamily)FontFamily;
			}
			else
			{
				markerProperty.FontFamily = FontFamily;
			}
			return markerProperty;
		}

		public override bool Equals(object o)
		{
			MarkerProperty markerProperty = o as MarkerProperty;
			if (markerProperty == null)
			{
				return false;
			}
			if (!Converter.AreEquals(FontFamily, markerProperty.FontFamily))
			{
				return false;
			}
			if (!Converter.AreEquals(MarkerChar, markerProperty.MarkerChar))
			{
				return false;
			}
			if (!Converter.AreEquals(AutoNumber, markerProperty.AutoNumber))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}
	}
}
