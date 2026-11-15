namespace PPTService.Ink.Core.Windows.Input
{
	public class DeviceInputDifferentialValue<T>
	{
		public T Mouse { get; set; }

		public T Touch { get; set; }

		public T Stylus { get; set; }

		public DeviceInputDifferentialValue(T uniformValue)
		{
			Mouse = (Touch = (Stylus = uniformValue));
		}

		public DeviceInputDifferentialValue(T mouse, T touch, T stylus)
		{
			Mouse = mouse;
			Touch = touch;
			Stylus = stylus;
		}
	}
}
