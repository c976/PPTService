using System.Windows.Input;

namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public class DeviceManipulationStartedEventArgs : DeviceManipulationEventArgs<ManipulationStartedEventArgs>
	{
		public DeviceManipulationStartedEventArgs(ManipulationStartedEventArgs args)
			: base((InputEventArgs)args)
		{
		}
	}
}
