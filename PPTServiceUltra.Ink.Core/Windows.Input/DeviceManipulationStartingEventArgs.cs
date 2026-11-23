using System.Windows.Input;

namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public class DeviceManipulationStartingEventArgs : DeviceManipulationEventArgs<ManipulationStartingEventArgs>
	{
		public DeviceManipulationStartingEventArgs(ManipulationStartingEventArgs args)
			: base((InputEventArgs)args)
		{
		}
	}
}
