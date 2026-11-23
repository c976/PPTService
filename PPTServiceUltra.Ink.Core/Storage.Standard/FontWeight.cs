using System.Runtime.CompilerServices;
using System.Windows;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public class FontWeight : EnumType
	{
		public static implicit operator FontWeight(System.Windows.FontWeight fontWeight)
		{
			return Convert(fontWeight);
		}

		public static implicit operator System.Windows.FontWeight(FontWeight fontWeight)
		{
			return ConvertBack(fontWeight);
		}

		private static FontWeight Convert(System.Windows.FontWeight fontWeight)
		{
			FontWeight fontWeight2 = new FontWeight
			{
				Value = "Normal"
			};
			if (fontWeight == FontWeights.Normal)
			{
				fontWeight2.Value = "Normal";
			}
			if (fontWeight == FontWeights.Bold)
			{
				fontWeight2.Value = "Bold";
			}
			if (fontWeight == FontWeights.Thin)
			{
				fontWeight2.Value = "Thin";
			}
			return fontWeight2;
		}

		private static System.Windows.FontWeight ConvertBack(FontWeight fontWeight)
		{
			if (fontWeight == null)
			{
				return FontWeights.Normal;
			}
			switch (fontWeight.Value)
			{
			case "Normal":
				return FontWeights.Normal;
			case "Bold":
				return FontWeights.Bold;
			case "Thin":
				return FontWeights.Thin;
			default:
				return FontWeights.Normal;
			}
		}
	}
}
