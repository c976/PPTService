using System.Windows.Media;

namespace Cvte.Paint.Framework
{
	public class ReflectionInfo
	{
		public double OffsetY { get; set; }

		public double Opacity { get; set; }

		public Brush OpacityMask { get; set; }

		public ReflectionType ReflectionType { get; set; }

		public ReflectionSaveInfo GetSaveInfo()
		{
			return new ReflectionSaveInfo
			{
				Opacity = Opacity,
				OffsetY = OffsetY,
				OpacityMask = OpacityMask
			};
		}

		public static ReflectionInfo ConvertFromSaveInfo(ReflectionSaveInfo saveInfo)
		{
			return new ReflectionInfo
			{
				Opacity = saveInfo.Opacity,
				OffsetY = saveInfo.OffsetY,
				OpacityMask = saveInfo.OpacityMask
			};
		}
	}
}
