using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using Cvte.Windows.Mathematics;
using Cvte.Windows.Mathematics.Geometries;

namespace Cvte.Paint.Framework
{
	public sealed class ElementsTransformer : ITransformAdapter
	{
		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass32_0
		{
			public Rect desiredBounds;

			public Size minSize;

			public Rect bounds;
		}

		private TransformBehavior _behavior;

		private Rect _originBounds;

		private double _originAngle;

		private Dictionary<Element, Tuple<Vector, double>> _originTransformDictionary;

		private Dictionary<Element, Tuple<Rect, double>> _transformingElementsDictionary;

		internal DependencyObject Container { get; }

		public IList<Element> Elements { get; }

		public Rect Bounds
		{
			get
			{
				if (Elements.Count != 1)
				{
					return Elements.TransformedBounds();
				}
				return Elements[0].Bounds;
			}
		}

		public double Rotation
		{
			get
			{
				if (Elements.Count != 1)
				{
					return 0.0;
				}
				return Elements[0].Rotation;
			}
		}

		public Rect DesiredBounds { get; private set; }

		public double DesiredRotation { get; private set; }

		public ElementsTransformer(DependencyObject container, IList<Element> elements)
		{
			Container = container;
			IList<Element> list;
			if (!elements.Any((Element x) => x.GetIsLocked()))
			{
				list = elements;
			}
			else
			{
				IList<Element> list2 = elements.Where((Element x) => !x.GetIsLocked()).ToArray();
				list = list2;
			}
			Elements = list;
		}

		public void StartTransform(TransformRoutedEventArgs args)
		{
			StartTransform(Bounds, Rotation, args);
		}

		internal void StartTransform(Rect originBounds, double originAngle, TransformRoutedEventArgs args)
		{
			_behavior = args.PrimaryBehavior;
			_originBounds = originBounds;
			_originAngle = originAngle;
			_transformingElementsDictionary = Elements.ToDictionary((Element e) => e, (Element e) => new Tuple<Rect, double>(e.Bounds, e.Rotation));
			_originTransformDictionary = Elements.ToDictionary((Element x) => x, (Element x) => GetTransform(x.RenderTransform.Value));
		}

		public void CompleteTransform(TransformRoutedEventArgs args)
		{
			_transformingElementsDictionary = null;
			_originTransformDictionary = null;
		}

		public void PerformTransform(Rect targetBounds, object[] args)
		{
			DesiredBounds = targetBounds;
			DesiredRotation = _originAngle;
		}

		public void PerformTransform(Vector movedOffset, object[] args)
		{
			Rect originBounds = _originBounds;
			originBounds.Offset(movedOffset);
			DesiredBounds = originBounds;
			DesiredRotation = _originAngle;
		}

		public void PerformTransform(double angle, object[] args)
		{
			double desiredRotation = _originAngle + angle;
			DesiredBounds = _originBounds;
			DesiredRotation = desiredRotation;
		}

		public void Apply(Rect finalBounds, double finalRotation)
		{
			if (_originTransformDictionary == null)
			{
				return;
			}
			Rect originBounds = _originBounds;
			Rect rect = finalBounds;
			switch (_behavior)
			{
			case TransformBehavior.Translate:
			{
				Vector offset = rect.Location - originBounds.Location;
				ApplyForTranslation(offset);
				break;
			}
			case TransformBehavior.Rotate:
			{
				foreach (Element element in Elements)
				{
					double angle = finalRotation - element.Rotation;
					element.RotateAtOrigin(angle);
				}
				break;
			}
			case TransformBehavior.ScaleLeftCenter:
			case TransformBehavior.ScaleTopLeft:
			case TransformBehavior.ScaleTopCenter:
			case TransformBehavior.ScaleTopRight:
			case TransformBehavior.ScaleRightCenter:
			case TransformBehavior.ScaleBottomRight:
			case TransformBehavior.ScaleBottomCenter:
			case TransformBehavior.ScaleBottomLeft:
			case TransformBehavior.Manipulate:
			case TransformBehavior.Manual:
			{
				foreach (Element element2 in Elements)
				{
					Rect item = _transformingElementsDictionary[element2].Item1;
					Rect rect2 = new Rect((item.X - originBounds.X) / originBounds.Width, (item.Y - originBounds.Y) / originBounds.Height, item.Width / originBounds.Width, item.Height / originBounds.Height);
					Rect desiredBounds = new Rect(rect.X + rect.Width * rect2.X, rect.Y + rect.Height * rect2.Y, rect.Width * rect2.Width, rect.Height * rect2.Height);
					Size minimumSizeByPermittedTransforms = GetMinimumSizeByPermittedTransforms(element2);
					desiredBounds = ExpandBounds(item, desiredBounds, _behavior, minimumSizeByPermittedTransforms);
					if (Nearly.Equals(originBounds.Width, 0.0))
					{
						desiredBounds = new Rect(item.X, desiredBounds.Y, item.Width, desiredBounds.Height);
					}
					else if (Nearly.Equals(item.Width, 0.0))
					{
						desiredBounds = new Rect(desiredBounds.X, desiredBounds.Y, item.Width, desiredBounds.Height);
					}
					if (Nearly.Equals(originBounds.Height, 0.0))
					{
						desiredBounds = new Rect(desiredBounds.X, item.Y, desiredBounds.Width, item.Height);
					}
					else if (Nearly.Equals(item.Height, 0.0))
					{
						desiredBounds = new Rect(desiredBounds.X, desiredBounds.Y, desiredBounds.Width, item.Height);
					}
					element2.Bounds = desiredBounds;
				}
				break;
			}
			case TransformBehavior.Secondary:
				break;
			}
		}

