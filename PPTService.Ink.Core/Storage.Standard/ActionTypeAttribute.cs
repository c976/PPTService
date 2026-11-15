using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public class ActionTypeAttribute : Attribute
	{
		public string Key { get; set; }

		public ActionTypeAttribute(string key)
		{
			Key = key;
		}
	}
}
