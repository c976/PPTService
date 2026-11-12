using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Cvte.ComponentModel;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework.Level;
using Cvte.Windows.Controls;
using Cvte.Windows.Mathematics;

namespace Cvte.Paint.Framework
{
	[ContentProperty("Children")]
	[DefaultProperty("Children")]
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof(ElementDebuggerTypeProxy))]
	public class Element : FrameworkElement, IExtensible<Element>, INotifyPropertyChanged, IIdDefinition, IDefaultRotationInfoHost, ILevelElement
	{
		protected class ElementDebuggerTypeProxy
		{
			private readonly Element _parent;

			[UsedImplicitly]
			public Type Type => _parent.GetType();

			[UsedImplicitly]
			public string Name => _parent.Name;

			[UsedImplicitly]
			public string Id => _parent.Id;

			[UsedImplicitly]
			public string Bounds => _parent.PrivateBounds?.ToString(CultureInfo.InvariantCulture);

			[UsedImplicitly]
			public double Rotation => _parent.Rotation;

			[UsedImplicitly]
			public double Level => Panel.GetZIndex(_parent);

			[UsedImplicitly]
			protected ElementDebuggerTypeProxy(Element parent)
			{
				_parent = parent;
			}
		}

		private string _id;

		[Obsolete("请使用 Children 属性替代以尽可能减少 Visual 个数。")]
		public static readonly DependencyProperty ChildProperty = DependencyProperty.Register("Child", typeof(UIElement), typeof(Element), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
		{
			Element element = (Element)o;
			UIElement uIElement = (UIElement)args.OldValue;
			UIElement uIElement2 = (UIElement)args.NewValue;
			if (uIElement != null)
			{
				element._childrenContainer.Children.Clear();
			}
			if (uIElement2 != null)
			{
				element._childrenContainer.Children.Add(uIElement2);
			}
		}));

		public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(Element), new PropertyMetadata(null, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
		{
			((Element)d)._childrenContainer.SetValue(ElementLogicalPanel.BackgroundProperty, e.NewValue);
		}));

		public static readonly RoutedEvent BoundsChangedEvent = EventManager.RegisterRoutedEvent("BoundsChangedEvent", RoutingStrategy.Bubble, typeof(ValueChangedRoutedEventHandler<Rect>), typeof(Element));

		public static readonly RoutedEvent BoundsChanged2Event = EventManager.RegisterRoutedEvent("BoundsChanged2Event", RoutingStrategy.Bubble, typeof(ValueChangedRoutedEventHandler<Rect>), typeof(Element));

		private bool _isRaisingBoundsChanged2;

		private readonly RotateTransform _rotateTransform;

		private readonly TranslateTransform _translateTransform;

		private readonly MatrixTransform _matrixTransform;

		private bool _isSettingFromOwnBoundsProperty;

		public static readonly RoutedEvent RotatedEvent = EventManager.RegisterRoutedEvent("RotatedEvent", RoutingStrategy.Bubble, typeof(ValueChangedRoutedEventHandler<double>), typeof(Element));

		private readonly RectangleGeometry _outlineGeometry = new RectangleGeometry();

		private readonly IList<UIElement> _children;

		private readonly EffectContainer _effectContainer;

		private readonly ElementLogicalPanel _childrenContainer;

		private readonly ElementLogicalPanel _attachmentContainer;

		[NotNull]
		public string Id
		{
			get
			{
				return _id ?? (_id = NewId());
			}
			private set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException("value");
				}
				if (!value.Equals(_id))
				{
					_id = value;
				}
			}
		}

		public string ShortId
		{
			get
			{
				if (Id.Length < 7)
				{
					return Id;
				}
				return Id.Substring(0, 7);
			}
		}

		public ExtensionCollection<Element> Extensions { get; }

		[Obsolete("请使用 Children 属性替代以尽可能减少 Visual 个数。如果需要获取所有内容合并后的 Visual，请调用 GetChildrenVisual 方法。")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public UIElement Child
		{
			get
			{
				return (UIElement)GetValue(ChildProperty);
			}
			set
			{
				SetValue(ChildProperty, value);
			}
		}

		public UIElementCollection Children => _childrenContainer.Children;

		public Brush Background
		{
			get
			{
				return (Brush)GetValue(BackgroundProperty);
			}
			set
			{
				SetValue(BackgroundProperty, value);
			}
		}

		private bool IsBoundsInitialzied
		{
			get
			{
				if (DesiredSize == default(Size))
				{
					if (!double.IsNaN(Width))
					{
						return !double.IsNaN(Height);
					}
					return false;
				}
				return true;
			}
		}

		private Rect? PrivateBounds
		{
			get
			{
				if (!IsBoundsInitialzied)
				{
					return null;
				}
				return new Rect(_translateTransform.X, _translateTransform.Y, Width, Height);
			}
		}

		public virtual Rect Bounds
		{
			get
			{
				return new Rect(Location, Size);
			}
			set
			{
				_isSettingFromOwnBoundsProperty = true;
				if (!IsBoundsInitialzied)
				{
					try
					{
						Location = value.Location;
						Size = value.Size;
						return;
					}
					finally
					{
						_isSettingFromOwnBoundsProperty = false;
					}
				}
				bool flag = false;
				bool flag2 = false;
				Rect bounds = Bounds;
				try
				{
					Point location = bounds.Location;
					Point location2 = value.Location;
					if (location != location2)
					{
						flag = true;
						Location = value.Location;
					}
					Size size = bounds.Size;
					Size size2 = value.Size;
					if (size != size2)
					{
						flag2 = true;
						Size = value.Size;
					}
				}
				finally
				{
					_isSettingFromOwnBoundsProperty = false;
				}
				if (flag | flag2)
				{
					OnBoundsChanged(bounds, value);
				}
			}
		}

		public virtual Point Location
		{
			get
			{
				return new Point(_translateTransform.X, _translateTransform.Y);
			}
			set
			{
				if (double.IsNaN(value.X) || double.IsNaN(value.Y))
				{
					throw new ArgumentException($"{GetType().Name}: 位置不允许被设置为 {double.NaN}。", "value");
				}
				Rect? rect = null;
				if (IsBoundsInitialzied)
				{
					rect = Bounds;
				}
				if (_isSettingFromOwnBoundsProperty || !IsBoundsInitialzied)
				{
					_translateTransform.X = value.X;
					_translateTransform.Y = value.Y;
				}
				else
				{
					_translateTransform.X = value.X;
					_translateTransform.Y = value.Y;
					OnBoundsChanged(rect.Value, Bounds);
				}
				if (rect.HasValue)
				{
					RaiseBoundsChanged2(rect.Value, Bounds);
				}
			}
		}

		protected virtual Size Size
		{
			get
			{
				if (!IsBoundsInitialzied)
				{
					UpdateLayout();
					if (!IsBoundsInitialzied)
					{
						Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
						if (!IsBoundsInitialzied)
						{
							throw new InvalidOperationException("无法测量得到元素的尺寸，这可能是实际元素布局过程中的 BUG。元素类型：" + GetType().Name + "。");
						}
					}
				}
				if (double.IsNaN(Width) || double.IsNaN(Height))
				{
					return DesiredSize;
				}
				return new Size(Width, Height);
			}
			set
			{
				if (!object.Equals(Width, value.Width) || !object.Equals(Height, value.Height))
				{
					if (Size.Empty.Equals(value))
					{
						throw new ArgumentException($"{GetType().Name}: 元素尺寸不允许被设置为 {Size.Empty}。", "value");
					}
					if (double.IsNaN(value.Width) || double.IsNaN(value.Height))
					{
						throw new ArgumentException($"{GetType().Name}: 元素尺寸不允许被设置为 {double.NaN}。", "value");
					}
					if (object.Equals(value.Width, 0.0) || object.Equals(value.Height, 0.0))
					{
						throw new ArgumentException(GetType().Name + ": 元素尺寸不允许被设置为 0。", "value");
					}
					if (_isSettingFromOwnBoundsProperty || !IsBoundsInitialzied)
					{
						Width = value.Width;
						Height = value.Height;
						return;
					}
					Rect bounds = Bounds;
					Width = value.Width;
					Height = value.Height;
					Rect bounds2 = Bounds;
					OnBoundsChanged(bounds, bounds2);
				}
			}
		}

		public virtual double Rotation
		{
			get
			{
				return _rotateTransform.Angle;
			}
			set
			{
				if (value < 0.0 || value >= 360.0)
				{
					value = (value % 360.0 + 360.0) % 360.0;
				}
				if (!object.Equals(_rotateTransform.Angle, value))
				{
					double angle = _rotateTransform.Angle;
					_rotateTransform.Angle = value;
					if (IsBoundsInitialzied)
					{
						OnRotationChanged(angle, value);
					}
				}
			}
		}

		public virtual Matrix Transform
		{
			get
			{
				return _matrixTransform.Matrix;
			}
			set
			{
				if (!object.Equals(_matrixTransform.Matrix, value))
				{
					_matrixTransform.Matrix = value;
				}
			}
		}

		public Point LogicalOriginPoint { get; set; }

		public IRotationCenterInfo RotationCenter { get; }

		bool IDefaultRotationInfoHost.IsVisibleByDefault => IsRotateOriginShownByDefault;

		protected virtual bool IsRotateOriginShownByDefault { get; }

		public ElementOutline Outline { get; }

		[Obsolete("请使用 Outline 获取和设置元素的叠加变换。")]
		public new Transform RenderTransform
		{
			get
			{
				return (Transform)GetValue(UIElement.RenderTransformProperty);
			}
			set
			{
				SetValue(UIElement.RenderTransformProperty, value);
			}
		}

		public UIElementCollection Covers => _attachmentContainer.Children;

		protected sealed override int VisualChildrenCount => 0 + ((_effectContainer.Children.Count != 0) ? 1 : 0) + (_childrenContainer.ContainsValidContent ? 1 : 0) + (_attachmentContainer.ContainsValidContent ? 1 : 0);

		protected override IEnumerator LogicalChildren
		{
			get
			{
				foreach (object child in _childrenContainer.Children)
				{
					yield return child;
				}
				foreach (object child2 in _attachmentContainer.Children)
				{
					yield return child2;
				}
			}
		}

		[UsedImplicitly]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string DebuggerDisplay
		{
			get
			{
				Rect? privateBounds = PrivateBounds;
				object obj;
				if (privateBounds.HasValue)
				{
					Rect valueOrDefault = privateBounds.GetValueOrDefault();
					obj = new Rect(Math.Round(valueOrDefault.X), Math.Round(valueOrDefault.Y), Math.Round(valueOrDefault.Width), Math.Round(valueOrDefault.Height)).ToString(CultureInfo.InvariantCulture);
				}
				else
				{
					obj = null;
				}
				string text = (string)obj;
				int zIndex = Panel.GetZIndex(this);
				return $"[{GetType().Name}: {ShortId}] [{text} {Math.Round(Rotation)}°] [{zIndex}]";
			}
		}

		[Obsolete("已经改名成 DefineOutline。")]
		public virtual Geometry RigidGeometryDefinition => DefineOutline(new OutlineDefinitionContext());

		public ElementEffect ElementEffect { get; set; }

		public bool HasMask { get; set; }

		public bool IsErasable { get; set; }

		public bool IsErased { get; set; }

		public virtual Geometry ClipGeometry
		{
			get
			{
				return _childrenContainer.Clip;
			}
			set
			{
				Geometry clip = _childrenContainer.Clip;
				_childrenContainer.Clip = value;
				RaiseEvent(new ElementClipGeometryChangedEventArgs(Board.ElementClipGeometryChangedEvent, this, clip, value));
			}
		}

		public event ValueChangedRoutedEventHandler<Rect> BoundsChanged
		{
			add
			{
				AddHandler(BoundsChangedEvent, value);
			}
			remove
			{
				RemoveHandler(BoundsChangedEvent, value);
			}
		}

		private event ValueChangedRoutedEventHandler<Rect> BoundsChanged2
		{
			add
			{
				AddHandler(BoundsChanged2Event, value);
			}
			remove
			{
				RemoveHandler(BoundsChanged2Event, value);
			}
		}

		public event ValueChangedRoutedEventHandler<double> Rotated
		{
			add
			{
				AddHandler(RotatedEvent, value);
			}
			remove
			{
				RemoveHandler(RotatedEvent, value);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public Element()
		{
			_rotateTransform = new RotateTransform();
			_translateTransform = new TranslateTransform();
			_matrixTransform = new MatrixTransform();
			_children = new List<UIElement>
			{
				(_effectContainer = new EffectContainer(this, AddVisualChild, RemoveVisualChild)),
				(_childrenContainer = new ElementLogicalPanel(this, AddVisualChild, RemoveVisualChild)),
				(_attachmentContainer = new ElementLogicalPanel(this, AddVisualChild, RemoveVisualChild))
			};
			LogicalOriginPoint = new Point(0.5, 0.5);
			TransformGroup renderTransform = new TransformGroup
			{
				Children = { (Transform)_rotateTransform }
			};
			foreach (UIElement item in _children.OfType<UIElement>())
			{
				item.RenderTransformOrigin = LogicalOriginPoint;
				item.RenderTransform = renderTransform;
			}
			RenderTransform = new TransformGroup
			{
				Children = 
				{
					(Transform)_translateTransform,
					(Transform)_matrixTransform
				}
			};
			Outline = new ElementOutline(this);
			Extensions = new ExtensionCollection<Element>(this);
			ElementEffect = new ElementEffect(this);
			SizeChanged += OnSizeChanged;
			RotationCenter = new RotationCenterInfo(this);
		}

		[Obsolete("Element 不应该被修改，如果需要在存储修改，请使用 IIdDefinition.SetId修改。")]
		public void SetId([NotNull] string id)
		{
			Id = id;
		}

		void IIdDefinition.SetId(string id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			Id = id;
		}

		public Visual GetChildrenVisual()
		{
			return _childrenContainer;
		}

		protected void OnBoundsChanged(Rect oldBounds, Rect newBounds)
		{
			if (!_isSettingFromOwnBoundsProperty)
			{
				if (Math.Abs(oldBounds.Width - newBounds.Width) > 0.01 || Math.Abs(oldBounds.Height - newBounds.Height) > 0.01)
				{
					SetErasedClip(null, true, new ScaleTransform(newBounds.Width / oldBounds.Width, newBounds.Height / oldBounds.Height));
					SetMaskClip(null, true, new ScaleTransform(newBounds.Width / oldBounds.Width, newBounds.Height / oldBounds.Height));
				}
				OnPropertyChanged("Bounds");
				RaiseEvent(new ValueChangedRoutedEventArgs<Rect>(BoundsChangedEvent, oldBounds, newBounds, this));
			}
		}

		private void RaiseBoundsChanged2(Rect oldBounds, Rect newBounds)
		{
			if (_isRaisingBoundsChanged2)
			{
				throw new InvalidOperationException("Bounds is not allowed to set explicitly or implicitly during BoundsChanged2 Event.");
			}
			_isRaisingBoundsChanged2 = true;
			try
			{
				RaiseEvent(new ValueChangedRoutedEventArgs<Rect>(BoundsChanged2Event, oldBounds, newBounds, this));
			}
			finally
			{
				_isRaisingBoundsChanged2 = false;
			}
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (!_isSettingFromOwnBoundsProperty)
			{
				OnPropertyChanged("Bounds");
				Point location = Location;
				RaiseBoundsChanged2(new Rect(location, e.PreviousSize), new Rect(location, e.NewSize));
			}
		}

		protected void OnRotationChanged(double oldAngle, double newAngle)
		{
			if (!_isSettingFromOwnBoundsProperty)
			{
				OnPropertyChanged("Rotation");
				RaiseEvent(new ValueChangedRoutedEventArgs<double>(RotatedEvent, oldAngle, newAngle, this));
			}
		}

		public virtual Geometry DefineOutline(IOutlineDefinetionContext context)
		{
			Size size = Size;
			if (!object.Equals(_outlineGeometry.Rect.Size, size))
			{
				_outlineGeometry.Rect = new Rect(Size);
			}
			return _outlineGeometry;
		}

		protected sealed override Visual GetVisualChild(int index)
		{
			if (_effectContainer.Children.Count != 0)
			{
				if (index == 0)
				{
					return _effectContainer;
				}
				index--;
			}
			if (_childrenContainer.ContainsValidContent)
			{
				if (index == 0)
				{
					return _childrenContainer;
				}
				index--;
			}
			if (_attachmentContainer.ContainsValidContent)
			{
				if (index == 0)
				{
					return _attachmentContainer;
				}
				index--;
			}
			return null;
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			Size size;
			if (!_childrenContainer.ContainsValidContent)
			{
				size = default(Size);
			}
			else
			{
				_childrenContainer.Measure(availableSize);
				size = _childrenContainer.DesiredSize;
			}
			if (!Nearly.Equals(size, default(Size)))
			{
				_attachmentContainer.Measure(size);
			}
			else if (!double.IsNaN(Width) && !double.IsNaN(Height))
			{
				_attachmentContainer.Measure(new Size(Width, Height));
			}
			return size;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			Rect finalRect;
			if (!_childrenContainer.ContainsValidContent)
			{
				finalRect = Bounds;
			}
			else
			{
				finalRect = new Rect(finalSize);
				_childrenContainer.Arrange(finalRect);
			}
			ITransformedBounds transformedBounds = this as ITransformedBounds;
			if (transformedBounds != null)
			{
				Rect transformedBounds2 = transformedBounds.TransformedBounds;
				finalRect = new Rect(transformedBounds2.Size);
				if (finalRect.IsEmpty)
				{
					if (DebuggingProperties.IsDebug && Debugger.IsAttached)
					{
						Debugger.Break();
						throw new ArgumentException("有元素 " + GetType().FullName + " 声明自己是 ITransformedBounds 但是返回的 Size 是空，不能在页面里面存放宽度或高度为空的元素");
					}
				}
				else
				{
					finalRect.Offset(transformedBounds2.Location - Location);
				}
			}
			if (!finalRect.IsEmpty)
			{
				_attachmentContainer.Arrange(finalRect);
			}
			return finalSize;
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private static string NewId()
		{
			return Guid.NewGuid().ToString("N");
		}

		public Visual GetContentPresenter()
		{
			return _childrenContainer;
		}

		public virtual void SetShadow(ShadowInfo shadowInfo)
		{
			ElementEffect.SetShadow(RigidGeometryDefinition, shadowInfo, Rotation);
		}

		public virtual void SetReflection(ReflectionInfo reflectionInfo)
		{
			ElementEffect.SetReflection(reflectionInfo);
		}

		public void AddEffect(Visual effect, bool isShadow)
		{
			if (isShadow)
			{
				_effectContainer.InsertEffect(0, effect);
			}
			else
			{
				_effectContainer.AddEffect(effect);
			}
		}

		public void RemoveEffect(Visual effect)
		{
			_effectContainer.RemoveEffect(effect);
		}

		public virtual void SetErasedClip(Geometry erasedGeometry, bool sizeChanged, ScaleTransform scale = null)
		{
			if (IsErasable && ClipGeometry != null)
			{
				if (!IsErased)
				{
					PathGeometry pathGeometry = new PathGeometry();
					pathGeometry.AddGeometry(new RectangleGeometry(new Rect(new Point(0.0, 0.0), Bounds.Size)));
					ClipGeometry = pathGeometry;
				}
				else if (sizeChanged && scale != null)
				{
					MatrixTransform matrixTransform = ClipGeometry.Transform as MatrixTransform;
					matrixTransform = ((matrixTransform == null) ? new MatrixTransform(new Matrix(scale.ScaleX, 0.0, 0.0, scale.ScaleY, 0.0, 0.0)) : new MatrixTransform(matrixTransform.Matrix.M11 * scale.ScaleX, 0.0, 0.0, matrixTransform.Matrix.M22 * scale.ScaleY, 0.0, 0.0));
					ClipGeometry.Transform = matrixTransform;
				}
				else if (erasedGeometry != null)
				{
					ClipGeometry = Geometry.Combine(ClipGeometry, erasedGeometry, GeometryCombineMode.Exclude, null);
				}
			}
		}

		public void SetMaskClip(Geometry erasedGeometry, bool sizeChanged, ScaleTransform scale = null)
		{
			if (HasMask && ClipGeometry != null)
			{
				if (sizeChanged && scale != null)
				{
					MatrixTransform matrixTransform = ClipGeometry.Transform as MatrixTransform;
					matrixTransform = ((matrixTransform == null) ? new MatrixTransform(new Matrix(scale.ScaleX, 0.0, 0.0, scale.ScaleY, 0.0, 0.0)) : new MatrixTransform(matrixTransform.Matrix.M11 * scale.ScaleX, 0.0, 0.0, matrixTransform.Matrix.M22 * scale.ScaleY, 0.0, 0.0));
					ClipGeometry.Transform = matrixTransform;
				}
				else if (erasedGeometry != null)
				{
					ClipGeometry = Geometry.Combine(ClipGeometry, erasedGeometry, GeometryCombineMode.Union, null);
				}
			}
		}
	}
}
