using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	[AttributeUsage(AttributeTargets.Property)]
	public class IntRangeCheckerAttribute : SaveInfoMemberCheckerAttribute
	{
		public int Minimum { get; set; }

		public int Maximun { get; set; }

		
		public override object Check(object value)
		{
			if (!(value is int))
			{
				return value;
			}
			int num = (int)value;
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
