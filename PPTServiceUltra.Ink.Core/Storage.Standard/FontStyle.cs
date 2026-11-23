using System.Runtime.CompilerServices;
using System.Windows;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public class FontStyle : EnumType
	{
		public static implicit operator FontStyle(System.Windows.FontStyle fontStyle)
		{
			return Convert(fontStyle);
		}

		public static implicit operator System.Windows.FontStyle(FontStyle fontStyle)
		{
			return ConvertBack(fontStyle);
		}

		private static FontStyle Convert(System.Windows.FontStyle fontStyle)
		{
			string value = string.Empty;
			if (fontStyle == FontStyles.Normal)
			{
				value = "Normal";
			}
			else if (fontStyle == FontStyles.Italic)
			{
				value = "Italic";
			}
			else if (fontStyle == FontStyles.Oblique)
			{
				value = "Oblique";
			}
			return new FontStyle
			{
				Value = value
			};
		}

		private static System.Windows.FontStyle ConvertBack(FontStyle fontStyle)
		{
			if (fontStyle == null)
			{
				return FontStyles.Normal;
			}
			switch (fontStyle.Value)
			{
			case "Normal":
				return FontStyles.Normal;
			case "Italic":
				return FontStyles.Italic;
			case "Oblique":
				return FontStyles.Oblique;
			default:
				return FontStyles.Normal;
			}
		}
	}
}
