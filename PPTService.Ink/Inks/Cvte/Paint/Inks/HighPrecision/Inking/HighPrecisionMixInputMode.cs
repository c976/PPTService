using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Inks.HighPrecision.Erasing;
using Cvte.Paint.Settings;
using Cvte.Windows.Input;

namespace Cvte.Paint.Inks.HighPrecision.Inking
{
	public class HighPrecisionMixInputMode : WritingPenMode
	{
		private readonly HighPrecisionDoublePenMode _penMode;

		private readonly ModeControllerOperationProxy _penModeProxy;

		private readonly HighPrecisionDoublePenMagicPenMode _magicPenMode;

		private readonly ModeControllerOperationProxy _magicPenModeProxy;

		private readonly LargeObjectOrFingerSensitiveEraserMode _eraserMode;

		private readonly ModeControllerOperationProxy _eraserModeProxy;

		private readonly Mode _selectionMode;

		private readonly ModeControllerOperationProxy _selectionModeProxy;

		private readonly HashSet<int> _eraserIDs = new HashSet<int>();

		private readonly HashSet<int> _selectionIDs = new HashSet<int>();

		private readonly HashSet<int> _penIDs = new HashSet<int>();

		private readonly HashSet<int> _magicPenIDs = new HashSet<int>();

		private InkBoardSettings _inkBoardSettings;

		private HighPrecisionDoublePenBoardSettings _doublePenSettings;

		public bool IsSmartMode { get; }

		public HighPrecisionMixInputMode(Mode selectionMode)
			: base(new ModeSettings
			{
				IsElementInteractionEnabled = (selectionMode != null),
				IsTouchCursorVisible = false
			})
		{
			if (selectionMode != null)
			{
				_selectionMode = selectionMode;
				_selectionModeProxy = new ModeControllerOperationProxy(selectionMode);
				_penMode = new HighPrecisionDoublePenMode();
				_penModeProxy = new ModeControllerOperationProxy(_penMode);
				_magicPenMode = new HighPrecisionDoublePenMagicPenMode();
				_magicPenModeProxy = new ModeControllerOperationProxy(_magicPenMode);
				_eraserMode = new LargeObjectOrFingerSensitiveEraserMode(true);
				_eraserModeProxy = new ModeControllerOperationProxy(_eraserMode);
				IsSmartMode = true;
			}
			else
			{
				_penMode = new HighPrecisionDoublePenMode(true);
				_penModeProxy = new ModeControllerOperationProxy(_penMode);
				_magicPenMode = new HighPrecisionDoublePenMagicPenMode(true);
				_magicPenModeProxy = new ModeControllerOperationProxy(_magicPenMode);
				_eraserMode = new LargeObjectOrFingerSensitiveEraserMode();
				_eraserModeProxy = new ModeControllerOperationProxy(_eraserMode);
			}
		}

		public bool GetIsMagicMode(bool isThinPen)
		{
			if (!isThinPen)
			{
				return _doublePenSettings.ThickPenSetting.InkType != InkType.Pen;
			}
			return _inkBoardSettings.InkType != InkType.Pen;
		}

		protected override void OnDeviceDown(ModeInputEventArgs e)
		{
			CurrentSlide.Unselect();
			if (e.Type == DeviceType.Mouse)
			{
				if (GetIsMagicMode(true))
				{
					_magicPenMode.RaiseDeviceDown(e);
				}
				else
				{
					_penMode.RaiseDeviceDown(e);
				}
			}
			else
			{
				StylusPoint arg = e.GetStylusPoints().First();
				switch (_doublePenSettings.GetDoublePenInputTypeByStylusPointFunc(arg))
				{
				case HighPrecisionDoublePenInputType.LargeObject:
					DoWithFinally(() =>
					{
						_eraserMode.RaiseDeviceDown(e);
					}, () =>
					{
						_eraserIDs.Add(e.Id);
					});
					break;
				case HighPrecisionDoublePenInputType.Finger:
					if (_selectionMode != null)
					{
						DoWithFinally(() =>
						{
							_selectionMode.RaiseDeviceDown(e);
						}, () =>
						{
							_selectionIDs.Add(e.Id);
						});
						break;
					}
					goto case HighPrecisionDoublePenInputType.ThinPen;
				case HighPrecisionDoublePenInputType.ThinPen:
				case HighPrecisionDoublePenInputType.SimulatePressure:
					if (GetIsMagicMode(true))
					{
						DoWithFinally(() =>
						{
							_magicPenMode.RaiseDeviceDown(e);
						}, () =>
						{
							_magicPenIDs.Add(e.Id);
						});
					}
					else
					{
						DoWithFinally(() =>
						{
							_penMode.RaiseDeviceDown(e);
						}, () =>
						{
							_penIDs.Add(e.Id);
						});
					}
					break;
				case HighPrecisionDoublePenInputType.ThickPen:
					if (GetIsMagicMode(false))
					{
						DoWithFinally(() =>
						{
							_magicPenMode.RaiseDeviceDown(e);
						}, () =>
						{
							_magicPenIDs.Add(e.Id);
						});
					}
					else
					{
						DoWithFinally(() =>
						{
							_penMode.RaiseDeviceDown(e);
						}, () =>
						{
							_penIDs.Add(e.Id);
						});
					}
					break;
				}
			}
			base.OnDeviceDown(e);
		}

