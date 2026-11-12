using System;
using System.Windows.Media;
using Cvte.Storage.Standard;

namespace Cvte.Paint.Framework
{
	public class ShadowInfo
	{
		public MatrixTransform ShadowTransform { get; set; }

		public double XScale
		{
			get
			{
				if (ShadowTransform == null)
				{
					return 0.0;
				}
				return 0.0 - ShadowTransform.Matrix.M21;
			}
		}

		public bool IsUp
		{
			get
			{
				if (ShadowTransform == null)
				{
					return false;
				}
				return ShadowTransform.Matrix.M22 > 0.0;
			}
		}

		public double OffsetY { get; set; }

		public bool IsDropShadow { get; set; }

		public double Angle { get; set; }

		public double ArroundOffsetX => ArroundOffset * Math.Sin(Angle * Math.PI / 180.0);

		public double ArroundOffsetY => (0.0 - ArroundOffset) * Math.Cos(Angle * Math.PI / 180.0);

		public double Opacity { get; set; }

		public double BlurRadius { get; set; }

		public System.Windows.Media.Brush ShadowBrush { get; set; }

		public double ArroundOffset { get; set; }

		public ShadowType ShadowType { get; set; }

		public ShadowInfo()
		{
			ShadowBrush = Brushes.Black;
			Opacity = 0.45;
			BlurRadius = 8.0;
			ArroundOffset = 4.0;
		}

		public ShadowSaveInfo GetSaveInfo()
		{
			if (IsDropShadow)
			{
				return new DropShadowSaveInfo
				{
					ShadowBrush = ShadowBrush,
					Opacity = Opacity,
					BlurRadius = BlurRadius,
					Angle = Angle,
					Offset = ArroundOffset
				};
			}
			PerspectiveShadowSaveInfo perspectiveShadowSaveInfo = new PerspectiveShadowSaveInfo
			{
				ShadowBrush = ShadowBrush,
				Opacity = Opacity,
				BlurRadius = BlurRadius,
				OffsetY = OffsetY
			};
			System.Windows.Media.Matrix? matrix = ShadowTransform?.Matrix;
			perspectiveShadowSaveInfo.ShadowMatrix = (matrix.HasValue ? ((Cvte.Storage.Standard.Matrix)matrix.GetValueOrDefault()) : null);
			return perspectiveShadowSaveInfo;
		}

		public static ShadowInfo GetShadowInfoFromSaveInfo(ShadowSaveInfo saveInfo)
		{
			if (saveInfo is DropShadowSaveInfo)
			{
				DropShadowSaveInfo dropShadowSaveInfo = (DropShadowSaveInfo)saveInfo;
				return new ShadowInfo
				{
					ShadowBrush = saveInfo.ShadowBrush,
					Opacity = saveInfo.Opacity,
					BlurRadius = saveInfo.BlurRadius,
					ArroundOffset = dropShadowSaveInfo.Offset,
					Angle = dropShadowSaveInfo.Angle,
					IsDropShadow = true
				};
			}
			if (saveInfo is PerspectiveShadowSaveInfo)
			{
				PerspectiveShadowSaveInfo perspectiveShadowSaveInfo = (PerspectiveShadowSaveInfo)saveInfo;
				return new ShadowInfo
				{
					ShadowBrush = saveInfo.ShadowBrush,
					Opacity = saveInfo.Opacity,
					BlurRadius = saveInfo.BlurRadius,
					OffsetY = perspectiveShadowSaveInfo.OffsetY,
					ShadowTransform = new MatrixTransform(perspectiveShadowSaveInfo.ShadowMatrix),
					IsDropShadow = false
				};
			}
			return null;
		}
	}
}
