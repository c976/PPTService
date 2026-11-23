using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class SaveInfoExtensionAttribute : Attribute
	{
		public string Name { get; }

		public string Description { get; set; }

		public Type TargetType { get; }

		public SaveInfoExtensionAttribute(string name, Type targetType)
		{
			Name = name;
			TargetType = targetType;
		}
	}
}
