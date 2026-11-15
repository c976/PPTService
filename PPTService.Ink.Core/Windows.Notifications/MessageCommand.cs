using System;
using System.Windows.Input;

namespace PPTService.Ink.Core.Windows.Notifications
{
	public sealed class MessageCommand : IMessageCommand, ICommand
	{
		public static readonly MessageCommand Yes = new MessageCommand(MessageResult.Positive);

		public static readonly MessageCommand Ok = new MessageCommand(MessageResult.Positive);

		public static readonly MessageCommand No = new MessageCommand(MessageResult.Negative);

		public static readonly MessageCommand Cancel = new MessageCommand(MessageResult.Canceled);

		public string Label { get; set; }

		public MessageResult Result { get; }

		public bool IsDefault { get; set; }

		public bool IsCancel { get; set; }

		public Action<object> Action { get; }

		MessageResult IMessageCommand.Result
		{
			get
			{
				return Result;
			}
			set
			{
				throw new NotSupportedException("MessageCommand 不允许更改执行结果。");
			}
		}

		private event EventHandler CanExecuteChanged;

		event EventHandler ICommand.CanExecuteChanged
		{
			add
			{
				CanExecuteChanged += value;
			}
			remove
			{
				CanExecuteChanged -= value;
			}
		}

		public MessageCommand(MessageResult result)
		{
			Result = result;
		}

		public MessageCommand(MessageResult result, Action action)
		{
			Result = result;
			Action = (object o) =>
			{
				action();
			};
		}

		public MessageCommand(MessageResult result, Action<object> action)
		{
			Result = result;
			Action = action;
		}

		bool ICommand.CanExecute(object parameter)
		{
			return true;
		}

		void ICommand.Execute(object parameter)
		{
			Action?.Invoke(parameter);
		}

		private void OnCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
