using System;
using Cvte.Paint.Framework;

namespace Cvte.Paint.Inks
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
