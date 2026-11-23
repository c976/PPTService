using System;

namespace PPTServiceUltra.Core.Net.Http
{
	[AttributeUsage(AttributeTargets.Property)]
	public class IgnoreMemberWhenNullAttribute : Attribute
	{
	}
}
