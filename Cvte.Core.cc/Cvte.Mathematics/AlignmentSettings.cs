namespace Cvte.Mathematics
{
	public struct AlignmentSettings
	{
		public readonly RectHorizontalAlignment HorizontalAlignment;

		public readonly RectVerticalAlignment VerticalAlignment;

		public static AlignmentSettings Center = new AlignmentSettings(RectHorizontalAlignment.Center, RectVerticalAlignment.Center);

		public static AlignmentSettings TopCenter = new AlignmentSettings(RectHorizontalAlignment.Center, RectVerticalAlignment.Top);

		public static AlignmentSettings BottomCenter = new AlignmentSettings(RectHorizontalAlignment.Center, RectVerticalAlignment.Bottom);

		public static AlignmentSettings CenterLeft = new AlignmentSettings(RectHorizontalAlignment.Left, RectVerticalAlignment.Center);

		public static AlignmentSettings CenterRight = new AlignmentSettings(RectHorizontalAlignment.Right, RectVerticalAlignment.Center);

		public static AlignmentSettings TopLeft = new AlignmentSettings(RectHorizontalAlignment.Left, RectVerticalAlignment.Top);

		public static AlignmentSettings TopRight = new AlignmentSettings(RectHorizontalAlignment.Right, RectVerticalAlignment.Top);

		public static AlignmentSettings BottomLeft = new AlignmentSettings(RectHorizontalAlignment.Left, RectVerticalAlignment.Bottom);

		public static AlignmentSettings BottomRight = new AlignmentSettings(RectHorizontalAlignment.Right, RectVerticalAlignment.Bottom);

		public AlignmentSettings(RectHorizontalAlignment horizontalAlignment, RectVerticalAlignment verticalAlignment)
		{
			HorizontalAlignment = horizontalAlignment;
			VerticalAlignment = verticalAlignment;
		}
	}
}
