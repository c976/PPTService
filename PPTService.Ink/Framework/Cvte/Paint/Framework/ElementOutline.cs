using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Cvte.ComponentModel;
using Cvte.Diagnostics.Annotations;
using Cvte.Windows.Mathematics;
using Cvte.Windows.Mathematics.Geometries;

namespace Cvte.Paint.Framework
{
	public sealed class ElementOutline : DispatcherObject
	{
		private readonly Element _associatedElement;

		public ElementOutline([NotNull] Element associatedElement)
		{
			if (associatedElement == null)
			{
				throw new ArgumentNullException("associatedElement");
			}
			_associatedElement = associatedElement;
		}

		internal Geometry GetTransformInfo(OutlineDefinitionContext context = null)
		{
			context = context ?? new OutlineDefinitionContext();
			ITransformedBounds transformedBounds = _associatedElement as ITransformedBounds;
			if (transformedBounds != null)
			{
				return new RectangleGeometry(transformedBounds.TransformedBounds);
			}
			Point point = new Rect(_associatedElement.Bounds.Size).Center();
			double rotation = _associatedElement.Rotation;
			Geometry geometry = _associatedElement.DefineOutline(context);
			if (geometry == null)
			{
				if (DebuggingProperties.IsDebug)
				{
					throw new ArgumentException("传入的 " + _associatedElement.GetType().FullName + " 获取不到外接矩形，此异常仅 Debug 下抛出");
				}
				return new RectangleGeometry(new Rect(default(Point), new Point(1.0, 1.0)));
			}
			Geometry geometry2 = geometry.Clone();
			TransformGroup transformGroup = new TransformGroup();
			transformGroup.Children.Add(new RotateTransform
			{
				Angle = rotation,
				CenterX = point.X,
				CenterY = point.Y
			});
			if (geometry2.Transform != null && !object.Equals(geometry2.Transform, Transform.Identity))
			{
				transformGroup.Children.Add(geometry2.Transform);
			}
			transformGroup.Children.Add(((UIElement)_associatedElement).RenderTransform);
			geometry2.Transform = transformGroup;
			return geometry2;
		}

		internal Geometry GetTransformInfo(OutlineDefinitionContext context, bool includeOutlineThickness)
		{
			OutlineDefinitionContext outlineDefinitionContext = new OutlineDefinitionContext();
			if (includeOutlineThickness)
			{
				Geometry transformInfo = GetTransformInfo(outlineDefinitionContext);
				return outlineDefinitionContext.IncludeLineThickness(transformInfo, context.Thickness);
			}
			return GetTransformInfo(outlineDefinitionContext);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ValueTuple<Rect, double> ToBoundsRotation()
		{
			return new ValueTuple<Rect, double>(_associatedElement.Bounds, _associatedElement.Rotation);
		}

		public ValueTuple<Rect, double> ToFinalBoundsRotation()
		{
			Geometry transformInfo = GetTransformInfo();
			Transform transform = transformInfo.Transform;
			transformInfo.Transform = null;
			Rect bounds = transformInfo.Bounds;
			transformInfo.Transform = transform;
			ValueTuple<Vector, double, Vector> valueTuple = TransformMatrix.MatrixToGroup(transform.Value, TransformMatrix.Centers.ScaleAtZeroRotateAtCenter(bounds.Size));
			Vector item = valueTuple.Item1;
			double item2 = valueTuple.Item2;
			Vector item3 = valueTuple.Item3;
			bounds.Scale(item.X, item.Y);
			bounds.Offset(item3);
			return new ValueTuple<Rect, double>(bounds, item2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Rect GetExternalBounds(bool includeOutlineThickness = true)
		{
			OutlineDefinitionContext outlineDefinitionContext = new OutlineDefinitionContext();
			Geometry transformInfo = GetTransformInfo(outlineDefinitionContext);
			if (includeOutlineThickness && !outlineDefinitionContext.IsThicknessIncluded && !Nearly.Equals(outlineDefinitionContext.Thickness, 0.0, 0.125))
			{
				return outlineDefinitionContext.IncludeLineThickness(transformInfo, outlineDefinitionContext.Thickness).Bounds;
			}
			return transformInfo.Bounds;
		}

		public Rect GetExternalBounds([NotNull] UIElement relativeTo, bool includeOutlineThickness = true)
		{
			if (relativeTo == null)
			{
				throw new ArgumentNullException("relativeTo");
			}
			Rect externalBounds = GetExternalBounds(includeOutlineThickness);
			Point point = _associatedElement.TranslatePoint(default(Point), relativeTo);
			externalBounds.Offset(point.X, point.Y);
			return externalBounds;
		}

		public void SetFinalBounds(Rect bounds)
		{
			_associatedElement.Bounds = bounds;
		}

		public void LimitInto(Rect bounds)
		{
			throw new NotImplementedException();
		}
	}
}
