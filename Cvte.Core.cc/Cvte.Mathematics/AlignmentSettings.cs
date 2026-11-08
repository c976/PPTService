namespace Cvte.Mathematics
{
	public struct AlignmentSettings
	{
		public readonly RectHorizontalAlignment HorizontalAlignment;

		public readonly RectVerticalAlignment VerticalAlignment;

		public static AlignmentSettings Center;

		public static AlignmentSettings TopCenter;

		public static AlignmentSettings BottomCenter;

		public static AlignmentSettings CenterLeft;

		public static AlignmentSettings CenterRight;

		public static AlignmentSettings TopLeft;

		public static AlignmentSettings TopRight;

		public static AlignmentSettings BottomLeft;

		public static AlignmentSettings BottomRight;

		public AlignmentSettings(RectHorizontalAlignment horizontalAlignment, RectVerticalAlignment verticalAlignment)
		{
			HorizontalAlignment = horizontalAlignment;
			VerticalAlignment = verticalAlignment;
		}

		static AlignmentSettings()
		{
			if (4 == 0)
			{
				goto IL_0039;
			}
			int num = 2;
			int num2;
			while (true)
			{
				num2 = 2;
				if (num2 == 0)
				{
					break;
				}
				Center = new AlignmentSettings((RectHorizontalAlignment)num, (RectVerticalAlignment)num2);
				TopCenter = new AlignmentSettings(RectHorizontalAlignment.Center, RectVerticalAlignment.Top);
				BottomCenter = new AlignmentSettings(RectHorizontalAlignment.Center, RectVerticalAlignment.Bottom);
				num = 0;
				if (num != 0)
				{
					continue;
				}
				goto IL_0025;
			}
			goto IL_0044;
			IL_0025:
			CenterLeft = new AlignmentSettings((RectHorizontalAlignment)num, RectVerticalAlignment.Center);
			if (4 != 0)
			{
				CenterRight = new AlignmentSettings(RectHorizontalAlignment.Right, RectVerticalAlignment.Center);
				goto IL_0039;
			}
			return;
			IL_0039:
			TopLeft = new AlignmentSettings(RectHorizontalAlignment.Left, RectVerticalAlignment.Top);
			num = 1;
			num2 = 0;
			goto IL_0044;
			IL_0044:
			if (num2 == 0)
			{
				TopRight = new AlignmentSettings((RectHorizontalAlignment)num, (RectVerticalAlignment)num2);
				BottomLeft = new AlignmentSettings(RectHorizontalAlignment.Left, RectVerticalAlignment.Bottom);
				num = 1;
				num2 = 1;
			}
			BottomRight = new AlignmentSettings((RectHorizontalAlignment)num, (RectVerticalAlignment)num2);
		}
	}
}
