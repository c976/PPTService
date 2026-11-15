using System;

namespace PPTService.Ink.Core.Diagnostics.Annotations
{
	[Obsolete("Use [ContractAnnotation('=> halt')] instead")]
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class TerminatesProgramAttribute : Attribute
	{
	}
}
