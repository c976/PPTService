using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Paint.Framework.SelectionBoxs;
using Cvte.Windows.Input;

namespace Cvte.Paint.Framework
{
	public abstract class SelectionForBoardModeOperationBase : IModePrepareOperation, IModeOperation, IModeInputPrepareOperation, IModeTapOperation, IModeDragOperation
	{
		private readonly PolyLineGeometryBuilder _hitTestGeometryBuilder;

		private Lazy<ISelectionBoxGenerator> _simulateBoxGenerator;

		private List<Element> _simulateSelectedElements = new List<Element>();

		private SelectionTraceRenderer _selectionRenderer;

		private List<Element> _oldSelectedElements = new List<Element>();

		private List<Element> _currentSelectedElements = new List<Element>();

		public EventHandler<SelectEventArgs> DragStartedEvent;

		public EventHandler<SelectEventArgs> DragDeltaEvent;

		public EventHandler<SelectEventArgs> DragCompletedEvent;

		public EventHandler<SelectEventArgs> DragCancelEvent;

		public bool IsSimulateEnabled { get; set; }

		public Slide CurrentSlide { get; set; }

		public AdornerLayer AdornerLayer => AdornerLayer.GetAdornerLayer(Board.GetRenderSource(CurrentSlide));

		private static bool WithShift => Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

		private static bool WithCtrl => Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

		protected SelectionForBoardModeOperationBase()
		{
			_hitTestGeometryBuilder = new PolyLineGeometryBuilder();
		}

		public void OnSwitchOn()
		{
			_simulateBoxGenerator = new Lazy<ISelectionBoxGenerator>(() =>
			{
				ISelectionBoxGenerator selectionBoxGenerator = CurrentSlide.GetSelectionBoxGenerator();
				ISelectionBoxGenerator selectionBoxGenerator2 = (selectionBoxGenerator as ISimulatedSelectionBoxGenerator)?.GetSimulatedGeneratorCopy();
				return selectionBoxGenerator2 ?? selectionBoxGenerator;
			}, LazyThreadSafetyMode.None);
			_selectionRenderer = new SelectionTraceRenderer(CurrentSlide);
		}

		public void OnSwitchOff()
		{
			_selectionRenderer?.Clear();
			_simulateBoxGenerator = null;
		}

		public void OnInputStarting(ModeInputEventArgs e)
		{
			_simulateSelectedElements = new List<Element>();
			_oldSelectedElements = CurrentSlide.GetSelectedElements().ToList();
		}

		public void OnInputCompleted(ModeInputEventArgs e)
		{
		}

		public void OnTap(ModeInputEventArgs e)
		{
			if (e.VirtualType != VirtualDeviceType.LostCapture && !WithShift && !WithCtrl)
			{
				TapSelect(e.Element);
				Tapped(e.Position, e.Type);
			}
		}

		public void OnDragStarted(ModeInputDragEventArgs e)
		{
			ClearSelection();
			_hitTestGeometryBuilder.Reset();
			_selectionRenderer.Start(e.Position);
			if (e.Type == DeviceType.Touch)
			{
				_hitTestGeometryBuilder.Add(e.Position);
			}
		}

		public void OnDragDelta(ModeInputDragEventArgs e)
		{
			if (e.Type == DeviceType.Touch)
			{
				_hitTestGeometryBuilder.Add(e.Position);
				_selectionRenderer.AddTracePoints(from x in e.GetStylusPoints()
					select x.ToPoint());
			}
			else
			{
				_selectionRenderer.SetRectEnd(e.Position);
			}
			if (IsSimulateEnabled)
			{
				IEnumerable<Element> elements = ((e.Type != DeviceType.Touch) ? ElementHitTester.HitElements(hitBounds: new Rect(e.OriginPosition, e.Position), slide: CurrentSlide) : CurrentSlide.HitElements(_hitTestGeometryBuilder.Geometry));
				SimulateSelect(elements);
			}
		}

		public void OnDragCompleted(ModeInputDragEventArgs e)
		{
			_selectionRenderer.Clear();
			IEnumerable<Element> enumerable;
			if (!IsSimulateEnabled)
			{
				enumerable = ((e.Type != DeviceType.Touch) ? ElementHitTester.HitElements(hitBounds: new Rect(e.OriginPosition, e.Position), slide: CurrentSlide) : CurrentSlide.HitElements(_hitTestGeometryBuilder.Geometry));
			}
			else
			{
				_simulateBoxGenerator.Value.RemoveSelectionBoxes(CurrentSlide, _simulateSelectedElements);
				enumerable = _currentSelectedElements;
			}
			if (WithShift || WithCtrl)
			{
				enumerable = enumerable?.Union(_oldSelectedElements).Distinct();
			}
			else if (e.Type == DeviceType.Touch)
			{
				DragSelected(_hitTestGeometryBuilder.Geometry);
			}
			else
			{
				Rect containedBounds = new Rect(e.OriginPosition, e.Position);
				DragSelected(containedBounds);
			}
			Element[] array = enumerable?.ToArray();
			_oldSelectedElements.Clear();
			_currentSelectedElements.Clear();
			DragSelect(array ?? new Element[0]);
		}

		public void OnDragCanceled(ModeInputDragEventArgs e)
		{
			OnDragCompleted(e);
		}

		private void SimulateSelect(IEnumerable<Element> elements)
		{
			elements = (elements as ICollection<Element>) ?? elements.ToArray();
			if (WithShift || WithCtrl)
			{
				elements = elements.Union(_oldSelectedElements).Distinct();
			}
			List<Element> list = (elements as List<Element>) ?? elements.ToList();
			Element[] array = list.Except(_simulateSelectedElements).ToArray();
			Element[] array2 = _simulateSelectedElements.Except(list).ToArray();
			if (array.Any())
			{
				_currentSelectedElements.AddRange(array);
				_simulateBoxGenerator.Value.AddSelectionBoxes(CurrentSlide, array);
			}
			if (array2.Any())
			{
				for (int i = 0; i < array2.Length; i++)
				{
					_currentSelectedElements.Remove(array2[i]);
				}
				_simulateBoxGenerator.Value.RemoveSelectionBoxes(CurrentSlide, array2);
			}
			_simulateSelectedElements = list;
		}

		protected abstract void ClearSelection();

		protected abstract void TapSelect(Element element);

		protected virtual void Tapped(Point position, DeviceType deviceType)
		{
		}

		protected abstract void DragSelect(ICollection<Element> elements);

		protected virtual void DragSelected(Geometry containedGeometry)
		{
		}

		protected virtual void DragSelected(Rect containedBounds)
		{
		}
	}
}
