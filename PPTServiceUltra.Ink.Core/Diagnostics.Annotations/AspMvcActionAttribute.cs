using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Diagnostics.Annotations
{
	
	
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
	public sealed class AspMvcActionAttribute : Attribute
	{
		public string AnonymousProperty { get; private set; }

		public AspMvcActionAttribute()
		{
		}

		public AspMvcActionAttribute(string anonymousProperty)
		{
			AnonymousProperty = anonymousProperty;
		}
	}
}
