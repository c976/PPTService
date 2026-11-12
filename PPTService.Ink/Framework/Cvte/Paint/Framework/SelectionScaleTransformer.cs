using System;
using System.Linq;
using System.Windows;
using Cvte.Paint.Framework.Settings;
using Cvte.Windows.Mathematics;
using Cvte.Windows.Mathematics.Geometries;

namespace Cvte.Paint.Framework
{
	internal class SelectionScaleTransformer : SelectionPartitionTransformer
	{
		private Rect _originBounds;

		private Point _originCenter;

		private double _originAngle;

		private double _sin;

		private double _cos;

		private Vector _cornerVector;

		private Point _originCorner;

		private Vector _offsetToCorner;

		private bool _originUniformScale;

		private bool _uniformScale;

		private bool _altPressed;

		private Size BoxMinSize { get; set; }

		private bool IsCornerScale
		{
			get
			{
				if (Behavior != TransformBehavior.ScaleTopLeft && Behavior != TransformBehavior.ScaleTopRight && Behavior != TransformBehavior.ScaleBottomRight)
				{
					return Behavior == TransformBehavior.ScaleBottomLeft;
				}
				return true;
			}
		}

		protected override void OnStarted()
		{
			BoxMinSize = Owner.Elements.FirstOrDefault().GetTransformMinimumSize();
			_originBounds = Owner.Bounds;
			_originCenter = _originBounds.Center();
			_originAngle = Owner.Rotation;
			_sin = Math.Sin(_originAngle * Math.PI / 180.0);
			_cos = Math.Cos(_originAngle * Math.PI / 180.0);
			Aligner.Prepare(Slide, Slide.Elements.Except(TransformingElements), TransformingElements);
			PermittedTransforms permittedTransform = TransformingElements.GetPermittedTransform();
			_originUniformScale = permittedTransform.HasFlag(PermittedTransforms.FixedRatioByDefaultFlag) || permittedTransform.HasFlag(PermittedTransforms.FixedRatioFlag) || Owner.Elements.Any((Element x) => !Nearly.Equals(x.Rotation, Owner.Rotation));
			Point point;
			switch (Behavior)
			{
			default:
				return;
			case TransformBehavior.ScaleLeftCenter:
				point = new Point(_originBounds.Left, _originCenter.Y);
				break;
			case TransformBehavior.ScaleTopLeft:
				point = _originBounds.TopLeft;
				break;
			case TransformBehavior.ScaleTopCenter:
				point = new Point(_originCenter.X, _originBounds.Top);
				break;
			case TransformBehavior.ScaleTopRight:
				point = _originBounds.TopRight;
				break;
			case TransformBehavior.ScaleRightCenter:
				point = new Point(_originBounds.Right, _originCenter.Y);
				break;
			case TransformBehavior.ScaleBottomRight:
				point = _originBounds.BottomRight;
				break;
			case TransformBehavior.ScaleBottomCenter:
				point = new Point(_originCenter.X, _originBounds.Bottom);
				break;
			case TransformBehavior.ScaleBottomLeft:
				point = _originBounds.BottomLeft;
				break;
			}
			_cornerVector = Geometry2D.Rotate(point - _originCenter, 0.0 - _originAngle);
			_originCorner = _originCenter + _cornerVector;
			_offsetToCorner = default(Vector);
		}

		protected override void OnDelta(Vector userOffset, object[] args)
		{
			_uniformScale = IsCornerScale && (_originUniformScale || SelectionPartitionTransformer.WithShift);
			Vector vector = UniformScale(userOffset + _offsetToCorner);
			Vector vector2 = default(Vector);
			AlignResult alignResult = null;
			Rect rect;
			if (SelectionPartitionTransformer.WithAlt)
			{
				rect = Scale(vector);
				if (!_altPressed)
				{
					Aligner.Clear();
					_altPressed = true;
				}
			}
			else
			{
				_altPressed = false;
				alignResult = AlignScale(vector);
				vector2 = UniformAlignedScale(alignResult, userOffset + _offsetToCorner, vector);
				rect = Scale(vector2);
			}
			if (SelectionPartitionTransformer.WithCtrl)
			{
				rect = ScaleForFullDirection(rect);
			}
			if (alignResult != null)
			{
				Rect bounds = rect.Rotate(_originAngle);
				if (Math.Abs(_originAngle % 90.0) > 0.1)
				{
					Aligner.Draw(bounds, _originCorner + vector2);
				}
				else
				{
					Aligner.Draw(bounds, _originCorner + vector2, true);
				}
			}
			DesiredBounds = rect;
		}

		protected override void OnCompleted()
		{
			Aligner?.Clear();
		}

