using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Diagnostics.Annotations
{
	
	
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class MustUseReturnValueAttribute : Attribute
	{
		public string Justification { get; private set; }

		public MustUseReturnValueAttribute()
		{
		}

		public MustUseReturnValueAttribute([NotNull] string justification)
		{
			Justification = justification;
		}
	}
}
