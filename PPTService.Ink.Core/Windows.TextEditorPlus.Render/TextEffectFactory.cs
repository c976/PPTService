using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.Media;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Render
{
	internal static class TextEffectFactory
	{
		private static readonly Dictionary<Type, DrawerBase> DrawerDic;

		internal static UIElementContainer CreateTextEffectChar(List<TextCharVisual> textCharVisuals, EffectArgsCollection effectArgs, double baseLine = 0.0, ArrangingType arrangingType = ArrangingType.Horizontal)
		{
			return CreateEffectChar(textCharVisuals, effectArgs, DrawType.TextChar, null, baseLine, arrangingType);
		}

		internal static UIElementContainer CreateEffectChar(List<TextCharVisual> textCharVisuals, EffectArgsCollection effectArgs, DrawType drawType, UnderlineRenderData underlineRenderData, double baseLine = 0.0, ArrangingType arrangingType = ArrangingType.Horizontal)
		{
			UIElementContainer uIElementContainer = new UIElementContainer();
			List<DrawingVisualContainer> textEffectContainers = GetTextEffectContainers(textCharVisuals, effectArgs, drawType, underlineRenderData, baseLine, arrangingType);
			uIElementContainer.AddChildren(textEffectContainers);
			return uIElementContainer;
		}

		internal static List<DrawingVisualContainer> GetTextEffectContainers(List<TextCharVisual> textCharVisuals, EffectArgsCollection effectArgs, DrawType drawType, UnderlineRenderData underlineRenderData, double baseLine, ArrangingType arrangingType = ArrangingType.Horizontal)
		{
			List<DrawingVisualContainer> list = new List<DrawingVisualContainer>();
			DrawingVisualContainer drawingVisualContainer = CreateEffectContainer(textCharVisuals, effectArgs, drawType, underlineRenderData, baseLine, arrangingType);
			list.Add(drawingVisualContainer);
			if (effectArgs.ContainsType(typeof(ReflectionEffectArgs)))
			{
				DrawingVisualContainer item = CreateReflectionContainer(textCharVisuals, effectArgs, drawType, baseLine, drawingVisualContainer, arrangingType);
				list.Add(item);
			}
			return list;
		}

		private static DrawingVisualContainer CreateReflectionContainer(List<TextCharVisual> textCharVisuals, EffectArgsCollection effectArgs, DrawType drawType, double baseLine, DrawingVisualContainer visualContainer, ArrangingType arrangingType = ArrangingType.Horizontal)
		{
			DrawingVisualContainer drawingVisualContainer = visualContainer.Clone();
			ReflectionEffectArgs reflectionEffectArgs = effectArgs.OfType<ReflectionEffectArgs>().First();
			int num = ((arrangingType == ArrangingType.Horizontal) ? 1 : (-1));
			int num2 = ((arrangingType != ArrangingType.Horizontal) ? 1 : (-1));
			if (drawType == DrawType.Underline)
			{
				double num3 = ((arrangingType == ArrangingType.Horizontal) ? textCharVisuals.First().Bounds.Top : textCharVisuals.First().Bounds.Left);
				drawingVisualContainer.RenderTransform = ((arrangingType == ArrangingType.Horizontal) ? new ScaleTransform(num, num2)
				{
					CenterY = baseLine - num3 + reflectionEffectArgs.ReflectionDistance
				} : new ScaleTransform(num, num2)
				{
					CenterX = baseLine - num3 - reflectionEffectArgs.ReflectionDistance
				});
			}
			else
			{
				drawingVisualContainer.RenderTransform = ((arrangingType == ArrangingType.Horizontal) ? new ScaleTransform(num, num2)
				{
					CenterY = baseLine + reflectionEffectArgs.ReflectionDistance
				} : new ScaleTransform(num, num2)
				{
					CenterX = baseLine - reflectionEffectArgs.ReflectionDistance
				});
			}
			LinearGradientBrush linearGradientBrush = new LinearGradientBrush
			{
				StartPoint = default(Point),
				EndPoint = ((arrangingType == ArrangingType.Horizontal) ? new Point(0.0, 1.0) : new Point(1.0, 0.0))
			};
			GradientStopCollection gradientStopCollection = new GradientStopCollection();
			double offset;
			double offset2;
			double offset3;
			if (arrangingType == ArrangingType.Horizontal)
			{
				offset = Math.Max(0.0, reflectionEffectArgs.ReflectionDepth - 0.4);
				offset2 = Math.Max(0.4, reflectionEffectArgs.ReflectionDepth);
				offset3 = 1.0;
			}
			else
			{
				offset = Math.Max(0.4, reflectionEffectArgs.ReflectionDepth);
				offset2 = Math.Max(0.0, reflectionEffectArgs.ReflectionDepth - 0.4);
				offset3 = 0.0;
			}
			gradientStopCollection.Add(new GradientStop(Colors.Transparent, offset));
			gradientStopCollection.Add(new GradientStop(BrushCreator.GetOrCreate("#22222222").Color, offset2));
			gradientStopCollection.Add(new GradientStop(BrushCreator.GetOrCreate("#ff000000").Color, offset3));
			linearGradientBrush.GradientStops = gradientStopCollection;
			drawingVisualContainer.Opacity = reflectionEffectArgs.ReflectionOpacity * reflectionEffectArgs.Opacity;
			drawingVisualContainer.OpacityMask = linearGradientBrush;
			return drawingVisualContainer;
		}

		private static DrawingVisualContainer CreateEffectContainer(List<TextCharVisual> textCharVisuals, EffectArgsCollection effectArgs, DrawType drawType, UnderlineRenderData underlineRenderData, double baseLine, ArrangingType arrangingType = ArrangingType.Horizontal)
		{
			DrawingVisualContainer drawingVisualContainer = new DrawingVisualContainer();
			foreach (ITextEffectArgs item in effectArgs.OrderBy((ITextEffectArgs x) => x.Priority).SkipWhile((ITextEffectArgs x) => x is ReflectionEffectArgs))
			{
				DrawerBase drawerBase = CreateTextEffectDrawer(item);
				drawerBase.SetDataInfo(item, textCharVisuals, baseLine, underlineRenderData, arrangingType);
				drawerBase.DrawEffect(drawType);
				drawingVisualContainer.AddChild(drawerBase.DrawingVisual);
			}
			if (effectArgs.Count == 1 && effectArgs.First() is ReflectionEffectArgs)
			{
				NormalDrawer normalDrawer = new NormalDrawer();
				normalDrawer.SetDataInfo(effectArgs.First(), textCharVisuals, baseLine, underlineRenderData, arrangingType);
				normalDrawer.DrawEffect(drawType);
				drawingVisualContainer.AddChild(normalDrawer.DrawingVisual);
			}
			return drawingVisualContainer;
		}

		static TextEffectFactory()
		{
			DrawerDic = new Dictionary<Type, DrawerBase>();
			DrawerDic.Add(typeof(FrameEffectArgs), new FrameDrawer());
			DrawerDic.Add(typeof(PerspectiveEffectArgs), new PerspectiveDrawer());
			DrawerDic.Add(typeof(FillEffectArgs), new FillDrawer());
			DrawerDic.Add(typeof(HaloEffectArgs), new HaloDrawer());
			DrawerDic.Add(typeof(ShadowEffectArgs), new ShadowDrawer());
		}

		private static DrawerBase CreateTextEffectDrawer(ITextEffectArgs effectArgs)
		{
			return DrawerDic[effectArgs.GetType()];
		}
	}
}
