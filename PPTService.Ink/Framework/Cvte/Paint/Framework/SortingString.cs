using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework
{
	public class SortingString : INotifyPropertyChanged, IComparable, IComparable<SortingString>
	{
		public string Content { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public SortingString(string content)
		{
			Content = content;
		}

		public override string ToString()
		{
			return Content;
		}

		public int CompareTo(object obj)
		{
			if (this == obj)
			{
				return 0;
			}
			if (obj == null)
			{
				return 1;
			}
			return CompareTo((SortingString)obj);
		}

		public int CompareTo(SortingString other)
		{
			if (this == other)
			{
				return 0;
			}
			if (other == null)
			{
				return 1;
			}
			string content = GetContent(Content);
			string content2 = GetContent(other.Content);
			int num = NumberStringComparer.CompareNumber(content, content2);
			if (num != 0)
			{
				return num;
			}
			return string.Compare(content, content2, StringComparison.CurrentCulture);
		}

		protected virtual string GetContent(string content)
		{
			return content;
		}

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
