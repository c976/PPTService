using System.Windows;

namespace PPTServiceUltra.Paint.Framework.SelectionBoxs
{
	public interface ISlideLayeredOperator
	{
		Rect Bounds { get; set; }

		double Rotation { get; set; }
	}
}
