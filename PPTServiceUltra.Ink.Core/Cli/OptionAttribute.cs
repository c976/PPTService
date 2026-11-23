using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Cli
{
	
	
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class OptionAttribute : CommandLinePropertyAttribute
	{
		public char? ShortName { get; }

		public string LongName { get; }

		public OptionAttribute()
		{
		}

		
		public OptionAttribute(string longName)
		{
			if (string.IsNullOrWhiteSpace(longName))
			{
				throw new ArgumentException("命令行参数的长名称不应该被指定为 null 或空字符串。", "longName");
			}
			LongName = longName;
		}

		
		public OptionAttribute(char shortName, string longName)
		{
			if (string.IsNullOrWhiteSpace(longName))
			{
				throw new ArgumentException("命令行参数的长名称不应该被指定为 null 或空字符串。", "longName");
			}
			ShortName = shortName;
			LongName = longName;
		}
	}
}
