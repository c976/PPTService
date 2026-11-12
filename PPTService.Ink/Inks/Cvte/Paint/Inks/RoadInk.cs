using System.Windows;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework;
using Cvte.Paint.Inks.Storage;
using Cvte.Storage;

namespace Cvte.Paint.Inks
{
	[Transform(PermittedTransforms.FixedRatioScaleOnlyWithTransform | PermittedTransforms.Rotate)]
	public class RoadInk : Element, ISavable<RoadInkSaveInfo>, ISavable, IInk, IPermittedTransform
	{
		private class RoadInkRenderer : FrameworkElement
		{
			private readonly RoadStrokeBase _innerStroke;

			public RoadInkRenderer([NotNull] RoadStrokeBase innerStroke)
			{
				_innerStroke = innerStroke;
			}

			protected override void OnRender(DrawingContext dc)
			{
				_innerStroke.Draw(dc);
			}

			protected override Size MeasureOverride(Size availableSize)
			{
				return _innerStroke.GetBounds().Size;
			}
		}

		private RoadStrokeBase _innerStroke;

		private InkType _strokeType;

		public RoadStrokeBase InnerStroke
		{
			get
			{
				return _innerStroke;
			}
			set
			{
				if (_innerStroke != value)
				{
					_innerStroke = value;
					Children.Clear();
					Children.Add(new RoadInkRenderer(value));
				}
			}
		}

		public InkType StrokeType
		{
			get
			{
				return _strokeType;
			}
			set
			{
				if (!object.Equals(_strokeType, value))
				{
					_strokeType = value;
				}
			}
		}

		PermittedTransforms IPermittedTransform.PermittedTransform => PermittedTransforms.FixedRatioScaleOnlyWithTransform | PermittedTransforms.Rotate;

		[UsedImplicitly]
		public RoadInk()
		{
			IsErasable = true;
		}

		public RoadInk(RoadStrokeBase innerStroke, InkType strokeType)
		{
			InnerStroke = innerStroke;
			IsErasable = true;
			StrokeType = strokeType;
			PathGeometry pathGeometry = new PathGeometry();
			pathGeometry.AddGeometry(new RectangleGeometry(Bounds));
			ClipGeometry = pathGeometry;
		}

		public RoadInkSaveInfo GetSaveInfo()
		{
			return new RoadInkSaveInfo
			{
				Points = InnerStroke.StylusPoints,
				StrokeType = StrokeType
			};
		}

		public void SetSaveInfo(RoadInkSaveInfo saveInfo)
		{
			StrokeType = saveInfo.StrokeType;
			switch (StrokeType)
			{
			case InkType.Railway:
				InnerStroke = new RailwayStroke(saveInfo.Points);
				break;
			case InkType.River:
				InnerStroke = new RiverStroke(saveInfo.Points);
				break;
			default:
				InnerStroke = new GreatwallStroke(saveInfo.Points);
				break;
			}
			PathGeometry pathGeometry = new PathGeometry();
			pathGeometry.AddGeometry(Geometry.Parse(saveInfo.ClipGeometry));
			ClipGeometry = pathGeometry;
		}
	}
}
