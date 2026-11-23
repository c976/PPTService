using System;
using System.Windows;
using System.Windows.Input;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public class DeviceManipulationInputSource
	{
		public UIElement Target { get; }

		public event EventHandler<DeviceManipulationStartedEventArgs> ManipulationStarted;

		public event EventHandler<DeviceManipulationInertiaStartingEventArgs> ManipulationInertiaStarting;

		public event EventHandler<DeviceManipulationStartingEventArgs> ManipulationStarting;

		public event EventHandler<DeviceManipulationDeltaEventArgs> ManipulationDelta;

		public event EventHandler<DeviceManipulationCompletedEventArgs> ManipulationCompleted;

		public DeviceManipulationInputSource([NotNull] UIElement target)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			Target = target;
			Attach();
		}

		public void Attach()
		{
			UIElement target = Target;
			Detach();
			target.ManipulationStarted += OnManipulationStarted;
			target.ManipulationInertiaStarting += OnManipulationInertiaStarting;
			target.ManipulationStarting += OnManipulationStarting;
			target.ManipulationDelta += OnManipulationDelta;
			target.ManipulationCompleted += OnManipulationCompleted;
		}

		public void Detach()
		{
			UIElement target = Target;
			target.ManipulationStarted -= OnManipulationStarted;
			target.ManipulationInertiaStarting -= OnManipulationInertiaStarting;
			target.ManipulationStarting -= OnManipulationStarting;
			target.ManipulationDelta -= OnManipulationDelta;
			target.ManipulationCompleted -= OnManipulationCompleted;
		}

		private bool CanRespond()
		{
			return (bool)Target.GetValue(DeviceInputSource.CanRespondProperty);
		}

		private void OnRaising(DeviceManipulationEventArgs e)
		{
			if (!CanRespond())
			{
				e.VirtualType = VirtualDeviceType.LostCapture;
			}
		}

		private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
		{
			DeviceManipulationCompletedEventArgs e2 = new DeviceManipulationCompletedEventArgs(e);
			OnRaising(e2);
			ManipulationCompleted?.Invoke(sender, e2);
		}

		private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
		{
			DeviceManipulationDeltaEventArgs e2 = new DeviceManipulationDeltaEventArgs(e);
			OnRaising(e2);
			ManipulationDelta?.Invoke(sender, e2);
		}

		private void OnManipulationStarting(object sender, ManipulationStartingEventArgs e)
		{
			DeviceManipulationStartingEventArgs e2 = new DeviceManipulationStartingEventArgs(e);
			OnRaising(e2);
			ManipulationStarting?.Invoke(sender, e2);
		}

		private void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
		{
			DeviceManipulationInertiaStartingEventArgs e2 = new DeviceManipulationInertiaStartingEventArgs(e);
			OnRaising(e2);
			ManipulationInertiaStarting?.Invoke(sender, e2);
		}

		private void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
		{
			DeviceManipulationStartedEventArgs e2 = new DeviceManipulationStartedEventArgs(e);
			OnRaising(e2);
			ManipulationStarted?.Invoke(sender, e2);
		}
	}
}