		private Vector UniformScale(Vector offset)
		{
			if (!_uniformScale)
			{
				return offset;
			}
			Vector offset2 = ToRotatedOffset(offset);
			Rect rect = ExpandBounds(offset2);
			double num = _originBounds.Width / _originBounds.Height;
			double num2 = rect.Height * num;
			double height = rect.Width / num;
			Size size = ((num2 > rect.Width) ? new Size(num2, rect.Height) : new Size(rect.Width, height));
			Vector offset3;
			switch (Behavior)
			{
			case TransformBehavior.ScaleTopLeft:
				offset3 = new Vector(_originBounds.Width - size.Width, _originBounds.Height - size.Height);
				break;
			case TransformBehavior.ScaleTopRight:
				offset3 = new Vector(size.Width - _originBounds.Width, _originBounds.Height - size.Height);
				break;
			case TransformBehavior.ScaleBottomRight:
				offset3 = new Vector(size.Width - _originBounds.Width, size.Height - _originBounds.Height);
				break;
			case TransformBehavior.ScaleBottomLeft:
				offset3 = new Vector(_originBounds.Width - size.Width, size.Height - _originBounds.Height);
				break;
			default:
				return offset;
			}
			return ToPlainOffset(offset3);
		}

		private AlignResult AlignScale(Vector offset)
		{
			Point point = _originCorner + offset;
			if (Math.Abs(_originAngle % 90.0) > 0.1 && !IsCornerScale)
			{
				return new AlignResult(new SingleAlignResult(), new SingleAlignResult());
			}
			return Aligner.Align(Scale(offset).Rotate(_originAngle), point);
		}

		private Vector UniformAlignedScale(AlignResult result, Vector originOffset, Vector uniformOffset)
		{
			if (!_uniformScale && !result.IsAligned)
			{
				return uniformOffset;
			}
			if (!_uniformScale && result.IsAligned)
			{
				return uniformOffset + result.Offset;
			}
			double num = Slide.GetSettings<ThresholdBoardSettings>().AlignThreshold / Board.GetContentScale(Slide);
			if ((result.Horizontal.IsAligned && !result.Vertical.IsAligned) || (result.Horizontal.IsAligned && result.Vertical.IsAligned && result.Horizontal.Offset <= result.Vertical.Offset))
			{
				double num2 = uniformOffset.X + result.Horizontal.Offset;
				double num3 = num2 * _cornerVector.Y / _cornerVector.X;
				if (Math.Abs(num2 - originOffset.X) <= num || Math.Abs(num3 - originOffset.Y) <= num)
				{
					return new Vector(num2, num3);
				}
			}
			else if ((!result.Horizontal.IsAligned && result.Vertical.IsAligned) || (result.Horizontal.IsAligned && result.Vertical.IsAligned && result.Vertical.Offset <= result.Horizontal.Offset))
			{
				double num4 = uniformOffset.Y + result.Vertical.Offset;
				double num5 = num4 * _cornerVector.X / _cornerVector.Y;
				if (Math.Abs(num5 - originOffset.X) <= num || Math.Abs(num4 - originOffset.Y) <= num)
				{
					return new Vector(num5, num4);
				}
			}
			return uniformOffset;
		}

		private Rect Scale(Vector offset)
		{
			double x = offset.X;
			double y = offset.Y;
			double x2 = x * _cos + y * _sin;
			double y2 = (0.0 - x) * _sin + y * _cos;
			Rect rect = ExpandBounds(new Vector(x2, y2));
			Point point = Geometry2D.Rotate(rect.Center() - _originCenter, 0.0 - _originAngle) + _originCenter;
			return new Rect(point.X - rect.Width / 2.0, point.Y - rect.Height / 2.0, rect.Width, rect.Height);
		}

		private Rect ScaleForFullDirection(Rect newBounds)
		{
			double num = newBounds.Left;
			double num2 = newBounds.Top;
			double num3 = newBounds.Right;
			double num4 = newBounds.Bottom;
			if (num >= _originCenter.X - 1.0)
			{
				num = _originCenter.X - 1.0;
			}
			if (num2 >= _originCenter.Y - 1.0)
			{
				num2 = _originCenter.Y - 1.0;
			}
			if (num3 <= _originCenter.X + 1.0)
			{
				num3 = _originCenter.X + 1.0;
			}
			if (num4 <= _originCenter.Y + 1.0)
			{
				num4 = _originCenter.Y + 1.0;
			}
			double num5 = newBounds.Width;
			double num6 = newBounds.Height;
			switch (Behavior)
			{
			case TransformBehavior.ScaleLeftCenter:
				num5 = Math.Abs(num - _originCenter.X) * 2.0;
				break;
			case TransformBehavior.ScaleTopCenter:
				num6 = Math.Abs(num2 - _originCenter.Y) * 2.0;
				break;
			case TransformBehavior.ScaleRightCenter:
				num5 = Math.Abs(num3 - _originCenter.X) * 2.0;
				break;
			case TransformBehavior.ScaleBottomCenter:
				num6 = Math.Abs(num4 - _originCenter.Y) * 2.0;
				break;
			case TransformBehavior.ScaleTopLeft:
				num5 = Math.Abs(num - _originCenter.X) * 2.0;
				num6 = Math.Abs(num2 - _originCenter.Y) * 2.0;
				break;
			case TransformBehavior.ScaleTopRight:
				num5 = Math.Abs(num3 - _originCenter.X) * 2.0;
				num6 = Math.Abs(num2 - _originCenter.Y) * 2.0;
				break;
			case TransformBehavior.ScaleBottomRight:
				num5 = Math.Abs(num3 - _originCenter.X) * 2.0;
				num6 = Math.Abs(num4 - _originCenter.Y) * 2.0;
				break;
			case TransformBehavior.ScaleBottomLeft:
				num5 = Math.Abs(num - _originCenter.X) * 2.0;
				num6 = Math.Abs(num4 - _originCenter.Y) * 2.0;
				break;
			}
			double x = _originCenter.X - num5 / 2.0;
			double y = _originCenter.Y - num6 / 2.0;
			return new Rect(x, y, num5, num6);
		}

