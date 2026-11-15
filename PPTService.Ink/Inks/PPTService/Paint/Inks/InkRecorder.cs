using System;
using PPTService.Paint.Framework;

namespace PPTService.Paint.Inks
{
	internal class InkRecorder
	{
		public Element Ink { get; }

		public int DeviceId { get; }

		public DateTime CreationTime { get; }

		public InkRecorder(Element ink, DateTime creationTime, int deviceId)
		{
			Ink = ink;
			CreationTime = creationTime;
			DeviceId = deviceId;
		}
	}
}
