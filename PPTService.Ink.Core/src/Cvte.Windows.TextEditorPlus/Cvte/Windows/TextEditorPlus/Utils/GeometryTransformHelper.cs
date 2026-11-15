using System.Windows.Media;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	internal static class GeometryTransformHelper
	{
		internal static bool IsTransformEditable(this Geometry geometry)
		{
			return !geometry.IsEmpty();
		}

		internal static void TransformTranslate(this Geometry geometry, double x, double y)
		{
			if (geometry.IsTransformEditable())
			{
				Matrix value = geometry.Transform.Value;
				Matrix value2 = new TranslateTransform(x, y).Value;
				geometry.Transform = new MatrixTransform(value * value2);
			}
		}

		internal static void TransformRotate(this Geometry geometry, double angle, double centerX, double centerY)
		{
			if (geometry.IsTransformEditable())
			{
				Matrix value = geometry.Transform.Value;
				Matrix value2 = new RotateTransform(angle, centerX, centerY).Value;
				geometry.Transform = new MatrixTransform(value * value2);
			}
		}

		internal static void SetMatrixTransform(this Geometry geometry, Matrix matrix)
		{
			if (geometry.IsTransformEditable())
			{
				geometry.Transform = new MatrixTransform(matrix);
			}
		}
	}
}
