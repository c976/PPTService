using System;
using System.Linq;
using Cvte.Diagnostics.Annotations;
using Cvte.Linq;

namespace Cvte.Paint.Framework
{
	public sealed class HitTestModeOperation : IModePrepareOperation, IModeOperation, IModeInputPrepareOperation, IModeTapOperation, IModeDragOperation
	{
		[NotNull]
		private readonly IModeOperation[] _operations;

		[NotNull]
		private readonly Func<Element, Type, int> _deliverIndexes;

		public Slide CurrentSlide { get; set; }

		public HitTestModeOperation([CanBeNull] IModeOperation hitOperation, [CanBeNull] IModeOperation notHitOperation)
		{
			if (hitOperation == null && notHitOperation == null)
			{
				_operations = new IModeOperation[0];
				_deliverIndexes = (Element h, Type t) => -1;
				return;
			}
			if (hitOperation == null)
			{
				_operations = new IModeOperation[1] { notHitOperation };
				_deliverIndexes = (Element h, Type t) => (h != null) ? (-1) : 0;
				return;
			}
			if (notHitOperation == null)
			{
				_operations = new IModeOperation[1] { hitOperation };
				_deliverIndexes = (Element h, Type t) => (h == null) ? (-1) : 0;
				return;
			}
			_operations = new IModeOperation[2] { hitOperation, notHitOperation };
			_deliverIndexes = (Element h, Type t) => (h == null) ? 1 : 0;
		}

		public HitTestModeOperation([NotNull] Func<Element, int> deliverIndexes, params IModeOperation[] operations)
		{
			if (deliverIndexes == null)
			{
				throw new ArgumentNullException("deliverIndexes");
			}
			_operations = operations;
			_deliverIndexes = (Element h, Type t) => deliverIndexes(h);
		}

		public HitTestModeOperation([NotNull] Func<Element, Type, int> deliverIndexes, params IModeOperation[] operations)
		{
			if (deliverIndexes == null)
			{
				throw new ArgumentNullException("deliverIndexes");
			}
			_operations = operations;
			_deliverIndexes = deliverIndexes;
		}

		private void DeliverEvents<TModeOperation>(Element hittedElement, Action<TModeOperation> deliver) where TModeOperation : class, IModeOperation
		{
			int num = _deliverIndexes(hittedElement, typeof(TModeOperation));
			if (num >= 0)
			{
				TModeOperation val = _operations[num] as TModeOperation;
				if (val != null)
				{
					deliver(val);
				}
			}
		}

		public void OnSwitchOn()
		{
			_operations.ForEach((IModeOperation o) =>
			{
				o.CurrentSlide = CurrentSlide;
			});
			_operations.OfType<IModePrepareOperation>().ForEach((IModePrepareOperation o) =>
			{
				o.OnSwitchOn();
			});
		}

		public void OnSwitchOff()
		{
			_operations.OfType<IModePrepareOperation>().ForEach((IModePrepareOperation o) =>
			{
				o.OnSwitchOff();
			});
			_operations.ForEach((IModeOperation o) =>
			{
				o.CurrentSlide = null;
			});
		}

		public void OnInputStarting(ModeInputEventArgs e)
		{
			_operations.OfType<IModeInputPrepareOperation>().ForEach((IModeInputPrepareOperation o) =>
			{
				o.OnInputStarting(e);
			});
		}

		public void OnInputCompleted(ModeInputEventArgs e)
		{
			_operations.OfType<IModeInputPrepareOperation>().ForEach((IModeInputPrepareOperation o) =>
			{
				o.OnInputCompleted(e);
			});
			DeliverEvents(e.Element, (IModeInputPrepareOperation o) =>
			{
				o.OnInputCompleted(e);
			});
		}

		public void OnTap(ModeInputEventArgs e)
		{
			DeliverEvents(e.Element, (IModeTapOperation ope) =>
			{
				ope.OnTap(e);
			});
		}

		public void OnDragStarted(ModeInputDragEventArgs e)
		{
			DeliverEvents(e.Element, (IModeDragOperation ope) =>
			{
				ope.OnDragStarted(e);
			});
		}

		public void OnDragDelta(ModeInputDragEventArgs e)
		{
			DeliverEvents(e.Element, (IModeDragOperation ope) =>
			{
				ope.OnDragDelta(e);
			});
		}

		public void OnDragCompleted(ModeInputDragEventArgs e)
		{
			DeliverEvents(e.Element, (IModeDragOperation ope) =>
			{
				ope.OnDragCompleted(e);
			});
		}

		public void OnDragCanceled(ModeInputDragEventArgs e)
		{
			DeliverEvents(e.Element, (IModeDragOperation ope) =>
			{
				ope.OnDragCanceled(e);
			});
		}
	}
}
