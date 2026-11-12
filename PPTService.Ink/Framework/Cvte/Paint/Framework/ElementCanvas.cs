using System.Windows.Controls;
using System.Windows.Media;

namespace Cvte.Paint.Framework
{
	internal class ElementCanvas : Canvas
	{
		internal ElementCollection ElementCollection { get; private set; }

		public ElementCanvas()
		{
			ElementCollection = new ElementCollection(Children);
			Background = Brushes.Transparent;
			ClipToBounds = false;
			Focusable = false;
		}
	}
}
