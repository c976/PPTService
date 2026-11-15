using System;
using System.Windows;

namespace PPTService.Paint.Inks
{
	internal class LegacyDevice
	{
		public int Id { get; set; }

		public DateTime DeathTime { get; set; }

		public Point DeathPosition { get; set; }
	}
}
