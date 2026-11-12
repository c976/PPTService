using System.Windows.Media;

namespace Cvte.Paint.Framework
{
	public static class DashCollection
	{
		public static readonly DoubleCollection Normal = new DoubleCollection();

		public static readonly DoubleCollection Dash = new DoubleCollection { 2.0, 2.0 };

		public static readonly DoubleCollection DotDash = new DoubleCollection { 5.0, 3.0 };

		public static readonly DoubleCollection LongDash = new DoubleCollection { 10.0, 2.0, 2.0, 2.0 };
	}
}
