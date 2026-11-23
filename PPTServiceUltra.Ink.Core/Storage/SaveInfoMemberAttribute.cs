using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	[AttributeUsage(AttributeTargets.Property)]
	public class SaveInfoMemberAttribute : Attribute
	{
		public string Name { get; set; }

		public bool IsRuntime { get; set; }

		public object DefaultValue { get; set; }

		public string Description { get; set; }

		public bool IsRequired { get; set; }

		public string[] Aliases { get; set; }

		public bool IsDeltaSupported { get; set; } = true;

		public bool IsFallbackArray { get; set; }

		public SaveInfoMemberAttribute(string name)
		{
			Name = name;
		}
	}
}
