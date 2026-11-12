using System.Windows;
using System.Windows.Media;

namespace Cvte.Paint.Framework
{
	public static class ReflectionInfoService
	{
		public static ReflectionInfo GetReflectionInfo(ReflectionType reflectionType)
		{
			ReflectionInfo reflectionInfo = null;
			switch (reflectionType)
			{
			case ReflectionType.Reflection0Pt:
				reflectionInfo = Create0PtInfo();
				break;
			case ReflectionType.Reflection4Pt:
				reflectionInfo = Create4PtInfo();
				break;
			case ReflectionType.Reflection8Pt:
				reflectionInfo = Create8PtInfo();
				break;
			case ReflectionType.Reflection0PtHalf:
				reflectionInfo = Create0PtHalfInfo();
				break;
			case ReflectionType.Reflection4PtHalf:
				reflectionInfo = Create4PtHalfInfo();
				break;
			case ReflectionType.Reflection8PtHalf:
				reflectionInfo = Create8PtHalfInfo();
				break;
			case ReflectionType.Reflection0PtAll:
				reflectionInfo = Create0PtAllInfo();
				break;
			case ReflectionType.Reflection4PtAll:
				reflectionInfo = Create4PtAllInfo();
				break;
			case ReflectionType.Reflection8PtAll:
				reflectionInfo = Create8PtAllInfo();
				break;
			}
			if (reflectionInfo == null)
			{
				return null;
			}
			reflectionInfo.ReflectionType = reflectionType;
			return reflectionInfo;
		}

		private static ReflectionInfo Create0PtInfo()
		{
			return new ReflectionInfo
			{
				OffsetY = 0.0,
				OpacityMask = new LinearGradientBrush
				{
					StartPoint = new Point(0.0, 0.0),
					EndPoint = new Point(0.0, 1.0),
					GradientStops = new GradientStopCollection
					{
						new GradientStop(new Color
						{
							A = 0,
							B = 0,
							G = 0,
							R = 0
						}, 0.8),
						new GradientStop(new Color
						{
							A = byte.MaxValue,
							B = 0,
							G = 0,
							R = 0
						}, 1.0)
					}
				},
				Opacity = 0.35
			};
		}

		private static ReflectionInfo Create8PtInfo()
		{
			return new ReflectionInfo
			{
				OffsetY = 8.0,
				OpacityMask = new LinearGradientBrush
				{
					StartPoint = new Point(0.0, 0.0),
					EndPoint = new Point(0.0, 1.0),
					GradientStops = new GradientStopCollection
					{
						new GradientStop(new Color
						{
							A = 0,
							B = 0,
							G = 0,
							R = 0
						}, 0.8),
						new GradientStop(new Color
						{
							A = byte.MaxValue,
							B = 0,
							G = 0,
							R = 0
						}, 1.0)
					}
				},
				Opacity = 0.35
			};
		}

		private static ReflectionInfo Create4PtInfo()
		{
			return new ReflectionInfo
			{
				OffsetY = 4.0,
				OpacityMask = new LinearGradientBrush
				{
					StartPoint = new Point(0.0, 0.0),
					EndPoint = new Point(0.0, 1.0),
					GradientStops = new GradientStopCollection
					{
						new GradientStop(new Color
						{
							A = 0,
							B = 0,
							G = 0,
							R = 0
						}, 0.8),
						new GradientStop(new Color
						{
							A = byte.MaxValue,
							B = 0,
							G = 0,
							R = 0
						}, 1.0)
					}
				},
				Opacity = 0.35
			};
		}

		private static ReflectionInfo Create0PtHalfInfo()
		{
			return new ReflectionInfo
			{
				OffsetY = 0.0,
				OpacityMask = new LinearGradientBrush
				{
					StartPoint = new Point(0.0, 0.0),
					EndPoint = new Point(0.0, 1.0),
					GradientStops = new GradientStopCollection
					{
						new GradientStop(new Color
						{
							A = 0,
							B = 0,
							G = 0,
							R = 0
						}, 0.5),
						new GradientStop(new Color
						{
							A = byte.MaxValue,
							B = 0,
							G = 0,
							R = 0
						}, 1.0)
					}
				},
				Opacity = 0.35
			};
		}

		private static ReflectionInfo Create4PtHalfInfo()
		{
			return new ReflectionInfo
			{
				OffsetY = 4.0,
				OpacityMask = new LinearGradientBrush
				{
					StartPoint = new Point(0.0, 0.0),
					EndPoint = new Point(0.0, 1.0),
					GradientStops = new GradientStopCollection
					{
						new GradientStop(new Color
						{
							A = 0,
							B = 0,
							G = 0,
							R = 0
						}, 0.5),
						new GradientStop(new Color
						{
							A = byte.MaxValue,
							B = 0,
							G = 0,
							R = 0
						}, 1.0)
					}
				},
				Opacity = 0.35
			};
		}

		private static ReflectionInfo Create8PtHalfInfo()
		{
			return new ReflectionInfo
			{
				OffsetY = 8.0,
				OpacityMask = new LinearGradientBrush
				{
					StartPoint = new Point(0.0, 0.0),
					EndPoint = new Point(0.0, 1.0),
					GradientStops = new GradientStopCollection
					{
						new GradientStop(new Color
						{
							A = 0,
							B = 0,
							G = 0,
							R = 0
						}, 0.5),
						new GradientStop(new Color
						{
							A = byte.MaxValue,
							B = 0,
							G = 0,
							R = 0
						}, 1.0)
					}
				},
				Opacity = 0.35
			};
		}

		private static ReflectionInfo Create0PtAllInfo()
		{
			return new ReflectionInfo
			{
				OffsetY = 0.0,
				OpacityMask = new LinearGradientBrush
				{
					StartPoint = new Point(0.0, 0.0),
					EndPoint = new Point(0.0, 1.0),
					GradientStops = new GradientStopCollection
					{
						new GradientStop(new Color
						{
							A = 0,
							B = 0,
							G = 0,
							R = 0
						}, 0.3),
						new GradientStop(new Color
						{
							A = byte.MaxValue,
							B = 0,
							G = 0,
							R = 0
						}, 1.0)
					}
				},
				Opacity = 0.35
			};
		}

		private static ReflectionInfo Create4PtAllInfo()
		{
			return new ReflectionInfo
			{
				OffsetY = 4.0,
				OpacityMask = new LinearGradientBrush
				{
					StartPoint = new Point(0.0, 0.0),
					EndPoint = new Point(0.0, 1.0),
					GradientStops = new GradientStopCollection
					{
						new GradientStop(new Color
						{
							A = 0,
							B = 0,
							G = 0,
							R = 0
						}, 0.3),
						new GradientStop(new Color
						{
							A = byte.MaxValue,
							B = 0,
							G = 0,
							R = 0
						}, 1.0)
					}
				},
				Opacity = 0.35
			};
		}

		private static ReflectionInfo Create8PtAllInfo()
		{
			return new ReflectionInfo
			{
				OffsetY = 8.0,
				OpacityMask = new LinearGradientBrush
				{
					StartPoint = new Point(0.0, 0.0),
					EndPoint = new Point(0.0, 1.0),
					GradientStops = new GradientStopCollection
					{
						new GradientStop(new Color
						{
							A = 0,
							B = 0,
							G = 0,
							R = 0
						}, 0.3),
						new GradientStop(new Color
						{
							A = byte.MaxValue,
							B = 0,
							G = 0,
							R = 0
						}, 1.0)
					}
				},
				Opacity = 0.35
			};
		}
	}
}
