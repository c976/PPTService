using System;

namespace Cvte.Diagnostics.Annotations
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
	public sealed class AspMvcActionSelectorAttribute : Attribute
	{
	}
}
