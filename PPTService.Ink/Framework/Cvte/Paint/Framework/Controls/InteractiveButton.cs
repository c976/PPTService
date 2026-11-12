using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cvte.Diagnostics.Annotations;
using Cvte.Windows.Mathematics;

namespace Cvte.Paint.Framework.Controls
{
	public class InteractiveButton : ContentControl, ICommandSource
	{
		internal enum ControlBoolFlags : ushort
		{
			ContentIsNotLogical = 1,
			IsSpaceKeyDown = 2,
			HeaderIsNotLogical = 4,
			CommandDisabled = 8,
			ContentIsItem = 0x10,
			HeaderIsItem = 0x20,
			ScrollHostValid = 0x40,
			ContainsSelection = 0x80,
			VisualStateChangeSuspended = 0x100
		}

		private Point _originButtonPosition;

		private readonly Dictionary<int, Point> _touchingPoints = new Dictionary<int, Point>();

		[CanBeNull]
		private UIElement _currentCaptureTarget;

		private const int MouseId = -1;

		private bool _isMoved;

		private Element _element;

		public static readonly RoutedEvent ClickEvent;

		public static readonly DependencyProperty CommandProperty;

		public static readonly DependencyProperty CommandParameterProperty;

		public static readonly DependencyProperty CommandTargetProperty;

		internal static readonly DependencyPropertyKey IsPressedPropertyKey;

		public static readonly DependencyProperty IsPressedProperty;

		internal ControlBoolFlags _controlBoolField;

		public static readonly RoutedEvent CheckedEvent;

		public static readonly RoutedEvent UncheckedEvent;

		public static readonly RoutedEvent IndeterminateEvent;

		public static readonly DependencyProperty IsCheckedProperty;

		public static readonly DependencyProperty IsThreeStateProperty;

		[Browsable(false)]
		[Category("Appearance")]
		[ReadOnly(true)]
		public bool IsPressed
		{
			get
			{
				return (bool)GetValue(IsPressedProperty);
			}
			private set
			{
				SetValue(IsPressedPropertyKey, value);
			}
		}

		[Bindable(true)]
		[Category("Action")]
		[Localizability(LocalizationCategory.NeverLocalize)]
		public ICommand Command
		{
			get
			{
				return (ICommand)GetValue(CommandProperty);
			}
			set
			{
				SetValue(CommandProperty, value);
			}
		}

		protected override bool IsEnabledCore
		{
			get
			{
				if (base.IsEnabledCore)
				{
					return CanExecute;
				}
				return false;
			}
		}

		[Bindable(true)]
		[Category("Action")]
		[Localizability(LocalizationCategory.NeverLocalize)]
		public object CommandParameter
		{
			get
			{
				return GetValue(CommandParameterProperty);
			}
			set
			{
				SetValue(CommandParameterProperty, value);
			}
		}

		[Bindable(true)]
		[Category("Action")]
		public IInputElement CommandTarget
		{
			get
			{
				return (IInputElement)GetValue(CommandTargetProperty);
			}
			set
			{
				SetValue(CommandTargetProperty, value);
			}
		}

		private bool CanExecute
		{
			get
			{
				return !ReadControlFlag(ControlBoolFlags.CommandDisabled);
			}
			set
			{
				if (value != CanExecute)
				{
					WriteControlFlag(ControlBoolFlags.CommandDisabled, !value);
					CoerceValue(UIElement.IsEnabledProperty);
				}
			}
		}

