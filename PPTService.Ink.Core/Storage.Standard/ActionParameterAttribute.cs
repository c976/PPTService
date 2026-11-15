using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public class ActionParameterAttribute : Attribute
	{
		public string Name { get; set; }

		public ActionParameterAttribute(string name)
		{
			Name = name;
		}
	}
}
