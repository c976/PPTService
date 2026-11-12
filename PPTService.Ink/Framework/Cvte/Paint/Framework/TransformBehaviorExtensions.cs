namespace Cvte.Paint.Framework
{
	public static class TransformBehaviorExtensions
	{
		public static bool IsTranslation(this TransformBehavior behavior)
		{
			return behavior == TransformBehavior.Translate;
		}

		public static bool IsRotation(this TransformBehavior behavior)
		{
			return behavior == TransformBehavior.Rotate;
		}

		public static bool IsScaling(this TransformBehavior behavior)
		{
			if (behavior >= TransformBehavior.ScaleLeftCenter)
			{
				return behavior <= TransformBehavior.ScaleBottomLeft;
			}
			return false;
		}

		public static PermittedTransforms ToPermittedTransforms(this TransformBehavior behavior)
		{
			switch (behavior)
			{
			case TransformBehavior.Translate:
				return PermittedTransforms.Translate;
			case TransformBehavior.Rotate:
				return PermittedTransforms.Rotate;
			case TransformBehavior.ScaleLeftCenter:
			case TransformBehavior.Manipulate:
				return PermittedTransforms.LeftCenterScale;
			case TransformBehavior.ScaleTopLeft:
				return PermittedTransforms.TopLeftConerScale;
			case TransformBehavior.ScaleTopCenter:
				return PermittedTransforms.TopCenterScale;
			case TransformBehavior.ScaleTopRight:
				return PermittedTransforms.TopRightConerScale;
			case TransformBehavior.ScaleRightCenter:
				return PermittedTransforms.RightCenterScale;
			case TransformBehavior.ScaleBottomRight:
				return PermittedTransforms.BottomRightCornerScale;
			case TransformBehavior.ScaleBottomCenter:
				return PermittedTransforms.BottomCenterScale;
			case TransformBehavior.ScaleBottomLeft:
				return PermittedTransforms.BottomLeftCornerScale;
			default:
				return PermittedTransforms.None;
			}
		}
	}
}
