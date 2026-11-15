using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Diagnostics.Annotations
{
	
	
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class RazorImportNamespaceAttribute : Attribute
	{
		public string Name { get; private set; }

		public RazorImportNamespaceAttribute(string name)
		{
			Name = name;
		}
	}
}
