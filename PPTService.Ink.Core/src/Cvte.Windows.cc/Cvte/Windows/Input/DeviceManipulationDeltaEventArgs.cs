using System.Windows.Input;

namespace PPTService.Ink.Core.Windows.Input
{
	public class DeviceManipulationDeltaEventArgs : DeviceManipulationEventArgs<ManipulationDeltaEventArgs>
	{
		public DeviceManipulationDeltaEventArgs(ManipulationDeltaEventArgs args)
			: base((InputEventArgs)args)
		{
		}
	}
}