		[Category("Appearance")]
		[TypeConverter(typeof(NullableBoolConverter))]
		[Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
		public bool? IsChecked
		{
			get
			{
				object value = GetValue(IsCheckedProperty);
				if (value == null)
				{
					return null;
				}
				return (bool)value;
			}
			set
			{
				SetValue(IsCheckedProperty, value.HasValue ? ((object)value.Value) : null);
			}
		}

		[Bindable(true)]
		[Category("Behavior")]
		public bool IsThreeState
		{
			get
			{
				return (bool)GetValue(IsThreeStateProperty);
			}
			set
			{
				SetValue(IsThreeStateProperty, value);
			}
		}

		[Category("Behavior")]
		public event RoutedEventHandler Click
		{
			add
			{
				AddHandler(ClickEvent, value);
			}
			remove
			{
				RemoveHandler(ClickEvent, value);
			}
		}

		[Category("Behavior")]
		public event RoutedEventHandler Checked
		{
			add
			{
				AddHandler(CheckedEvent, value);
			}
			remove
			{
				RemoveHandler(CheckedEvent, value);
			}
		}

		[Category("Behavior")]
		public event RoutedEventHandler Unchecked
		{
			add
			{
				AddHandler(UncheckedEvent, value);
			}
			remove
			{
				RemoveHandler(UncheckedEvent, value);
			}
		}

		[Category("Behavior")]
		public event RoutedEventHandler Indeterminate
		{
			add
			{
				AddHandler(IndeterminateEvent, value);
			}
			remove
			{
				RemoveHandler(IndeterminateEvent, value);
			}
		}

		static InteractiveButton()
		{
			ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(InteractiveButton));
			CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(InteractiveButton), new FrameworkPropertyMetadata(null, OnCommandChanged));
			CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(InteractiveButton), new FrameworkPropertyMetadata(null));
			CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(InteractiveButton), new FrameworkPropertyMetadata(null));
			IsPressedPropertyKey = DependencyProperty.RegisterReadOnly("IsPressed", typeof(bool), typeof(InteractiveButton), new FrameworkPropertyMetadata(false, OnIsPressedChanged));
			IsPressedProperty = IsPressedPropertyKey.DependencyProperty;
			CheckedEvent = EventManager.RegisterRoutedEvent("Checked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(InteractiveButton));
			UncheckedEvent = EventManager.RegisterRoutedEvent("Unchecked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(InteractiveButton));
			IndeterminateEvent = EventManager.RegisterRoutedEvent("Indeterminate", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(InteractiveButton));
			IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool?), typeof(InteractiveButton), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, OnIsCheckedChanged));
			IsThreeStateProperty = DependencyProperty.Register("IsThreeState", typeof(bool), typeof(InteractiveButton), new FrameworkPropertyMetadata(false));
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(InteractiveButton), new FrameworkPropertyMetadata(typeof(InteractiveButton)));
		}

		public InteractiveButton()
		{
			PreviewStylusDown += OnStylusDown;
			PreviewStylusMove += OnStylusMove;
			PreviewStylusUp += OnStylusUp;
			PreviewMouseLeftButtonDown += OnPreviewMouseDown;
			PreviewMouseMove += OnPreviewMouseMove;
			PreviewMouseLeftButtonUp += OnPreviewMouseUp;
		}

		private void OnLostStylusCapture(object sender, StylusEventArgs e)
		{
			HandleLostCapture(e, true);
		}

		private void OnLostMouseCapture(object sender, MouseEventArgs e)
		{
			HandleLostCapture(e, false);
		}

		private void HandleLostCapture(InputEventArgs e, bool isStylus)
		{
			if (_currentCaptureTarget == null)
			{
				return;
			}
			_currentCaptureTarget.LostStylusCapture -= OnLostStylusCapture;
			_currentCaptureTarget.LostMouseCapture -= OnLostMouseCapture;
			_touchingPoints.Clear();
			if (!IsPressed)
			{
				return;
			}
			if (isStylus)
			{
				Dispatcher.BeginInvoke((Action)(() =>
				{
					if (IsPressed)
					{
						HandleUp(e, true);
					}
				}));
			}
			else
			{
				HandleUp(e, true);
			}
		}

		private void OnStylusDown(object sender, StylusDownEventArgs e)
		{
			if (_currentCaptureTarget != null)
			{
				return;
			}
			bool flag = _touchingPoints.Any();
			_touchingPoints.Add(e.StylusDevice.Id, e.GetPosition(this));
			if (!flag)
			{
				_currentCaptureTarget = e.OriginalSource as UIElement;
				if (_currentCaptureTarget != null)
				{
					_currentCaptureTarget.LostStylusCapture += OnLostStylusCapture;
				}
				HandleDown(e);
			}
		}

		private void OnStylusMove(object sender, StylusEventArgs e)
		{
			if (_touchingPoints.Any())
			{
				HandleMove();
			}
		}

		private void OnStylusUp(object sender, StylusEventArgs e)
		{
			_touchingPoints.Remove(e.StylusDevice.Id);
			if (_touchingPoints.Any())
			{
				return;
			}
			try
			{
				HandleUp(e, false);
			}
			catch (Exception ex)
			{
				PaintAggregateExceptionBuilder.InvokeThrowException(Dispatcher, ex);
			}
		}

		private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (_currentCaptureTarget == null && e.StylusDevice == null)
			{
				_currentCaptureTarget = e.OriginalSource as UIElement;
				if (_currentCaptureTarget != null)
				{
					_currentCaptureTarget.LostMouseCapture += OnLostMouseCapture;
				}
				HandleDown(e);
			}
		}

		private void OnPreviewMouseMove(object sender, MouseEventArgs e)
		{
			if (e.StylusDevice == null)
			{
				HandleMove();
			}
		}

		private void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.StylusDevice == null)
			{
				HandleUp(e, false);
			}
		}

		private void HandleDown(InputEventArgs e)
		{
			_isMoved = false;
			SetIsPressed(true);
			_originButtonPosition = TranslatePoint(default(Point), Window.GetWindow(this));
			PrepairMouseOver(e);
		}

		private void HandleMove()
		{
			if (Window.GetWindow(this) == null || CheckMoved())
			{
				_isMoved = true;
			}
		}

		private void HandleUp(InputEventArgs e, bool isCanceled)
		{
			if (_currentCaptureTarget != null)
			{
				_currentCaptureTarget.LostStylusCapture -= OnLostStylusCapture;
				_currentCaptureTarget.LostMouseCapture -= OnLostMouseCapture;
				_currentCaptureTarget = null;
				SetIsPressed(false);
				Window window = Window.GetWindow(this);
				if (!isCanceled && !_isMoved && window != null && !CheckMoved() && CheckMouseOver(e))
				{
					PerformClick();
				}
			}
		}

		public void PerformClick()
		{
			OnToggle();
			OnClick();
		}

		private static void OnIsPressedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
		}

		private bool CheckMoved()
		{
			Window window = Window.GetWindow(this);
			if (window != null)
			{
				return !Nearly.Equals(_originButtonPosition, TranslatePoint(default(Point), window), 1.0);
			}
			return true;
		}

		private void PrepairMouseOver(InputEventArgs e)
		{
			StylusEventArgs e2 = e as StylusEventArgs;
			MouseEventArgs e3 = e as MouseEventArgs;
			if (e2 != null)
			{
				PrepairMouseOver(e2.GetPosition);
			}
			else if (e3 != null)
			{
				PrepairMouseOver(e3.GetPosition);
			}
		}

		private bool CheckMouseOver(InputEventArgs e)
		{
			StylusEventArgs e2 = e as StylusEventArgs;
			MouseEventArgs e3 = e as MouseEventArgs;
			if (e2 != null)
			{
				return CheckMouseOver(e2.GetPosition);
			}
			if (e3 != null)
			{
				return CheckMouseOver(e3.GetPosition);
			}
			return true;
		}

		private void PrepairMouseOver(Func<Slide, Point> getPosition)
		{
			if (Slide.GetSlide(this) != null)
			{
				_element = this.TopGroupElement();
			}
		}

		private bool CheckMouseOver(Func<Slide, Point> getPosition)
		{
			Slide slide = Slide.GetSlide(this);
			if (slide == null)
			{
				return true;
			}
			Element objA = null;
			if (this.TopGroupElement() != null)
			{
				objA = slide.HitElement(getPosition(slide), 0.0, ElementHitTestLevel.Always);
			}
			bool result = object.Equals(objA, _element);
			_element = null;
			return result;
		}

		protected virtual void OnClick()
		{
			RoutedEventArgs e = new RoutedEventArgs(ClickEvent, this);
			RaiseEvent(e);
			ExecuteCommandSource(this);
		}

		private void SetIsPressed(bool pressed)
		{
			if (pressed)
			{
				SetValue(IsPressedPropertyKey, pressed);
			}
			else
			{
				ClearValue(IsPressedPropertyKey);
			}
		}

		private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((InteractiveButton)d).OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
		}

		private void OnCommandChanged(ICommand oldCommand, ICommand newCommand)
		{
			if (oldCommand != null)
			{
				UnhookCommand(oldCommand);
			}
			if (newCommand != null)
			{
				HookCommand(newCommand);
			}
		}

		private void UnhookCommand(ICommand command)
		{
			CanExecuteChangedEventManager.RemoveHandler(command, OnCanExecuteChanged);
			UpdateCanExecute();
		}

		private void HookCommand(ICommand command)
		{
			CanExecuteChangedEventManager.AddHandler(command, OnCanExecuteChanged);
			UpdateCanExecute();
		}

		private void OnCanExecuteChanged(object sender, EventArgs e)
		{
			UpdateCanExecute();
		}

		private void UpdateCanExecute()
		{
			if (Command != null)
			{
				CanExecute = CanExecuteCommandSource(this);
			}
			else
			{
				CanExecute = true;
			}
		}

		internal static bool CanExecuteCommandSource(ICommandSource commandSource)
		{
			ICommand command = commandSource.Command;
			if (command != null)
			{
				object commandParameter = commandSource.CommandParameter;
				IInputElement inputElement = commandSource.CommandTarget;
				RoutedCommand routedCommand = command as RoutedCommand;
				if (routedCommand != null)
				{
					if (inputElement == null)
					{
						inputElement = commandSource as IInputElement;
					}
					return routedCommand.CanExecute(commandParameter, inputElement);
				}
				return command.CanExecute(commandParameter);
			}
			return false;
		}

		internal static void ExecuteCommandSource(ICommandSource commandSource)
		{
			ICommand command = commandSource.Command;
			if (command == null)
			{
				return;
			}
			object commandParameter = commandSource.CommandParameter;
			IInputElement inputElement = commandSource.CommandTarget;
			RoutedCommand routedCommand = command as RoutedCommand;
			if (routedCommand != null)
			{
				if (inputElement == null)
				{
					inputElement = commandSource as IInputElement;
				}
				if (routedCommand.CanExecute(commandParameter, inputElement))
				{
					routedCommand.Execute(commandParameter, inputElement);
				}
			}
			else if (command.CanExecute(commandParameter))
			{
				command.Execute(commandParameter);
			}
		}

		internal bool ReadControlFlag(ControlBoolFlags reqFlag)
		{
			return (_controlBoolField & reqFlag) != 0;
		}

		internal void WriteControlFlag(ControlBoolFlags reqFlag, bool set)
		{
			if (set)
			{
				_controlBoolField |= reqFlag;
			}
			else
			{
				_controlBoolField &= (ControlBoolFlags)(ushort)(~(int)reqFlag);
			}
		}

		internal void ChangeValidationVisualState(bool useTransitions)
		{
			if (Validation.GetHasError(this))
			{
				if (IsKeyboardFocused)
				{
					VisualStateManager.GoToState(this, "InvalidFocused", useTransitions);
				}
				else
				{
					VisualStateManager.GoToState(this, "InvalidUnfocused", useTransitions);
				}
			}
			else
			{
				VisualStateManager.GoToState(this, "Valid", useTransitions);
			}
		}

		private static object OnGetIsChecked(DependencyObject d)
		{
			return ((InteractiveButton)d).IsChecked;
		}

		private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			InteractiveButton interactiveButton = (InteractiveButton)d;
			bool? flag2 = (bool?)e.OldValue;
			bool? flag = (bool?)e.NewValue;
			if (flag == true)
			{
				interactiveButton.OnChecked(new RoutedEventArgs(CheckedEvent));
			}
			else if (flag == false)
			{
				interactiveButton.OnUnchecked(new RoutedEventArgs(UncheckedEvent));
			}
			else
			{
				interactiveButton.OnIndeterminate(new RoutedEventArgs(IndeterminateEvent));
			}
			interactiveButton.ChangeValidationVisualState(true);
		}

		protected virtual void OnChecked(RoutedEventArgs e)
		{
			RaiseEvent(e);
		}

		protected virtual void OnUnchecked(RoutedEventArgs e)
		{
			RaiseEvent(e);
		}

		protected virtual void OnIndeterminate(RoutedEventArgs e)
		{
			RaiseEvent(e);
		}

		protected internal virtual void OnToggle()
		{
			bool? flag;
			if (IsChecked != true)
			{
				flag = IsChecked.HasValue;
			}
			else
			{
				flag = (IsThreeState ? ((bool?)null) : new bool?(false));
			}
			SetCurrentValue(IsCheckedProperty, flag);
		}
	}
}
