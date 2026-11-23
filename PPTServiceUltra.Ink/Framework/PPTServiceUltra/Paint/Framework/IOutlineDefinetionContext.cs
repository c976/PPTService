using System.Windows.Media;

namespace PPTServiceUltra.Paint.Framework
{
	public interface IOutlineDefinetionContext
	{
		void DefineLineThickness(double thickness);

		Geometry IncludeLineThickness(Geometry baseGeometry, double thickness);
	}
}
