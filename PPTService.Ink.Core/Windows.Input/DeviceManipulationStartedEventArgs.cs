using System.Windows.Input;

namespace PPTService.Ink.Core.Windows.Input
{
	public class DeviceManipulationStartedEventArgs : DeviceManipulationEventArgs<ManipulationStartedEventArgs>
	{
		public DeviceManipulationStartedEventArgs(ManipulationStartedEventArgs args)
			: base((InputEventArgs)args)
		{
		}
	}
}
