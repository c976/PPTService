using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Cvte.ComponentModel;
using Cvte.Diagnostics.Annotations;
using Cvte.Diagnostics.Trace;
using Cvte.IO;
using Cvte.Linq;
using Cvte.Windows.Input;

namespace Cvte.Paint.Framework
{
	public class ModeController : IModeController, IActiveModeController, IModeHitTestController
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass34_0
		{
			public ModeController _003C_003E4__this;

			public Mode oldMode;

			public Mode newMode;

			public bool suppressUnselect;

			internal void _003CSwitchModeCore_003Eg__RaiseModeChangingEvent_007C0()
			{
				_003C_003E4__this.EventSource?.RaiseEvent(new RoutedPropertyChangedEventArgs<Mode>(oldMode, newMode, Board.ModeChangingEvent));
			}

			internal void _003CSwitchModeCore_003Eg__SwitchActiveModeAndCurrentMode_007C1()
			{
				if (_003C_003E4__this._activeMode != null)
				{
					Slide currentSlide = _003C_003E4__this._activeMode.CurrentSlide;
					Mode activeMode = _003C_003E4__this._activeMode;
					_003C_003E4__this._activeMode = null;
					try
					{
						activeMode.SwitchOff();
						return;
					}
					finally
					{
						_003C_003E4__this.ClearModeInfo(activeMode, currentSlide);
						activeMode.CurrentSlide = null;
					}
				}
				if (!suppressUnselect)
				{
					_003C_003E4__this._currentMode?.CurrentSlide.Unselect();
				}
				if (_003C_003E4__this._currentMode != null)
				{
					_003C_003E4__this.SwitchOff();
				}
			}

			internal void _003CSwitchModeCore_003Eg__SwitchOnCurrentModeInner_007C2()
			{
				_003C_003E4__this._currentMode = newMode;
				_003C_003E4__this.SwitchOn();
			}

			internal void _003CSwitchModeCore_003Eg__RaiseModeChangedEvent_007C3()
			{
				_003C_003E4__this.EventSource?.RaiseEvent(new RoutedPropertyChangedEventArgs<Mode>(oldMode, newMode, Board.ModeChangedEvent));
			}
		}

		public static readonly DependencyProperty ControllerProperty = DependencyProperty.RegisterAttached("Controller", typeof(ModeController), typeof(ModeController), new FrameworkPropertyMetadata(null, ControllerPropertyChangedCallback));

		[NotNull]
		private readonly Dictionary<int, ModeInputEventArgs> _pressingArgs = new Dictionary<int, ModeInputEventArgs>();

		[NotNull]
		private readonly Dictionary<int, ModeInputEventArgs> _modePressedArgs = new Dictionary<int, ModeInputEventArgs>();

		private IModeHitTestProvider _currentHitTester;

		private UIElement _eventSource;

		private bool _isCurrentModeOn;

		private bool _isCurrentModeSwitching;

		private Mode _currentDeliveringMode;

		private Mode _currentMode = new NoneMode();

		private ModifierKeys _currentModifierKeys;

		private Func<Slide> _getCurrentSlide;

		private readonly List<IModeInputSource> _dataSources = new List<IModeInputSource>
		{
			new DeviceModeInputSource()
		};

		private readonly List<Filter> _filters = new List<Filter>();

		private Mode _activeMode;

		private bool _currentHitTestValue = true;

		public UIElement EventSource
		{
			get
			{
				return _eventSource;
			}
			set
			{
				if (object.Equals(_eventSource, value))
				{
					return;
				}
				if (_eventSource != null)
				{
					foreach (IModeInputSource dataSource in _dataSources)
					{
						DetachSource(dataSource);
					}
				}
				_eventSource = value;
				if (value == null)
				{
					return;
				}
				foreach (IModeInputSource dataSource2 in _dataSources)
				{
					AttachSource(dataSource2);
				}
			}
		}

		public UIElement Container { get; set; }

		private UIElement InternalContainer => Container ?? _eventSource;

