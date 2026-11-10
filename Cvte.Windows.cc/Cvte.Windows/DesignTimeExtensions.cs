using System.ComponentModel;
using System.Windows;

namespace Cvte.Windows
{
	public static class DesignTimeExtensions
	{
		public static bool IsDesignTimeWindow(this DependencyObject window)
		{
			if (DesignerProperties.GetIsInDesignMode(window))
			{
				string fullName = window.GetType().FullName;
				if (object.Equals("Microsoft.Expression.WpfPlatform.InstanceBuilders.WindowInstance", fullName) || object.Equals("Microsoft.VisualStudio.DesignTools.WpfDesigner.InstanceBuilders.WindowInstance", fullName))
				{
					return true;
				}
			}
			return false;
		}
	}
}
