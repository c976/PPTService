using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Diagnostics.Annotations
{
	
	
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class AspRequiredAttributeAttribute : Attribute
	{
		public string Attribute { get; private set; }

		public AspRequiredAttributeAttribute([NotNull] string attribute)
		{
			Attribute = attribute;
		}
	}
}
