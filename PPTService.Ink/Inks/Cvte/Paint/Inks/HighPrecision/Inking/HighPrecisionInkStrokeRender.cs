using System;
using Cvte.Paint.Inks.PointToGeometry;

namespace Cvte.Paint.Inks.HighPrecision.Inking
{
	internal class HighPrecisionInkStrokeRender : InkStrokeRender
	{
		private static readonly Lazy<HighPrecisionInkStrokeRender> HighPrecisionInkStrokeRenderLazy = new Lazy<HighPrecisionInkStrokeRender>(() => new HighPrecisionInkStrokeRender());

		public static HighPrecisionInkStrokeRender Instance => HighPrecisionInkStrokeRenderLazy.Value;

		private HighPrecisionInkStrokeRender()
			: base(GetHighPrecisionInkStrokeRenderSettings())
		{
		}

		private static InkStrokeRenderSettings GetHighPrecisionInkStrokeRenderSettings()
		{
			return new InkStrokeRenderSettings
			{
				PressureFactorToScaleConverter = new HighPrecisionPressureFactorToScaleConverter()
			};
		}
	}
}
