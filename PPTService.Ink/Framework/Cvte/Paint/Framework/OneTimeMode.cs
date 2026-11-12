using System;
using System.Windows;
using System.Windows.Input;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework
{
	public sealed class OneTimeMode : Mode, ICanControlModes, IOneTimeMode, IModeSwitching, IModeSwitched, IElementInteractionEnabled
	{
		private IModeController _modeController;

		private bool _isInnerModeOn;

		public Mode InnerMode { get; }

		bool IElementInteractionEnabled.IsElementInteractionEnabled => ((IElementInteractionEnabled)InnerMode).IsElementInteractionEnabled;

		public bool AutoSwitchOff { get; set; } = true;

		public bool IsTransientMode { get; }

		bool IOneTimeMode.IsActive => _isInnerModeOn;

		IModeController ICanControlModes.ModeController
		{
			get
			{
				return _modeController;
			}
			set
			{
				_modeController = value;
			}
		}

		internal Mode LastMode { get; private set; }

		public OneTimeMode([NotNull] Mode mode)
		{
			if (mode == null)
			{
				throw new ArgumentNullException("mode");
			}
			if (mode is OneTimeMode)
			{
				throw new ArgumentException("不允许包装 OneTimeMode 自身。", "mode");
			}
			InnerMode = mode;
		}

		public OneTimeMode([NotNull] Mode mode, bool isTransientMode)
			: this(mode)
		{
			IsTransientMode = isTransientMode;
		}

		void IOneTimeMode.EndOneTimeMode()
		{
			_modeController.CurrentMode = LastMode;
		}

		void IModeSwitching.OnSwitchingFrom(ModeSwitchingArgs args)
		{
			(InnerMode as IModeSwitching)?.OnSwitchingFrom(args);
		}

		void IModeSwitching.OnSwitchingTo(ModeSwitchingArgs args)
		{
			LastMode = args.SourceMode;
			while (LastMode is OneTimeMode)
			{
				LastMode = ((OneTimeMode)LastMode).LastMode;
			}
			(InnerMode as IModeSwitching)?.OnSwitchingTo(args);
		}

		void IModeSwitched.OnSwitchedFrom(ModeSwitchedArgs args)
		{
			(InnerMode as IModeSwitched)?.OnSwitchedFrom(args);
		}

		void IModeSwitched.OnSwitchedTo(ModeSwitchedArgs args)
		{
			(InnerMode as IModeSwitched)?.OnSwitchedTo(args);
		}

		protected override void OnSwitchOn()
		{
			FrameworkElement obj = ((FrameworkElement)CurrentSlide.Parent) ?? CurrentSlide;
			obj.Focus();
			obj.PreviewKeyDown += OnKeyDown;
			InnerMode.CurrentSlide = CurrentSlide;
			InnerMode.HitTestController = HitTestController;
			IForOneTimeMode forOneTimeMode = InnerMode as IForOneTimeMode;
			if (forOneTimeMode != null)
			{
				forOneTimeMode.OneTimeMode = this;
			}
			InnerMode.SwitchOn();
			_isInnerModeOn = true;
		}

		protected override void OnSwitchOff()
		{
			if (_isInnerModeOn)
			{
				_isInnerModeOn = false;
				(((FrameworkElement)CurrentSlide.Parent) ?? CurrentSlide).PreviewKeyDown -= OnKeyDown;
				InnerMode.SwitchOff();
				InnerMode.CurrentSlide = null;
				InnerMode.HitTestController = null;
				IForOneTimeMode forOneTimeMode = InnerMode as IForOneTimeMode;
				if (forOneTimeMode != null)
				{
					forOneTimeMode.OneTimeMode = null;
				}
			}
		}

		protected override void OnDeviceHover(ModeInputEventArgs e)
		{
			InnerMode.RaiseDeviceHover(e);
		}

		protected override void OnDeviceDown(ModeInputEventArgs e)
		{
			InnerMode.RaiseDeviceDown(e);
		}

		protected override void OnDeviceMove(ModeInputEventArgs e)
		{
			InnerMode.RaiseDeviceMove(e);
		}

		protected override void OnDeviceUp(ModeInputEventArgs e)
		{
			InnerMode.RaiseDeviceUp(e);
		}

		protected override void OnInputCompleted(ModeInputEventArgs e)
		{
			if (AutoSwitchOff)
			{
				((IOneTimeMode)this).EndOneTimeMode();
			}
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				((IOneTimeMode)this).EndOneTimeMode();
			}
		}
	}
}
