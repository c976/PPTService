using Cvte.ComponentModel;
using Cvte.Windows.Notifications;

namespace Cvte.Paint.Framework
{
	public class NoneMode : Mode, IModeSwitching
	{
		private bool _hasNotified;

		internal NoneMode()
		{
		}

		public void OnSwitchingFrom(ModeSwitchingArgs args)
		{
			args.SuppressSourceModeUnselect = true;
		}

		public void OnSwitchingTo(ModeSwitchingArgs args)
		{
		}

		protected override void OnSwitchOn()
		{
			_hasNotified = false;
		}

		protected override async void OnDeviceUp(ModeInputEventArgs e)
		{
			if (!_hasNotified)
			{
				_hasNotified = true;
				if (DebuggingProperties.IsDebug)
				{
					await DialogService.Current.ShowAsync("当前 Mode=NoneMode，这是意料之内的吗？", "可能尚未初始化");
				}
			}
		}
	}
}
