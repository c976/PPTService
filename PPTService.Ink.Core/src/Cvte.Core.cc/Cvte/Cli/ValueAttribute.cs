using System;

namespace PPTService.Ink.Core.Cli
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class ValueAttribute : CommandLinePropertyAttribute
	{
		public int Index { get; }

		public int Length { get; }

		public ValueAttribute(int index)
		{
			Index = index;
			Length = 1;
		}

		public ValueAttribute(int index, int length)
		{
			if (index < 0)
			{
				throw new ArgumentException("命令中值的位置必须大于或等于 0。", "length");
			}
			if (length <= 0)
			{
				throw new ArgumentException("命令中值的长度必须大于或等于 1。", "length");
			}
			Index = index;
			Length = length;
		}
	}
}
