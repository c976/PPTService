using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PPTServiceUltra.Ink.Core.Windows.Input;
using PPTServiceUltra.Ink.Core.Windows.Localization;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Editing
{
	internal static class MenuItemExtension
	{
		internal static MenuItem Clone(this MenuItem menuItem)
		{
			MenuItem menuItem2 = new MenuItem
			{
				Command = menuItem.Command,
				Header = menuItem.Header,
				Style = menuItem.Style,
				InputGestureText = menuItem.InputGestureText
			};
			foreach (MenuItem item in (IEnumerable)menuItem.Items)
			{
				menuItem2.Items.Add(item.Clone());
			}
			return menuItem2;
		}

		internal static List<MenuItem> GetHyperlinkMenuItem(TextArea textArea, Style menuItemStyle)
		{
			List<MenuItem> list = new List<MenuItem>();
			if (HyperlinkHelper.CanAddHyperlink(textArea))
			{
				list.Add(new MenuItem
				{
					Header = Lang.Get("Lang.TextEditor.AddHyperlink"),
					Style = menuItemStyle,
					Command = new DelegateCommand(textArea.OnHyperlinkAdded),
					InputGestureText = "Ctrl+K"
				});
			}
			if (HyperlinkHelper.CanEditHyperlink(textArea))
			{
				list.Add(new MenuItem
				{
					Header = Lang.Get("Lang.TextEditor.EditHyperlink"),
					Style = menuItemStyle,
					Command = new DelegateCommand(textArea.OnHyperlinkEdited),
					InputGestureText = "Ctrl+K"
				});
			}
			if (HyperlinkHelper.CanRemoveHyperlink(textArea))
			{
				list.Add(new MenuItem
				{
					Header = Lang.Get("Lang.TextEditor.RemoveHyperlink"),
					Command = SetFormatCommands.RemoveHyperlinkCommand,
					Style = menuItemStyle
				});
			}
			if (HyperlinkHelper.CanRaiseHyperlink(textArea) && textArea.DocumentManager.CurrentFeature.HasFeature(Features.ContextMenuOpenHyperlinkInner))
			{
				list.Add(new MenuItem
				{
					Header = Lang.Get("Lang.TextEditor.OpenHyperlink"),
					Command = SetFormatCommands.RaiseHyperlinkCommand,
					Style = menuItemStyle
				});
			}
			return list;
		}

		internal static void AddNormalMenuItem(ContextMenu textContextMenu, Feature currentFeature, Style menuItemStyle)
		{
			if (currentFeature.HasFeature(Features.CopyInner))
			{
				textContextMenu.Items.Add(new MenuItem
				{
					Header = Lang.Get("Lang.TextEditor.Copy"),
					Command = ApplicationCommands.Copy,
					Style = menuItemStyle,
					InputGestureText = "Ctrl+C"
				});
			}
			if (currentFeature.HasFeature(Features.PasteInner))
			{
				textContextMenu.Items.Add(new MenuItem
				{
					Header = Lang.Get("Lang.TextEditor.Paste"),
					Command = ApplicationCommands.Paste,
					Style = menuItemStyle,
					InputGestureText = "Ctrl+V"
				});
			}
			if (currentFeature.HasFeature(Features.CutInner))
			{
				textContextMenu.Items.Add(new MenuItem
				{
					Header = Lang.Get("Lang.TextEditor.Cut"),
					Command = ApplicationCommands.Cut,
					Style = menuItemStyle,
					InputGestureText = "Ctrl+X"
				});
			}
		}
	}
}