		protected override void OnDeviceMove(ModeInputEventArgs e)
		{
			if (e.Type == DeviceType.Mouse)
			{
				if (GetIsMagicMode(true))
				{
					_magicPenMode.RaiseDeviceMove(e);
				}
				else
				{
					_penMode.RaiseDeviceMove(e);
				}
			}
			else if (_selectionIDs.Contains(e.Id))
			{
				_selectionMode.RaiseDeviceMove(e);
			}
			else if (_eraserIDs.Contains(e.Id))
			{
				_eraserMode.RaiseDeviceMove(e);
			}
			else if (_penIDs.Contains(e.Id))
			{
				_penMode.RaiseDeviceMove(e);
			}
			else if (_magicPenIDs.Contains(e.Id))
			{
				_magicPenMode.RaiseDeviceMove(e);
			}
			base.OnDeviceMove(e);
		}

		protected override void OnDeviceUp(ModeInputEventArgs e)
		{
			if (e.Type == DeviceType.Mouse)
			{
				if (GetIsMagicMode(true))
				{
					_magicPenMode.RaiseDeviceUp(e);
				}
				else
				{
					_penMode.RaiseDeviceUp(e);
				}
			}
			else if (_selectionIDs.Contains(e.Id))
			{
				DoWithFinally(() =>
				{
					_selectionMode.RaiseDeviceUp(e);
				}, () =>
				{
					_selectionIDs.Remove(e.Id);
				});
			}
			else if (_eraserIDs.Contains(e.Id))
			{
				DoWithFinally(() =>
				{
					_eraserMode.RaiseDeviceUp(e);
				}, () =>
				{
					_eraserIDs.Remove(e.Id);
				});
			}
			else if (_penIDs.Contains(e.Id))
			{
				DoWithFinally(() =>
				{
					_penMode.RaiseDeviceUp(e);
				}, () =>
				{
					_penIDs.Remove(e.Id);
				});
			}
			else if (_magicPenIDs.Contains(e.Id))
			{
				DoWithFinally(() =>
				{
					_magicPenMode.RaiseDeviceUp(e);
				}, () =>
				{
					_magicPenIDs.Remove(e.Id);
				});
			}
			base.OnDeviceUp(e);
		}

		protected override void OnSwitchOn()
		{
			base.OnSwitchOn();
			_doublePenSettings = GetBoard().GetSettings<HighPrecisionDoublePenBoardSettings>();
			_inkBoardSettings = GetBoard().GetSettings<InkBoardSettings>();
			_penModeProxy.SetCurrentSlide(CurrentSlide);
			_penModeProxy.SetHitTestController(HitTestController);
			_penMode.SwitchOn();
			_eraserModeProxy.SetCurrentSlide(CurrentSlide);
			_eraserModeProxy.SetHitTestController(HitTestController);
			_eraserMode.SwitchOn();
			_magicPenModeProxy.SetCurrentSlide(CurrentSlide);
			_magicPenModeProxy.SetHitTestController(HitTestController);
			_magicPenMode.SwitchOn();
			if (_selectionMode != null)
			{
				_selectionModeProxy.SetCurrentSlide(CurrentSlide);
				_selectionModeProxy.SetHitTestController(HitTestController);
				_selectionMode.SwitchOn();
			}
		}

		protected override void OnSwitchOff()
		{
			base.OnSwitchOff();
			_doublePenSettings = null;
			_penMode.SwitchOff();
			_eraserMode.SwitchOff();
			_magicPenMode.SwitchOff();
			_selectionMode?.SwitchOff();
		}

		private void DoWithFinally(Action action, Action finallyAction)
		{
			try
			{
				action();
			}
			finally
			{
				finallyAction();
			}
		}
	}
}
