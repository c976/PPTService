using System.Windows.Input;

namespace PPTService.Ink.Core.Windows.Input
{
	public class DeviceManipulationCompletedEventArgs : DeviceManipulationEventArgs<ManipulationCompletedEventArgs>
	{
		public DeviceManipulationCompletedEventArgs(ManipulationCompletedEventArgs args)
			: base((InputEventArgs)args)
		{
		}
	}
}
