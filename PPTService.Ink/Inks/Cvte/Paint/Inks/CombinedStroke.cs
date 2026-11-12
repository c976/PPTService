using System.Collections.Generic;
using System.Linq;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Paint.Inks.Storage;

namespace Cvte.Paint.Inks
{
	public class CombinedStroke : HardPenStroke
	{
		public IEnumerable<Stroke> Strokes { get; private set; }

		internal CombinedStroke()
		{
		}

		public CombinedStroke(IList<Stroke> splittedStrokes, DrawingAttributes drawingAttributes)
			: base(Merge(splittedStrokes), drawingAttributes.Color, drawingAttributes.Width)
		{
			DrawingAttributes = drawingAttributes;
			Strokes = splittedStrokes;
		}

		public CombinedStroke(Stroke stroke, IEnumerable<Stroke> splittedStrokes)
			: base(stroke.StylusPoints, stroke.DrawingAttributes.Color, stroke.DrawingAttributes.Width)
		{
			Strokes = splittedStrokes.ToArray();
		}

		public CombinedStroke(IList<Stroke> splittedStrokes)
			: this(splittedStrokes, DefaultDrawingAttributes(splittedStrokes))
		{
		}

		public CombinedStroke(StylusPointCollection stylusPointCollection, DrawingAttributes drawingAttributes)
			: base(stylusPointCollection, drawingAttributes.Color, drawingAttributes.Width)
		{
			DrawingAttributes = drawingAttributes;
		}

		protected override void DrawCore(DrawingContext drawingContext, DrawingAttributes drawingAttributes)
		{
			if (Strokes == null)
			{
				base.DrawCore(drawingContext, DrawingAttributes);
				return;
			}
			foreach (Stroke stroke in Strokes)
			{
				stroke.Draw(drawingContext);
			}
			Strokes = null;
		}

		public override HardPenInkSaveInfo GetSaveInfo()
		{
			HardPenInkSaveInfo saveInfo = base.GetSaveInfo();
			saveInfo.SetType("CombinedStroke");
			saveInfo.FitToCurve = DrawingAttributes.FitToCurve;
			saveInfo.StylusTipTransform = DrawingAttributes.StylusTipTransform;
			return saveInfo;
		}

		public override void SetSaveInfo(HardPenInkSaveInfo saveInfo)
		{
			base.SetSaveInfo(saveInfo);
			DrawingAttributes.FitToCurve = saveInfo.FitToCurve;
			DrawingAttributes.StylusTipTransform = saveInfo.StylusTipTransform;
		}

		private static StylusPointCollection Merge(IEnumerable<Stroke> strokes)
		{
			return new StylusPointCollection(strokes.SelectMany((Stroke x) => x.StylusPoints));
		}

		private static DrawingAttributes DefaultDrawingAttributes(IEnumerable<Stroke> strokes)
		{
			return strokes.First().DrawingAttributes;
		}
	}
}
