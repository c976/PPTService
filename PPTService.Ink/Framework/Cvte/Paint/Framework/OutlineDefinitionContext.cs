using System.Windows.Media;

namespace Cvte.Paint.Framework
{
	internal class OutlineDefinitionContext : IOutlineDefinetionContext
	{
		private static readonly Brush DefaultPenBrush = Brushes.White;

		internal bool IsThicknessIncluded { get; private set; }

		internal double Thickness { get; private set; }

		public void DefineLineThickness(double thickness)
		{
			Thickness = thickness;
		}

		public Geometry IncludeLineThickness(Geometry baseGeometry, double thickness)
		{
			IsThicknessIncluded = true;
			Thickness = thickness;
			return Geometry.Combine(baseGeometry.GetWidenedPathGeometry(new Pen(DefaultPenBrush, thickness)), baseGeometry, GeometryCombineMode.Union, null);
		}
	}
}
