namespace Cvte.Paint.Framework
{
	public abstract class EventConverter : IEventConverter
	{
		public Slide CurrentSlide { get; set; }

		protected EventConverter(InputProcessor inputProcessor)
		{
			inputProcessor.AddEventConverter(this);
		}

		void IEventConverter.InputStarting(ModeInputEventArgs e)
		{
			OnInputStarting(e);
		}

		void IEventConverter.InputCompleted(ModeInputEventArgs e)
		{
			OnInputCompleted(e);
		}

		void IEventConverter.DeviceDown(ModeInputEventArgs e)
		{
			OnDeviceDown(e);
		}

		void IEventConverter.DeviceMove(ModeInputEventArgs e)
		{
			OnDeviceMove(e);
		}

		void IEventConverter.DeviceUp(ModeInputEventArgs e)
		{
			OnDeviceUp(e);
		}

		void IEventConverter.SwitchOn()
		{
			OnSwitchOn();
		}

		void IEventConverter.SwitchOff()
		{
			OnSwitchOff();
		}

		protected virtual void OnInputStarting(ModeInputEventArgs e)
		{
		}

		protected virtual void OnInputCompleted(ModeInputEventArgs e)
		{
		}

		protected virtual void OnDeviceDown(ModeInputEventArgs e)
		{
		}

		protected virtual void OnDeviceMove(ModeInputEventArgs e)
		{
		}

		protected virtual void OnDeviceUp(ModeInputEventArgs e)
		{
		}

		protected virtual void OnSwitchOn()
		{
		}

		protected virtual void OnSwitchOff()
		{
		}
	}
}
