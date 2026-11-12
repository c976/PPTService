using System;
using System.Runtime.CompilerServices;

namespace PPTService.Core.Net.Http
{
	
	
	public class RestfulMemberAttribute : Attribute
	{
		public string Name { get; set; }
	}
}
