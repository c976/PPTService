using System;

namespace PPTServiceUltra.Ink.Core.Mathematics
{
	[Obsolete("相关类型无法实现跨平台，因此请使用 CoreLib.Windows 中的同名类型。")]
	public struct AlignmentSettings
	{
		public RectHorizontalAlignment HorizontalAlignment;

		public RectVerticalAlignment VerticalAlignment;

		public static readonly AlignmentSettings Center = new AlignmentSettings(RectHorizontalAlignment.Center, RectVerticalAlignment.Center);

		public static readonly AlignmentSettings TopCenter = new AlignmentSettings(RectHorizontalAlignment.Center, RectVerticalAlignment.Top);

		public static readonly AlignmentSettings BottomCenter = new AlignmentSettings(RectHorizontalAlignment.Center, RectVerticalAlignment.Bottom);

		public static readonly AlignmentSettings CenterLeft = new AlignmentSettings(RectHorizontalAlignment.Left, RectVerticalAlignment.Center);

		public static readonly AlignmentSettings CenterRight = new AlignmentSettings(RectHorizontalAlignment.Right, RectVerticalAlignment.Center);

		public static readonly AlignmentSettings TopLeft = new AlignmentSettings(RectHorizontalAlignment.Left, RectVerticalAlignment.Top);

		public static readonly AlignmentSettings TopRight = new AlignmentSettings(RectHorizontalAlignment.Right, RectVerticalAlignment.Top);

		public static readonly AlignmentSettings BottomLeft = new AlignmentSettings(RectHorizontalAlignment.Left, RectVerticalAlignment.Bottom);

		public static readonly AlignmentSettings BottomRight = new AlignmentSettings(RectHorizontalAlignment.Right, RectVerticalAlignment.Bottom);

		public AlignmentSettings(RectHorizontalAlignment horizontalAlignment, RectVerticalAlignment verticalAlignment)
		{
			HorizontalAlignment = horizontalAlignment;
			VerticalAlignment = verticalAlignment;
		}
	}
}
