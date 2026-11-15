using System;

namespace PPTService.Ink.Core.Diagnostics.Annotations
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public sealed class AspMvcSuppressViewErrorAttribute : Attribute
	{
	}
}
