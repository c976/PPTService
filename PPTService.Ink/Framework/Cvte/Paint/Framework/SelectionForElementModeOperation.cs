using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Cvte.Documents;
using Cvte.Linq;
using Cvte.Paint.Framework.Elements;
using Cvte.Paint.Framework.Operations;
using Cvte.Windows;
using Cvte.Windows.Input;

namespace Cvte.Paint.Framework
{
	public sealed class SelectionForElementModeOperation : IModePrepareOperation, IModeOperation, IModeTapOperation, IModeDragOperation
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass11_0
		{
			public ModeInputEventArgs e;

			public Element hitElement;

			internal bool _003COnTap_003Eb__0(Element ee)
			{
				return ee != e.Element;
			}

			internal bool _003COnTap_003Eb__1(Element element)
			{
				return element.Bounds.Contains(e.Position);
			}

			internal bool _003COnTap_003Eg__IsGroupInnerSelected_007C2()
			{
				if (hitElement != null && hitElement.IsGroupInnerElement() && hitElement.IsGroupInnerSelected())
				{
					return true;
				}
				return false;
			}
		}

		private Transformer _transformer;

		private bool _canClone;

		private bool _isEnoughForClone;

		private bool _isGroupInnerTranslating;

		private double _currentMoveThreshold;

		public Slide CurrentSlide { get; set; }

		public Func<bool> CanCloneCore { get; set; }

		private static bool WithShift => Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

		private static bool WithCtrl => Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

		public void OnSwitchOn()
		{
			_transformer = Transformer.GetTransformer(CurrentSlide);
		}

		public void OnSwitchOff()
		{
			_transformer = null;
		}

		public void OnTap(ModeInputEventArgs e)
		{
			_003C_003Ec__DisplayClass11_0 CS_0024_003C_003E8__locals21 = new _003C_003Ec__DisplayClass11_0();
			CS_0024_003C_003E8__locals21.e = e;
			if (CS_0024_003C_003E8__locals21.e.VirtualType == VirtualDeviceType.LostCapture)
			{
				return;
			}
			CS_0024_003C_003E8__locals21.hitElement = CS_0024_003C_003E8__locals21.e.HittedSource.VisualAncestor<Element>();
			bool flag = CS_0024_003C_003E8__locals21._003COnTap_003Eg__IsGroupInnerSelected_007C2();
			bool flag2 = CurrentSlide.GetSelectedElements().Contains(CS_0024_003C_003E8__locals21.e.Element);
			ISelectionTapping selectionTapping = CS_0024_003C_003E8__locals21.e.Element as ISelectionTapping;
			if ((CS_0024_003C_003E8__locals21.e.Type == DeviceType.Touch) & flag2)
			{
				selectionTapping?.Tapped(CS_0024_003C_003E8__locals21.e);
			}
			else if (!((CS_0024_003C_003E8__locals21.e.Button == DeviceButton.Context) & flag2))
			{
				selectionTapping?.Tapped(CS_0024_003C_003E8__locals21.e);
			}
			if (CS_0024_003C_003E8__locals21._003COnTap_003Eg__IsGroupInnerSelected_007C2())
			{
				CurrentSlide.AppendUnselect((from ee in CurrentSlide.GetSelectedElements()
					where ee != CS_0024_003C_003E8__locals21.e.Element
					select ee).ToArray());
			}
			else
			{
				if (flag)
				{
					return;
				}
				if (WithShift || WithCtrl)
				{
					CurrentSlide.GetSelectedElements().OfType<GroupElementBase>().ToList()
						.ForEach((GroupElementBase groupElement) =>
						{
							groupElement.RemoveInnerSelectionBoxes();
						});
					if (CS_0024_003C_003E8__locals21.e.Button == DeviceButton.LeftButton)
					{
						CurrentSlide.AppendSwitchingSelection(new Element[1] { CS_0024_003C_003E8__locals21.e.Element });
					}
					else
					{
						CurrentSlide.AppendSelect(CS_0024_003C_003E8__locals21.e.Element);
					}
				}
				else if (CS_0024_003C_003E8__locals21.e.Button == DeviceButton.LeftButton || (CS_0024_003C_003E8__locals21.e.Button == DeviceButton.Context && !CS_0024_003C_003E8__locals21.e.Element.GetIsSelected() && !CurrentSlide.GetSelectedElements().Any((Element element) => element.Bounds.Contains(CS_0024_003C_003E8__locals21.e.Position))))
				{
					CurrentSlide.ClearAndSelect(CS_0024_003C_003E8__locals21.e.Element);
				}
			}
		}

