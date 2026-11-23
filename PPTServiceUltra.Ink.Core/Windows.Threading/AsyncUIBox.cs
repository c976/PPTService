using System;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Ink.Core.Windows.Controls;

namespace PPTServiceUltra.Ink.Core.Windows.Threading
{
	[ContentProperty("Child")]
	public class AsyncUIBox : FrameworkElement
	{
		private static readonly ConcurrentDictionary<Dispatcher, DispatcherAsyncOperation<Dispatcher>> RelatedAsyncDispatchers = new ConcurrentDictionary<Dispatcher, DispatcherAsyncOperation<Dispatcher>>();

		[NotNull]
		private readonly HostVisual _hostVisual;

		[NotNull]
		private readonly ContentPresenter _contentPresenter;

		[CanBeNull]
		private readonly Func<UIElement> _childCreator;

		[CanBeNull]
		private UIElement _child;

		[CanBeNull]
		private VisualTargetPresentationSource _targetSource;

		[CanBeNull]
		private LoadingView _loadingView;

		private bool _isChildReadyToLoad;

		[CanBeNull]
		public UIElement Child
		{
			get
			{
				return _child;
			}
			set
			{
				if (!object.Equals(_child, value))
				{
					if (value != null)
					{
						RemoveLogicalChild(value);
					}
					_child = value;
					if (_isChildReadyToLoad)
					{
						ActivateChild();
					}
				}
			}
		}

		protected override int VisualChildrenCount
		{
			get
			{
				if (_loadingView == null)
				{
					return 0;
				}
				return 2;
			}
		}

		protected override IEnumerator LogicalChildren
		{
			get
			{
				if (_isChildReadyToLoad)
				{
					yield return _contentPresenter;
				}
			}
		}

		public AsyncUIBox()
		{
			_hostVisual = new HostVisual();
			_contentPresenter = new ContentPresenter();
			Loaded += OnLoaded;
		}

		public AsyncUIBox(Func<UIElement> childCreator)
			: this()
		{
			_childCreator = childCreator;
		}

		[NotNull]
		private DispatcherAsyncOperation<Dispatcher> GetAsyncDispatcherAsync()
		{
			return RelatedAsyncDispatchers.GetOrAdd(Dispatcher, NewDispatcherValueFactory);
		}

		[NotNull]
		private static DispatcherAsyncOperation<Dispatcher> NewDispatcherValueFactory(Dispatcher _)
		{
			return UIDispatcher.RunNewAsync("AsyncUIBox");
		}

		[NotNull]
		private LoadingView CreateLoadingView()
		{
			Style style = (Style)FindResource(typeof(LoadingView));
			return new LoadingView
			{
				Style = style
			};
		}

		private async void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
			{
				_loadingView = await (await GetAsyncDispatcherAsync()).InvokeAsync(() =>
				{
					LoadingView loadingView = CreateLoadingView();
					loadingView.IsLoading = true;
					_targetSource = new VisualTargetPresentationSource(_hostVisual)
					{
						RootVisual = loadingView
					};
					return loadingView;
				});
				AddVisualChild(_contentPresenter);
				AddVisualChild(_hostVisual);
				await LayoutAsync();
				await Dispatcher.Yield(DispatcherPriority.Background);
				_isChildReadyToLoad = true;
				if (_childCreator != null)
				{
					Child = _childCreator();
				}
				ActivateChild();
				await Dispatcher.Yield(DispatcherPriority.Background);
				await _loadingView.Dispatcher.InvokeAsync(() => _loadingView.IsLoading = false);
			}
		}

		private void ActivateChild()
		{
			UIElement child = Child;
			if (child != null)
			{
				_contentPresenter.Content = child;
				AddLogicalChild(child);
				InvalidateMeasure();
			}
		}

		private async Task LayoutAsync()
		{
			await (await GetAsyncDispatcherAsync()).InvokeAsync(() =>
			{
				if (_loadingView != null)
				{
					_loadingView.Measure(RenderSize);
					_loadingView.Arrange(new Rect(RenderSize));
				}
			});
		}

		protected override Visual GetVisualChild(int index)
		{
			switch (index)
			{
			case 0:
				return _contentPresenter;
			case 1:
				return _hostVisual;
			default:
				return null;
			}
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			if (_isChildReadyToLoad)
			{
				_contentPresenter.Measure(availableSize);
				return _contentPresenter.DesiredSize;
			}
			return base.MeasureOverride(availableSize);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			if (_isChildReadyToLoad)
			{
				_contentPresenter.Arrange(new Rect(finalSize));
				Size renderSize = _contentPresenter.RenderSize;
				LayoutAsync();
				return renderSize;
			}
			Size result = base.ArrangeOverride(finalSize);
			LayoutAsync();
			return result;
		}
	}
}
