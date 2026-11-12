using System.Windows.Media;

namespace Cvte.Paint.Framework
{
	public static class ShadowInfoService
	{
		private static ShadowInfo CreateRightUp()
		{
			return new ShadowInfo
			{
				ShadowTransform = new MatrixTransform(1.0, 0.0, -0.3, 0.3, 0.0, 0.0),
				ShadowType = ShadowType.PerspectiveRightUp,
				Opacity = 0.3,
				BlurRadius = 20.0
			};
		}

		private static ShadowInfo CreateLeftUp()
		{
			return new ShadowInfo
			{
				ShadowTransform = new MatrixTransform(1.0, 0.0, 0.3, 0.3, 0.0, 0.0),
				ShadowType = ShadowType.PerspectiveLeftUp,
				Opacity = 0.3,
				BlurRadius = 20.0
			};
		}

		private static ShadowInfo CreateRightDown()
		{
			return new ShadowInfo
			{
				ShadowTransform = new MatrixTransform(1.0, 0.0, -0.3, -0.2, 0.0, 0.0),
				ShadowType = ShadowType.PerspectiveRightDown,
				Opacity = 0.3,
				BlurRadius = 16.0
			};
		}

		private static ShadowInfo CreateLeftDown()
		{
			return new ShadowInfo
			{
				ShadowTransform = new MatrixTransform(1.0, 0.0, 0.3, -0.2, 0.0, 0.0),
				ShadowType = ShadowType.PerspectiveLeftDown,
				Opacity = 0.3,
				BlurRadius = 16.0
			};
		}

		private static ShadowInfo CreateDown()
		{
			return new ShadowInfo
			{
				ShadowTransform = new MatrixTransform(1.0, 0.0, 0.0, -0.08, 0.0, 0.0),
				OffsetY = 8.0,
				ShadowType = ShadowType.PerspectiveDown,
				Opacity = 0.36,
				BlurRadius = 24.0
			};
		}

		private static ShadowInfo CreateArround()
		{
			return new ShadowInfo
			{
				IsDropShadow = true,
				Angle = 360.0,
				ShadowType = ShadowType.Arround
			};
		}

		public static ShadowInfo GetShadowInfo(ShadowType type)
		{
			switch (type)
			{
			case ShadowType.PerspectiveRightUp:
				return CreateRightUp();
			case ShadowType.PerspectiveLeftUp:
				return CreateLeftUp();
			case ShadowType.PerspectiveRightDown:
				return CreateRightDown();
			case ShadowType.PerspectiveLeftDown:
				return CreateLeftDown();
			case ShadowType.PerspectiveDown:
				return CreateDown();
			case ShadowType.Arround:
				return CreateArround();
			case ShadowType.Arround135:
				return new ShadowInfo
				{
					Angle = 135.0,
					IsDropShadow = true,
					ShadowType = type
				};
			case ShadowType.Arround180:
				return new ShadowInfo
				{
					Angle = 180.0,
					IsDropShadow = true,
					ShadowType = type
				};
			case ShadowType.Arround225:
				return new ShadowInfo
				{
					Angle = 225.0,
					IsDropShadow = true,
					ShadowType = type
				};
			default:
				return null;
			}
		}
	}
}
