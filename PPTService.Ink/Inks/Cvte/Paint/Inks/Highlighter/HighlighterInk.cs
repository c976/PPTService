using System.Linq;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework;
using Cvte.Storage;

namespace Cvte.Paint.Inks.Highlighter
{
	[Transform(PermittedTransforms.FixedRatioScaleOnlyWithTransform | PermittedTransforms.Rotate)]
	public class HighlighterInk : Element, IInk, ISavable<HighlighterInkSaveInfo>, ISavable, IPermittedTransform
	{
		private class HighlighterInkRenderer : FrameworkElement
		{
			private readonly HighlighterInk _highlighterInk;

			private Geometry Stroke { get; }

			public HighlighterInkRenderer(HighlighterInk highlighterInk)
			{
				_highlighterInk = highlighterInk;
				Stroke = _highlighterInk.InnerStroke.GetGeometry();
			}

			protected override void OnRender(DrawingContext drawingContext)
			{
				drawingContext.PushOpacity(_highlighterInk.HighlighterOpacity);
				drawingContext.DrawGeometry(_highlighterInk.Foreground, null, Stroke);
				drawingContext.Pop();
			}

			protected override Size MeasureOverride(Size availableSize)
			{
				return _highlighterInk.InnerStroke.GetBounds().Size;
			}
		}

		private const string HighlighterInkType = "HighlighterInk";

		private Brush _foreground;

		private double _highlighterOpacity = 0.3;

		private Stroke _innerStroke;

		public double HighlighterOpacity
		{
			get
			{
				return _highlighterOpacity;
			}
			set
			{
				if (!value.Equals(_highlighterOpacity))
				{
					_highlighterOpacity = value;
					OnPropertyChanged("HighlighterOpacity");
				}
			}
		}

		public Stroke InnerStroke
		{
			get
			{
				return _innerStroke;
			}
			protected set
			{
				_innerStroke = value;
				Children.Clear();
				Children.Add(new HighlighterInkRenderer(this));
			}
		}

		public Brush Foreground
		{
			get
			{
				if (_foreground == null && InnerStroke != null)
				{
					_foreground = new SolidColorBrush(InnerStroke.DrawingAttributes.Color);
					_foreground.Freeze();
				}
				return _foreground;
			}
			set
			{
				_foreground = value;
				InvalidateVisual();
			}
		}

		PermittedTransforms IPermittedTransform.PermittedTransform => PermittedTransforms.FixedRatioScaleOnlyWithTransform | PermittedTransforms.Rotate;

		public HighlighterInk(Stroke innerStroke)
			: this()
		{
			InnerStroke = innerStroke;
		}

		[UsedImplicitly]
		protected HighlighterInk()
		{
			Name = "荧光笔";
			IsErasable = true;
		}

		public static HighlighterInk CreateHighlighterInkFromSlideStroke(Stroke stroke)
		{
			Point location = stroke.GetBounds().Location;
			stroke = new CombinedStroke(new StylusPointCollection(stroke.StylusPoints.Select((StylusPoint temp) => new StylusPoint(temp.X - location.X, temp.Y - location.Y, temp.PressureFactor))), stroke.DrawingAttributes);
			Rect bounds = stroke.GetBounds();
			PathGeometry clipGeometry = PathGeometry.CreateFromGeometry(new RectangleGeometry(bounds));
			return new HighlighterInk(stroke)
			{
				ClipGeometry = clipGeometry,
				Location = location
			};
		}

		public HighlighterInkSaveInfo GetSaveInfo()
		{
			Stroke innerStroke = InnerStroke;
			HighlighterInkSaveInfo highlighterInkSaveInfo = new HighlighterInkSaveInfo();
			highlighterInkSaveInfo.Points = innerStroke.StylusPoints;
			highlighterInkSaveInfo.Thickness = innerStroke.DrawingAttributes.Width;
			highlighterInkSaveInfo.Foreground = innerStroke.DrawingAttributes.Color;
			highlighterInkSaveInfo.HighlighterOpacity = HighlighterOpacity;
			highlighterInkSaveInfo.SetType("HighlighterInk");
			return highlighterInkSaveInfo;
		}

		public void SetSaveInfo(HighlighterInkSaveInfo saveInfo)
		{
			CombinedStroke innerStroke = new CombinedStroke(saveInfo.Points, new DrawingAttributes
			{
				Width = saveInfo.Thickness,
				Height = saveInfo.Thickness,
				Color = saveInfo.Foreground
			});
			HighlighterOpacity = HighlighterOpacity;
			InnerStroke = innerStroke;
			ClipGeometry = PathGeometry.CreateFromGeometry(Geometry.Parse(saveInfo.ClipGeometry));
		}
	}
}
