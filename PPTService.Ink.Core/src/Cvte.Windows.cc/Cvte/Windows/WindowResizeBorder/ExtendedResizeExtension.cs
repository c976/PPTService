using System;
using System.Windows;
using System.Windows.Markup;

namespace PPTService.Ink.Core.Windows.WindowResizeBorder
{
	public class ExtendedResizeExtension : MarkupExtension
	{
		public Thickness Radius { get; set; } = new Thickness(8.0);

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new ExtendedResizeBorder
			{
				Radius = Radius
			};
		}
	}
}
