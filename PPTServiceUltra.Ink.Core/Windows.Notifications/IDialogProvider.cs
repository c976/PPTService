using System;
using System.Threading.Tasks;

namespace PPTServiceUltra.Ink.Core.Windows.Notifications
{
	public interface IDialogProvider
	{
		Task<IMessageCommand> ShowAsync(string text);

		[Obsolete("请使用 ShowAsync 替代。")]
		IMessageCommand Show(string text);

		Task<IMessageCommand> ShowAsync(string text, string title);

		[Obsolete("请使用 ShowAsync 替代。")]
		IMessageCommand Show(string text, string title);

		Task<IMessageCommand> ShowAsync(MessageDialog md);

		[Obsolete("请使用 ShowAsync 替代。")]
		IMessageCommand Show(MessageDialog md);

		void ShowInfo(string info);

		MessageResult ShowFileDialog(FileDialog fd);

		MessageResult ShowFolderDialog(FolderDialog properties);

		void ShowLoadingDialog(string message, int autoCloseSeconds = 3600);

		void CloseLoadingDialog();
	}
}
