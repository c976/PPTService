using System;
using System.Windows;
using System.Windows.Media;
using Cvte.ComponentModel;
using Cvte.Diagnostics.Annotations;
using Cvte.Diagnostics.Trace;
using Cvte.Storage.Standard;

namespace Cvte.Paint.Framework
{
	public static class CommonElementSaveInfoProvider
	{
		public static void GetCommonElementSaveInfo(ElementSaveInfo elementSaveInfo, Element element)
		{
			elementSaveInfo.Id = element.Id;
			System.Windows.Rect bounds = element.Bounds;
			elementSaveInfo.X = bounds.X;
			elementSaveInfo.Y = bounds.Y;
			elementSaveInfo.Width = bounds.Width;
			elementSaveInfo.Height = bounds.Height;
			elementSaveInfo.Rotation = element.Rotation;
			elementSaveInfo.Transform = (System.Windows.Media.Matrix.Identity.Equals(element.Transform) ? null : ((Cvte.Storage.Standard.Matrix)element.Transform));
			elementSaveInfo.IsErased = element.IsErased;
			elementSaveInfo.IsErasable = element.IsErasable;
			elementSaveInfo.HasMask = element.HasMask;
			if (element.ClipGeometry != null)
			{
				PathGeometry pathGeometry = element.ClipGeometry as PathGeometry;
				if (pathGeometry != null)
				{
					elementSaveInfo.ClipGeometry = pathGeometry.ToString();
				}
				else
				{
					PathGeometry pathGeometry2 = new PathGeometry();
					pathGeometry2.AddGeometry(System.Windows.Media.Geometry.Parse(elementSaveInfo.ClipGeometry));
					elementSaveInfo.ClipGeometry = pathGeometry2.ToString();
				}
				System.Windows.Media.Matrix? matrix = ((MatrixTransform)(element.ClipGeometry?.Transform))?.Matrix;
				elementSaveInfo.ClipMatrix = (matrix.HasValue ? ((Cvte.Storage.Standard.Matrix)matrix.GetValueOrDefault()) : null);
			}
			elementSaveInfo.RotateOrigin = element.LogicalOriginPoint;
			RotationCenterInfo rotationCenterInfo = element.AsRotationCenterInfo();
			if (rotationCenterInfo.IsSetByUser)
			{
				elementSaveInfo.ShowRotateOrigin = rotationCenterInfo.IsVisible;
			}
			else
			{
				elementSaveInfo.ShowRotateOrigin = null;
			}
		}

		public static void SetCommonElementSaveInfo(ElementSaveInfo saveInfo, Element element)
		{
			if (string.IsNullOrWhiteSpace(saveInfo.Id))
			{
				if (DebuggingProperties.IsDebug)
				{
					throw new ArgumentNullException("saveInfo.Id", "ElementSaveInfo 的 Id 不能为空，仅调试下抛出，请找德熙。发布版本将会给随机一个 Id 号");
				}
				saveInfo.Id = Guid.NewGuid().ToString("N");
				Tracer.Warning("[CommonElementSaveInfoProvider][SetCommonElementSaveInfo] ElementSaveInfo.Id is empty or null. ElementSaveInfoType=" + saveInfo.GetType().FullName, "Paint", "SaveInfo", "PaintSaveInfo", "CommonElementSaveInfoProvider", "SetCommonElementSaveInfo");
			}
			((IIdDefinition)element).SetId(saveInfo.Id);
			element.Bounds = new System.Windows.Rect(saveInfo.X, saveInfo.Y, saveInfo.Width, saveInfo.Height);
			element.Rotation = saveInfo.Rotation;
			if (saveInfo.Transform != null)
			{
				element.Transform = saveInfo.Transform;
			}
			element.IsErasable = saveInfo.IsErasable;
			element.IsErased = saveInfo.IsErased;
			element.HasMask = saveInfo.HasMask;
			if (saveInfo.RotateOrigin != null)
			{
				element.LogicalOriginPoint = saveInfo.RotateOrigin;
				if (saveInfo.ShowRotateOrigin.HasValue)
				{
					element.AsRotationCenterInfo().SetVisible(saveInfo.ShowRotateOrigin.Value, true);
				}
			}
			if (!string.IsNullOrEmpty(saveInfo.ClipGeometry))
			{
				PathGeometry pathGeometry = new PathGeometry();
				pathGeometry.AddGeometry(System.Windows.Media.Geometry.Parse(saveInfo.ClipGeometry));
				element.ClipGeometry = pathGeometry;
				if (saveInfo.ClipMatrix != null)
				{
					element.ClipGeometry.Transform = new MatrixTransform(saveInfo.ClipMatrix);
				}
			}
			if (saveInfo.HasMask && string.IsNullOrEmpty(saveInfo.ClipGeometry))
			{
				element.ClipGeometry = new PathGeometry();
			}
		}

		[NotNull]
		private static RotationCenterInfo AsRotationCenterInfo([NotNull] this Element element)
		{
			RotationCenterInfo rotationCenterInfo = element.RotationCenter as RotationCenterInfo;
			if (rotationCenterInfo != null)
			{
				return rotationCenterInfo;
			}
			throw new NotSupportedException("Element.RotationCenter 暂只支持使用 RotationCenterInfo 类型。");
		}
	}
}