		private Rect ExpandBounds(Vector offset)
		{
			switch (Behavior)
			{
			case TransformBehavior.ScaleLeftCenter:
				if (_originBounds.Width - offset.X < BoxMinSize.Width)
				{
					offset.X = _originBounds.Width - BoxMinSize.Width;
				}
				return _originBounds.Expand(0.0 - offset.X, 0.0, 0.0, 0.0);
			case TransformBehavior.ScaleTopLeft:
				if (_originBounds.Width - offset.X < BoxMinSize.Width)
				{
					offset.X = _originBounds.Width - BoxMinSize.Width;
				}
				if (_originBounds.Height - offset.Y < BoxMinSize.Height)
				{
					offset.Y = _originBounds.Height - BoxMinSize.Height;
				}
				return _originBounds.Expand(0.0 - offset.X, 0.0 - offset.Y, 0.0, 0.0);
			case TransformBehavior.ScaleTopCenter:
				if (_originBounds.Height - offset.Y < BoxMinSize.Height)
				{
					offset.Y = _originBounds.Height - BoxMinSize.Height;
				}
				return _originBounds.Expand(0.0, 0.0 - offset.Y, 0.0, 0.0);
			case TransformBehavior.ScaleTopRight:
				if (_originBounds.Width + offset.X < BoxMinSize.Width)
				{
					offset.X = BoxMinSize.Width - _originBounds.Width;
				}
				if (_originBounds.Height - offset.Y < BoxMinSize.Height)
				{
					offset.Y = _originBounds.Height - BoxMinSize.Height;
				}
				return _originBounds.Expand(0.0, 0.0 - offset.Y, offset.X, 0.0);
			case TransformBehavior.ScaleRightCenter:
				if (_originBounds.Width + offset.X < BoxMinSize.Width)
				{
					offset.X = BoxMinSize.Width - _originBounds.Width;
				}
				return _originBounds.Expand(0.0, 0.0, offset.X, 0.0);
			case TransformBehavior.ScaleBottomRight:
				if (_originBounds.Width + offset.X < BoxMinSize.Width)
				{
					offset.X = BoxMinSize.Width - _originBounds.Width;
				}
				if (_originBounds.Height + offset.Y < BoxMinSize.Height)
				{
					offset.Y = BoxMinSize.Height - _originBounds.Height;
				}
				return _originBounds.Expand(0.0, 0.0, offset.X, offset.Y);
			case TransformBehavior.ScaleBottomCenter:
				if (_originBounds.Height + offset.Y < BoxMinSize.Height)
				{
					offset.Y = BoxMinSize.Height - _originBounds.Height;
				}
				return _originBounds.Expand(0.0, 0.0, 0.0, offset.Y);
			case TransformBehavior.ScaleBottomLeft:
				if (_originBounds.Width - offset.X < BoxMinSize.Width)
				{
					offset.X = _originBounds.Width - BoxMinSize.Width;
				}
				if (_originBounds.Height + offset.Y < BoxMinSize.Height)
				{
					offset.Y = BoxMinSize.Height - _originBounds.Height;
				}
				return _originBounds.Expand(0.0 - offset.X, 0.0, 0.0, offset.Y);
			default:
				return _originBounds;
			}
		}

		private Vector ToRotatedOffset(Vector offset)
		{
			return new Vector(offset.X * _cos + offset.Y * _sin, (0.0 - offset.X) * _sin + offset.Y * _cos);
		}

		private Vector ToPlainOffset(Vector offset)
		{
			return new Vector(offset.X * _cos - offset.Y * _sin, offset.X * _sin + offset.Y * _cos);
		}
	}
}
