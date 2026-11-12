using System;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Media;

namespace Cvte.Paint.Inks
{
	[Obsolete("在maxhub完成合并，删除这个类")]
	internal class TempInkContainer : UIElement
	{
		private readonly StrokeCollection _strokes;

		public StrokeCollection Strokes => _strokes;

		public TempInkContainer(StrokeCollection strokes)
		{
			_strokes = strokes;
			_strokes.StrokesChanged += Strokes_StrokesChanged;
		}

		public TempInkContainer()
			: this(new StrokeCollection())
		{
		}

		private void Strokes_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
		{
			InvalidateVisual();
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			Strokes.Draw(drawingContext);
		}
	}
}
