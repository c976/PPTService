using System.Windows.Media;

namespace PPTService.Paint.Framework
{
	public interface IOutlineDefinetionContext
	{
		void DefineLineThickness(double thickness);

		Geometry IncludeLineThickness(Geometry baseGeometry, double thickness);
	}
}
