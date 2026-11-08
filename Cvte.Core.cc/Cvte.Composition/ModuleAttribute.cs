using System;
using System.ComponentModel.Composition;

namespace Cvte.Composition
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ModuleAttribute : ExportAttribute
	{
		public string[] DependsOn { get; set; }

		public ModuleAttribute()
			: this(new string[0])
		{
		}

		public ModuleAttribute(string[] dependesOnModules)
			: base(typeof(IModule))
		{
			DependsOn = dependesOnModules;
		}
	}
}
