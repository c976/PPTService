using System;

namespace Cvte.Diagnostics.Annotations
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
	public sealed class ProvidesContextAttribute : Attribute
	{
	}
}
