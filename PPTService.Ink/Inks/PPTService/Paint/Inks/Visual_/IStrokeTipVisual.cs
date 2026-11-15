using System.Windows.Input;

namespace PPTService.Paint.Inks.Visual_
{
	public interface IStrokeTipVisual
	{
		void Redraw(StylusPointCollection stylusPoints);
	}
}
