using System.Windows.Media;

namespace Cvte.Paint.Framework
{
	public interface IOutlineDefinetionContext
	{
		void DefineLineThickness(double thickness);

		Geometry IncludeLineThickness(Geometry baseGeometry, double thickness);
	}
}
