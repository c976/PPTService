using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class FeedbackProperty
	{
		public double? FontSize { get; set; }

		public FontStyle FontStyle { get; set; }

		public FontWeight FontWeight { get; set; }

		public FontFamily FontFamily { get; set; }

		public Brush Foreground { get; set; }

		public FontVariants FontVariants { get; set; }

		public TextDecorationCollection TextDecoration { get; set; }

		public EffectArgsCollection TextEffects { get; set; }

		public bool IsFormatPainterEnable { get; set; }

		public double? LineSpacing { get; set; }

		public double? ParagraphBefore { get; set; }

		public double? ParagraphAfter { get; set; }

		public TextHorizontalAlignment? TextHorizontalAlignment { get; set; }

		public TextVerticalAlignment? TextVerticalAlignment { get; set; }

		public MarkerStyle? MarkerStyle { get; set; }

		public ArrangingType? ArrangingType { get; set; }

		public FlowDirection ParagraphDirection { get; set; }

		[Obsolete("拼写错的单词，请使用 IndentLevel 代替")]
		public int? IndentLeval
		{
			get
			{
				return IndentLevel;
			}
			set
			{
				IndentLevel = value;
			}
		}

		public int? IndentLevel { get; set; }

		public double? SpaceBefore { get; set; }

		public double? SpaceAfter { get; set; }

		public Brush ShadowBrush { get; set; }

		public double? ShadowAngle { get; set; }

		public double ShadowBlur { get; set; }

		public double ShadowOffset { get; set; }

		public double? ShadowOpacity { get; set; }

		public double? ReflectionOpacity { get; set; }

		public double? Opacity { get; set; }

		public FeedbackProperty()
		{
		}

		public FeedbackProperty(FeedbackProperty data)
		{
			FontSize = data.FontSize;
			FontStyle = data.FontStyle;
			FontWeight = data.FontWeight;
			FontFamily = data.FontFamily;
			FontVariants = data.FontVariants;
			TextDecoration = data.TextDecoration;
			LineSpacing = data.LineSpacing;
			TextHorizontalAlignment = data.TextHorizontalAlignment;
			MarkerStyle = data.MarkerStyle;
			TextVerticalAlignment = data.TextVerticalAlignment;
			IsFormatPainterEnable = data.IsFormatPainterEnable;
			TextEffects = data.TextEffects;
			Foreground = data.Foreground;
			ShadowBrush = data.ShadowBrush;
			ShadowAngle = data.ShadowAngle;
			ShadowBlur = data.ShadowBlur;
			ShadowOffset = data.ShadowOffset;
			ShadowOpacity = data.ShadowOpacity;
			ReflectionOpacity = data.ReflectionOpacity;
			Opacity = data.Opacity;
			ArrangingType = data.ArrangingType;
			ParagraphDirection = data.ParagraphDirection;
		}

		public void SetRunProperty(RunProperty runProperty)
		{
			FontSize = runProperty.FontSize;
			FontStyle = runProperty.FontStyle;
			FontWeight = runProperty.FontWeight;
			FontFamily = runProperty.FontFamily;
			FontVariants = runProperty.FontVariants;
			Opacity = runProperty.Opacity;
			TextDecoration = runProperty.TextDecoration;
			TextEffects = runProperty.EffectsArgs;
			Foreground = runProperty.Foreground;
			if (!runProperty.EffectsArgs.ContainsType(typeof(ShadowEffectArgs)) && !runProperty.EffectsArgs.ContainsType(typeof(PerspectiveEffectArgs)))
			{
				ShadowAngle = null;
				ShadowBlur = 0.0;
				ShadowBrush = null;
				ShadowOffset = 0.0;
				ShadowOpacity = null;
			}
			else if (runProperty.EffectsArgs.ContainsType(typeof(ShadowEffectArgs)))
			{
				ShadowEffectArgs shadowEffectArgs = runProperty.EffectsArgs.OfType<ShadowEffectArgs>().First();
				ShadowAngle = shadowEffectArgs.ShadowDirection;
				ShadowBlur = shadowEffectArgs.ShadowBlur;
				ShadowBrush = shadowEffectArgs.ShadowBrush;
				ShadowOffset = shadowEffectArgs.ShadowDistance;
				ShadowOpacity = shadowEffectArgs.ShadowOpacity;
			}
			else if (runProperty.EffectsArgs.ContainsType(typeof(PerspectiveEffectArgs)))
			{
				PerspectiveEffectArgs perspectiveEffectArgs = runProperty.EffectsArgs.OfType<PerspectiveEffectArgs>().First();
				ShadowAngle = perspectiveEffectArgs.PerspectiveAngle;
				ShadowBlur = perspectiveEffectArgs.PerspectiveBlur;
				ShadowBrush = perspectiveEffectArgs.PerspectiveBrush;
				ShadowOffset = perspectiveEffectArgs.PerspectiveDepth;
				ShadowOpacity = perspectiveEffectArgs.PerspectiveOpacity;
			}
			if (!runProperty.EffectsArgs.ContainsType(typeof(ReflectionEffectArgs)))
			{
				ReflectionOpacity = null;
				return;
			}
			ReflectionEffectArgs reflectionEffectArgs = runProperty.EffectsArgs.OfType<ReflectionEffectArgs>().First();
			ReflectionOpacity = reflectionEffectArgs.ReflectionOpacity;
		}

		public void SetParagraphProperty(ParagraphProperty paragraphProperty)
		{
			LineSpacing = paragraphProperty.LineSpacing;
			ParagraphBefore = paragraphProperty.ParagraphBefore;
			ParagraphAfter = paragraphProperty.ParagraphAfter;
			TextHorizontalAlignment = paragraphProperty.TextAlignment.ToTextHorizontalAlignment();
			MarkerStyle = paragraphProperty.MarkerProperty.ToMarkerStyle();
			ParagraphDirection = paragraphProperty.Direction;
		}
	}
}
