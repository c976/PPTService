using System.Windows.Ink;
using System.Windows.Input;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Inks.Storage;

namespace Cvte.Paint.Inks
{
	public abstract class StrokeBase : Stroke
	{
		[CanBeNull]
		public StrokePointCollection StrokePointDetailsList { get; set; }

		protected StrokeBase()
			: base(new StylusPointCollection(new StylusPoint[1]
			{
				new StylusPoint(0.0, 0.0)
			}))
		{
		}

		protected StrokeBase([NotNull] StylusPointCollection stylusPoints)
			: base(stylusPoints)
		{
		}

		protected StrokeBase([NotNull] StylusPointCollection stylusPoints, [NotNull] DrawingAttributes drawingAttributes)
			: base(stylusPoints, drawingAttributes)
		{
		}

		public virtual InkSaveInfo GetInkSaveInfo()
		{
			return null;
		}

		public virtual void SetInkSaveInfo(InkSaveInfo saveInfo)
		{
		}
	}
	public abstract class StrokeBase<TSaveInfo> : StrokeBase where TSaveInfo : InkSaveInfo
	{
		protected StrokeBase()
			: base(new StylusPointCollection(new StylusPoint[1]
			{
				new StylusPoint(0.0, 0.0)
			}))
		{
		}

		protected StrokeBase([NotNull] StylusPointCollection stylusPoints)
			: base(stylusPoints)
		{
		}

		protected StrokeBase([NotNull] StylusPointCollection stylusPoints, [NotNull] DrawingAttributes drawingAttributes)
			: base(stylusPoints, drawingAttributes)
		{
		}

		public abstract TSaveInfo GetSaveInfo();

		public abstract void SetSaveInfo(TSaveInfo saveInfo);

		public override InkSaveInfo GetInkSaveInfo()
		{
			return GetSaveInfo();
		}

		public override void SetInkSaveInfo(InkSaveInfo saveInfo)
		{
			SetSaveInfo(saveInfo as TSaveInfo);
		}
	}
}
