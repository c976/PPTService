using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace PPTService.Ink.Core.Windows.Input
{
	[Obsolete("本类不支持UIElement3D、ContentElement，请使用PointerInput")]
	public static class DeviceInput
	{
		public static readonly RoutedEvent DeviceClickEvent = EventManager.RegisterRoutedEvent("DeviceClick", RoutingStrategy.Direct, typeof(DeviceEventHandler), typeof(DeviceInput));

		public static readonly RoutedEvent DeviceDragStartedEvent = EventManager.RegisterRoutedEvent("DeviceDragStarted", RoutingStrategy.Direct, typeof(DeviceDragEventHandler), typeof(DeviceInput));

		public static readonly RoutedEvent DeviceDragDeltaEvent = EventManager.RegisterRoutedEvent("DeviceDragDelta", RoutingStrategy.Direct, typeof(DeviceDragEventHandler), typeof(DeviceInput));

		public static readonly RoutedEvent DeviceDragCompletedEvent = EventManager.RegisterRoutedEvent("DeviceDragCompleted", RoutingStrategy.Direct, typeof(DeviceDragEventHandler), typeof(DeviceInput));

		private static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached("Position", typeof(System.Windows.Point), typeof(DeviceInput), new PropertyMetadata(default(System.Windows.Point)));

		private static readonly DependencyProperty IsDragingProperty = DependencyProperty.RegisterAttached("IsDraging", typeof(bool), typeof(DeviceInput), new PropertyMetadata(false));

		private static readonly DependencyProperty DeviceIDProperty = DependencyProperty.RegisterAttached("DeviceID", typeof(int), typeof(DeviceInput), new PropertyMetadata(0));

		public static readonly RoutedEvent DeviceDownEvent = EventManager.RegisterRoutedEvent("DeviceDown", RoutingStrategy.Bubble, typeof(DeviceEventHandler), typeof(DeviceInput));

		public static readonly RoutedEvent DeviceDoubleTapEvent = EventManager.RegisterRoutedEvent("DeviceDoubleTap", RoutingStrategy.Bubble, typeof(DeviceEventHandler), typeof(DeviceInput));

		public static readonly RoutedEvent DeviceMoveEvent = EventManager.RegisterRoutedEvent("DeviceMove", RoutingStrategy.Bubble, typeof(DeviceEventHandler), typeof(DeviceInput));

		public static readonly RoutedEvent DeviceUpEvent = EventManager.RegisterRoutedEvent("DeviceUp", RoutingStrategy.Bubble, typeof(DeviceEventHandler), typeof(DeviceInput));

		public static readonly RoutedEvent PreviewDeviceDownEvent = EventManager.RegisterRoutedEvent("PreviewDeviceDown", RoutingStrategy.Tunnel, typeof(DeviceEventHandler), typeof(DeviceInput));

		public static readonly RoutedEvent PreviewDeviceMoveEvent = EventManager.RegisterRoutedEvent("PreviewDeviceMove", RoutingStrategy.Tunnel, typeof(DeviceEventHandler), typeof(DeviceInput));

		public static readonly RoutedEvent PreviewDeviceUpEvent = EventManager.RegisterRoutedEvent("PreviewDeviceUp", RoutingStrategy.Tunnel, typeof(DeviceEventHandler), typeof(DeviceInput));

		private static readonly DependencyPropertyKey DeviceActionsPropertyKey = DependencyProperty.RegisterAttachedReadOnly("DeviceActionsInternal", typeof(DeviceActionCollection), typeof(DeviceInput), new PropertyMetadata((object)null));

		public static readonly DependencyProperty DeviceActionsProperty = DeviceActionsPropertyKey.DependencyProperty;

		private static readonly Stopwatch ClickWatch = new Stopwatch();

		private static System.Windows.Point _currentPoint;

		private static System.Windows.Point _lastPoint;

		private static int _clickCount = 0;

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.AddDeviceClickHandler")]
		public static void AddDeviceClickHandler(DependencyObject element, DeviceEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				AddDeviceClickEvent(uIElement);
				uIElement.AddHandler(DeviceClickEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.RemoveDeviceClickHandler")]
		public static void RemoveDeviceClickHandler(DependencyObject element, DeviceEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				RemoveDeviceClickEvent(uIElement);
				uIElement.RemoveHandler(DeviceClickEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.AddDeviceDragStartedHandler")]
		public static void AddDeviceDragStartedHandler(DependencyObject element, DeviceDragEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				uIElement.AddHandler(DeviceDragStartedEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.RemoveDeviceDragStartedHandler")]
		public static void RemoveDeviceDragStartedHandler(DependencyObject element, DeviceDragEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				uIElement.RemoveHandler(DeviceDragStartedEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.AddDeviceDragDeltaHandler")]
		public static void AddDeviceDragDeltaHandler(DependencyObject element, DeviceDragEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				AddDeviceDragStartedEvent(uIElement);
				AddDeviceDragDeltaEvent(uIElement);
				AddDeviceDragCompletedEvent(uIElement);
				uIElement.AddHandler(DeviceDragDeltaEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.RemoveDeviceDragDeltaHandler")]
		public static void RemoveDeviceDragDeltaHandler(DependencyObject element, DeviceDragEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				RemoveDeviceDragStartedEvent(uIElement);
				RemoveDeviceDragDeltaEvent(uIElement);
				RemoveDeviceDragCompletedEvent(uIElement);
				uIElement.RemoveHandler(DeviceDragDeltaEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.AddDeviceDragCompletedHandler")]
		public static void AddDeviceDragCompletedHandler(DependencyObject element, DeviceDragEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				uIElement.AddHandler(DeviceDragCompletedEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.RemoveDeviceDragCompletedHandler")]
		public static void RemoveDeviceDragCompletedHandler(DependencyObject element, DeviceDragEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				uIElement.RemoveHandler(DeviceDragCompletedEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.AddDeviceDownHandler")]
		public static void AddDeviceDownHandler(DependencyObject element, DeviceEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				AddDeviceDownEvent(uIElement);
				uIElement.AddHandler(DeviceDownEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.RemoveDeviceDownHandler")]
		public static void RemoveDeviceDownHandler(DependencyObject element, DeviceEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				RemoveDeviceDownEvent(uIElement);
				uIElement.RemoveHandler(DeviceDownEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.AddDeviceDoubleTapHandler")]
		public static void AddDeviceDoubleTapHandler(DependencyObject element, DeviceEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				AddDeviceDoubleTapEvent(uIElement);
				uIElement.AddHandler(DeviceDoubleTapEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.RemoveDeviceDoubleTapHandler")]
		public static void RemoveDeviceDoubleTapHandler(DependencyObject element, DeviceEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				RemoveDoubleTapEvent(uIElement);
				uIElement.RemoveHandler(DeviceDoubleTapEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.AddDeviceMoveHandler")]
		public static void AddDeviceMoveHandler(DependencyObject element, DeviceEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				AddDeviceMoveEvent(uIElement);
				uIElement.AddHandler(DeviceMoveEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.RemoveDeviceMoveHandler")]
		public static void RemoveDeviceMoveHandler(DependencyObject element, DeviceEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				RemoveDeviceMoveEvent(uIElement);
				uIElement.RemoveHandler(DeviceMoveEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.AddDeviceUpHandler")]
		public static void AddDeviceUpHandler(DependencyObject element, DeviceEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				AddDeviceUpEvent(uIElement);
				uIElement.AddHandler(DeviceUpEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.RemoveDeviceUpHandler")]
		public static void RemoveDeviceUpHandler(DependencyObject element, DeviceEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				RemoveDeviceUpEvent(uIElement);
				uIElement.RemoveHandler(DeviceUpEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.AddPreviewDeviceDownHandler")]
		public static void AddPreviewDeviceDownHandler(DependencyObject element, DeviceEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				AddPreviewDeviceDownEvent(uIElement);
				uIElement.AddHandler(PreviewDeviceDownEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.RemovePreviewDeviceDownHandler")]
		public static void RemovePreviewDeviceDownHandler(DependencyObject element, DeviceEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				RemovePreviewDeviceDownEvent(uIElement);
				uIElement.RemoveHandler(PreviewDeviceDownEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.AddPreviewDeviceMoveHandler")]
		public static void AddPreviewDeviceMoveHandler(DependencyObject element, DeviceEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				AddPreviewDeviceMoveEvent(uIElement);
				uIElement.AddHandler(PreviewDeviceMoveEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.RemovePreviewDeviceMoveHandler")]
		public static void RemovePreviewDeviceMoveHandler(DependencyObject element, DeviceEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				RemovePreviewDeviceMoveEvent(uIElement);
				uIElement.RemoveHandler(PreviewDeviceMoveEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.AddPreviewDeviceUpHandler")]
		public static void AddPreviewDeviceUpHandler(DependencyObject element, DeviceEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				AddPreviewDeviceUpEvent(uIElement);
				uIElement.AddHandler(PreviewDeviceUpEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.RemovePreviewDeviceUpHandler")]
		public static void RemovePreviewDeviceUpHandler(DependencyObject element, DeviceEventHandler handler)
		{
			UIElement uIElement = element as UIElement;
			if (uIElement != null && handler != null)
			{
				RemovePreviewDeviceUpEvent(uIElement);
				uIElement.RemoveHandler(PreviewDeviceUpEvent, handler);
			}
		}

		[Obsolete("本方法不支持UIElement3D、ContentElement，请使用PointInput.GetDeviceActions")]
		public static DeviceActionCollection GetDeviceActions(DependencyObject element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			DeviceActionCollection deviceActionCollection = element.GetValue(DeviceActionsProperty) as DeviceActionCollection;
			if (deviceActionCollection != null)
			{
				return deviceActionCollection;
			}
			deviceActionCollection = new DeviceActionCollection(element);
			element.SetValue(DeviceActionsPropertyKey, deviceActionCollection);
			return deviceActionCollection;
		}

		internal static void AddDeviceClickEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				uiElement.AddHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(OnDeviceClick), true);
				uiElement.AddHandler(UIElement.StylusDownEvent, new StylusDownEventHandler(OnDeviceClick), true);
				uiElement.AddHandler(UIElement.TouchDownEvent, new EventHandler<TouchEventArgs>(OnDeviceClick), true);
			}
		}

		internal static void AddDeviceDragStartedEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				uiElement.AddHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(OnDeviceDragStarted), true);
				uiElement.AddHandler(UIElement.StylusDownEvent, new StylusDownEventHandler(OnDeviceDragStarted), true);
				uiElement.AddHandler(UIElement.TouchDownEvent, new EventHandler<TouchEventArgs>(OnDeviceDragStarted), true);
			}
		}

		internal static void AddDeviceDragDeltaEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				uiElement.AddHandler(UIElement.MouseMoveEvent, new System.Windows.Input.MouseEventHandler(OnDeviceDragDelta), true);
				uiElement.AddHandler(UIElement.StylusMoveEvent, new StylusEventHandler(OnDeviceDragDelta), true);
				uiElement.AddHandler(UIElement.TouchMoveEvent, new EventHandler<TouchEventArgs>(OnDeviceDragDelta), true);
			}
		}

		internal static void AddDeviceDragCompletedEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				uiElement.AddHandler(UIElement.MouseUpEvent, new MouseButtonEventHandler(OnDeviceDragCompleted), true);
				uiElement.AddHandler(UIElement.StylusUpEvent, new StylusEventHandler(OnDeviceDragCompleted), true);
				uiElement.AddHandler(UIElement.TouchUpEvent, new EventHandler<TouchEventArgs>(OnDeviceDragCompleted), true);
			}
		}

		internal static void AddDeviceDownEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				Mouse.AddMouseDownHandler(uiElement, OnDeviceDown);
				Stylus.AddStylusDownHandler(uiElement, OnDeviceDown);
				uiElement.AddHandler(UIElement.TouchDownEvent, new EventHandler<TouchEventArgs>(OnDeviceDown));
			}
		}

		internal static void AddDeviceDoubleTapEvent(UIElement uiElement)
		{
			uiElement?.AddHandler(UIElement.TouchDownEvent, new EventHandler<TouchEventArgs>(OnDeviceTouchDown));
		}

		internal static void AddDeviceMoveEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				Mouse.AddMouseMoveHandler(uiElement, OnDeviceMove);
				Stylus.AddStylusMoveHandler(uiElement, OnDeviceMove);
				uiElement.AddHandler(UIElement.TouchMoveEvent, new EventHandler<TouchEventArgs>(OnDeviceMove));
			}
		}

		internal static void AddDeviceUpEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				Mouse.AddMouseUpHandler(uiElement, OnDeviceUp);
				Stylus.AddStylusUpHandler(uiElement, OnDeviceUp);
				uiElement.AddHandler(UIElement.TouchUpEvent, new EventHandler<TouchEventArgs>(OnDeviceUp));
			}
		}

		internal static void AddPreviewDeviceDownEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				Mouse.AddPreviewMouseDownHandler(uiElement, OnPreviewDeviceDown);
				Stylus.AddPreviewStylusDownHandler(uiElement, OnPreviewDeviceDown);
				uiElement.AddHandler(UIElement.PreviewTouchDownEvent, new EventHandler<TouchEventArgs>(OnPreviewDeviceDown));
			}
		}

		internal static void AddPreviewDeviceMoveEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				Mouse.AddPreviewMouseMoveHandler(uiElement, OnPreviewDeviceMove);
				Stylus.AddPreviewStylusMoveHandler(uiElement, OnPreviewDeviceMove);
				uiElement.AddHandler(UIElement.PreviewTouchMoveEvent, new EventHandler<TouchEventArgs>(OnPreviewDeviceMove));
			}
		}

		internal static void AddPreviewDeviceUpEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				Mouse.AddPreviewMouseUpHandler(uiElement, OnPreviewDeviceUp);
				Stylus.AddPreviewStylusUpHandler(uiElement, OnPreviewDeviceUp);
				uiElement.AddHandler(UIElement.PreviewTouchUpEvent, new EventHandler<TouchEventArgs>(OnPreviewDeviceUp));
			}
		}

		internal static void RemoveDeviceClickEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				uiElement.RemoveHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(OnDeviceClick));
				uiElement.RemoveHandler(UIElement.StylusDownEvent, new StylusDownEventHandler(OnDeviceClick));
				uiElement.RemoveHandler(UIElement.TouchDownEvent, new EventHandler<TouchEventArgs>(OnDeviceClick));
			}
		}

		internal static void RemoveDeviceDragStartedEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				uiElement.RemoveHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(OnDeviceDragStarted));
				uiElement.RemoveHandler(UIElement.StylusDownEvent, new StylusDownEventHandler(OnDeviceDragStarted));
				uiElement.RemoveHandler(UIElement.TouchDownEvent, new EventHandler<TouchEventArgs>(OnDeviceDragStarted));
			}
		}

		internal static void RemoveDeviceDragDeltaEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				uiElement.RemoveHandler(UIElement.MouseMoveEvent, new System.Windows.Input.MouseEventHandler(OnDeviceDragDelta));
				uiElement.RemoveHandler(UIElement.StylusMoveEvent, new StylusEventHandler(OnDeviceDragDelta));
				uiElement.RemoveHandler(UIElement.TouchMoveEvent, new EventHandler<TouchEventArgs>(OnDeviceDragDelta));
			}
		}

		internal static void RemoveDeviceDragCompletedEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				uiElement.RemoveHandler(UIElement.MouseUpEvent, new MouseButtonEventHandler(OnDeviceDragCompleted));
				uiElement.RemoveHandler(UIElement.StylusUpEvent, new StylusEventHandler(OnDeviceDragCompleted));
				uiElement.RemoveHandler(UIElement.TouchUpEvent, new EventHandler<TouchEventArgs>(OnDeviceDragCompleted));
			}
		}

		internal static void RemoveDeviceDownEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				Mouse.RemoveMouseDownHandler(uiElement, OnDeviceDown);
				Stylus.RemoveStylusDownHandler(uiElement, OnDeviceDown);
				uiElement.RemoveHandler(UIElement.TouchDownEvent, new EventHandler<TouchEventArgs>(OnDeviceDown));
			}
		}

		internal static void RemoveDoubleTapEvent(UIElement uiElement)
		{
			uiElement?.RemoveHandler(UIElement.TouchDownEvent, new EventHandler<TouchEventArgs>(OnDeviceTouchDown));
		}

		internal static void RemoveDeviceMoveEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				Mouse.RemoveMouseMoveHandler(uiElement, OnDeviceMove);
				Stylus.RemoveStylusMoveHandler(uiElement, OnDeviceMove);
				uiElement.RemoveHandler(UIElement.TouchMoveEvent, new EventHandler<TouchEventArgs>(OnDeviceMove));
			}
		}

		internal static void RemoveDeviceUpEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				Mouse.RemoveMouseUpHandler(uiElement, OnDeviceUp);
				Stylus.RemoveStylusUpHandler(uiElement, OnDeviceUp);
				uiElement.RemoveHandler(UIElement.TouchUpEvent, new EventHandler<TouchEventArgs>(OnDeviceUp));
			}
		}

		internal static void RemovePreviewDeviceDownEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				Mouse.RemovePreviewMouseDownHandler(uiElement, OnPreviewDeviceDown);
				Stylus.RemovePreviewStylusDownHandler(uiElement, OnPreviewDeviceDown);
				uiElement.RemoveHandler(UIElement.PreviewTouchDownEvent, new EventHandler<TouchEventArgs>(OnPreviewDeviceDown));
			}
		}

		internal static void RemovePreviewDeviceMoveEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				Mouse.RemovePreviewMouseMoveHandler(uiElement, OnPreviewDeviceMove);
				Stylus.RemovePreviewStylusMoveHandler(uiElement, OnPreviewDeviceMove);
				uiElement.RemoveHandler(UIElement.PreviewTouchMoveEvent, new EventHandler<TouchEventArgs>(OnPreviewDeviceMove));
			}
		}

		internal static void RemovePreviewDeviceUpEvent(UIElement uiElement)
		{
			if (uiElement != null)
			{
				Mouse.RemovePreviewMouseUpHandler(uiElement, OnPreviewDeviceUp);
				Stylus.RemovePreviewStylusUpHandler(uiElement, OnPreviewDeviceUp);
				uiElement.RemoveHandler(UIElement.PreviewTouchUpEvent, new EventHandler<TouchEventArgs>(OnPreviewDeviceUp));
			}
		}

		internal static void OnDeviceClick(object sender, InputEventArgs e)
		{
			if (!IsFromTouch(e))
			{
				RaiseDeviceEvent(sender, e, DeviceClickEvent);
				e.Handled = true;
			}
		}

		internal static void OnDeviceDragStarted(object sender, InputEventArgs e)
		{
			if (IsFromTouch(e))
			{
				return;
			}
			UIElement uIElement = sender as UIElement;
			if (uIElement != null)
			{
				bool flag = (bool)uIElement.GetValue(IsDragingProperty);
				e.Handled = true;
				if (!flag)
				{
					uIElement.SetValue(IsDragingProperty, true);
					System.Windows.Point position = ((DeviceEventArgs)e).GetPosition((IInputElement)sender);
					uIElement.SetValue(PositionProperty, position);
					DeviceDragEventArgs e2 = new DeviceDragEventArgs(e, 0.0, 0.0)
					{
						RoutedEvent = DeviceDragStartedEvent,
						OldPosition = position,
						NewPosition = position,
						Source = e.OriginalSource
					};
					uIElement.SetValue(DeviceIDProperty, e2.DeviceId);
					uIElement.RaiseEvent(e2);
					uIElement.CaptureDevice();
					uIElement.LostMouseCapture += DragedLostCapture;
					uIElement.LostStylusCapture += DragedLostCapture;
					uIElement.LostTouchCapture += DragedLostCapture;
				}
			}
		}

		private static void DragedLostCapture(object sender, InputEventArgs e)
		{
			UIElement uIElement = sender as UIElement;
			if (uIElement != null && uIElement.GetValue(IsDragingProperty) is bool && (bool)uIElement.GetValue(IsDragingProperty))
			{
				DeviceEventArgs e2 = new DeviceEventArgs(e);
				if (e2.DeviceType == DeviceType.Mouse)
				{
					System.Windows.Input.MouseEventArgs e3 = (System.Windows.Input.MouseEventArgs)e;
					uIElement.RaiseEvent(new MouseButtonEventArgs(e3.MouseDevice, e.Timestamp, MouseButton.Left)
					{
						RoutedEvent = UIElement.MouseUpEvent
					});
				}
				else if (e2.DeviceType == DeviceType.Stylus)
				{
					StylusEventArgs e4 = (StylusEventArgs)e;
					uIElement.RaiseEvent(new StylusEventArgs(e4.StylusDevice, e.Timestamp)
					{
						RoutedEvent = UIElement.StylusUpEvent
					});
				}
				else
				{
					TouchEventArgs e5 = (TouchEventArgs)e;
					uIElement.RaiseEvent(new TouchEventArgs(e5.TouchDevice, e.Timestamp)
					{
						RoutedEvent = UIElement.StylusUpEvent
					});
				}
			}
		}

		internal static void OnDeviceDragDelta(object sender, InputEventArgs e)
		{
			if (IsFromTouch(e))
			{
				return;
			}
			UIElement uIElement = sender as UIElement;
			if (uIElement == null)
			{
				return;
			}
			bool flag = uIElement.GetValue(IsDragingProperty) is bool && (bool)uIElement.GetValue(IsDragingProperty);
			DeviceEventArgs e2 = (DeviceEventArgs)e;
			if (e2.DeviceState == DeviceState.Pressed && flag)
			{
				e.Handled = true;
				if ((int)uIElement.GetValue(DeviceIDProperty) == e2.DeviceId)
				{
					System.Windows.Point position = e2.GetPosition((IInputElement)sender);
					System.Windows.Point point = (System.Windows.Point)uIElement.GetValue(PositionProperty);
					Vector vector = position - point;
					uIElement.RaiseEvent(new DeviceDragEventArgs(e, vector.X, vector.Y)
					{
						RoutedEvent = DeviceDragDeltaEvent,
						OldPosition = point,
						NewPosition = position,
						Source = e.OriginalSource
					});
				}
			}
		}

		internal static void OnDeviceDragCompleted(object sender, InputEventArgs e)
		{
			if (IsFromTouch(e))
			{
				return;
			}
			UIElement uIElement = sender as UIElement;
			if (uIElement != null)
			{
				System.Windows.Point position = ((DeviceEventArgs)e).GetPosition((IInputElement)sender);
				DeviceDragEventArgs e2 = new DeviceDragEventArgs(e, 0.0, 0.0)
				{
					RoutedEvent = DeviceDragCompletedEvent,
					OldPosition = position,
					NewPosition = position,
					Source = e.OriginalSource
				};
				if ((int)uIElement.GetValue(DeviceIDProperty) == e2.DeviceId)
				{
					uIElement.SetValue(IsDragingProperty, false);
					uIElement.SetValue(PositionProperty, position);
					uIElement.SetValue(DeviceIDProperty, 0);
					uIElement.RaiseEvent(e2);
					uIElement.ReleaseDeviceCapture();
				}
			}
		}

		internal static void OnDeviceDown(object sender, InputEventArgs e)
		{
			if (!IsFromTouch(e))
			{
				RaiseDeviceEvent(sender, e, DeviceDownEvent);
				e.Handled = true;
			}
		}

		internal static void OnDeviceTouchDown(object sender, TouchEventArgs e)
		{
			_currentPoint = e.GetTouchPoint((IInputElement)sender).Position;
			if (_clickCount == 0)
			{
				ClickWatch.Restart();
				_clickCount++;
			}
			else if (_clickCount == 1)
			{
				_clickCount = 0;
				TimeSpan elapsed = ClickWatch.Elapsed;
				double length = (_currentPoint - _lastPoint).Length;
				System.Drawing.Size doubleClickSize = SystemInformation.DoubleClickSize;
				double num = Math.Sqrt(doubleClickSize.Width * doubleClickSize.Width + doubleClickSize.Height * doubleClickSize.Height);
				if (elapsed.TotalMilliseconds <= (double)SystemInformation.DoubleClickTime && length <= num)
				{
					RaiseDeviceEvent(sender, e, DeviceDoubleTapEvent);
				}
			}
			_lastPoint = _currentPoint;
		}

		internal static void OnDeviceMove(object sender, InputEventArgs e)
		{
			if (!IsFromTouch(e))
			{
				RaiseDeviceEvent(sender, e, DeviceMoveEvent);
				e.Handled = true;
			}
		}

		internal static void OnDeviceUp(object sender, InputEventArgs e)
		{
			if (!IsFromTouch(e) && (e is StylusEventArgs || e is System.Windows.Input.MouseEventArgs))
			{
				RaiseDeviceEvent(sender, e, DeviceUpEvent);
			}
		}

		internal static void OnPreviewDeviceDown(object sender, InputEventArgs e)
		{
			if (!IsFromTouch(e))
			{
				RaiseDeviceEvent(sender, e, PreviewDeviceDownEvent);
				e.Handled = true;
			}
		}

		internal static void OnPreviewDeviceMove(object sender, InputEventArgs e)
		{
			if (!IsFromTouch(e))
			{
				RaiseDeviceEvent(sender, e, PreviewDeviceMoveEvent);
				e.Handled = true;
			}
		}

		internal static void OnPreviewDeviceUp(object sender, InputEventArgs e)
		{
			if (!IsFromTouch(e))
			{
				RaiseDeviceEvent(sender, e, PreviewDeviceUpEvent);
				e.Handled = true;
			}
		}

		private static bool IsFromTouch(InputEventArgs e)
		{
			System.Windows.Input.MouseEventArgs e2 = e as System.Windows.Input.MouseEventArgs;
			if (e2 != null && e2.StylusDevice != null)
			{
				return true;
			}
			return false;
		}

		private static void RaiseDeviceEvent(object sender, InputEventArgs e, RoutedEvent routedEvent)
		{
			(sender as UIElement)?.RaiseEvent(new DeviceEventArgs(e)
			{
				RoutedEvent = routedEvent,
				Source = e.OriginalSource
			});
		}

		public static void CaptureDevice(this UIElement element, TouchDevice device = null)
		{
			element.CaptureMouse();
			element.CaptureStylus();
			if (device != null)
			{
				element.CaptureTouch(device);
			}
		}

		public static void ReleaseDeviceCapture(this UIElement element, TouchDevice device = null)
		{
			element.ReleaseMouseCapture();
			element.ReleaseStylusCapture();
			if (device != null)
			{
				element.ReleaseTouchCapture(device);
			}
		}

		public static bool IsDeviceCaptured(this UIElement element)
		{
			if (!element.IsMouseCaptured)
			{
				return element.IsStylusCaptured;
			}
			return true;
		}
	}
}
