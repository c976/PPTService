using System;

namespace Cvte.Diagnostics.Annotations
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class LocalizationRequiredAttribute : Attribute
	{
		public bool Required { get; private set; }

		public LocalizationRequiredAttribute()
			: this(true)
		{
		}

		public LocalizationRequiredAttribute(bool required)
		{
			Required = required;
		}
	}
}
