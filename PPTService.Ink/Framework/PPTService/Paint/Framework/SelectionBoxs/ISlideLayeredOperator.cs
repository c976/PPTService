using System.Windows;

namespace PPTService.Paint.Framework.SelectionBoxs
{
	public interface ISlideLayeredOperator
	{
		Rect Bounds { get; set; }

		double Rotation { get; set; }
	}
}
