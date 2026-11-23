using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	[AttributeUsage(AttributeTargets.Property)]
	public abstract class SaveInfoMemberCheckerAttribute : Attribute
	{
		
		public abstract object Check(object value);
	}
}
