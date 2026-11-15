using System;
using PPTService.Ink.Core.ComponentModel;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Storage
{
	internal static class EffectSaveInfoExtension
	{
		public static ITextEffectArgs ToEffectArgs(this EffectSaveInfo effectSaveInfo)
		{
			FillEffectSaveInfo fillEffectSaveInfo = effectSaveInfo as FillEffectSaveInfo;
			if (fillEffectSaveInfo == null)
			{
				FrameEffectSaveInfo frameEffectSaveInfo = effectSaveInfo as FrameEffectSaveInfo;
				if (frameEffectSaveInfo == null)
				{
					HaloEffectSaveInfo haloEffectSaveInfo = effectSaveInfo as HaloEffectSaveInfo;
					if (haloEffectSaveInfo == null)
					{
						ShadowEffectSaveInfo shadowEffectSaveInfo = effectSaveInfo as ShadowEffectSaveInfo;
						if (shadowEffectSaveInfo == null)
						{
							ReflectionEffectSaveInfo reflectionEffectSaveInfo = effectSaveInfo as ReflectionEffectSaveInfo;
							if (reflectionEffectSaveInfo == null)
							{
								PerspectiveEffectSaveInfo perspectiveEffectSaveInfo = effectSaveInfo as PerspectiveEffectSaveInfo;
								if (perspectiveEffectSaveInfo != null)
								{
									if (perspectiveEffectSaveInfo.Opacity < 0.0)
									{
										perspectiveEffectSaveInfo.Opacity = 0.0;
									}
									if (perspectiveEffectSaveInfo.Opacity > 1.0)
									{
										perspectiveEffectSaveInfo.Opacity = 1.0;
									}
									if (perspectiveEffectSaveInfo.Blur < 0.0)
									{
										perspectiveEffectSaveInfo.Blur = 0.0;
									}
									if (perspectiveEffectSaveInfo.Depth < 0.0)
									{
										perspectiveEffectSaveInfo.Depth = 0.0;
									}
									if (perspectiveEffectSaveInfo.Depth > 1.0)
									{
										perspectiveEffectSaveInfo.Depth = 1.0;
									}
									return new PerspectiveEffectArgs
									{
										PerspectiveBrush = perspectiveEffectSaveInfo.Brush,
										PerspectiveAngle = 30.0,
										PerspectiveBlur = perspectiveEffectSaveInfo.Blur,
										PerspectiveDepth = perspectiveEffectSaveInfo.Depth,
										PerspectiveOpacity = perspectiveEffectSaveInfo.Opacity
									};
								}
								if (DebuggingProperties.IsDebug)
								{
									throw new ArgumentException("存在文本不认识的特效，仅调试下抛出，请找德熙");
								}
								return new FillEffectArgs();
							}
							return new ReflectionEffectArgs
							{
								ReflectionDepth = reflectionEffectSaveInfo.Depth.CoerceValue(0.0, 1.0),
								ReflectionDistance = reflectionEffectSaveInfo.Distance.CoerceValue(0.0, double.MaxValue),
								ReflectionOpacity = reflectionEffectSaveInfo.Opacity.CoerceValue(0.0, 1.0)
							};
						}
						return new ShadowEffectArgs
						{
							ShadowBrush = shadowEffectSaveInfo.Brush,
							ShadowBlur = shadowEffectSaveInfo.Blur.CoerceValue(0.0, 200.0),
							ShadowDirection = shadowEffectSaveInfo.Direction.CoerceValue(0.0, 450.0),
							ShadowDistance = shadowEffectSaveInfo.Distance.CoerceValue(0.0, 200.0),
							ShadowOpacity = shadowEffectSaveInfo.Opacity.CoerceValue(0.0, 1.0)
						};
					}
					double num = haloEffectSaveInfo.Opacity;
					double num2 = haloEffectSaveInfo.Weight;
					if (num < 0.0)
					{
						num = 0.0;
					}
					if (num > 1.0)
					{
						num = 1.0;
					}
					if (num2 < 0.0)
					{
						num2 = 0.0;
					}
					return new HaloEffectArgs
					{
						HaloColor = haloEffectSaveInfo.Color,
						HaloOpacity = num,
						HaloWeight = num2
					};
				}
				double num3 = frameEffectSaveInfo.FrameThinkness;
				double num4 = frameEffectSaveInfo.FrameOpacity;
				if (num3 < 0.0)
				{
					num3 = 0.0;
				}
				if (num4 < 0.0)
				{
					num4 = 0.0;
				}
				if (num4 > 1.0)
				{
					num4 = 1.0;
				}
				return new FrameEffectArgs
				{
					FrameBrush = frameEffectSaveInfo.FrameBrush,
					FrameThinkness = num3,
					FrameOpacity = num4
				};
			}
			return new FillEffectArgs
			{
				FillBrush = fillEffectSaveInfo.FillBrush
			};
		}
	}
}