		public void OnDragStarted(ModeInputDragEventArgs e)
		{
			GroupElementBase groupElementBase = e.Element as GroupElementBase;
			if (groupElementBase != null && groupElementBase.InnerSelections.Count() > 0)
			{
				_isGroupInnerTranslating = true;
			}
			else
			{
				_isGroupInnerTranslating = false;
			}
			if ((WithShift || WithCtrl) && !_isGroupInnerTranslating)
			{
				if (!e.Element.GetIsSelected())
				{
					_currentMoveThreshold = 16.0 / Board.GetContentScale(CurrentSlide);
					_isEnoughForClone = e.OffsetToOrigin.Length >= _currentMoveThreshold;
					CurrentSlide.ClearAndSelect(e.Element);
				}
				else
				{
					_currentMoveThreshold = 0.0;
					_isEnoughForClone = true;
					CurrentSlide.AppendSelect(e.Element);
				}
			}
			else if (!e.Element.GetIsSelected())
			{
				CurrentSlide.ClearAndSelect(e.Element);
			}
			_canClone = CanCloneCore == null || CanCloneCore();
			PermittedTransforms permittedTransform = e.Element.GetPermittedTransform();
			if (_isGroupInnerTranslating && groupElementBase.Elements.Any((Element innerElement) => innerElement.IsGroupInnerSelected()))
			{
				Element element = e.HittedSource.VisualAncestor<Element>();
				if (element == null)
				{
					return;
				}
				element = ElementGroupExtensions.FindGroupControlledChildOrRootElement(element);
				if (element.IsGroupInnerSelected())
				{
					Element[] elements = groupElementBase.Elements.Where((Element groupInnerElement) => groupInnerElement.IsGroupInnerSelected() && groupInnerElement.GetPermittedTransform().HasFlag(PermittedTransforms.Translate)).ToArray();
					_transformer.StartTransform(elements, TransformBehavior.Translate);
					return;
				}
				groupElementBase.Elements.ForEach((Element element2) =>
				{
					element2.SetIsGroupInnerSelected(false);
				});
				groupElementBase.InnerSelect(element);
				PermittedTransforms permittedTransform2 = element.GetPermittedTransform();
				if (permittedTransform2.HasFlag(PermittedTransforms.Translate))
				{
					_transformer.StartTransform(element, TransformBehavior.Translate);
				}
			}
			else if (permittedTransform.HasFlag(PermittedTransforms.Translate))
			{
				_transformer.StartTransform(e.Element, TransformBehavior.Translate);
			}
		}

		public void OnDragDelta(ModeInputDragEventArgs e)
		{
			_isEnoughForClone = _isEnoughForClone || e.OffsetToOrigin.Length > _currentMoveThreshold;
			_transformer.ProxyBehavior = (CanClone() ? TransformProxyBehavior.ProxyWithBody : TransformProxyBehavior.None);
			Vector modifieredOffset = GetModifieredOffset(e.OffsetToOrigin);
			_transformer.PerformTransform(modifieredOffset);
		}

		public void OnDragCompleted(ModeInputDragEventArgs e)
		{
			_isEnoughForClone = _isEnoughForClone || e.OffsetToOrigin.Length > _currentMoveThreshold;
			if (CanClone())
			{
				Element[] array = (from x in CurrentSlide.GetSelectedElements()
					where !x.GetIsLocked()
					select x.GetCounterpart() into x
					where x != null
					select x).ToArray();
				if (array.Any())
				{
					_transformer.CompleteTransform(true);
					_transformer.ProxyBehavior = TransformProxyBehavior.None;
					using (IUndoRedoTransaction undoRedoTransaction = Board.GetUndoRedoProvider(e.Element).BeginTransaction())
					{
						CurrentSlide.Add(array);
						undoRedoTransaction.Insert(new AddElementsOperation(CurrentSlide, array));
						CurrentSlide.ClearAndSelect(array);
						_transformer.StartTransform(array, TransformBehavior.Translate, TransformStepKind.RelativeToOrigin, true);
						_transformer.PerformTransform(GetModifieredOffset(e.OffsetToOrigin));
						_transformer.CompleteTransform();
						undoRedoTransaction.Commit();
						return;
					}
				}
				_transformer.ProxyBehavior = TransformProxyBehavior.None;
				_transformer.PerformTransform(GetModifieredOffset(e.OffsetToOrigin));
			}
			try
			{
				_transformer.CompleteTransform();
			}
			finally
			{
				_transformer.ProxyBehavior = TransformProxyBehavior.None;
			}
		}

		public void OnDragCanceled(ModeInputDragEventArgs e)
		{
			_transformer.ProxyBehavior = TransformProxyBehavior.None;
			_transformer.CompleteTransform(true);
		}

		private bool CanClone()
		{
			if (_isEnoughForClone && WithCtrl && _canClone)
			{
				return !_isGroupInnerTranslating;
			}
			return false;
		}

		private static Vector GetModifieredOffset(Vector offsetToOrigin)
		{
			if (WithShift)
			{
				return (Math.Abs(offsetToOrigin.X) >= Math.Abs(offsetToOrigin.Y)) ? new Vector(offsetToOrigin.X, 0.0) : new Vector(0.0, offsetToOrigin.Y);
			}
			return offsetToOrigin;
		}
	}
}
