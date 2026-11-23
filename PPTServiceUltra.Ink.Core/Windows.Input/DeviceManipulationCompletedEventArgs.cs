using System.Windows.Input;

namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	public class DeviceManipulationCompletedEventArgs : DeviceManipulationEventArgs<ManipulationCompletedEventArgs>
	{
		public DeviceManipulationCompletedEventArgs(ManipulationCompletedEventArgs args)
			: base((InputEventArgs)args)
		{
		}
	}
}
