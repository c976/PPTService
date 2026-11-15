using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using PPTService.Ink.Core.Linq;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	internal class RenderRunProperty : RunProperty
	{
		private RunProperty OwnerRunProperty { get; }

		public RenderRunProperty(RunProperty runProperty)
		{
			RunProperty.CopyRunProperty(runProperty, this);
			ToRenderRunProperty(runProperty, this);
			OwnerRunProperty = runProperty;
			TextRunPropertyChanged += RenderRunProperty_TextRunPropertyChanged;
		}

		private void RenderRunProperty_TextRunPropertyChanged(object sender, EventArgs e)
		{
			OwnerRunProperty.InvalidateRenderRunProperty();
		}

		private static RenderRunProperty ToRenderRunProperty(RunProperty sourceProperty, RenderRunProperty renderRunProperty)
		{
			if (sourceProperty.Hyperlink != null)
			{
				renderRunProperty.TextDecoration = TextDecorations.Underline;
				renderRunProperty.Foreground = (renderRunProperty.IsHyperlinkHit ? InnerTextContext.HyperlinkHitBrush : InnerTextContext.HyperlinkBrush);
			}
			else if (renderRunProperty.EffectsArgs.Any())
			{
				renderRunProperty.EffectsArgs.ForEach((ITextEffectArgs x) =>
				{
					x.Opacity = renderRunProperty.Opacity;
				});
				FillEffectArgs fillEffectArgs = renderRunProperty.EffectsArgs.OfType<FillEffectArgs>().FirstOrDefault();
				if (fillEffectArgs != null)
				{
					Brush fillBrush = fillEffectArgs.FillBrush;
					if (fillBrush != null)
					{
						if (!fillBrush.IsFrozen)
						{
							fillBrush.Opacity = renderRunProperty.Opacity;
						}
						renderRunProperty.Foreground = fillBrush.Clone();
					}
				}
				_003CToRenderRunProperty_003Eg__ChangeOpacity_007C5_0(renderRunProperty);
			}
			else
			{
				_003CToRenderRunProperty_003Eg__ChangeOpacity_007C5_0(renderRunProperty);
			}
			return renderRunProperty;
		}

		[CompilerGenerated]
		internal static void _003CToRenderRunProperty_003Eg__ChangeOpacity_007C5_0(RunProperty rp)
		{
			if (!Nearly.Equals(rp.Opacity, 1.0) || !Nearly.Equals(rp.Foreground.Opacity, 1.0))
			{
				Brush brush = rp.Foreground.CloneCurrentValue();
				if (!brush.IsFrozen)
				{
					brush.Opacity = rp.Opacity;
				}
				rp.Foreground = brush;
			}
		}
	}
}
