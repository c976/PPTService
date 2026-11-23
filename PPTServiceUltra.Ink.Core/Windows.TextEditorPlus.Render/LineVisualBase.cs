using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils.Glyph;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	internal sealed class LineVisualBase : UIElement, IDisposable
	{
		private readonly List<Visual> _children = new List<Visual>();

		private readonly List<UnderlineRenderData> _underlines;

		private readonly Dictionary<ExtensionCharVisual, Rect> _extCharVisualBoundsCache = new Dictionary<ExtensionCharVisual, Rect>();

		private bool _disposedValue;

		private DrawingVisual _visualBase;

		private VisualBrush _visualBrush;

		internal LineRenderData LineRenderData { get; }

		internal double Width { get; set; }

		internal double Height { get; set; }

		internal double StartArrangeOffset { get; set; }

		protected override int VisualChildrenCount => _children.Count;

		internal bool IsAlive { get; set; }

		public bool IsDisposed => _disposedValue;

		static LineVisualBase()
		{
			UIElement.IsHitTestVisibleProperty.OverrideMetadata(typeof(LineVisualBase), new UIPropertyMetadata(false));
			UIElement.SnapsToDevicePixelsProperty.OverrideMetadata(typeof(LineVisualBase), new PropertyMetadata(true));
		}

		internal LineVisualBase(LineRenderData lineRenderData, ArrangingType arrangingType)
		{
			LineVisualBase lineVisualBase = this;
			LineRenderData = lineRenderData;
			List<ExtensionCharVisual> list = (from visual in lineRenderData.CharVisuals.OfType<ExtensionCharVisual>()
				where !visual.IsHitTestVisible
				select visual).ToList();
			_underlines = lineRenderData.Underlines;
			lineRenderData.EffectCharVisuals.ForEach((UiElementCharVisual x) =>
			{
				lineVisualBase.AddVisualChild(x.CharView);
				lineVisualBase._children.Add(x.CharView);
			});
			DrawingVisual drawingVisual = CreateTextVisual();
			AddVisualChild(drawingVisual);
			_children.Add(drawingVisual);
			if (!list.Any())
			{
				return;
			}
			list.ForEach((ExtensionCharVisual x) =>
			{
				(VisualTreeHelper.GetParent(x.View) as LineVisualBase)?.Clear();
				lineVisualBase.AddVisualChild(x.View);
				lineVisualBase._children.Add(x.View);
				if (x.IsNeedRotationUnderVertical && arrangingType != ArrangingType.Horizontal)
				{
					Rect value = new Rect(x.Bounds.Location, new Size(x.Bounds.Height, x.Bounds.Width));
					lineVisualBase._extCharVisualBoundsCache.Add(x, value);
				}
			});
		}

		~LineVisualBase()
		{
			Dispose(false);
		}

		internal void Clear()
		{
			_children.ForEach(RemoveVisualChild);
			_children.Clear();
		}

		private DrawingVisual CreateTextVisual()
		{
			DrawingVisual drawingVisual = (_visualBase = new DrawingVisual());
			DrawingContext drawingContext = drawingVisual.RenderOpen();
			foreach (var item3 in OptimizeGeometry())
			{
				Geometry item = item3.Item1;
				Brush item2 = item3.Item2;
				drawingContext.DrawGeometry(item2, null, item);
			}
			DrawGlyphRun(drawingContext);
			DrawUnderline(drawingContext);
			DrawTextEditorDecoration(drawingContext);
			drawingContext.Close();
			DrawingVisual drawingVisual2 = new DrawingVisual();
			using (DrawingContext drawingContext2 = drawingVisual2.RenderOpen())
			{
				VisualBrush visualBrush = new VisualBrush(drawingVisual)
				{
					Stretch = Stretch.None
				};
				drawingContext2.DrawRectangle(visualBrush, null, drawingVisual.ContentBounds);
				_visualBrush = visualBrush;
				return drawingVisual2;
			}
		}

		private void DrawGlyphRun(DrawingContext drawingContext)
		{
			drawingContext.DrawingTextCharVisuals(LineRenderData.CharVisuals.OfType<TextCharVisual>());
		}

		private void DrawTextEditorDecoration(DrawingContext drawingContext)
		{
			foreach (AttachedDecorationData attachedDecorationData in LineRenderData.AttachedDecorationDataList)
			{
				TextEditorDecoration textEditorDecoration = attachedDecorationData.TextEditorDecoration;
				Rect bounds = attachedDecorationData.Bounds;
				Brush foreground = attachedDecorationData.Foreground;
				Drawing drawing = textEditorDecoration.BuildDecoration(bounds, foreground);
				drawingContext.DrawDrawing(drawing);
			}
		}

		private void DrawUnderline(DrawingContext drawingContext)
		{
			foreach (UnderlineRenderData underline in _underlines)
			{
				Rect underlineBounds = underline.UnderlineBounds;
				double num = underlineBounds.Height / 2.0;
				drawingContext.DrawLine(point0: new Point(underlineBounds.Left, underlineBounds.Y + num), point1: new Point(underlineBounds.Right, underlineBounds.Y + num), pen: new Pen(underline.Foreground, underlineBounds.Height)
				{
					DashStyle = underline.DashStyle
				});
			}
		}

		private void AddTextCharBorder(DrawingContext drawingContext)
		{
			SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(16, byte.MaxValue, 0, 0));
			foreach (List<int> samePropIndex in LineRenderData.SamePropIndexes)
			{
				foreach (int item in samePropIndex)
				{
					drawingContext.DrawRectangle(null, new Pen(brush, 1.0), LineRenderData.CharVisuals[item].Bounds);
				}
			}
		}

		private List<ValueTuple<Geometry, Brush>> OptimizeGeometry()
		{
			List<ValueTuple<Geometry, Brush>> list = new List<ValueTuple<Geometry, Brush>>();
			foreach (List<int> samePropIndex in LineRenderData.SamePropIndexes)
			{
				GeometryGroup gg = new GeometryGroup
				{
					FillRule = FillRule.Nonzero
				};
				samePropIndex.ForEach((int x) =>
				{
					TextCharVisual textCharVisual = (TextCharVisual)LineRenderData.CharVisuals[x];
					if (textCharVisual.GlyphData == null)
					{
						Geometry textGeometry = textCharVisual.TextGeometry;
						gg.Children.Add(textGeometry);
					}
				});
				list.Add(new ValueTuple<Geometry, Brush>(gg, ((TextCharVisual)LineRenderData.CharVisuals[samePropIndex[0]]).Foreground));
			}
			return list;
		}

		protected override void ArrangeCore(Rect finalRect)
		{
			if (_disposedValue)
			{
				throw new ObjectDisposedException("LineVisualBase", "行渲染已经被释放，不能再使用");
			}
			base.ArrangeCore(finalRect);
			foreach (ExtensionCharVisual item in from visual in LineRenderData.CharVisuals.OfType<ExtensionCharVisual>()
				where !visual.IsHitTestVisible
				select visual)
			{
				if (item.IsNeedRotationUnderVertical && _extCharVisualBoundsCache.Any())
				{
					item.View.Arrange(_extCharVisualBoundsCache[item]);
				}
				else
				{
					item.View.Arrange(item.Bounds);
				}
			}
			foreach (UiElementCharVisual effectCharVisual in LineRenderData.EffectCharVisuals)
			{
				effectCharVisual.CharView.Arrange(effectCharVisual.Bounds);
			}
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
		}

		protected override Visual GetVisualChild(int index)
		{
			return _children[index];
		}

		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return null;
		}

		protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
		{
			return null;
		}

		private void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					_visualBrush.Visual = null;
					_visualBase.Children.Clear();
					Clear();
				}
				_underlines.Clear();
				_extCharVisualBoundsCache.Clear();
				IsAlive = true;
				_disposedValue = true;
				_visualBrush = null;
				_visualBase = null;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
