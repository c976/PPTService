using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace Cvte.Paint.Inks
{
	public class StrokeVisual : DrawingVisual
	{
		private readonly DrawingAttributes _drawingAttributes;

		public int PointCount
		{
			get
			{
				if (Stroke == null)
				{
					return 0;
				}
				return Stroke.StylusPoints.Count;
			}
		}

		public Stroke Stroke { get; set; }

		public StrokeVisual()
		{
		}

		public StrokeVisual(DrawingAttributes drawingAttributes)
		{
			_drawingAttributes = drawingAttributes;
		}

		public void Add(StylusPoint point)
		{
			if (Stroke == null)
			{
				StylusPointCollection stylusPoints = new StylusPointCollection { point };
				Stroke = new Stroke(stylusPoints)
				{
					DrawingAttributes = _drawingAttributes
				};
			}
			else
			{
				Stroke.StylusPoints.Add(point);
			}
		}

		public void SyncPoints(Stroke stroke, int count, int offset)
		{
			for (int i = 1; Stroke.StylusPoints.Count - i >= 0; i++)
			{
				Stroke.StylusPoints[Stroke.StylusPoints.Count - i] = stroke.StylusPoints[stroke.StylusPoints.Count - i - offset];
			}
		}

		public void Redraw()
		{
			using (DrawingContext context = RenderOpen())
			{
				Stroke.Draw(context);
			}
		}
	}
}
