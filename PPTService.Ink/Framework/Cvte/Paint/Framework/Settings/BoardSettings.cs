using System.ComponentModel;
using System.Runtime.CompilerServices;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework.Settings
{
	public abstract class BoardSettings : INotifyPropertyChanged
	{
		public Board Board { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public new BoardSettings MemberwiseClone()
		{
			return (BoardSettings)base.MemberwiseClone();
		}

		[NotifyPropertyChangedInvocator]
		protected bool SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (object.Equals(field, value))
			{
				return false;
			}
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
