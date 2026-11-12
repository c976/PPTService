using System;

namespace PPTService.Core.Net.Http
{
	[AttributeUsage(AttributeTargets.Property)]
	public class IgnoreMemberWhenNullAttribute : Attribute
	{
	}
}
