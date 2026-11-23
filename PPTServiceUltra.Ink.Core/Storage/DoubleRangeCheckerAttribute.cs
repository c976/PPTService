using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DoubleRangeCheckerAttribute : SaveInfoMemberCheckerAttribute
	{
		public double Minimum { get; set; }

		public double Maximun { get; set; }

		
		public override object Check(object value)
		{
			if (!(value is double))
			{
				return value;
			}
			double num = (double)value;
			if (num > Maximun)
			{
				return Maximun;
			}
			if (num < Minimum)
			{
				return Minimum;
			}
			return value;
		}
	}
}
