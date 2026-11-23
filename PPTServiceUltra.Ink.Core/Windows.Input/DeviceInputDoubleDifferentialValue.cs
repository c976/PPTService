using System.ComponentModel;

namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	[TypeConverter(typeof(DeviceInputDoubleDifferentialValueConverter))]
	public sealed class DeviceInputDoubleDifferentialValue : DeviceInputDifferentialValue<double>
	{
		public static readonly DeviceInputDoubleDifferentialValue DragTolerance = new DeviceInputDoubleDifferentialValue(1.0, 16.0, 4.0);

		public static readonly DeviceInputDoubleDifferentialValue HitTestTolerance = new DeviceInputDoubleDifferentialValue(3.0, 10.0, 5.0);

		public DeviceInputDoubleDifferentialValue(double uniformValue)
			: base(uniformValue)
		{
		}

		public DeviceInputDoubleDifferentialValue(double mouse, double touch, double stylus)
			: base(mouse, touch, stylus)
		{
		}
	}
}
