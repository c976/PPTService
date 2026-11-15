using System;

namespace PPTService.Ink.Core.Diagnostics.Annotations
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	public sealed class CannotApplyEqualityOperatorAttribute : Attribute
	{
	}
}
