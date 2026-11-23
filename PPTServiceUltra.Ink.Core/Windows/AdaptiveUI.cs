using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace PPTServiceUltra.Ink.Core.Windows
{
	[ContentProperty("Triggers")]
	public class AdaptiveUI : DependencyObject
	{
		public static readonly DependencyProperty AdaptiveProperty = DependencyProperty.RegisterAttached("Adaptive", typeof(AdaptiveUI), typeof(AdaptiveUI), new PropertyMetadata(null, (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
		{
			if (!DesignerProperties.GetIsInDesignMode(o))
			{
				(args.OldValue as AdaptiveUI)?.Detach();
				(args.NewValue as AdaptiveUI)?.Attach(o);
			}
		}));

		public static readonly DependencyProperty AdaptAcrossOwnerProperty = DependencyProperty.RegisterAttached("AdaptAcrossOwner", typeof(bool), typeof(AdaptiveUI), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

		public static readonly DependencyProperty TriggersProperty = DependencyProperty.Register("Triggers", typeof(AdaptiveTriggerCollection), typeof(AdaptiveUI), new PropertyMetadata(object.Equals(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue, true) ? new AdaptiveTriggerCollection() : null));

		public static readonly DependencyProperty DesignWindowWidthProperty = DependencyProperty.RegisterAttached("DesignWindowWidth", typeof(double), typeof(AdaptiveUI), new FrameworkPropertyMetadata(1920.0, FrameworkPropertyMetadataOptions.Inherits, InvalidateDesignSize), (object value) => (double)value >= 0.0);

		public static readonly DependencyProperty DesignWindowHeightProperty = DependencyProperty.RegisterAttached("DesignWindowHeight", typeof(double), typeof(AdaptiveUI), new FrameworkPropertyMetadata(1080.0, FrameworkPropertyMetadataOptions.Inherits, InvalidateDesignSize), (object value) => (double)value >= 0.0);

		private WeakReference<FrameworkElement> _visualReference;

		private AdaptiveCondition _currentWidthCondition;

		private AdaptiveCondition _currentHeightCondition;

		private AdaptiveCondition _currentSizeCondition;

		
		private readonly List<ValueTuple<DependencyObject, DependencyProperty, object>> _backupedWidthValues = new List<ValueTuple<DependencyObject, DependencyProperty, object>>();

		
		private readonly List<ValueTuple<DependencyObject, DependencyProperty, object>> _backupedHeightValues = new List<ValueTuple<DependencyObject, DependencyProperty, object>>();

		
		private readonly List<ValueTuple<DependencyObject, DependencyProperty, object>> _backupedSizeValues = new List<ValueTuple<DependencyObject, DependencyProperty, object>>();

		[DependsOn("DesignWindowWidth")]
		[DependsOn("DesignWindowHeight")]
		public AdaptiveTriggerCollection Triggers
		{
			get
			{
				return (AdaptiveTriggerCollection)GetValue(TriggersProperty);
			}
			set
			{
				SetValue(TriggersProperty, value);
			}
		}

		public double DesignWindowWidth
		{
			get
			{
				return (double)GetValue(DesignWindowWidthProperty);
			}
			set
			{
				SetValue(DesignWindowWidthProperty, value);
			}
		}

		public double DesignWindowHeight
		{
			get
			{
				return (double)GetValue(DesignWindowHeightProperty);
			}
			set
			{
				SetValue(DesignWindowHeightProperty, value);
			}
		}

		private DependencyObject AssociatedObject { get; set; }

		public static AdaptiveUI GetAdaptive(DependencyObject element)
		{
			return (AdaptiveUI)element.GetValue(AdaptiveProperty);
		}

		public static void SetAdaptive(DependencyObject element, AdaptiveUI value)
		{
			element.SetValue(AdaptiveProperty, value);
		}

		public static bool GetAdaptAcrossOwner(DependencyObject element)
		{
			return (bool)element.GetValue(AdaptAcrossOwnerProperty);
		}

		public static void SetAdaptAcrossOwner(DependencyObject element, bool value)
		{
			element.SetValue(AdaptAcrossOwnerProperty, value);
		}

		public AdaptiveUI()
		{
			Triggers = new AdaptiveTriggerCollection();
		}

		internal AdaptiveUI(AdaptiveTriggerCollection triggers)
		{
			Triggers = triggers;
		}

		private static void InvalidateDesignSize(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (DesignerProperties.GetIsInDesignMode(d))
			{
				AdaptiveUI adaptiveUI = d as AdaptiveUI;
				adaptiveUI?.UpdateSize(adaptiveUI.DesignWindowWidth, adaptiveUI.DesignWindowHeight);
			}
		}

		private void Attach(DependencyObject element)
		{
			AssociatedObject = element;
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				UpdateSize(DesignWindowWidth, DesignWindowHeight);
				return;
			}
			FrameworkElement frameworkElement = FindHwndRootElement(element);
			FrameworkElement frameworkElement2 = element as FrameworkElement;
			if (frameworkElement == null || (frameworkElement2 != null && !frameworkElement2.IsLoaded))
			{
				if (frameworkElement2 != null)
				{
					frameworkElement2.Loaded += OnLoaded;
				}
			}
			else
			{
				AddHandler(frameworkElement);
			}
		}

		private void Detach()
		{
			FrameworkElement frameworkElement = AssociatedObject as FrameworkElement;
			if (frameworkElement != null)
			{
				frameworkElement.Loaded -= OnLoaded;
			}
			RemoveHandler();
			AssociatedObject = null;
		}

		internal FrameworkElement FindHwndRootElement(DependencyObject element)
		{
			Window window = Window.GetWindow(element);
			if (window != null)
			{
				Window window2 = window;
				DependencyObject element2 = element;
				while (window2 != null && GetAdaptAcrossOwner(element2))
				{
					window2 = window2.Owner;
					if (window2 != null)
					{
						window = window2;
						element2 = window2;
					}
				}
				return window;
			}
			Visual visual = element as Visual;
			if (visual != null)
			{
				HwndSource hwndSource = PresentationSource.FromVisual(visual) as HwndSource;
				if (hwndSource != null)
				{
					FrameworkElement frameworkElement = hwndSource.RootVisual as FrameworkElement;
					if (frameworkElement != null)
					{
						return frameworkElement;
					}
				}
			}
			return null;
		}

		private void AddHandler(FrameworkElement root)
		{
			_visualReference = new WeakReference<FrameworkElement>(root);
			WeakEventManager<FrameworkElement, SizeChangedEventArgs>.AddHandler(root, FrameworkElement.SizeChangedEvent.Name, OnSizeChanged);
			if (!object.Equals(root.ActualWidth, 0.0) && !object.Equals(root.ActualHeight, 0.0))
			{
				UpdateSize(root.ActualWidth, root.ActualHeight);
			}
		}

		private void RemoveHandler()
		{
			FrameworkElement target;
			if (_visualReference != null && _visualReference.TryGetTarget(out target))
			{
				WeakEventManager<FrameworkElement, SizeChangedEventArgs>.RemoveHandler(target, FrameworkElement.SizeChangedEvent.Name, OnSizeChanged);
			}
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			FrameworkElement frameworkElement = (FrameworkElement)sender;
			frameworkElement.Loaded -= OnLoaded;
			FrameworkElement frameworkElement2 = FindHwndRootElement(frameworkElement);
			if (frameworkElement2 == null)
			{
				throw new NotSupportedException("Unexpected adaptive situation that the PresentationSource returns null even when target is loaded. Target is " + sender.GetType().FullName + " with name " + (frameworkElement.Name ?? "null") + ".");
			}
			AddHandler(frameworkElement2);
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateSize(e.NewSize.Width, e.NewSize.Height);
		}

		private void UpdateSize(double width, double height)
		{
			AdaptiveTriggerCollection triggers = Triggers;
			AdaptiveCondition currentWidthCondition = _currentWidthCondition;
			AdaptiveCondition currentHeightCondition = _currentHeightCondition;
			AdaptiveCondition currentSizeCondition = _currentSizeCondition;
			AdaptiveCondition value = triggers.WidthSortedList.LastOrDefault((KeyValuePair<double, AdaptiveCondition> pair) => pair.Key <= width).Value;
			AdaptiveCondition value2 = triggers.HeightSortedList.LastOrDefault((KeyValuePair<double, AdaptiveCondition> pair) => pair.Key <= height).Value;
			AdaptiveCondition item = triggers.SizeSortedList.LastOrDefault(( KeyValuePair<double, ValueTuple<Size, AdaptiveCondition>> pair) => pair.Value.Item1.Width <= width && pair.Value.Item1.Height <= height).Value.Item2;
			if (object.Equals(value, currentWidthCondition) && object.Equals(value2, currentHeightCondition) && object.Equals(item, currentSizeCondition))
			{
				return;
			}
			if (!object.Equals(value, currentWidthCondition))
			{
				if (currentWidthCondition != null)
				{
					RestoreTrigger(currentWidthCondition, _backupedWidthValues);
				}
				_currentWidthCondition = value;
				if (value != null)
				{
					BackupSetters(_backupedWidthValues, value.Setters);
					ApplyTrigger(value);
				}
			}
			if (!object.Equals(value2, currentHeightCondition))
			{
				if (currentHeightCondition != null && !object.Equals(currentWidthCondition, currentHeightCondition))
				{
					RestoreTrigger(currentHeightCondition, _backupedHeightValues);
				}
				_currentHeightCondition = value2;
				if (value2 != null && !object.Equals(value, value2))
				{
					BackupSetters(_backupedHeightValues, value2.Setters);
					ApplyTrigger(value2);
				}
			}
			if (!object.Equals(item, currentSizeCondition))
			{
				if (currentSizeCondition != null && !object.Equals(currentSizeCondition, currentWidthCondition) && !object.Equals(currentSizeCondition, currentHeightCondition))
				{
					RestoreTrigger(currentSizeCondition, _backupedSizeValues);
				}
				_currentSizeCondition = item;
				if (item != null && !object.Equals(item, value) && !object.Equals(item, value2))
				{
					BackupSetters(_backupedSizeValues, item.Setters);
					ApplyTrigger(item);
				}
			}
		}

		private void BackupSetters( List<ValueTuple<DependencyObject, DependencyProperty, object>> list, SetterBaseCollection setters)
		{
			list.Clear();
			list.AddRange(from setter in setters.OfType<Setter>()
				let target = FindTarget(setter)
				let property = setter.Property
				where target != null
				select new ValueTuple<DependencyObject, DependencyProperty, object>(target, property, target.ReadLocalValue(property)));
		}

		private void RestoreTrigger(AdaptiveCondition condition,  List<ValueTuple<DependencyObject, DependencyProperty, object>> list)
		{
			foreach (var item3 in list)
			{
				DependencyObject item = item3.Item1;
				DependencyProperty item2 = item3.Item2;
				item.InvalidateProperty(item2);
				BindingOperations.GetBindingExpression(item, item2)?.UpdateTarget();
			}
			InvokeActions(condition.ExitActions);
		}

		private void ApplyTrigger(AdaptiveCondition condition)
		{
			foreach (Setter item in condition.Setters.OfType<Setter>())
			{
				DependencyObject dependencyObject = FindTarget(item);
				object value = GetValue(item);
				dependencyObject?.SetCurrentValue(item.Property, value);
			}
			InvokeActions(condition.EnterActions);
		}

		private void InvokeActions(TriggerActionCollection actions)
		{
			foreach (TriggerAction action in actions)
			{
				BeginStoryboard beginStoryboard = action as BeginStoryboard;
				if (beginStoryboard != null)
				{
					if (beginStoryboard.Storyboard != null)
					{
						beginStoryboard.Storyboard.Begin();
						continue;
					}
					throw new ArgumentException("传入的actions存在Storyboard为空");
				}
				MethodInfo method = action.GetType().GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[1] { typeof(FrameworkElement) }, null);
				if (method != null)
				{
					method.Invoke(action, new object[1] { AssociatedObject });
				}
			}
		}

		private object GetValue(Setter setter)
		{
			return (setter as AdaptiveSetter)?.GetValue() ?? setter.Value;
		}

		private DependencyObject FindTarget(Setter setter)
		{
			DependencyObject dependencyObject = (setter as AdaptiveSetter)?.Target;
			if (dependencyObject == null)
			{
				if (setter.TargetName != null)
				{
					return FindName(setter.TargetName);
				}
				dependencyObject = AssociatedObject;
			}
			return dependencyObject;
		}

		private DependencyObject FindName(string name)
		{
			return (AssociatedObject as FrameworkElement)?.FindName(name) as DependencyObject;
		}
	}
}
