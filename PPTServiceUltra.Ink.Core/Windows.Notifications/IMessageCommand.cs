using System.Windows.Input;

namespace PPTServiceUltra.Ink.Core.Windows.Notifications
{
	public interface IMessageCommand : ICommand
	{
		string Label { get; set; }

		MessageResult Result { get; set; }
	}
}
