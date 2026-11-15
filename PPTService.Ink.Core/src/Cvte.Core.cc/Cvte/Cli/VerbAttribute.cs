using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Cli
{
	
	
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class VerbAttribute : CommandLineAttribute
	{
		public string VerbName { get; }

		public VerbAttribute(string verbName)
		{
			if (verbName == null)
			{
				throw new ArgumentNullException("verbName");
			}
			if (string.IsNullOrWhiteSpace(verbName))
			{
				throw new ArgumentException("命令行谓词不能是空白字符串。", "verbName");
			}
			VerbName = verbName;
		}
	}
}
