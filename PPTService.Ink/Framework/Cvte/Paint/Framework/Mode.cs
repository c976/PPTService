using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Cvte.Paint.Framework
{
	public abstract class Mode : InputProcessor, IElementInteractionEnabled
	{
		[DefaultValue(true)]
		private readonly bool _isElementInteractionEnabled = true;

		[Obsolete("请使用 InputLockedSettings.IsModeLocked", true)]
		public static bool IsModeLocked { get; set; }

		protected internal bool IsOn { get; private set; }

		[Obsolete("因为这个属性只能在构造设置，所以请在构造调用带 ModeSettings 参数的 Mode 的构造函数传入设置。", true)]
		public bool IsElementInteractionEnabled { get; }

		private bool IsTouchCursorVisible { get; } = true;

		protected internal IModeHitTestController HitTestController { get; set; }

		bool IElementInteractionEnabled.IsElementInteractionEnabled => _isElementInteractionEnabled;

		protected Mode()
		{
		}

		protected Mode(ModeSettings modeSettings)
			: this()
		{
			_isElementInteractionEnabled = modeSettings.IsElementInteractionEnabled;
			IsTouchCursorVisible = modeSettings.IsTouchCursorVisible;
		}

		public sealed override void SwitchOn()
		{
			IsOn = true;
			_003CSwitchOn_003Eg__CheckElementInteractionEnabled_007C10_0();
			base.SwitchOn();
		}

		public sealed override void SwitchOff()
		{
			try
			{
				if (CurrentSlide != null && !_isElementInteractionEnabled)
				{
					HitTestController.Restore();
				}
				base.SwitchOff();
			}
			finally
			{
				IsOn = false;
			}
		}

		protected override void OnDeviceDown(ModeInputEventArgs e)
		{
			if (!IsInputLocked)
			{
				base.OnDeviceDown(e);
			}
		}

		protected override void OnDeviceMove(ModeInputEventArgs e)
		{
			if (!ContainsDevice(e.Id))
			{
				if (!IsInputLocked)
				{
					RaiseDeviceDown(e);
				}
			}
			else
			{
				base.OnDeviceMove(e);
			}
		}

		protected override void OnDeviceUp(ModeInputEventArgs e)
		{
			if (!IsInputLocked)
			{
				base.OnDeviceUp(e);
			}
		}

		[CompilerGenerated]
		private void _003CSwitchOn_003Eg__CheckElementInteractionEnabled_007C10_0()
		{
			if (CurrentSlide != null && !_isElementInteractionEnabled)
			{
				HitTestController.Disable();
			}
		}
	}
}
