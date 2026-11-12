using System;
using System.Collections.Generic;
using System.Linq;

namespace Cvte.Paint.Framework
{
	public abstract class CompositeMode : Mode
	{
		private List<IModeOperation> _modeOperations;

		protected List<IModeOperation> ModeOperations
		{
			get
			{
				return _modeOperations ?? (_modeOperations = new List<IModeOperation>());
			}
			set
			{
				_modeOperations = value;
			}
		}

		protected CompositeMode()
		{
		}

		protected CompositeMode(ModeSettings modeSettings)
			: base(modeSettings)
		{
		}

		protected void DeliverEvents<TModeOperation>(Action<TModeOperation> deliver) where TModeOperation : class, IModeOperation
		{
			if (ModeOperations != null && ModeOperations.Any())
			{
				ModeOperations.OfType<TModeOperation>().ForEachWithPaintException(deliver, "Failed to deliver mode event for " + typeof(TModeOperation).Name + ".");
			}
		}

		protected override void OnSwitchOn()
		{
			base.OnSwitchOn();
			DeliverEvents((IModeOperation o) =>
			{
				o.CurrentSlide = CurrentSlide;
			});
			DeliverEvents((IModePrepareOperation o) =>
			{
				o.OnSwitchOn();
			});
		}

		protected override void OnSwitchOff()
		{
			base.OnSwitchOff();
			try
			{
				DeliverEvents((IModePrepareOperation o) =>
				{
					o.OnSwitchOff();
				});
			}
			finally
			{
				DeliverEvents((IModeOperation o) =>
				{
					o.CurrentSlide = null;
				});
			}
		}

		protected override void OnInputStarting(ModeInputEventArgs e)
		{
			DeliverEvents((IModeInputPrepareOperation o) =>
			{
				o.OnInputStarting(e);
			});
		}

		protected override void OnInputCompleted(ModeInputEventArgs e)
		{
			DeliverEvents((IModeInputPrepareOperation o) =>
			{
				o.OnInputCompleted(e);
			});
		}

		protected override void OnDeviceHover(ModeInputEventArgs e)
		{
			DeliverEvents((IModeHoverOperation o) =>
			{
				o.OnHover(e);
			});
		}

		protected override void OnElementClicked(ModeInputEventArgs e)
		{
			DeliverEvents((IModeTapOperation o) =>
			{
				o.OnTap(e);
			});
		}

		protected override void OnElementDragStarted(ModeInputDragEventArgs e)
		{
			DeliverEvents((IModeDragOperation o) =>
			{
				o.OnDragStarted(e);
			});
		}

		protected override void OnElementDragDelta(ModeInputDragEventArgs e)
		{
			DeliverEvents((IModeDragOperation o) =>
			{
				o.OnDragDelta(e);
			});
		}

		protected override void OnElementDragCompleted(ModeInputDragEventArgs e)
		{
			DeliverEvents((IModeDragOperation o) =>
			{
				o.OnDragCompleted(e);
			});
		}

		protected override void OnElementDragCanceled(ModeInputDragEventArgs e)
		{
			DeliverEvents((IModeDragOperation o) =>
			{
				o.OnDragCanceled(e);
			});
		}

		protected override void OnDeviceDown(ModeInputEventArgs e)
		{
			DeliverEvents((IModeDownMoveUpOperation o) =>
			{
				o.OnDeviceDown(e);
			});
		}

		protected override void OnDeviceMove(ModeInputEventArgs e)
		{
			DeliverEvents((IModeDownMoveUpOperation o) =>
			{
				o.OnDeviceMove(e);
			});
		}

		protected override void OnDeviceUp(ModeInputEventArgs e)
		{
			DeliverEvents((IModeDownMoveUpOperation o) =>
			{
				o.OnDeviceUp(e);
			});
		}
	}
}
