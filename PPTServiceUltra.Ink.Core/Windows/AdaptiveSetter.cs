using System;
using System.ComponentModel;
using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows
{
	public class AdaptiveSetter : Setter
	{
		internal DependencyObject Target { get; set; }

		internal Func<object> InternalValue { get; set; }

		[DefaultValue(null)]
		public new string TargetName
		{
			get
			{
				return base.TargetName;
			}
			set
			{
				base.TargetName = value;
			}
		}

		public object GetValue()
		{
			return InternalValue?.Invoke() ?? Value;
		}
	}
}
