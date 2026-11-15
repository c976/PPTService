using System;

namespace PPTService.Ink.Core.Diagnostics.Annotations
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
	public sealed class AspMvcViewComponentViewAttribute : Attribute
	{
	}
}
