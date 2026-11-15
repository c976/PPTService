using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Ink.Core.Windows.Notifications
{
	public class DialogProvider : IDialogProvider
	{
		protected virtual Window GetWindow(DependencyObject owner)
		{
			if (owner != null)
			{
				return Window.GetWindow(owner);
			}
			return null;
		}

		public Task<IMessageCommand> ShowAsync([NotNull] string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException("text");
			}
			return ShowAsync(new MessageDialog(text));
		}

		[Obsolete("请使用 ShowAsync 替代。")]
		public IMessageCommand Show([NotNull] string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException("text");
			}
			return Show(new MessageDialog(text));
		}

		public Task<IMessageCommand> ShowAsync([NotNull] string text, string title)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException("text");
			}
			return ShowAsync(new MessageDialog(text, title));
		}

		[Obsolete("请使用 ShowAsync 替代。")]
		public IMessageCommand Show([NotNull] string text, string title)
		{
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentNullException("text");
			}
			return Show(new MessageDialog(text, title));
		}

		public virtual async Task<IMessageCommand> ShowAsync([NotNull] MessageDialog md)
		{
			return await Dispatcher.CurrentDispatcher.InvokeAsync(() => DefaultShow(md));
		}

		[Obsolete("请使用 ShowAsync 替代。")]
		public virtual IMessageCommand Show([NotNull] MessageDialog md)
		{
			return DefaultShow(md);
		}

		private IMessageCommand DefaultShow([NotNull] MessageDialog md)
		{
			if (md == null)
			{
				throw new ArgumentNullException("md");
			}
			bool flag = md.Commands.Any((IMessageCommand x) => object.Equals(x.Result, MessageResult.Positive));
			bool flag2 = md.Commands.Any((IMessageCommand x) => object.Equals(x.Result, MessageResult.Negative));
			bool flag3 = md.Commands.Any((IMessageCommand x) => object.Equals(x.Result, MessageResult.Canceled));
			MessageBoxButton? messageBoxButton;
			if (flag & flag2 & flag3)
			{
				messageBoxButton = MessageBoxButton.YesNoCancel;
			}
			else if (flag & flag3)
			{
				messageBoxButton = MessageBoxButton.OKCancel;
			}
			else if (flag & flag2)
			{
				messageBoxButton = MessageBoxButton.YesNo;
			}
			else
			{
				messageBoxButton = ((flag || flag2 || flag3) ? new MessageBoxButton?(MessageBoxButton.OK) : ((MessageBoxButton?)null));
			}
			if (!messageBoxButton.HasValue)
			{
				throw new NotImplementedException();
			}
			MessageBoxResult result = System.Windows.MessageBox.Show(md.Content.ToString(), md.Title, messageBoxButton.Value);
			return ConvertResult(result, md.Commands);
		}

		private static IMessageCommand ConvertResult(MessageBoxResult result, IEnumerable<IMessageCommand> mcc)
		{
			switch (result)
			{
			case MessageBoxResult.OK:
			case MessageBoxResult.Yes:
				return mcc.First((IMessageCommand x) => object.Equals(x.Result, MessageResult.Positive));
			case MessageBoxResult.No:
				return mcc.First((IMessageCommand x) => object.Equals(x.Result, MessageResult.Negative));
			case MessageBoxResult.Cancel:
				return mcc.First((IMessageCommand x) => object.Equals(x.Result, MessageResult.Canceled));
			default:
				return null;
			}
		}

		public virtual void ShowInfo(string info)
		{
			throw new NotImplementedException();
		}

		public virtual MessageResult ShowFileDialog(FileDialog fd)
		{
			if (fd.Type == FileDialogType.OpenFile)
			{
				OpenFileDialog openFileDialog = new OpenFileDialog
				{
					AddExtension = fd.AddExtension,
					DefaultExt = fd.DefaultExt,
					FileName = fd.FileName,
					Filter = fd.Filter,
					FilterIndex = fd.FilterIndex,
					InitialDirectory = fd.InitialDirectory,
					Multiselect = fd.Multiselect,
					Title = fd.Title
				};
				using (openFileDialog)
				{
					MessageResult result = ConvertResult(openFileDialog.ShowDialog());
					fd.FileName = openFileDialog.FileName;
					fd.FileNames = openFileDialog.FileNames;
					fd.FilterIndex = openFileDialog.FilterIndex;
					return result;
				}
			}
			if (fd.Type == FileDialogType.SaveFile)
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog
				{
					AddExtension = fd.AddExtension,
					DefaultExt = fd.DefaultExt,
					FileName = fd.FileName,
					Filter = fd.Filter,
					FilterIndex = fd.FilterIndex,
					InitialDirectory = fd.InitialDirectory
				};
				using (saveFileDialog)
				{
					MessageResult result2 = ConvertResult(saveFileDialog.ShowDialog());
					fd.FileName = saveFileDialog.FileName;
					fd.FileNames = saveFileDialog.FileNames;
					fd.SafeFileName = saveFileDialog.FileName;
					fd.SafeFileNames = saveFileDialog.FileNames;
					fd.FilterIndex = saveFileDialog.FilterIndex;
					return result2;
				}
			}
			throw new NotImplementedException();
		}

		internal static MessageResult ConvertResult(DialogResult result)
		{
			switch (result)
			{
			case DialogResult.OK:
			case DialogResult.Yes:
				return MessageResult.Positive;
			case DialogResult.No:
				return MessageResult.Negative;
			default:
				return MessageResult.Canceled;
			}
		}

		public virtual MessageResult ShowFolderDialog(FolderDialog properties)
		{
			FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
			{
				ShowNewFolderButton = true,
				SelectedPath = properties.SelectedPath,
				Description = properties.Description
			};
			using (folderBrowserDialog)
			{
				MessageResult result = ConvertResult(folderBrowserDialog.ShowDialog());
				properties.SelectedPath = folderBrowserDialog.SelectedPath;
				return result;
			}
		}

		public virtual void ShowLoadingDialog(string title, int autoCloseSeconds = 3600)
		{
		}

		private void ShowLoadingDialogInternal(string message, bool isMinimized, bool isMaximized, Rect mainWindowScreenBounds, double mainWindowWidth, double mainWindowHeight, Rect mainWindowActualBounds, int autoCloseSeconds)
		{
		}

		public void CloseLoadingDialog()
		{
		}

		protected virtual Window CreateLoadingWindow()
		{
			return null;
		}
	}
}
