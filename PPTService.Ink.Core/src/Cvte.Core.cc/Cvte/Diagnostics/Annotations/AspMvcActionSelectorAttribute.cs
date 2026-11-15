using System;

namespace PPTService.Ink.Core.Diagnostics.Annotations
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
	public sealed class AspMvcActionSelectorAttribute : Attribute
	{
	}
}
