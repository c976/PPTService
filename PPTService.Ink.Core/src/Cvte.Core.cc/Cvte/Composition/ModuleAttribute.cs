using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Composition
{
	
	
	[AttributeUsage(AttributeTargets.Class)]
	[Obsolete("基础库 Cvte.Core 不具备实现 MefExportedTypeDetector 的能力，请寻找其他替代方案。")]
	public class ModuleAttribute : Attribute
	{
		public string[] DependsOn { get; set; }

		public ModuleAttribute()
			: this(new string[0])
		{
		}

		public ModuleAttribute(string[] dependesOnModules)
		{
			DependsOn = dependesOnModules;
		}
	}
}
