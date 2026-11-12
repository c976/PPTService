using System.Windows;

namespace Cvte.Paint.Framework.SelectionBoxs
{
	public interface ISlideLayeredOperator
	{
		Rect Bounds { get; set; }

		double Rotation { get; set; }
	}
}
