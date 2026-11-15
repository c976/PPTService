using System.Windows.Input;

namespace PPTService.Ink.Core.Windows.Input
{
	public class DelegatedCommandBinding : CommandBinding
	{
		public ICommand DelegatedCommand { get; set; }

		public DelegatedCommandBinding()
		{
			Executed += DelegatedCommandBinding_Executed;
			CanExecute += DelegatedCommandBinding_CanExecute;
		}

		private void DelegatedCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if (DelegatedCommand != null)
			{
				e.CanExecute = DelegatedCommand.CanExecute(e.Parameter);
			}
		}

		private void DelegatedCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			DelegatedCommand?.Execute(e.Parameter);
		}
	}
}
