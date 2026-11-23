using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Cli
{
	
	
	public abstract class CommandLineAttribute : Attribute
	{
		public string Description
		{
			get;
			[param: DisallowNull]
			set;
		}

		public string LocalizableDescription
		{
			get;
			[param: DisallowNull]
			set;
		}

		internal CommandLineAttribute()
		{
		}
	}
}
