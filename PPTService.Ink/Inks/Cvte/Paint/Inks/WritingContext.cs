using System;
using System.Collections.Generic;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Inks
{
	public class WritingContext
	{
		public DateTime WritingStartTime { get; }

		public int FirstTimestamp { get; internal set; }

		public int LatestTimestamp { get; internal set; }

		public RawStylusInput LatestStylusInput { get; internal set; }

		[CanBeNull]
		public StylusDevice StylusDevice { get; internal set; }

		internal WritingDynamicRenderer WritingDynamicRenderer { get; }

		public DrawingAttributes DrawingAttributes => WritingDynamicRenderer?.DrawingAttributes;

		internal int DeviceId { get; }

		public List<StylusPoint> CollectedStylusPoints { get; }

		internal bool IsDynamicUpTriggered { get; set; }

		public object Data { get; set; }

		internal bool IsCanceled { get; private set; }

		internal bool IsProcessedByUI { get; set; }

		internal WritingContext(RawStylusInput rawStylusInput, WritingDynamicRenderer writingDynamicRenderer)
		{
			FirstTimestamp = rawStylusInput.Timestamp;
			LatestTimestamp = rawStylusInput.Timestamp;
			WritingDynamicRenderer = writingDynamicRenderer;
			DeviceId = rawStylusInput.StylusDeviceId;
			CollectedStylusPoints = new List<StylusPoint>();
			WritingStartTime = DateTime.Now;
		}

		[Obsolete("此API极不稳定，请避免使用（纯东除外）。此方法仅在WritingStarting时有效。")]
		public void Cancel()
		{
			IsCanceled = true;
		}
	}
}
