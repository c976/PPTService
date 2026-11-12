using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Cvte.Paint.Inks.HardPenSlideWritingMode_
{
	[Obsolete("这个类还没实现完成，暂时放在这里")]
	public class HardPenDynamicRenderer : DynamicRenderer
	{
		public bool IsInkInBoard { get; }

		public Matrix TransformFromBoard { get; set; }

		public HardPenDynamicRenderer(InkCanvas inkCanvas, bool isInkInBoard)
			: base(inkCanvas)
		{
			IsInkInBoard = isInkInBoard;
		}

		protected override void OnDown(StylusPointCollectionInfo stylusPointCollectionInfo)
		{
			if (IsInkInBoard)
			{
				CollectPoints(stylusPointCollectionInfo.StylusPlugInCollection);
			}
			base.OnDown(stylusPointCollectionInfo);
		}

		protected override void OnMove(StylusPointCollectionInfo stylusPointCollectionInfo)
		{
			if (IsInkInBoard)
			{
				CollectPoints(stylusPointCollectionInfo.StylusPlugInCollection);
			}
			base.OnMove(stylusPointCollectionInfo);
		}

		protected override void OnUp(StylusPointCollectionInfo stylusPointCollectionInfo)
		{
			if (IsInkInBoard)
			{
				CollectPoints(stylusPointCollectionInfo.StylusPlugInCollection);
			}
			base.OnUp(stylusPointCollectionInfo);
		}

		private void CollectPoints(StylusPointCollection stylusPlugInCollection)
		{
			TransformPoints(stylusPlugInCollection, TransformFromBoard);
			Matrix transformFromBoard = TransformFromBoard;
			transformFromBoard.Invert();
			TransformPoints(stylusPlugInCollection, transformFromBoard);
		}

		private void TransformPoints(StylusPointCollection points, Matrix matrix)
		{
			for (int i = 0; i < points.Count; i++)
			{
				StylusPoint value = points[i];
				Point point = value.ToPoint();
				point = matrix.Transform(point);
				value.X = point.X;
				value.Y = point.Y;
				points[i] = value;
			}
		}
	}
}
