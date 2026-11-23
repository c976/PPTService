using System.Windows.Input;

namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public class DeviceManipulationInertiaStartingEventArgs : DeviceManipulationEventArgs<ManipulationInertiaStartingEventArgs>
	{
		public DeviceManipulationInertiaStartingEventArgs(ManipulationInertiaStartingEventArgs args)
			: base((InputEventArgs)args)
		{
		}
	}
}
