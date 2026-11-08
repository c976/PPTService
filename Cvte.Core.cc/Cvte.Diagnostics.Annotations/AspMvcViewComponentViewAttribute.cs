using System;

namespace Cvte.Diagnostics.Annotations
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
	public sealed class AspMvcViewComponentViewAttribute : Attribute
	{
	}
}
