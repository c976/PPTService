using System.Windows.Input;

namespace PPTService.Ink.Core.Windows.Notifications
{
	public interface IMessageCommand : ICommand
	{
		string Label { get; set; }

		MessageResult Result { get; set; }
	}
}
