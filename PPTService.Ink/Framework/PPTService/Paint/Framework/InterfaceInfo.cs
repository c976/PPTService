using System;

namespace PPTService.Paint.Framework
{
	public class InterfaceInfo
	{
		public bool SingleInstance { get; set; }

		public Delegate CreateInterfaceHandler { get; set; }

		public object Instance { get; set; }
	}
}
