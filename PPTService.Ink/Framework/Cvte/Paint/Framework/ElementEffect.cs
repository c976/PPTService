using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Cvte.Paint.Framework
{
	public class ElementEffect
	{
		private bool _isShadowTransparent;

		private ShadowInfo _shadowInfo;

		private DrawingVisual _shadow;

		private ReflectionInfo _reflectionInfo;

		private DrawingVisual _reflection;

		private readonly Element _owner;

		public ShadowType ShadowType
		{
			get
			{
				if (_shadow != null)
				{
					return _shadowInfo.ShadowType;
				}
				return ShadowType.None;
			}
			set
			{
				if (value == ShadowType.None)
				{
					_owner.RemoveEffect(_shadow);
					ShadowInfo = null;
					_shadow = null;
				}
				else
				{
					ShadowInfo = ShadowInfoService.GetShadowInfo(value);
				}
			}
		}

		public bool HaveEffect
		{
			get
			{
				if (_shadowInfo == null)
				{
					return _reflectionInfo != null;
				}
				return true;
			}
		}

		public Brush ShadowBrush
		{
			get
			{
				if (_shadowInfo != null)
				{
					return _shadowInfo.ShadowBrush;
				}
				return Brushes.Transparent;
			}
			set
			{
				if (value != null && ContainShadow)
				{
					_shadowInfo.ShadowBrush = value;
					_owner.SetShadow(_shadowInfo);
				}
			}
		}

		public double ShadowOpacity
		{
			get
			{
				if (_shadow != null)
				{
					return _shadowInfo.Opacity;
				}
				return 1.0;
			}
			set
			{
				if (ContainShadow)
				{
					_shadowInfo.Opacity = value;
					_shadow.Opacity = value;
				}
			}
		}

		public double ShadowBlurRadius
		{
			get
			{
				if (_shadow != null)
				{
					return _shadowInfo.BlurRadius;
				}
				return 0.0;
			}
			set
			{
				if (ContainShadow)
				{
					_shadowInfo.BlurRadius = value;
					((BlurEffect)_shadow.Effect).Radius = value;
				}
			}
		}

		public double ShadowAngle
		{
			get
			{
				if (_shadow != null)
				{
					return _shadowInfo.Angle;
				}
				return 0.0;
			}
			set
			{
				if (ContainShadow && _shadowInfo.IsDropShadow && value <= 360.0 && value >= 0.0)
				{
					_shadowInfo.Angle = value;
					_shadow.Transform = new TranslateTransform(_shadowInfo.ArroundOffsetX, _shadowInfo.ArroundOffsetY);
				}
			}
		}

		public double ShadowOffset
		{
			get
			{
				if (_shadow != null)
				{
					return _shadowInfo.ArroundOffset;
				}
				return 0.0;
			}
			set
			{
				if (ContainShadow && _shadowInfo.IsDropShadow)
				{
					_shadowInfo.ArroundOffset = value;
					if (Math.Abs(_shadowInfo.Angle - 360.0) < 0.1)
					{
						_owner.SetShadow(_shadowInfo);
					}
					else
					{
						_shadow.Transform = new TranslateTransform(_shadowInfo.ArroundOffsetX, _shadowInfo.ArroundOffsetY);
					}
				}
			}
		}

		public bool IsShadowTransparent
		{
			get
			{
				return _isShadowTransparent;
			}
			set
			{
				_isShadowTransparent = value;
				if (HaveEffect && _shadowInfo != null)
				{
					_owner.SetShadow(_shadowInfo);
				}
			}
		}

		public ReflectionType ReflectionType
		{
			get
			{
				return _reflectionInfo?.ReflectionType ?? ReflectionType.None;
			}
			set
			{
				if (value == ReflectionType.None)
				{
					_owner.RemoveEffect(_reflection);
					_reflection = null;
					_reflectionInfo = null;
				}
				else
				{
					_reflectionInfo = ReflectionInfoService.GetReflectionInfo(value);
					_owner.SetReflection(_reflectionInfo);
				}
			}
		}

		public double ReflectionOpacity
		{
			get
			{
				if (_reflection == null)
				{
					return 1.0;
				}
				return _reflectionInfo.Opacity;
			}
			set
			{
				if (ContainReflection)
				{
					_reflectionInfo.Opacity = value;
					_reflection.Opacity = value;
				}
			}
		}

		public ShadowInfo ShadowInfo
		{
			get
			{
				return _shadowInfo;
			}
			set
			{
				_shadowInfo = value;
				if (value != null)
				{
					_owner.SetShadow(_shadowInfo);
				}
			}
		}

		public ReflectionInfo ReflectionInfo
		{
			get
			{
				return _reflectionInfo;
			}
			set
			{
				_reflectionInfo = value;
				if (value != null)
				{
					_owner.SetReflection(_reflectionInfo);
				}
			}
		}

		public bool ContainShadow => _shadowInfo != null;

		public bool ContainReflection => _reflectionInfo != null;

		public void InitShadowTransparent(Brush brush)
		{
			SolidColorBrush solidColorBrush = brush as SolidColorBrush;
			if (solidColorBrush == null)
			{
				_isShadowTransparent = false;
			}
			else
			{
				_isShadowTransparent = solidColorBrush.Color.Equals(Colors.Transparent);
			}
		}

		public ElementEffect(Element owner)
		{
			_owner = owner;
		}

		public void SetShadow(Geometry geometry, ShadowInfo shadowInfo, double rotation, double thickness = 0.0, double opacity = 1.0)
		{
			if (ContainShadow)
			{
				_owner.RemoveEffect(_shadow);
			}
			_shadowInfo = shadowInfo;
			DrawingVisual shadow = ElementEffectService.CreateShadow(geometry, shadowInfo, rotation, new Point(_owner.Bounds.Width / 2.0, _owner.Bounds.Height / 2.0), thickness, _isShadowTransparent, opacity);
			_shadow = shadow;
			_owner.AddEffect(_shadow, true);
		}

		public void SetReflection(ReflectionInfo reflectionInfo, Rect rotateRect = default(Rect), Rect rect = default(Rect))
		{
			if (ContainReflection)
			{
				_owner.RemoveEffect(_reflection);
			}
			_reflectionInfo = reflectionInfo;
			if (Math.Abs(rotateRect.Width) < 0.001)
			{
				rotateRect = GetRotatedBoundGeometry().Bounds;
			}
			if (Math.Abs(rect.Width - 0.0) < 0.001)
			{
				rect = _owner.RigidGeometryDefinition.Bounds;
			}
			RectangleGeometry rectangleGeometry = new RectangleGeometry(rect)
			{
				Transform = new RotateTransform(_owner.Rotation, _owner.Bounds.Width / 2.0, _owner.Bounds.Height / 2.0)
			};
			Visual contentPresenter = _owner.GetContentPresenter();
			_reflection = ElementEffectService.CreateReflectionVisual(contentPresenter, rotateRect, reflectionInfo, rectangleGeometry.Bounds);
			_owner.AddEffect(_reflection, false);
		}

		public Geometry GetRotatedBoundGeometry()
		{
			Geometry geometry = _owner.RigidGeometryDefinition.Clone();
			TransformGroup transformGroup = new TransformGroup();
			if (geometry.Transform != null)
			{
				transformGroup.Children.Add(geometry.Transform);
			}
			transformGroup.Children.Add(new RotateTransform(_owner.Rotation, _owner.Bounds.Width / 2.0, _owner.Bounds.Size.Height / 2.0));
			geometry.Transform = transformGroup;
			return geometry;
		}

		public void RemoveEffect()
		{
			if (ContainShadow)
			{
				_owner.RemoveEffect(_shadow);
			}
			if (ContainReflection)
			{
				_owner.RemoveEffect(_reflection);
			}
		}

		public void SetEffect()
		{
			if (ContainShadow)
			{
				_owner.SetShadow(_shadowInfo);
			}
			if (ContainReflection)
			{
				_owner.SetReflection(_reflectionInfo);
			}
		}

		public void RefreshEffect()
		{
			RemoveEffect();
			SetEffect();
		}

		public void SetShadowOpacity(double opacity)
		{
			_shadow.Opacity = opacity * _shadowInfo.Opacity;
		}

		public void ClearEffect()
		{
			ShadowType = ShadowType.None;
			ReflectionType = ReflectionType.None;
		}
	}
}
