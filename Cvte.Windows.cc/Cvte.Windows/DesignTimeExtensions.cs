using System.ComponentModel;
using System.Windows;

namespace Cvte.Windows
{
	public static class DesignTimeExtensions
	{
		public static bool IsDesignTimeWindow(this DependencyObject window)
		{
			string fullName = default(string);
			while (DesignerProperties.GetIsInDesignMode(window))
			{
				if (2 != 0)
				{
					fullName = window.GetType().FullName;
					if (7 == 0)
					{
						continue;
					}
					if (object.Equals("Microsoft.Expression.WpfPlatform.InstanceBuilders.WindowInstance", fullName))
					{
						goto IL_002d;
					}
				}
				if (!object.Equals("Microsoft.VisualStudio.DesignTools.WpfDesigner.InstanceBuilders.WindowInstance", fullName))
				{
					break;
				}
				goto IL_002d;
				IL_002d:
				return true;
			}
			return false;
		}
	}
}
