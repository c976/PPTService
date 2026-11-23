using System.Collections.ObjectModel;

namespace PPTServiceUltra.Ink.Core.Windows.Notifications
{
	public sealed class MessageCommandCollection : Collection<IMessageCommand>
	{
		public static readonly MessageCommandCollection YesNoCancel = new MessageCommandCollection
		{
			new MessageCommand(MessageResult.Positive)
			{
				IsDefault = true
			},
			new MessageCommand(MessageResult.Negative),
			new MessageCommand(MessageResult.Canceled)
			{
				IsCancel = true
			}
		};

		public static readonly MessageCommandCollection YesNo = new MessageCommandCollection
		{
			new MessageCommand(MessageResult.Positive)
			{
				IsDefault = true
			},
			new MessageCommand(MessageResult.Negative)
			{
				IsCancel = true
			}
		};

		public static readonly MessageCommandCollection OkCancel = new MessageCommandCollection
		{
			new MessageCommand(MessageResult.Positive)
			{
				IsDefault = true
			},
			new MessageCommand(MessageResult.Canceled)
			{
				IsCancel = true
			}
		};

		public static readonly MessageCommandCollection Ok = new MessageCommandCollection
		{
			new MessageCommand(MessageResult.Positive)
			{
				IsDefault = true,
				IsCancel = true
			}
		};

		public static MessageCommandCollection NotifyOnly = new MessageCommandCollection();
	}
}
