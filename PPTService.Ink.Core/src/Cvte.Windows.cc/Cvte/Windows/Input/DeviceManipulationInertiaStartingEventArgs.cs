using System.Windows.Input;

namespace PPTService.Ink.Core.Windows.Input
{
	public class DeviceManipulationInertiaStartingEventArgs : DeviceManipulationEventArgs<ManipulationInertiaStartingEventArgs>
	{
		public DeviceManipulationInertiaStartingEventArgs(ManipulationInertiaStartingEventArgs args)
			: base((InputEventArgs)args)
		{
		}
	}
}
