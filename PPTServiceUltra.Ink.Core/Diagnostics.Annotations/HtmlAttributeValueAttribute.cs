using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Diagnostics.Annotations
{
	
	
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
	public sealed class HtmlAttributeValueAttribute : Attribute
	{
		[NotNull]
		public string Name { get; private set; }

		public HtmlAttributeValueAttribute([NotNull] string name)
		{
			Name = name;
		}
	}
}
