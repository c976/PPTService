using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.Media;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	internal static class InnerTextContext
	{
		public static readonly TextDecoration Strikethrough;

		public static readonly TextDecoration Underline;

		public static readonly TextDecoration Baseline;

		public static readonly TextDecoration OverLine;

		public const string BaselineText = "Baseline";

		public const string OverLineText = "OverLine";

		public const string UnderlineText = "Underline";

		public const string StrikethroughText = "Strikethrough";

		internal static readonly string[] ImageSupportedType;

		internal const double MaxWidth = 32000.0;

		internal const double DefaultFontSize = 40.0;

		public const char NotChar = '\ufffe';

		internal static readonly Brush DefaultForeground;

		internal const double EmptyLineSelectionSize = 10.0;

		internal const double LineSpaceRatio = 0.8;

		internal const double FontSizeChangeStep = 1.0;

		internal const double IndentChangeStep = 24.0;

		internal const double UnderlineHeightRatio = 0.06;

		internal static readonly Brush CaretDragOverBrush;

		internal const int UnderlineLocationOffsetRatio = 8;

		internal static readonly TextChar DefaultTextChar;

		internal const string DefaultText = "1";

		internal static readonly List<PropertyType> AffectArrangePropertyTypes;

		internal const double RenderEpsilon = 0.001;

		internal static Brush HyperlinkBrush => BrushCreator.GetOrCreate("#4183d9");

		internal static Brush HyperlinkHitBrush => BrushCreator.GetOrCreate("#a047cc");

		static InnerTextContext()
		{
			ImageSupportedType = new string[5] { ".png", ".bmp", ".jpg", ".jpeg", ".gif" };
			DefaultForeground = Brushes.Black;
			CaretDragOverBrush = Brushes.DarkGray;
			DefaultTextChar = new TextChar("1");
			AffectArrangePropertyTypes = new List<PropertyType>
			{
				PropertyType.FontFamily,
				PropertyType.FontSize,
				PropertyType.FontStyle,
				PropertyType.FontVariants,
				PropertyType.FontWeight,
				PropertyType.RunProperty,
				PropertyType.All
			};
			Strikethrough = TextDecorations.Strikethrough[0];
			Underline = TextDecorations.Underline[0];
			Baseline = TextDecorations.Baseline[0];
			OverLine = TextDecorations.OverLine[0];
		}
	}
}
