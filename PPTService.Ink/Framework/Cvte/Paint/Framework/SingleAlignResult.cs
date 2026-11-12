using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Cvte.Paint.Framework
{
	[DebuggerDisplay("SingleAlignResult: IsAligned={IsAligned,nq}, Offset={Offset,nq}, Count={AlignmentData.Count,nq}")]
	public sealed class SingleAlignResult
	{
		public AlignmentDataCollection AlignmentData { get; }

		public bool IsAligned => AlignmentData.Any();

		public double Offset { get; }

		public SingleAlignResult()
		{
			AlignmentData = new AlignmentDataCollection();
		}

		public SingleAlignResult(double offset, IEnumerable<AlignmentData> data)
		{
			Offset = offset;
			AlignmentData = new AlignmentDataCollection();
			AlignmentData.Merge(data);
		}
	}
}
