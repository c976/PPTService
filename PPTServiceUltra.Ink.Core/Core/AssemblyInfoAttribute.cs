using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core
{
	
	
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class AssemblyInfoAttribute : Attribute
	{
		public string Key { get; set; }

		public string Value { get; set; }

		public AssemblyInfoAttribute(string key, string value)
		{
			Key = key;
			Value = value;
		}
	}
}
