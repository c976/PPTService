using System;

namespace Cvte.Paint.Framework
{
	[Flags]
	public enum PermittedTransforms
	{
		None = 0,
		TopLeftConerScale = 0x100,
		TopRightConerScale = 0x200,
		BottomRightCornerScale = 0x400,
		BottomLeftCornerScale = 0x800,
		LeftCenterScale = 0x1000,
		TopCenterScale = 0x2000,
		RightCenterScale = 0x4000,
		BottomCenterScale = 0x8000,
		Translate = 1,
		Rotate = 2,
		CornerScale = TopLeftConerScale | TopRightConerScale | BottomRightCornerScale | BottomLeftCornerScale,
		CenterScale = LeftCenterScale | TopCenterScale | RightCenterScale | BottomCenterScale,
		FullScale = CornerScale | CenterScale,
		FreeTransform = FullScale | Translate | Rotate,
		FixedRatioByDefaultFlag = 0x10000,
		FixedRatioFlag = 0x20000,
		FixedRatioTransformByDefault = FreeTransform | FixedRatioByDefaultFlag,
		FixedRatioTransform = FreeTransform | FixedRatioFlag,
		TranslateTransform = 0x100000,
		ScaleTransform = 0x200000,
		RotateTransform = 0x400000,
		RenderTransform = TranslateTransform | ScaleTransform | RotateTransform,
		FlippingFlag = 0x3000000,
		FreeRenderTransform = FreeTransform | RenderTransform,
		FixedRatioScaleOnly = CornerScale | Translate | FixedRatioFlag | TranslateTransform,
		FixedRatioScaleOnlyWithTransform = FixedRatioScaleOnly | ScaleTransform,
		Default = FreeRenderTransform
	}
}