		private Size GetMinimumSizeByPermittedTransforms(Element element)
		{
			Size result = element.GetTransformMinimumSize();
			PermittedTransforms permittedTransform = element.GetPermittedTransform();
			Size size = element.Bounds.Size;
			if (permittedTransform.HasFlag(PermittedTransforms.FixedRatioByDefaultFlag) || permittedTransform.HasFlag(PermittedTransforms.FixedRatioFlag))
			{
				double num = result.Width / element.Width;
				double num2 = result.Height / element.Height;
				result = ((!(num > num2)) ? size.UniformSetHeight(result.Height) : size.UniformSetWidth(result.Width));
			}
			return result;
		}

		private Rect ExpandBounds(Rect bounds, Rect desiredBounds, TransformBehavior behavior, Size minSize)
		{
			_003C_003Ec__DisplayClass32_0 obj = default(_003C_003Ec__DisplayClass32_0);
			obj.desiredBounds = desiredBounds;
			obj.minSize = minSize;
			obj.bounds = bounds;
			switch (behavior)
			{
			case TransformBehavior.ScaleBottomLeft:
				obj.desiredBounds.Width = _003CExpandBounds_003Eg__GetMinWidth_007C32_0(ref obj);
				obj.desiredBounds.Height = _003CExpandBounds_003Eg__GetMinHeight_007C32_1(ref obj);
				break;
			case TransformBehavior.ScaleLeftCenter:
				obj.desiredBounds.Width = _003CExpandBounds_003Eg__GetMinWidth_007C32_0(ref obj);
				break;
			case TransformBehavior.ScaleTopLeft:
				obj.desiredBounds.Width = _003CExpandBounds_003Eg__GetMinWidth_007C32_0(ref obj);
				obj.desiredBounds.Height = _003CExpandBounds_003Eg__GetMinHeight_007C32_1(ref obj);
				break;
			case TransformBehavior.ScaleTopCenter:
				obj.desiredBounds.Height = _003CExpandBounds_003Eg__GetMinHeight_007C32_1(ref obj);
				break;
			case TransformBehavior.ScaleTopRight:
				obj.desiredBounds.Width = _003CExpandBounds_003Eg__GetMinWidth_007C32_0(ref obj);
				obj.desiredBounds.Height = _003CExpandBounds_003Eg__GetMinHeight_007C32_1(ref obj);
				break;
			case TransformBehavior.ScaleRightCenter:
				obj.desiredBounds.Width = _003CExpandBounds_003Eg__GetMinWidth_007C32_0(ref obj);
				break;
			case TransformBehavior.ScaleBottomRight:
				obj.desiredBounds.Height = _003CExpandBounds_003Eg__GetMinHeight_007C32_1(ref obj);
				obj.desiredBounds.Width = _003CExpandBounds_003Eg__GetMinWidth_007C32_0(ref obj);
				break;
			case TransformBehavior.ScaleBottomCenter:
				obj.desiredBounds.Height = _003CExpandBounds_003Eg__GetMinHeight_007C32_1(ref obj);
				break;
			}
			return obj.desiredBounds;
		}

		private void ApplyForTranslation(Vector offset)
		{
			foreach (Element element in Elements)
			{
				Tuple<Vector, double> tuple = _originTransformDictionary[element];
				Matrix identity = Matrix.Identity;
				identity.Rotate(tuple.Item2);
				Vector offsetVector = identity.Transform(offset);
				offsetVector = new Vector(offsetVector.X / tuple.Item1.X, offsetVector.Y / tuple.Item1.Y);
				Rect item = _transformingElementsDictionary[element].Item1;
				item.Offset(offsetVector);
				element.Bounds = item;
			}
		}

		private static Tuple<Vector, double> GetTransform(Matrix matrix)
		{
			Vector vector = new Vector(0.0, 1.0);
			double num = Vector.AngleBetween(matrix.Transform(vector), vector);
			matrix.Rotate(num);
			Rect rect = new Rect(0.0, 0.0, 1.0, 1.0);
			Rect rect2 = rect;
			rect2.Transform(matrix);
			return new Tuple<Vector, double>(new Vector(rect2.Width / rect.Width, rect2.Height / rect.Height), num);
		}

		[CompilerGenerated]
		private static double _003CExpandBounds_003Eg__GetMinWidth_007C32_0(ref _003C_003Ec__DisplayClass32_0 P_0)
		{
			if (!(P_0.desiredBounds.Width > P_0.minSize.Width))
			{
				return P_0.minSize.Width;
			}
			return P_0.desiredBounds.Width;
		}

		[CompilerGenerated]
		private static double _003CExpandBounds_003Eg__GetMinHeight_007C32_1(ref _003C_003Ec__DisplayClass32_0 P_0)
		{
			if (!(P_0.desiredBounds.Height > P_0.minSize.Height))
			{
				return P_0.minSize.Height;
			}
			return P_0.desiredBounds.Height;
		}
	}
}