		public Mode CurrentMode
		{
			get
			{
				return _currentMode;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "Mode 不能被设置为 null，请使用 NoneMode 替代。");
				}
				if (_currentMode != value)
				{
					SwitchMode(value);
				}
			}
		}

		private bool IsOn { get; set; }

		private IEnumerable<Filter> SortedFilters => _filters;

		Mode IActiveModeController.ActiveMode => _activeMode;

		public static void SetController(DependencyObject element, ModeController value)
		{
			element.SetValue(ControllerProperty, value);
		}

		public static ModeController GetController(DependencyObject element)
		{
			return (ModeController)element.GetValue(ControllerProperty);
		}

		private static void ControllerPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Slide slide = d as Slide;
			ModeController modeController = (ModeController)e.OldValue;
			ModeController modeController2 = (ModeController)e.NewValue;
			if (slide != null && modeController?.EventSource is Slide)
			{
				modeController.EventSource = null;
				modeController._getCurrentSlide = null;
			}
			if (slide != null && modeController2 != null && (modeController2.EventSource == null || modeController2.EventSource is Slide))
			{
				modeController2._getCurrentSlide = () => slide;
				modeController2.UpdateCurrentMode((Mode x) =>
				{
					x.CurrentSlide = slide;
				});
				modeController2.EventSource = modeController2.EventSource ?? slide;
			}
		}

		public ModeController(Func<Slide> getSlide)
		{
			_getCurrentSlide = getSlide;
		}

		private void ModeSlide_KeyDownOrUp(object sender, KeyEventArgs e)
		{
			if (e.IsRepeat || !_modePressedArgs.Any() || (!_isCurrentModeOn && _isCurrentModeSwitching))
			{
				return;
			}
			bool num = _currentModifierKeys.HasFlag(ModifierKeys.Shift) && (e.Key == Key.LeftShift || e.Key == Key.RightShift);
			bool flag = _currentModifierKeys.HasFlag(ModifierKeys.Control) && (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl);
			bool flag2 = _currentModifierKeys.HasFlag(ModifierKeys.Alt) && (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt);
			if (!(num | flag | flag2))
			{
				return;
			}
			foreach (KeyValuePair<int, ModeInputEventArgs> args in _modePressedArgs)
			{
				HandleFilter(args.Value, (Filter filter, ModeInputEventArgs a) =>
				{
					filter.RaiseDeviceMove(a);
				});
				DeliverModeEvent((Mode x) =>
				{
					x.RaiseDeviceMove(args.Value);
				});
			}
		}

		private void OnSourceInputDown(object sender, ModeInputEventArgs e)
		{
			if (!IsOn)
			{
				CleanInputData();
			}
			else if (_isCurrentModeOn)
			{
				if (!_pressingArgs.Any())
				{
					_currentDeliveringMode = CurrentMode;
					_currentHitTester = e.HitTestProvider;
				}
				else if (_currentHitTester != null)
				{
					e.HitTestProvider = _currentHitTester;
				}
				_pressingArgs[e.Id] = e;
				HandleFilter(e, (Filter filter, ModeInputEventArgs args) =>
				{
					filter.RaiseDeviceDown(args);
				});
				_modePressedArgs[e.Id] = e;
				DeliverModeEvent((Mode x) =>
				{
					x.RaiseDeviceDown(e);
				});
			}
		}

		private void OnSourceInputMove(object sender, ModeInputEventArgs e)
		{
			if (!IsOn)
			{
				CleanInputData();
			}
			else
			{
				if (!_isCurrentModeOn && _isCurrentModeSwitching)
				{
					return;
				}
				if (_currentHitTester != null)
				{
					e.HitTestProvider = _currentHitTester;
				}
				HandleFilter(e, (Filter filter, ModeInputEventArgs args) =>
				{
					filter.RaiseDeviceMove(args);
				});
				if (e != null)
				{
					_modePressedArgs[e.Id] = e;
					DeliverModeEvent((Mode x) =>
					{
						x.RaiseDeviceMove(e);
					});
				}
			}
		}

		private void OnSourceInputHover(object sender, ModeInputEventArgs e)
		{
			if (!IsOn)
			{
				CleanInputData();
			}
			else if (_isCurrentModeOn || !_isCurrentModeSwitching)
			{
				HandleFilter(e, (Filter filter, ModeInputEventArgs args) =>
				{
					filter.RaiseDeviceHover(args);
				});
				DeliverModeEvent((Mode x) =>
				{
					x.RaiseDeviceHover(e);
				});
			}
		}

		private void OnSourceInputUp(object sender, ModeInputEventArgs e)
		{
			if (!IsOn)
			{
				CleanInputData();
			}
			else
			{
				if (!_isCurrentModeOn)
				{
					return;
				}
				if (_currentHitTester != null)
				{
					e.HitTestProvider = _currentHitTester;
				}
				try
				{
					try
					{
						_modePressedArgs.Remove(e.Id);
						HandleFilter(e, (Filter filter, ModeInputEventArgs args) =>
						{
							filter.RaiseDeviceUp(args);
						});
						DeliverModeEvent((Mode x) =>
						{
							x.RaiseDeviceUp(e);
						});
						if (e.Slide == null)
						{
							Dispatcher.CurrentDispatcher.InvokeAsync(() =>
							{
								throw new InvalidOperationException("在 ModeController.OnSourceInputUp 中发现 Slide 为 null，这是不符合预期的。");
							});
						}
					}
					finally
					{
						if (IsWholeProgressCompleted())
						{
							OnWholeProgressCompleted(e);
						}
					}
				}
				catch (Exception ex)
				{
					Exception ex2 = ex;
					Exception ex3 = ex2;
					Application.Current?.Dispatcher.InvokeAsync(() =>
					{
						ExceptionDispatchInfo.Capture(ex3).Throw();
					});
				}
			}
		}

		private bool IsWholeProgressCompleted()
		{
			return !_modePressedArgs.Any();
		}

		private void OnWholeProgressCompleted(ModeInputEventArgs e)
		{
			HandleFilter(e, (Filter filter, ModeInputEventArgs args) =>
			{
				filter.RaiseWholeProgressCompleted();
			});
			CleanInputData();
		}

		private void CleanInputData()
		{
			_currentDeliveringMode = null;
			_currentHitTester = null;
			_pressingArgs.Clear();
			_modePressedArgs.Clear();
			if (_currentHitTestValue)
			{
				EnableElementsHitTestPrivate();
			}
			else
			{
				DisableElementsHitTestPrivate();
			}
		}

		private void HandleFilter<T>(T e, Action<Filter, T> process) where T : ModeInputEventArgs
		{
			Filter[] array = SortedFilters.Where((Filter filter) =>
			{
				if (DebuggingProperties.IsDebug && filter.CurrentSlide == null)
				{
					throw new ArgumentNullException("CurrentSlide", "保留现场，寻找黄腾霄或者林德熙。处理filter时，currentslide不能为空");
				}
				return filter.CurrentSlide != null && filter.CurrentSlide.IsLoaded && filter.GetBoard() != null && filter.IsEnabled;
			}).ToArray();
			foreach (Filter arg in array)
			{
				try
				{
					process(arg, e);
				}
				catch (Exception e2)
				{
					if (DebuggingProperties.IsDebug)
					{
						throw;
					}
					Log.Error(e2);
				}
			}
		}

		private void SwitchMode(Mode value)
		{
			if (!IsWholeProgressCompleted() && !(CurrentMode is OneTimeMode))
			{
				StackTrace stackTrace = new StackTrace();
				Tracer.Error(new InvalidOperationException("不能在整个输入完成之前切换当前模式，从" + (CurrentMode?.GetType().FullName ?? "null") + "切换到" + value.GetType().FullName + "模式 \r\n " + stackTrace.ToString()), "Mode", "模式切换", "Paint");
			}
			Mode currentMode = _currentMode;
			ModeSwitchingArgs modeSwitchingArgs = new ModeSwitchingArgs
			{
				SourceMode = currentMode,
				TargetMode = value
			};
			ModeSwitchingArgs modeSwitchingArgs2 = new ModeSwitchingArgs
			{
				SourceMode = currentMode,
				TargetMode = value
			};
			(currentMode as IModeSwitching)?.OnSwitchingFrom(modeSwitchingArgs);
			(value as IModeSwitching)?.OnSwitchingTo(modeSwitchingArgs2);
			bool suppressUnselect = modeSwitchingArgs.SuppressSourceModeUnselect || modeSwitchingArgs2.SuppressSourceModeUnselect;
			SwitchModeCore(currentMode, value, suppressUnselect);
			ModeSwitchedArgs args = new ModeSwitchedArgs
			{
				SourceMode = currentMode,
				TargetMode = value
			};
			ModeSwitchedArgs args2 = new ModeSwitchedArgs
			{
				SourceMode = currentMode,
				TargetMode = value
			};
			(currentMode as IModeSwitched)?.OnSwitchedFrom(args);
			(value as IModeSwitched)?.OnSwitchedTo(args2);
		}

		private void SwitchModeCore(Mode oldMode, Mode newMode, bool suppressUnselect)
		{
			_003C_003Ec__DisplayClass34_0 obj = new _003C_003Ec__DisplayClass34_0();
			obj._003C_003E4__this = this;
			obj.oldMode = oldMode;
			obj.newMode = newMode;
			obj.suppressUnselect = suppressUnselect;
			PaintAggregateExceptionBuilder.DoActionsWithException("切换 Mode 时发生了异常，从" + (obj.oldMode?.GetType().Name ?? "一个空的模式") + "切换到" + (obj.newMode?.GetType().Name ?? "一个空的模式") + "。异常的详细信息如下：", obj._003CSwitchModeCore_003Eg__RaiseModeChangingEvent_007C0, obj._003CSwitchModeCore_003Eg__SwitchActiveModeAndCurrentMode_007C1, obj._003CSwitchModeCore_003Eg__SwitchOnCurrentModeInner_007C2, obj._003CSwitchModeCore_003Eg__RaiseModeChangedEvent_007C3);
		}

		public void SwitchOn()
		{
			if (CurrentMode is NoneMode)
			{
				return;
			}
			_isCurrentModeSwitching = true;
			try
			{
				SwitchOnCurrentMode();
			}
			finally
			{
				_isCurrentModeOn = true;
				_isCurrentModeSwitching = false;
				IsOn = true;
			}
		}

		public void SwitchOff()
		{
			Mode currentMode = CurrentMode;
			if (currentMode is NoneMode || currentMode.CurrentSlide == null)
			{
				return;
			}
			_isCurrentModeSwitching = true;
			Mouse.Capture(null);
			Stylus.Capture(null);
			IsOn = false;
			_isCurrentModeOn = false;
			try
			{
				try
				{
					((IActiveModeController)this).InvalidateActiveMode();
					((IModeHitTestController)this).Restore();
				}
				finally
				{
					SwitchOffCurrentMode(currentMode, true);
				}
			}
			finally
			{
				ClearModeInfo(currentMode);
				currentMode.CurrentSlide = null;
				UpdateCurrentMode((Mode x) =>
				{
					x.CurrentSlide = null;
				});
				Slide slide = _getCurrentSlide();
				SortedFilters.ForEach((Filter x) =>
				{
					x.CurrentMode = null;
				});
				if (slide != null)
				{
					slide.ElementsAdded -= OnElementAdded;
					slide.ElementsRemoved -= OnElementRemoved;
				}
				_isCurrentModeSwitching = false;
			}
		}

		internal void SwitchOnFilters(Slide slide)
		{
			SortedFilters.ForEach((Filter x) =>
			{
				x.CurrentSlide = slide;
				x.SwitchOn();
			});
		}

		internal void SwitchOffFilters()
		{
			SortedFilters.ForEach((Filter x) =>
			{
				x.SwitchOff();
				x.CurrentSlide = null;
			});
		}

		private void SetModeInfo([NotNull] Mode currentMode, IModeController controller)
		{
			ICanControlModes canControlModes = currentMode as ICanControlModes;
			if (canControlModes != null)
			{
				canControlModes.ModeController = controller;
			}
			currentMode.HitTestController = this;
			IModeKeyboardSensitive modeKeyboardSensitive = currentMode as IModeKeyboardSensitive;
			if (modeKeyboardSensitive != null)
			{
				ModifierKeys modifiers = modeKeyboardSensitive.Modifiers;
				if (modifiers != ModifierKeys.None)
				{
					_currentModifierKeys = modifiers;
					currentMode.CurrentSlide.PreviewKeyDown += ModeSlide_KeyDownOrUp;
					currentMode.CurrentSlide.PreviewKeyUp += ModeSlide_KeyDownOrUp;
					_modePressedArgs.Clear();
				}
			}
		}

		private void ClearModeInfo([NotNull] Mode currentMode, Slide slide = null)
		{
			if (currentMode == null)
			{
				try
				{
					throw new ArgumentNullException("currentMode");
				}
				catch (Exception ex)
				{
					if (DebuggingProperties.IsDebug)
					{
						throw;
					}
					Log.Error(ex);
					Tracer.Error(ex);
					return;
				}
			}
			ICanControlModes canControlModes = currentMode as ICanControlModes;
			if (canControlModes != null)
			{
				canControlModes.ModeController = null;
			}
			currentMode.HitTestController = null;
			_currentModifierKeys = ModifierKeys.None;
			slide = slide ?? currentMode.CurrentSlide;
			if (slide == null)
			{
				try
				{
					throw new ArgumentException("在清理 Mode 的时候，发现当前 Mode 的 CurrentSlide 为空" + currentMode.GetType().Name);
				}
				catch (Exception e)
				{
					if (DebuggingProperties.IsDebug)
					{
						throw;
					}
					Log.Error(e);
					return;
				}
			}
			slide.PreviewKeyDown -= ModeSlide_KeyDownOrUp;
			slide.PreviewKeyUp -= ModeSlide_KeyDownOrUp;
		}

		private void UpdateCurrentMode(Action<Mode> update)
		{
			update(CurrentMode);
			if (_activeMode != null)
			{
				update(_activeMode);
			}
		}

		private void DeliverModeEvent(Action<Mode> deliver)
		{
			Mode mode = _activeMode ?? _currentDeliveringMode ?? CurrentMode;
			if (mode.IsOn)
			{
				deliver(mode);
			}
		}

		public void AddDataSource(IModeInputSource inputSource)
		{
			if (_dataSources.Contains(inputSource))
			{
				throw new InvalidOperationException("不能重复添加 Mode 的操作数据源。");
			}
			_dataSources.Add(inputSource);
			AttachSource(inputSource);
		}

		[PublicAPI]
		public void RemoveDataSource(IModeInputSource inputSource)
		{
			if (!_dataSources.Contains(inputSource))
			{
				throw new InvalidOperationException("不能移除一个不存在的 Mode 操作数据源。");
			}
			_dataSources.Remove(inputSource);
			DetachSource(inputSource);
		}

		public ReadOnlyCollection<IModeInputSource> GetCurrentDataSources()
		{
			return new ReadOnlyCollection<IModeInputSource>(_dataSources);
		}

		private void AttachSource(IModeInputSource source)
		{
			source.Attach(EventSource, () => InternalContainer);
			source.InputDown += OnSourceInputDown;
			source.InputMove += OnSourceInputMove;
			source.InputHover += OnSourceInputHover;
			source.InputUp += OnSourceInputUp;
		}

		private void DetachSource(IModeInputSource source)
		{
			source.InputDown -= OnSourceInputDown;
			source.InputMove -= OnSourceInputMove;
			source.InputHover -= OnSourceInputHover;
			source.InputUp -= OnSourceInputUp;
			source.Detach();
		}

		public void AddFilter(Filter filter)
		{
			if (_filters.Contains(filter))
			{
				throw new InvalidOperationException("不能重复添加 Mode 的操作过滤器。");
			}
			filter.CurrentSlide = _getCurrentSlide();
			filter.CurrentMode = CurrentMode;
			filter.ActiveModeController = this;
			int num = 0;
			foreach (Filter filter2 in _filters)
			{
				if (filter2.Priority > filter.Priority)
				{
					num++;
				}
			}
			filter.Attach();
			_filters.Insert(num, filter);
			filter.SwitchOnWhenLoaded();
		}

		public void RemoveFilter(Filter filter)
		{
			if (!_filters.Contains(filter))
			{
				throw new InvalidOperationException("不能移除一个不存在的 Mode 操作过滤器。");
			}
			filter.SwitchOff();
			filter.Detach();
			_filters.Remove(filter);
			filter.CurrentSlide = _getCurrentSlide();
			filter.CurrentMode = null;
			filter.ActiveModeController = null;
		}

		public IEnumerable<Filter> GetFilters()
		{
			return SortedFilters;
		}

		void IActiveModeController.SetActiveMode(Mode mode)
		{
			if (mode == null)
			{
				throw new ArgumentNullException("mode");
			}
			if (mode is OneTimeMode)
			{
				throw new ArgumentException("请不要把 OneTimeMode 设置在 ActiveMode ，直接设置需要的Mode就可以在结束的时候回到当前的模式");
			}
			if (_activeMode == null)
			{
				try
				{
					Mode currentMode = CurrentMode;
					CompletePressedUp(currentMode);
				}
				finally
				{
					SwitchOffCurrentMode(CurrentMode, false);
				}
			}
			else
			{
				SwitchOffActiveMode();
			}
			_activeMode = mode;
			mode.CurrentSlide = _getCurrentSlide();
			SetModeInfo(mode, this);
			mode.SwitchOn();
			foreach (KeyValuePair<int, ModeInputEventArgs> modePressedArg in _modePressedArgs)
			{
				mode.RaiseDeviceDown(modePressedArg.Value);
			}
		}

		private void CompletePressedUp(Mode mode)
		{
			foreach (ModeInputEventArgs item in _modePressedArgs.Select((KeyValuePair<int, ModeInputEventArgs> pair) => pair.Value.Clone()))
			{
				item.VirtualType = VirtualDeviceType.LostCapture;
				mode.RaiseDeviceUp(item);
			}
		}

		private void SwitchOffActiveMode()
		{
			if (_activeMode == null)
			{
				return;
			}
			Mode activeMode = _activeMode;
			_activeMode = null;
			try
			{
				CompletePressedUp(activeMode);
			}
			finally
			{
				activeMode.SwitchOff();
				ClearModeInfo(activeMode);
				activeMode.CurrentSlide = null;
			}
		}

		private void SwitchOnCurrentMode()
		{
			Mode currentMode = CurrentMode;
			if (_getCurrentSlide != null)
			{
				Slide slide = _getCurrentSlide();
				UpdateCurrentMode((Mode x) =>
				{
					x.CurrentSlide = slide;
				});
				SortedFilters.ForEach((Filter x) =>
				{
					x.CurrentMode = currentMode;
				});
				if (slide != null)
				{
					slide.ElementsAdded -= OnElementAdded;
					slide.ElementsRemoved -= OnElementRemoved;
					slide.ElementsAdded += OnElementAdded;
					slide.ElementsRemoved += OnElementRemoved;
				}
			}
			SetModeInfo(currentMode, this);
			currentMode.SwitchOn();
		}

		private void SwitchOffCurrentMode(Mode currentMode, bool isFromSwitchOff)
		{
			currentMode.SwitchOff();
			OneTimeMode oneTimeMode = currentMode as OneTimeMode;
			if (oneTimeMode != null && (isFromSwitchOff || oneTimeMode.IsTransientMode))
			{
				_currentMode = oneTimeMode.LastMode;
			}
		}

		void IActiveModeController.InvalidateActiveMode()
		{
			if (_activeMode == null)
			{
				return;
			}
			try
			{
				SwitchOffActiveMode();
			}
			finally
			{
				SwitchOnCurrentMode();
				foreach (KeyValuePair<int, ModeInputEventArgs> modePressedArg in _modePressedArgs)
				{
					CurrentMode.RaiseDeviceDown(modePressedArg.Value);
				}
			}
		}

		void IModeHitTestController.Restore()
		{
			try
			{
				_currentHitTestValue = true;
				if (!_modePressedArgs.Any())
				{
					EnableElementsHitTestPrivate();
				}
			}
			finally
			{
				SetIsSlideHitTestVisible(CurrentMode.CurrentSlide, true);
			}
		}

		void IModeHitTestController.Disable()
		{
			try
			{
				_currentHitTestValue = false;
				if (!_pressingArgs.Any())
				{
					DisableElementsHitTestPrivate();
				}
			}
			finally
			{
				SetIsSlideHitTestVisible(CurrentMode.CurrentSlide, false);
			}
		}

		private void EnableElementsHitTestPrivate()
		{
			foreach (Element element in _getCurrentSlide().Elements)
			{
				element.ClearValue(UIElement.IsHitTestVisibleProperty);
			}
		}

		private void DisableElementsHitTestPrivate()
		{
			foreach (Element element in _getCurrentSlide().Elements)
			{
				element.IsHitTestVisible = false;
			}
		}

		private void OnElementRemoved(object sender, RoutedCollectionChangedEventArgs<Element> e)
		{
			foreach (Element item in e.Items)
			{
				item.ClearValue(UIElement.IsHitTestVisibleProperty);
			}
		}

		private void OnElementAdded(object sender, RoutedCollectionChangedEventArgs<Element> e)
		{
			if (_currentHitTestValue)
			{
				foreach (Element item in e.Items)
				{
					item.ClearValue(UIElement.IsHitTestVisibleProperty);
				}
				return;
			}
			foreach (Element item2 in e.Items)
			{
				item2.IsHitTestVisible = false;
			}
		}

		private static void SetIsSlideHitTestVisible([NotNull] Slide slide, bool value)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			if (value)
			{
				slide.ClearValue(ElementHitTester.IsSlideHitTestVisibleProperty);
			}
			else
			{
				slide.SetValue(ElementHitTester.IsSlideHitTestVisibleProperty, false);
			}
		}
	}
}
