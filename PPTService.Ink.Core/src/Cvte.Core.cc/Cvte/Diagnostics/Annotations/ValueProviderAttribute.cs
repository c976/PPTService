using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Diagnostics.Annotations
{
	
	
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
	public sealed class ValueProviderAttribute : Attribute
	{
		[NotNull]
		public string Name { get; private set; }

		public ValueProviderAttribute(string name)
		{
			Name = name;
		}
	}
}
