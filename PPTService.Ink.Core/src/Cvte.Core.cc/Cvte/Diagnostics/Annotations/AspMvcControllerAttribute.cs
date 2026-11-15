using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Diagnostics.Annotations
{
	
	
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
	public sealed class AspMvcControllerAttribute : Attribute
	{
		public string AnonymousProperty { get; private set; }

		public AspMvcControllerAttribute()
		{
		}

		public AspMvcControllerAttribute(string anonymousProperty)
		{
			AnonymousProperty = anonymousProperty;
		}
	}
}
