using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace PPTServiceUltra.Ink.Core.Windows
{
	public class ShadowBorderExtension : MarkupExtension
	{
		public Color Color { get; set; } = Colors.Black;

		public Color WindowOutterBorderColor { get; set; } = Color.FromArgb(11, 0, 0, 0);

		public Thickness WindowOutterBorderThickness { get; set; } = new Thickness(0.0);

		public Color InactiveColor { get; set; } = Color.FromArgb(153, 0, 0, 0);

		public Thickness Radius { get; set; } = new Thickness(60.0);

		public double Opacity { get; set; } = 1.0;

		public Thickness Shrink { get; set; } = new Thickness(27.0, 27.0, 27.0, 25.0);

		public bool CanResize { get; set; } = true;

		public ShadowGradientFunction GradientFunction { get; set; } = ShadowGradientFunction.Cubic;

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new ShadowBorder
			{
				Color = Color,
				InactiveColor = InactiveColor,
				Radius = Radius,
				Opacity = Opacity,
				Shrink = Shrink,
				WindowOutterBorderColor = WindowOutterBorderColor,
				WindowOutterBorderThickness = WindowOutterBorderThickness,
				GradientFunc = Func(GradientFunction, Opacity),
				CanResize = CanResize
			};
		}

		private static Func<double, double> Func(ShadowGradientFunction function, double opacity)
		{
			if (opacity < 1.0)
			{
				opacity = 1.0;
			}
			switch (function)
			{
			case ShadowGradientFunction.Liear:
				return (double x) => x * opacity;
			case ShadowGradientFunction.Quadratic:
				return (double x) => Math.Pow(x, 2.0) * opacity;
			case ShadowGradientFunction.Cubic:
				return (double x) => Math.Pow(x, 3.0) * opacity;
			case ShadowGradientFunction.Sine:
				return (double x) => (double)Math.Sign(x) * opacity;
			default:
				throw new NotSupportedException();
			}
		}
	}
}
