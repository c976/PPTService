using System.Windows.Input;

namespace PPTService.Ink.Core.Windows.Input
{
	public class DeviceManipulationEventArgs
	{
		public VirtualDeviceType VirtualType { get; set; }

		public InputEventArgs OriginEventArgs { get; set; }

		public DeviceManipulationEventArgs(InputEventArgs args)
		{
			OriginEventArgs = args;
		}
	}
	public class DeviceManipulationEventArgs<T> : DeviceManipulationEventArgs where T : InputEventArgs
	{
		public new T OriginEventArgs
		{
			get
			{
				return (T)base.OriginEventArgs;
			}
			set
			{
				base.OriginEventArgs = value;
			}
		}

		public DeviceManipulationEventArgs(InputEventArgs args)
			: base(args)
		{
		}
	}
}
