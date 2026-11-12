using System.Windows;
using System.Windows.Markup;

namespace Cvte.Paint.Framework.Controls
{
	public class BoardThicknessUnit : BoardUnitExtension
	{
		[ConstructorArgument("value")]
		public Thickness ThicknessValue { get; set; }

		protected override object Value => ThicknessValue;

		public BoardThicknessUnit()
		{
		}

		public BoardThicknessUnit(Thickness value)
		{
			ThicknessValue = value;
		}
	}
}
