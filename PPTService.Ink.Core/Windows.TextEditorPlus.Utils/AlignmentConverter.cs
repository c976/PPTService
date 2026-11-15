using System;
using System.Windows;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	public static class AlignmentConverter
	{
		public static TextVerticalAlignment ToTextVerticalAlignment(this VerticalAlignment alignment)
		{
			TextVerticalAlignment textVerticalAlignment = TextVerticalAlignment.Top;
			switch (alignment)
			{
			case VerticalAlignment.Top:
				return TextVerticalAlignment.Top;
			case VerticalAlignment.Center:
				return TextVerticalAlignment.Center;
			case VerticalAlignment.Bottom:
				return TextVerticalAlignment.Bottom;
			case VerticalAlignment.Stretch:
				return TextVerticalAlignment.Stretch;
			default:
				throw new ArgumentOutOfRangeException("alignment", alignment, null);
			}
		}

		public static VerticalAlignment ToVerticalAlignment(this TextVerticalAlignment alignment)
		{
			VerticalAlignment result = VerticalAlignment.Top;
			switch (alignment)
			{
			case TextVerticalAlignment.Top:
				result = VerticalAlignment.Top;
				break;
			case TextVerticalAlignment.Center:
				result = VerticalAlignment.Center;
				break;
			case TextVerticalAlignment.Bottom:
				result = VerticalAlignment.Bottom;
				break;
			case TextVerticalAlignment.Stretch:
				result = VerticalAlignment.Stretch;
				break;
			default:
				throw new ArgumentOutOfRangeException("alignment", alignment, null);
			case TextVerticalAlignment.Disabled:
				break;
			}
			return result;
		}

		public static TextHorizontalAlignment ToTextHorizontalAlignment(this TextAlignment alignment)
		{
			TextHorizontalAlignment textHorizontalAlignment = TextHorizontalAlignment.Left;
			switch (alignment)
			{
			case TextAlignment.Left:
				return TextHorizontalAlignment.Left;
			case TextAlignment.Right:
				return TextHorizontalAlignment.Right;
			case TextAlignment.Center:
				return TextHorizontalAlignment.Center;
			case TextAlignment.Justify:
				return TextHorizontalAlignment.Justify;
			default:
				throw new ArgumentOutOfRangeException("alignment", alignment, null);
			}
		}

		public static TextAlignment ToTextAlignment(this TextHorizontalAlignment alignment)
		{
			TextAlignment result = TextAlignment.Left;
			switch (alignment)
			{
			case TextHorizontalAlignment.Left:
				result = TextAlignment.Left;
				break;
			case TextHorizontalAlignment.Right:
				result = TextAlignment.Right;
				break;
			case TextHorizontalAlignment.Center:
				result = TextAlignment.Center;
				break;
			case TextHorizontalAlignment.Justify:
				result = TextAlignment.Justify;
				break;
			default:
				throw new ArgumentOutOfRangeException("alignment", alignment, null);
			case TextHorizontalAlignment.Disabled:
				break;
			}
			return result;
		}
	}
}
