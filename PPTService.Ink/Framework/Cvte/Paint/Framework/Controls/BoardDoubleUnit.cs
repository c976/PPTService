using System.Windows.Markup;

namespace Cvte.Paint.Framework.Controls
{
	public class BoardDoubleUnit : BoardUnitExtension
	{
		[ConstructorArgument("value")]
		public double DoubleValue { get; set; }

		protected override object Value => DoubleValue;

		public BoardDoubleUnit()
		{
		}

		public BoardDoubleUnit(double value)
		{
			DoubleValue = value;
		}
	}
}
