using System;

namespace PPTServiceUltra.Ink.Core.Diagnostics.Annotations
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.Delegate)]
	public sealed class NotNullAttribute : Attribute
	{
	}
}
