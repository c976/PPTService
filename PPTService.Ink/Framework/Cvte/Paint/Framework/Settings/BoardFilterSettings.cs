namespace Cvte.Paint.Framework.Settings
{
	public abstract class BoardFilterSettings : BoardSettings
	{
		private bool _isEnabled = true;

		public bool IsEnabled
		{
			get
			{
				return _isEnabled;
			}
			set
			{
				_isEnabled = value;
				OnPropertyChanged("IsEnabled");
			}
		}
	}
}
