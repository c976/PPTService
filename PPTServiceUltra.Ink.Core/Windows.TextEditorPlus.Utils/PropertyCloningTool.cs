using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils
{
	internal static class PropertyCloningTool
	{
		public static T Clone<T>(T freezable) where T : Freezable
		{
			if (freezable == null)
			{
				return null;
			}
			if (freezable.IsFrozen || freezable.IsSealed)
			{
				return freezable;
			}
			return (T)freezable.Clone();
		}
	}
}
