using System.Collections.Generic;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	internal static class GeometryPoolExtensions
	{
		public static void DirtyGeometriesFromCharChanged(this GeometryPool geometryPool, CharChangeEventArgs args)
		{
			geometryPool.RemoveRangeCharGeometriesInfo(args.Offset, args.RemoveLength);
			if (args.InsertLength > 0)
			{
				List<CharGeometryInfo> charGeometriesInfos = geometryPool.CreateCharGeometriesInfos(args.InsertLength);
				geometryPool.InsertRangeCharGeometriesInfos(args.Offset, charGeometriesInfos);
			}
		}

		public static void DirtyGeometriesFromStyleChanged(this GeometryPool geometryPool, StyleChangeEventArgs args)
		{
			geometryPool.SetDirty(args.ChangedSegment.FrontOffset, args.ChangedSegment.Length);
		}
	}
}
