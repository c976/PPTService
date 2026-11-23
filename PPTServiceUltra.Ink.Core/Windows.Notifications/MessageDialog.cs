using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Ink.Core.Windows.Notifications
{
	public sealed class MessageDialog
	{
		public string Title { get; private set; }

		public object Content { get; private set; }

		public Window OwnerWindow { get; set; }

		public TimeSpan Timeout { get; set; }

		public IList<IMessageCommand> Commands { get; set; } = MessageCommandCollection.Ok;

		public IDialogProvider Provider { get; set; }

		public MessageDialog([NotNull] object content)
		{
			Content = content;
		}

		public MessageDialog([NotNull] object content, string title)
			: this(content)
		{
			Title = title;
		}

		public async Task<IMessageCommand> ShowAsync()
		{
			return await (Provider ?? DialogService.Current).ShowAsync(this);
		}
	}
}
