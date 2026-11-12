using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;
using Cvte.Paint.Framework;
using Cvte.Paint.Inks.HighPrecision.Inking;
using Cvte.Paint.Inks.PointToGeometry;

namespace Cvte.Paint.Inks.HighPrecision.Elements
{
	public class HighPrecisionInkElement : Ink
	{
		public static readonly DependencyProperty IsDebugModeProperty = DependencyProperty.Register("IsDebugMode", typeof(bool), typeof(HighPrecisionInkElement), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

		public InkStrokeRender InkStrokeRender { get; set; }

		public bool IsDebugMode
		{
			get
			{
				return (bool)GetValue(IsDebugModeProperty);
			}
			set
			{
				SetValue(IsDebugModeProperty, value);
			}
		}

		public HighPrecisionInkElement(Stroke stroke, InkStrokeRenderSettings inkStrokeRenderSettings)
			: base(stroke)
		{
			if (inkStrokeRenderSettings?.PressureFactorToScaleConverter == null || inkStrokeRenderSettings.PressureFactorToScaleConverter is HighPrecisionPressureFactorToScaleConverter)
			{
				InkStrokeRender = HighPrecisionInkStrokeRender.Instance;
			}
			else
			{
				InkStrokeRender = new InkStrokeRender(inkStrokeRenderSettings);
			}
		}

		private HighPrecisionInkElement(Stroke stroke, InkStrokeRender inkStrokeRender)
			: base(stroke)
		{
			InkStrokeRender = inkStrokeRender;
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			if (((IErasableInk)this).IsErasing)
			{
				foreach (Stroke item in ((IErasableInk)this).TempInkContainer)
				{
					DrawStroke(drawingContext, item);
				}
				return;
			}
			DrawStroke(drawingContext, InnerStroke);
		}

		protected override Element CreateNewInkOnErased(Stroke stroke)
		{
			return new HighPrecisionInkElement(stroke, InkStrokeRender);
		}

		private void DrawStroke(DrawingContext drawingContext, Stroke stroke)
		{
			InkStrokeRender.RenderInk(drawingContext, stroke.StylusPoints, stroke.DrawingAttributes, new SolidColorBrush(stroke.DrawingAttributes.Color), IsDebugMode);
		}
	}
}
