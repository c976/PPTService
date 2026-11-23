using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace PPTServiceUltra.Ink.Core.Windows.Input
{
	[Obsolete("请使用 PointerInput 代码")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class PointerInput3<T> where T : DependencyObject, IInputElement
	{
		public static readonly RoutedEvent PointerClickEvent = EventManager.RegisterRoutedEvent("PointerClick", RoutingStrategy.Direct, typeof(DeviceEventHandler), typeof(PointerInput3<T>));

		public static readonly RoutedEvent PointerDragStartedEvent = EventManager.RegisterRoutedEvent("PointerDragStarted", RoutingStrategy.Direct, typeof(DeviceDragEventHandler), typeof(PointerInput3<T>));

		public static readonly RoutedEvent PointerDragDeltaEvent = EventManager.RegisterRoutedEvent("PointerDragDelta", RoutingStrategy.Direct, typeof(DeviceDragEventHandler), typeof(PointerInput3<T>));

		public static readonly RoutedEvent PointerDragCompletedEvent = EventManager.RegisterRoutedEvent("PointerDragCompleted", RoutingStrategy.Direct, typeof(DeviceDragEventHandler), typeof(PointerInput3<T>));

		private static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached("Position", typeof(System.Windows.Point), typeof(PointerInput3<T>), new PropertyMetadata(default(System.Windows.Point)));

		private static readonly DependencyProperty IsDragingProperty = DependencyProperty.RegisterAttached("IsDraging", typeof(bool), typeof(PointerInput3<T>), new PropertyMetadata(false));

		private static readonly DependencyProperty PointerIDProperty = DependencyProperty.RegisterAttached("PointerID", typeof(int), typeof(PointerInput3<T>), new PropertyMetadata(0));

		public static readonly RoutedEvent PointerDownEvent = EventManager.RegisterRoutedEvent("PointerDown", RoutingStrategy.Bubble, typeof(DeviceEventHandler), typeof(PointerInput3<T>));

		public static readonly RoutedEvent PointerDoubleTapEvent = EventManager.RegisterRoutedEvent("PointerDoubleTap", RoutingStrategy.Bubble, typeof(DeviceEventHandler), typeof(PointerInput3<T>));

		public static readonly RoutedEvent PointerMoveEvent = EventManager.RegisterRoutedEvent("PointerMove", RoutingStrategy.Bubble, typeof(DeviceEventHandler), typeof(PointerInput3<T>));

		public static readonly RoutedEvent PointerUpEvent = EventManager.RegisterRoutedEvent("PointerUp", RoutingStrategy.Bubble, typeof(DeviceEventHandler), typeof(PointerInput3<T>));

		public static readonly RoutedEvent PreviewPointerDownEvent = EventManager.RegisterRoutedEvent("PreviewPointerDown", RoutingStrategy.Tunnel, typeof(DeviceEventHandler), typeof(PointerInput3<T>));

		public static readonly RoutedEvent PreviewPointerMoveEvent = EventManager.RegisterRoutedEvent("PreviewPointerMove", RoutingStrategy.Tunnel, typeof(DeviceEventHandler), typeof(PointerInput3<T>));

		public static readonly RoutedEvent PreviewPointerUpEvent = EventManager.RegisterRoutedEvent("PreviewPointerUp", RoutingStrategy.Tunnel, typeof(DeviceEventHandler), typeof(PointerInput3<T>));

		private static readonly DependencyPropertyKey DeviceActionsPropertyKey = DependencyProperty.RegisterAttachedReadOnly("DeviceActionsInternal", typeof(DeviceActionCollection), typeof(PointerInput3<T>), new PropertyMetadata((object)null));

		public static readonly DependencyProperty PointerActionsProperty = DeviceActionsPropertyKey.DependencyProperty;

		private readonly Stopwatch ClickWatch = new Stopwatch();

		private System.Windows.Point _currentPoint;

		private System.Windows.Point _lastPoint;

		private int _clickCount;

		private static DeviceInputDoubleDifferentialValue _tolerance = new DeviceInputDoubleDifferentialValue(1.0, 4.0, 2.0);

		private System.Windows.Point _position;

		private int? _deviceId;

		private const int MouseDeviceId = -1;

		private T Element { get; }

		public bool HandleOrignalArgs { get; set; }

		public PointerInput3(T element)
		{
			Element = element;
		}

		public void AddDownMoveUpHandler(DeviceEventHandler inputDown, DeviceEventHandler inputMove, DeviceEventHandler inputUp)
		{
			AddPointerDownHandler(inputDown);
			AddPointerMoveHandler(inputMove);
			AddPointerUpHandler(inputUp);
		}

		public void AddDragHandler(DeviceDragEventHandler dragStartedHandle, DeviceDragEventHandler dragDeltaHandler, DeviceDragEventHandler dragCompletedHandler)
		{
			AddPointerDragStartedHandler(dragStartedHandle);
			AddPointerDragDeltaHandler(dragDeltaHandler);
			AddPointerDragCompletedHandler(dragCompletedHandler);
		}

		public void AddPointerClickHandler(DeviceEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				element.AddHandler(PointerClickEvent, handler);
			}
		}

		public void RemovePointerClickHandler(DeviceEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				element.RemoveHandler(PointerClickEvent, handler);
			}
		}

		public void AddPointerDragStartedHandler(DeviceDragEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				AddPointerDragStartedEvent(Element);
				element.AddHandler(PointerDragStartedEvent, handler);
			}
		}

		public void RemovePointerDragStartedHandler(DeviceDragEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				element.RemoveHandler(PointerDragStartedEvent, handler);
			}
		}

		public void AddPointerDragDeltaHandler(DeviceDragEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				AddPointerDragStartedEvent(element);
				AddPointerDragDeltaEvent(element);
				AddPointerDragCompletedEvent(element);
				element.AddHandler(PointerDragDeltaEvent, handler);
			}
		}

		public void RemovePointerDragDeltaHandler(DeviceDragEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				RemovePointerDragStartedEvent(element);
				RemovePointerDragDeltaEvent(element);
				RemovePointerDragCompletedEvent(element);
				element.RemoveHandler(PointerDragDeltaEvent, handler);
			}
		}

		public void AddPointerDragCompletedHandler(DeviceDragEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				AddPointerDragCompletedEvent(Element);
				element.AddHandler(PointerDragCompletedEvent, handler);
			}
		}

		public void RemovePointerDragCompletedHandler(DeviceDragEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				element.RemoveHandler(PointerDragCompletedEvent, handler);
			}
		}

		public void AddPointerDownHandler(DeviceEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				AddPointerDownEvent(element);
				element.AddHandler(PointerDownEvent, handler);
			}
		}

		public void RemovePointerDownHandler(DeviceEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				RemovePointerDownEvent(element);
				element.RemoveHandler(PointerDownEvent, handler);
			}
		}

		public void AddPointerDoubleTapHandler(DeviceEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				AddPointerDoubleTapEvent(element);
				element.AddHandler(PointerDoubleTapEvent, handler);
			}
		}

		public void RemovePointerDoubleTapHandler(DeviceEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				RemoveDoubleTapEvent(element);
				element.RemoveHandler(PointerDoubleTapEvent, handler);
			}
		}

		public void AddPointerMoveHandler(DeviceEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				AddPointerMoveEvent(element);
				element.AddHandler(PointerMoveEvent, handler);
			}
		}

		public void RemovePointerMoveHandler(DeviceEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				RemovePointerMoveEvent(element);
				element.RemoveHandler(PointerMoveEvent, handler);
			}
		}

		public void AddPointerUpHandler(DeviceEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				AddPointerUpEvent(element);
				element.AddHandler(PointerUpEvent, handler);
			}
		}

		public void RemovePointerUpHandler(DeviceEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				RemovePointerUpEvent(element);
				element.RemoveHandler(PointerUpEvent, handler);
			}
		}

		public void AddPreviewPointerDownHandler(DeviceEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				AddPreviewPointerDownEvent(element);
				element.AddHandler(PreviewPointerDownEvent, handler);
			}
		}

		public void RemovePreviewPointerDownHandler(DeviceEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				RemovePreviewPointerDownEvent(element);
				element.RemoveHandler(PreviewPointerDownEvent, handler);
			}
		}

		public void AddPreviewPointerMoveHandler(DeviceEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				AddPreviewPointerMoveEvent(element);
				element.AddHandler(PreviewPointerMoveEvent, handler);
			}
		}

		public void RemovePreviewPointerMoveHandler(DeviceEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				RemovePreviewPointerMoveEvent(element);
				element.RemoveHandler(PreviewPointerMoveEvent, handler);
			}
		}

		public void AddPreviewPointerUpHandler(DeviceEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				AddPreviewPointerUpEvent(element);
				element.AddHandler(PreviewPointerUpEvent, handler);
			}
		}

		public void RemovePreviewPointerUpHandler(DeviceEventHandler handler)
		{
			T element = Element;
			if (element != null && handler != null)
			{
				RemovePreviewPointerUpEvent(element);
				element.RemoveHandler(PreviewPointerUpEvent, handler);
			}
		}

		public DeviceActionCollection GetPointerActions()
		{
			if (Element == null)
			{
				throw new ArgumentNullException("Element");
			}
			DeviceActionCollection deviceActionCollection = Element.GetValue(PointerActionsProperty) as DeviceActionCollection;
			if (deviceActionCollection != null)
			{
				return deviceActionCollection;
			}
			deviceActionCollection = new DeviceActionCollection(Element);
			Element.SetValue(DeviceActionsPropertyKey, deviceActionCollection);
			return deviceActionCollection;
		}

		internal void AddPointerDragStartedEvent(T uiElement)
		{
			if (uiElement != null)
			{
				AddHandler(uiElement, UIElement.MouseDownEvent, new MouseButtonEventHandler(OnPointerDragStarted));
				AddHandler(uiElement, UIElement.StylusDownEvent, new StylusDownEventHandler(OnPointerDragStarted));
				AddHandler(uiElement, UIElement.TouchDownEvent, new EventHandler<TouchEventArgs>(OnPointerDragStarted));
			}
		}

		internal void AddPointerDragDeltaEvent(T uiElement)
		{
			if (uiElement != null)
			{
				AddHandler(uiElement, UIElement.MouseMoveEvent, new System.Windows.Input.MouseEventHandler(OnPointerDragDelta));
				AddHandler(uiElement, UIElement.StylusMoveEvent, new StylusEventHandler(OnPointerDragDelta));
				AddHandler(uiElement, UIElement.TouchMoveEvent, new EventHandler<TouchEventArgs>(OnPointerDragDelta));
			}
		}

		internal void AddPointerDragCompletedEvent(T uiElement)
		{
			if (uiElement != null)
			{
				AddHandler(uiElement, UIElement.MouseUpEvent, new MouseButtonEventHandler(OnPointerDragCompleted));
				AddHandler(uiElement, UIElement.StylusUpEvent, new StylusEventHandler(OnPointerDragCompleted));
				AddHandler(uiElement, UIElement.TouchUpEvent, new EventHandler<TouchEventArgs>(OnPointerDragCompleted));
			}
		}

		private void AddPointerDownEvent(T uiElement)
		{
			if (uiElement != null)
			{
				Mouse.AddMouseDownHandler(uiElement, OnPointerDown);
				Stylus.AddStylusDownHandler(uiElement, OnPointerDown);
				uiElement.AddHandler(UIElement.TouchDownEvent, new EventHandler<TouchEventArgs>(OnPointerDown));
			}
		}

		internal void AddPointerDoubleTapEvent(T uiElement)
		{
			if (uiElement != null)
			{
				Mouse.AddMouseDownHandler(uiElement, OnPointerTouchDown);
				Stylus.AddStylusDownHandler(uiElement, OnPointerTouchDown);
				uiElement?.AddHandler(UIElement.TouchDownEvent, new EventHandler<TouchEventArgs>(OnPointerTouchDown));
			}
		}

		internal void AddPointerMoveEvent(T uiElement)
		{
			if (uiElement != null)
			{
				Mouse.AddMouseMoveHandler(uiElement, OnPointerMove);
				Stylus.AddStylusMoveHandler(uiElement, OnPointerMove);
				uiElement.AddHandler(UIElement.TouchMoveEvent, new EventHandler<TouchEventArgs>(OnPointerMove));
			}
		}

		internal void AddPointerUpEvent(T uiElement)
		{
			if (uiElement != null)
			{
				Mouse.AddMouseUpHandler(uiElement, OnPointerUp);
				Stylus.AddStylusUpHandler(uiElement, OnPointerUp);
				uiElement.AddHandler(UIElement.TouchUpEvent, new EventHandler<TouchEventArgs>(OnPointerUp));
			}
		}

		internal void AddPreviewPointerDownEvent(T uiElement)
		{
			if (uiElement != null)
			{
				Mouse.AddPreviewMouseDownHandler(uiElement, OnPreviewPointerDown);
				Stylus.AddPreviewStylusDownHandler(uiElement, OnPreviewPointerDown);
				uiElement.AddHandler(UIElement.PreviewTouchDownEvent, new EventHandler<TouchEventArgs>(OnPreviewPointerDown));
			}
		}

		internal void AddPreviewPointerMoveEvent(T uiElement)
		{
			if (uiElement != null)
			{
				Mouse.AddPreviewMouseMoveHandler(uiElement, OnPreviewPointerMove);
				Stylus.AddPreviewStylusMoveHandler(uiElement, OnPreviewPointerMove);
				uiElement.AddHandler(UIElement.PreviewTouchMoveEvent, new EventHandler<TouchEventArgs>(OnPreviewPointerMove));
			}
		}

		internal void AddPreviewPointerUpEvent(T uiElement)
		{
			if (uiElement != null)
			{
				Mouse.AddPreviewMouseUpHandler(uiElement, OnPreviewPointerUp);
				Stylus.AddPreviewStylusUpHandler(uiElement, OnPreviewPointerUp);
				uiElement.AddHandler(UIElement.PreviewTouchUpEvent, new EventHandler<TouchEventArgs>(OnPreviewPointerUp));
			}
		}

		internal void RemovePointerDragStartedEvent(T uiElement)
		{
			if (uiElement != null)
			{
				RemoveHandler(uiElement, UIElement.MouseDownEvent, new System.Windows.Input.MouseEventHandler(OnPointerDragStarted));
				RemoveHandler(uiElement, UIElement.StylusDownEvent, new StylusEventHandler(OnPointerDragStarted));
				RemoveHandler(uiElement, UIElement.TouchDownEvent, new EventHandler<TouchEventArgs>(OnPointerDragStarted));
			}
		}

		internal void RemovePointerDragDeltaEvent(T uiElement)
		{
			if (uiElement != null)
			{
				RemoveHandler(uiElement, UIElement.MouseMoveEvent, new System.Windows.Input.MouseEventHandler(OnPointerDragDelta));
				RemoveHandler(uiElement, UIElement.StylusMoveEvent, new StylusEventHandler(OnPointerDragDelta));
				RemoveHandler(uiElement, UIElement.TouchMoveEvent, new EventHandler<TouchEventArgs>(OnPointerDragDelta));
			}
		}

		internal void RemovePointerDragCompletedEvent(T uiElement)
		{
			if (uiElement != null)
			{
				RemoveHandler(uiElement, UIElement.MouseUpEvent, new System.Windows.Input.MouseEventHandler(OnPointerDragCompleted));
				RemoveHandler(uiElement, UIElement.StylusUpEvent, new StylusEventHandler(OnPointerDragCompleted));
				RemoveHandler(uiElement, UIElement.TouchUpEvent, new EventHandler<TouchEventArgs>(OnPointerDragCompleted));
			}
		}

		internal void RemovePointerDownEvent(T uiElement)
		{
			if (uiElement != null)
			{
				Mouse.RemoveMouseDownHandler(uiElement, OnPointerDown);
				Stylus.RemoveStylusDownHandler(uiElement, OnPointerDown);
				uiElement.RemoveHandler(UIElement.TouchDownEvent, new EventHandler<TouchEventArgs>(OnPointerDown));
			}
		}

		internal void RemoveDoubleTapEvent(T uiElement)
		{
			uiElement?.RemoveHandler(UIElement.TouchDownEvent, new EventHandler<TouchEventArgs>(OnPointerTouchDown));
		}

		internal void RemovePointerMoveEvent(T uiElement)
		{
			if (uiElement != null)
			{
				Mouse.RemoveMouseMoveHandler(uiElement, OnPointerMove);
				Stylus.RemoveStylusMoveHandler(uiElement, OnPointerMove);
				uiElement.RemoveHandler(UIElement.TouchMoveEvent, new EventHandler<TouchEventArgs>(OnPointerMove));
			}
		}

		internal void RemovePointerUpEvent(T uiElement)
		{
			if (uiElement != null)
			{
				Mouse.RemoveMouseUpHandler(uiElement, OnPointerUp);
				Stylus.RemoveStylusUpHandler(uiElement, OnPointerUp);
				uiElement.RemoveHandler(UIElement.TouchUpEvent, new EventHandler<TouchEventArgs>(OnPointerUp));
			}
		}

		internal void RemovePreviewPointerDownEvent(T uiElement)
		{
			if (uiElement != null)
			{
				Mouse.RemovePreviewMouseDownHandler(uiElement, OnPreviewPointerDown);
				Stylus.RemovePreviewStylusDownHandler(uiElement, OnPreviewPointerDown);
				uiElement.RemoveHandler(UIElement.PreviewTouchDownEvent, new EventHandler<TouchEventArgs>(OnPreviewPointerDown));
			}
		}

		internal void RemovePreviewPointerMoveEvent(T uiElement)
		{
			if (uiElement != null)
			{
				Mouse.RemovePreviewMouseMoveHandler(uiElement, OnPreviewPointerMove);
				Stylus.RemovePreviewStylusMoveHandler(uiElement, OnPreviewPointerMove);
				uiElement.RemoveHandler(UIElement.PreviewTouchMoveEvent, new EventHandler<TouchEventArgs>(OnPreviewPointerMove));
			}
		}

		internal void RemovePreviewPointerUpEvent(T uiElement)
		{
			if (uiElement != null)
			{
				Mouse.RemovePreviewMouseUpHandler(uiElement, OnPreviewPointerUp);
				Stylus.RemovePreviewStylusUpHandler(uiElement, OnPreviewPointerUp);
				uiElement.RemoveHandler(UIElement.PreviewTouchUpEvent, new EventHandler<TouchEventArgs>(OnPreviewPointerUp));
			}
		}

		private void AddHandler(DependencyObject d, RoutedEvent routedEvent, Delegate handler, bool handledEventsToo = false)
		{
			(d as UIElement)?.AddHandler(routedEvent, handler, handledEventsToo);
			(d as ContentElement)?.AddHandler(routedEvent, handler, handledEventsToo);
			(d as UIElement3D)?.AddHandler(routedEvent, handler, handledEventsToo);
		}

		private void RemoveHandler(DependencyObject d, RoutedEvent routedEvent, Delegate handler)
		{
			(d as UIElement)?.RemoveHandler(routedEvent, handler);
			(d as ContentElement)?.RemoveHandler(routedEvent, handler);
			(d as UIElement3D)?.RemoveHandler(routedEvent, handler);
		}

		internal void OnPointerDragStarted(object sender, InputEventArgs e)
		{
			if (IsFromTouch(e))
			{
				return;
			}
			IInputElement inputElement = sender as IInputElement;
			DependencyObject dependencyObject = sender as DependencyObject;
			if (inputElement == null || dependencyObject == null)
			{
				return;
			}
			bool flag = (bool)dependencyObject.GetValue(IsDragingProperty);
			if (HandleOrignalArgs)
			{
				e.Handled = true;
			}
			if (!flag)
			{
				dependencyObject.SetValue(IsDragingProperty, true);
				System.Windows.Point position = ((DeviceEventArgs)e).GetPosition((IInputElement)sender);
				dependencyObject.SetValue(PositionProperty, position);
				DeviceDragEventArgs e2 = new DeviceDragEventArgs(e, 0.0, 0.0)
				{
					RoutedEvent = PointerDragStartedEvent,
					OldPosition = position,
					NewPosition = position,
					Source = e.OriginalSource
				};
				dependencyObject.SetValue(PointerIDProperty, e2.DeviceId);
				inputElement.RaiseEvent(e2);
				CaptureDevice();
				inputElement.LostMouseCapture += DragedLostCapture;
				inputElement.LostStylusCapture += DragedLostCapture;
				if (inputElement is UIElement)
				{
					((UIElement)inputElement).LostTouchCapture += DragedLostCapture;
				}
				else if (inputElement is UIElement3D)
				{
					((UIElement3D)inputElement).LostTouchCapture += DragedLostCapture;
				}
				else if (inputElement is ContentElement)
				{
					((ContentElement)inputElement).LostTouchCapture += DragedLostCapture;
				}
			}
		}

		private void DragedLostCapture(object sender, InputEventArgs e)
		{
			IInputElement inputElement = sender as IInputElement;
			DependencyObject dependencyObject = sender as DependencyObject;
			if (inputElement != null && dependencyObject != null && dependencyObject.GetValue(IsDragingProperty) is bool && (bool)dependencyObject.GetValue(IsDragingProperty))
			{
				DeviceEventArgs e2 = new DeviceEventArgs(e);
				if (e2.DeviceType == DeviceType.Mouse)
				{
					System.Windows.Input.MouseEventArgs e3 = (System.Windows.Input.MouseEventArgs)e;
					inputElement.RaiseEvent(new MouseButtonEventArgs(e3.MouseDevice, e.Timestamp, MouseButton.Left)
					{
						RoutedEvent = UIElement.MouseUpEvent
					});
				}
				else if (e2.DeviceType == DeviceType.Stylus)
				{
					StylusEventArgs e4 = (StylusEventArgs)e;
					inputElement.RaiseEvent(new StylusEventArgs(e4.StylusDevice, e.Timestamp)
					{
						RoutedEvent = UIElement.StylusUpEvent
					});
				}
				else
				{
					TouchEventArgs e5 = (TouchEventArgs)e;
					inputElement.RaiseEvent(new TouchEventArgs(e5.TouchDevice, e.Timestamp)
					{
						RoutedEvent = UIElement.TouchUpEvent
					});
				}
			}
		}

		internal void OnPointerDragDelta(object sender, InputEventArgs e)
		{
			if (IsFromTouch(e))
			{
				return;
			}
			IInputElement inputElement = sender as IInputElement;
			DependencyObject dependencyObject = sender as DependencyObject;
			if (inputElement == null || dependencyObject == null)
			{
				return;
			}
			bool flag = dependencyObject.GetValue(IsDragingProperty) is bool && (bool)dependencyObject.GetValue(IsDragingProperty);
			DeviceEventArgs e2 = (DeviceEventArgs)e;
			if (e2.DeviceState == DeviceState.Pressed && flag)
			{
				if (HandleOrignalArgs)
				{
					e.Handled = true;
				}
				if ((int)dependencyObject.GetValue(PointerIDProperty) == e2.DeviceId)
				{
					System.Windows.Point position = e2.GetPosition((IInputElement)sender);
					System.Windows.Point point = (System.Windows.Point)dependencyObject.GetValue(PositionProperty);
					Vector vector = position - point;
					inputElement.RaiseEvent(new DeviceDragEventArgs(e, vector.X, vector.Y)
					{
						RoutedEvent = PointerDragDeltaEvent,
						OldPosition = point,
						NewPosition = position,
						Source = e.OriginalSource
					});
				}
			}
		}

		internal void OnPointerDragCompleted(object sender, InputEventArgs e)
		{
			if (IsFromTouch(e))
			{
				return;
			}
			IInputElement inputElement = sender as IInputElement;
			DependencyObject dependencyObject = sender as DependencyObject;
			if (inputElement != null && dependencyObject != null)
			{
				System.Windows.Point position = ((DeviceEventArgs)e).GetPosition((IInputElement)sender);
				DeviceDragEventArgs e2 = new DeviceDragEventArgs(e, 0.0, 0.0)
				{
					RoutedEvent = PointerDragCompletedEvent,
					OldPosition = position,
					NewPosition = position,
					Source = e.OriginalSource
				};
				if ((int)dependencyObject.GetValue(PointerIDProperty) == e2.DeviceId)
				{
					dependencyObject.SetValue(IsDragingProperty, false);
					dependencyObject.SetValue(PositionProperty, position);
					dependencyObject.SetValue(PointerIDProperty, 0);
					inputElement.RaiseEvent(e2);
					ReleaseDeviceCapture();
				}
			}
		}

		internal void OnPointerTouchDown(object sender, InputEventArgs e)
		{
			_currentPoint = new DeviceEventArgs(e).GetPosition((IInputElement)sender);
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
					RaisePointerEvent(sender, e, PointerDoubleTapEvent);
				}
			}
			_lastPoint = _currentPoint;
		}

		private void OnPointerMove(object sender, InputEventArgs e)
		{
			if (!IsFromTouch(e))
			{
				RaisePointerEvent(sender, e, PointerMoveEvent);
				if (HandleOrignalArgs)
				{
					e.Handled = true;
				}
			}
		}

		private void OnPointerUp(object sender, InputEventArgs e)
		{
			if (IsFromTouch(e))
			{
				return;
			}
			if (e is StylusEventArgs || e is System.Windows.Input.MouseEventArgs)
			{
				RaisePointerEvent(sender, e, PointerUpEvent);
			}
			if (_deviceId == GetDeviceId(e))
			{
				System.Windows.Point position = GetPosition(e);
				if ((_position - position).Length < GetTolerance(e))
				{
					RaisePointerEvent(Element, e, PointerClickEvent);
				}
			}
		}

		private void OnPointerDown(object sender, InputEventArgs e)
		{
			if (!IsFromTouch(e))
			{
				RaisePointerEvent(sender, e, PointerDownEvent);
				if (HandleOrignalArgs)
				{
					e.Handled = true;
				}
				_deviceId = GetDeviceId(e);
				_position = GetPosition(e);
			}
		}

		private int GetDeviceId(InputEventArgs originalArgs)
		{
			if (originalArgs is System.Windows.Input.MouseEventArgs)
			{
				return -1;
			}
			StylusEventArgs e = originalArgs as StylusEventArgs;
			if (e != null)
			{
				return e.StylusDevice.Id;
			}
			TouchEventArgs e2 = originalArgs as TouchEventArgs;
			if (e2 != null)
			{
				return e2.TouchDevice.Id;
			}
			throw new ArgumentException("自定的 InputEventArgs 无法获得 id，所以这里异常");
		}

		private double GetTolerance(InputEventArgs originalArgs)
		{
			if (!(originalArgs is System.Windows.Input.MouseEventArgs))
			{
				if (originalArgs is StylusEventArgs)
				{
					return _tolerance.Stylus;
				}
				return _tolerance.Touch;
			}
			return _tolerance.Mouse;
		}

		private System.Windows.Point GetPosition(InputEventArgs originalArgs)
		{
			System.Windows.Input.MouseEventArgs e = originalArgs as System.Windows.Input.MouseEventArgs;
			if (e != null)
			{
				return e.GetPosition(Element);
			}
			StylusEventArgs e2 = originalArgs as StylusEventArgs;
			if (e2 != null)
			{
				return e2.GetPosition(Element);
			}
			TouchEventArgs e3 = originalArgs as TouchEventArgs;
			if (e3 != null)
			{
				return e3.GetTouchPoint(Element).Position;
			}
			throw new ArgumentException("自定的 InputEventArgs 无法获得 点击，所以这里异常");
		}

		private void OnPreviewPointerDown(object sender, InputEventArgs e)
		{
			if (!IsFromTouch(e))
			{
				RaisePointerEvent(sender, e, PreviewPointerDownEvent);
				if (HandleOrignalArgs)
				{
					e.Handled = true;
				}
			}
		}

		private void OnPreviewPointerMove(object sender, InputEventArgs e)
		{
			if (!IsFromTouch(e))
			{
				RaisePointerEvent(sender, e, PreviewPointerMoveEvent);
				if (HandleOrignalArgs)
				{
					e.Handled = true;
				}
			}
		}

		private void OnPreviewPointerUp(object sender, InputEventArgs e)
		{
			if (!IsFromTouch(e))
			{
				RaisePointerEvent(sender, e, PreviewPointerUpEvent);
			}
		}

		private bool IsFromTouch(InputEventArgs e)
		{
			if ((e as System.Windows.Input.MouseEventArgs)?.StylusDevice != null)
			{
				return true;
			}
			return false;
		}

		private void RaisePointerEvent(object sender, InputEventArgs e, RoutedEvent routedEvent)
		{
			IInputElement inputElement = sender as IInputElement;
			DependencyObject dependencyObject = sender as DependencyObject;
			if (inputElement != null && dependencyObject != null)
			{
				DeviceEventArgs e2 = new DeviceEventArgs(e)
				{
					RoutedEvent = routedEvent,
					Source = e.OriginalSource
				};
				if (!(bool)dependencyObject.GetValue(DeviceInputSource.CanRespondProperty))
				{
					e2.VirtualType = VirtualDeviceType.LostCapture;
				}
				inputElement.RaiseEvent(e2);
			}
		}

		private void CaptureDevice(TouchDevice device = null)
		{
			Element.CaptureMouse();
			Element.CaptureStylus();
			device?.Capture(Element);
		}

		private void ReleaseDeviceCapture(TouchDevice device = null)
		{
			Element.ReleaseMouseCapture();
			Element.ReleaseStylusCapture();
			device?.Capture(null);
		}

		public bool IsDeviceCaptured()
		{
			if (!Element.IsMouseCaptured)
			{
				return Element.IsStylusCaptured;
			}
			return true;
		}
	}
}
