using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTService.Ink.Core.Windows.TextEditorPlus
{
	internal class MongolianTextRunProperties : TextRunProperties
	{
		public override Typeface Typeface => SetTypeface;

		public Typeface SetTypeface { get; set; }

		public TextRunTypographyProperties SetTypographyProperties { get; set; }

		public override TextEffectCollection TextEffects => null;

		public override TextRunTypographyProperties TypographyProperties => SetTypographyProperties;

		public override double FontRenderingEmSize => SetFontRenderingEmSize;

		public override double FontHintingEmSize => 0.0;

		public override TextDecorationCollection TextDecorations => SetTextDecorations;

		public override Brush ForegroundBrush => SetForegroundBrush;

		public override Brush BackgroundBrush => SetBackgroundBrush;

		public override CultureInfo CultureInfo => CultureInfo.CurrentCulture;

		public double SetFontRenderingEmSize { get; set; }

		public Brush SetForegroundBrush { get; set; }

		public Brush SetBackgroundBrush { get; set; }

		public TextDecorationCollection SetTextDecorations { get; set; }

		public BaselineAlignment SetBaselineAlignment { get; set; } = BaselineAlignment.Baseline;

		internal MongolianTextRunProperties(RunProperty prop)
		{
			SetFontRenderingEmSize = prop.FontSize;
			SetBackgroundBrush = prop.Background;
			SetForegroundBrush = prop.Foreground;
			SetTypeface = new Typeface(prop.FontFamily, prop.FontStyle, prop.FontWeight, FontStretches.Normal);
			SetTypographyProperties = new MongolianTextRunTypographyProperties
			{
				SetVariants = prop.FontVariants
			};
			SetBaselineAlignment = prop.BaselineAlignment;
		}
	}
}
