using System;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace Cvte.Paint.Inks
{
	public class RoadStrokeVisual : DrawingVisual
	{
		private readonly Type _type;

		private readonly double _scale;

		public Stroke Stroke;

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

		public RoadStrokeVisual(MagicStrokeInfo strokeInfo)
		{
			_type = strokeInfo.Type;
			_scale = strokeInfo.Scale;
		}

		public void Add(StylusPoint point)
		{
			if (Stroke == null)
			{
				StylusPointCollection stylusPointCollection = new StylusPointCollection { point };
				if (typeof(RoadStrokeBase).IsAssignableFrom(_type))
				{
					Stroke = (Stroke)Activator.CreateInstance(_type, stylusPointCollection, _scale);
				}
				else
				{
					Stroke = (Stroke)Activator.CreateInstance(_type, stylusPointCollection);
				}
			}
			else
			{
				Stroke.StylusPoints.Add(point);
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
