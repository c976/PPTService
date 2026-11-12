using System.Windows;
using Cvte.Windows.Input;

namespace Cvte.Paint.Framework
{
	public sealed class DeviceModeInputSource : ModeInputSource
	{
		private DeviceInputSource _inputSource;

		protected override void OnAttached(UIElement eventSource)
		{
			if (_inputSource != null)
			{
				_inputSource.InputStarting -= InputStarting;
				_inputSource.Detach();
			}
			_inputSource = new DeviceInputSource(eventSource)
			{
				Container = Container
			};
			_inputSource.InputStarting += InputStarting;
			_inputSource.Attach(InputDown, InputMove, InputUp);
			_inputSource.InputHover += InputHover;
		}

		protected override void OnDetaching()
		{
			if (_inputSource != null)
			{
				_inputSource.InputStarting -= InputStarting;
				_inputSource.Detach();
			}
			_inputSource = null;
		}

		private void InputStarting(object sender, DeviceInputStartingEventArgs e)
		{
			_inputSource.Container = Container;
		}

		private void InputDown(object sender, DeviceInputEventArgs e)
		{
			RaiseInputDown(ModeInputEventArgs.FromDeviceInput(e));
		}

		private void InputMove(object sender, DeviceInputEventArgs e)
		{
			RaiseInputMove(ModeInputEventArgs.FromDeviceInput(e));
		}

		private void InputHover(object sender, DeviceInputEventArgs e)
		{
			_inputSource.Container = Container;
			RaiseInputHover(ModeInputEventArgs.FromDeviceInput(e));
		}

		private void InputUp(object sender, DeviceInputEventArgs e)
		{
			RaiseInputUp(ModeInputEventArgs.FromDeviceInput(e));
		}
	}
}
