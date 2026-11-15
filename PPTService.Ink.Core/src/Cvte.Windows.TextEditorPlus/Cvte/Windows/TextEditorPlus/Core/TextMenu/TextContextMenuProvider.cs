using System.Windows;
using System.Windows.Controls;
using PPTService.Ink.Core.Windows.TextEditorPlus.Editing;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Core.TextMenu
{
	public abstract class TextContextMenuProvider
	{
		public static readonly DependencyProperty CustomTextContextMenuProviderProperty = DependencyProperty.RegisterAttached("CustomTextContextMenuProvider", typeof(TextContextMenuProvider), typeof(TextContextMenuProvider), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

		public abstract ContextMenu BuildContextMenu(TextArea textArea);

		public static void SetCustomTextContextMenu(DependencyObject element, TextContextMenuProvider value)
		{
			element.SetValue(CustomTextContextMenuProviderProperty, value);
		}

		public static TextContextMenuProvider GetCustomTextContextMenu(DependencyObject element)
		{
			return (TextContextMenuProvider)element.GetValue(CustomTextContextMenuProviderProperty);
		}
	}
}
