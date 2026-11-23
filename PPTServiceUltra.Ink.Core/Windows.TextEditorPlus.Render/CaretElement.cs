using System;
using System.Windows;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class CaretElement : FrameworkElement
	{
		private static readonly DependencyProperty CaretInfoProperty = DependencyProperty.Register("CaretInfo", typeof(CaretInfo), typeof(CaretElement), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		private CaretInfo CaretInfo
		{
			get
			{
				return (CaretInfo)GetValue(CaretInfoProperty);
			}
			set
			{
				SetValue(CaretInfoProperty, value);
			}
		}

		internal CaretElement()
		{
			RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
			SnapsToDevicePixels = true;
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			Point point = default(Point);
			CaretInfo caretInfo = CaretInfo;
			Point point2;
			switch (caretInfo.ArrangingType)
			{
			case ArrangingType.Horizontal:
			{
				GuidelineSet guidelines = new GuidelineSet(new double[1] { 0.0 - caretInfo.Bounds.Width / 2.0 }, null);
				drawingContext.PushGuidelineSet(guidelines);
				point2 = new Point(0.0, caretInfo.Bounds.Height);
				break;
			}
			case ArrangingType.Vertical:
			case ArrangingType.Mongolian:
			{
				GuidelineSet guidelines = new GuidelineSet(null, new double[1] { 0.0 - caretInfo.Bounds.Height / 2.0 });
				drawingContext.PushGuidelineSet(guidelines);
				point2 = new Point(caretInfo.Bounds.Width, 0.0);
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
			double thickness = ((caretInfo.ArrangingType == ArrangingType.Horizontal) ? caretInfo.Bounds.Width : caretInfo.Bounds.Height);
			drawingContext.DrawLine(new Pen(caretInfo.Brush, thickness), point, point2);
			drawingContext.Pop();
		}

		internal void SetCaretInfo(CaretInfo caretInfo)
		{
			CaretInfo = null;
			CaretInfo = caretInfo;
		}
	}
}
