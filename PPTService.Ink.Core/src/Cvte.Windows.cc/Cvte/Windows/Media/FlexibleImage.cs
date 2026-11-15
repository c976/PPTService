using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using PPTService.Ink.Core.Diagnostics.Annotations;
using PPTService.Ink.Core.Linq;
using PPTService.Ink.Core.Windows.Mathematics;
using PPTService.Ink.Core.Windows.Mathematics.Geometries;

namespace PPTService.Ink.Core.Windows.Media
{
	public class FlexibleImage : FrameworkElement
	{
		private sealed class FlexibleImageReloadingContext : IDestroyableImages, IDisposable
		{
			private Action _redestroyAction;

			internal FlexibleImageReloadingContext(Action redestroyAction)
			{
				_redestroyAction = redestroyAction;
			}

			void IDisposable.Dispose()
			{
				DestroyAgain();
			}

			public void DestroyAgain()
			{
				_redestroyAction?.Invoke();
				_redestroyAction = null;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass15_0
		{
			public HashSet<FlexibleImage> watchingImages;

			public Action<FlexibleImage> _003C_003E9__5;

			internal void _003CWatchRenderBoundsAsync_003Eb__5(FlexibleImage x)
			{
				watchingImages.Remove(x);
			}

			internal IEnumerable<FlexibleImage> _003CWatchRenderBoundsAsync_003Eg__WatchIfNeeded_007C1()
			{
				foreach (FlexibleImage x in watchingImages)
				{
					if (!x.IsLoaded)
					{
						yield return x;
					}
					_003CWatchRenderBoundsAsync_003Eg__WatchBounds_007C15_2(x);
				}
			}
		}

		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass15_1
		{
			public FlexibleImage x;
		}

		private Point _lastWatchedLocation;

		private Size _lastWatchedSize;

		private bool _isLoading;

		private bool _isLoaded;

		private int _imageGenerationCount;

		private int _imageGenerationFailedCount;

		[ThreadStatic]
		private static readonly HashSet<FlexibleImage> WatchingImages;

		private static bool _isWatchingBoundsChanging;

		private static readonly TimeSpan MinReloadDelay;

		private static readonly BitmapCachePool Pool;

		private readonly DispatcherTimer _timer;

		private FileInfo _file;

		
		private ValueTuple<WeakReference<BitmapSource>, Size, Size> _cachedBitmapInfo;

		private BitmapSource _targetImage;

		private Size _naturalSize;

		private Size _lastImageSize;

		private Size _lastDesiredImageSize;

		private bool _isUpdating;

		private BitmapRenderingMode _renderingModeDuringLoad;

		private bool _isRendering;

		private bool _isDestroying;

		private readonly Lazy<MethodInfo> _renderOpenLazy = new Lazy<MethodInfo>(() => typeof(UIElement).GetMethod("RenderOpen", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod), false);

		private bool _isDecodingScaleEnabled = true;

		public static readonly DependencyProperty FileNameProperty;

		public static readonly DependencyProperty ReloadDelayProperty;

		public static readonly DependencyProperty RenderingModeProperty;

		public static readonly DependencyProperty OutOfTreeSizeProperty;

		private static readonly DependencyProperty AutoRenderImageWhenResizingProperty;

		public static readonly DependencyProperty ViewboxProperty;

		public static readonly DependencyProperty BitmapScalingModeSwitchProperty;

		public static readonly RoutedEvent ReloadingEvent;

		public static readonly RoutedEvent ReloadedEvent;

		public static readonly RoutedEvent ManuallyRenderEvent;

		public string FileName
		{
			get
			{
				return (string)GetValue(FileNameProperty);
			}
			set
			{
				SetValue(FileNameProperty, value);
			}
		}

		public TimeSpan ReloadDelay
		{
			get
			{
				return (TimeSpan)GetValue(ReloadDelayProperty);
			}
			set
			{
				SetValue(ReloadDelayProperty, value);
			}
		}

		[DefaultValue(BitmapRenderingMode.WhenArranged)]
		public BitmapRenderingMode RenderingMode
		{
			get
			{
				return (BitmapRenderingMode)GetValue(RenderingModeProperty);
			}
			set
			{
				SetValue(RenderingModeProperty, value);
			}
		}

		public Size OutOfTreeSize
		{
			get
			{
				return (Size)GetValue(OutOfTreeSizeProperty);
			}
			set
			{
				SetValue(OutOfTreeSizeProperty, value);
			}
		}

		public Rect Viewbox
		{
			get
			{
				return (Rect)GetValue(ViewboxProperty);
			}
			set
			{
				SetValue(ViewboxProperty, value);
			}
		}

		public bool BitmapScalingModeSwitch
		{
			get
			{
				return (bool)GetValue(BitmapScalingModeSwitchProperty);
			}
			set
			{
				SetValue(BitmapScalingModeSwitchProperty, value);
			}
		}

		public bool AutoRenderImageWhenResizing
		{
			get
			{
				return (bool)GetValue(AutoRenderImageWhenResizingProperty);
			}
			set
			{
				SetValue(AutoRenderImageWhenResizingProperty, value);
			}
		}

		public Size NaturalSize
		{
			get
			{
				if (!IsFileExists)
				{
					return default(Size);
				}
				if (_naturalSize == default(Size))
				{
					_naturalSize = GetImageSize(_file.FullName);
				}
				return _naturalSize;
			}
		}

		public BitmapSource CurrentImage => (_targetImage as CroppedBitmap)?.Source ?? _targetImage;

		[NotNull]
		[ItemNotNull]
		public IList<UIElement> SnapTargets { get; } = new List<UIElement>();

		private bool IsFileExists => _file?.Exists ?? false;

		private Size ScalingFactor => this.GetTransformInfoToDevice().Item1;

		private Size ScaledSize => RenderSize.ToScaledSize(ScalingFactor);

		private Size DesiredImageSize => ScaledSize.ToScaledSize(1.0 / Viewbox.Width, 1.0 / Viewbox.Height);

		private bool IsSmallEnoughForNaturalRender => NaturalSize.Width * NaturalSize.Height < 90000.0;

		public bool IsDecodingScaleEnabled
		{
			get
			{
				return _isDecodingScaleEnabled;
			}
			set
			{
				_isDecodingScaleEnabled = value;
				_isRendering = true;
				Pool.Clear();
				_cachedBitmapInfo = default(ValueTuple<WeakReference<BitmapSource>, Size, Size>);
				InvalidateRender();
			}
		}

		private event RoutedEventHandler Reloading
		{
			add
			{
				AddHandler(ReloadingEvent, value);
			}
			remove
			{
				RemoveHandler(ReloadingEvent, value);
			}
		}

		private event RoutedEventHandler Reloaded
		{
			add
			{
				AddHandler(ReloadedEvent, value);
			}
			remove
			{
				RemoveHandler(ReloadedEvent, value);
			}
		}

		private event RoutedEventHandler ManuallyRender
		{
			add
			{
				AddHandler(ManuallyRenderEvent, value);
			}
			remove
			{
				RemoveHandler(ManuallyRenderEvent, value);
			}
		}

		static FlexibleImage()
		{
			WatchingImages = new HashSet<FlexibleImage>();
			MinReloadDelay = TimeSpan.FromSeconds(0.25);
			Pool = new BitmapCachePool();
			FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(FlexibleImage), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
			{
				FlexibleImage flexibleImage = (FlexibleImage)d;
				string text = (string)e.NewValue;
				if (!string.IsNullOrEmpty(text))
				{
					flexibleImage._file = new FileInfo(text);
					try
					{
						if (!flexibleImage._file.Exists)
						{
							throw new FileNotFoundException("Specified file not found for FlexibleImage.", flexibleImage._file.FullName);
						}
						Pool.GetImageSize(flexibleImage._file.FullName);
						if (flexibleImage.IsLoaded)
						{
							WatchRenderBoundsAsync(flexibleImage);
						}
					}
					catch
					{
						flexibleImage.ClearValue(FileNameProperty);
						throw;
					}
					finally
					{
						flexibleImage.ClearRenderState();
					}
					if (flexibleImage.IsLoaded)
					{
						flexibleImage._isRendering = true;
					}
				}
			}));
			ReloadDelayProperty = DependencyProperty.Register("ReloadDelay", typeof(TimeSpan), typeof(FlexibleImage), new PropertyMetadata(MinReloadDelay, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
			{
				if ((TimeSpan)e.NewValue < MinReloadDelay)
				{
					d.SetValue(ReloadDelayProperty, MinReloadDelay);
				}
				else
				{
					((FlexibleImage)d)._timer.Interval = (TimeSpan)e.NewValue;
				}
			}));
			RenderingModeProperty = DependencyProperty.RegisterAttached("RenderingMode", typeof(BitmapRenderingMode), typeof(FlexibleImage), new FrameworkPropertyMetadata(BitmapRenderingMode.WhenArranged, FrameworkPropertyMetadataOptions.Inherits));
			OutOfTreeSizeProperty = DependencyProperty.RegisterAttached("OutOfTreeSize", typeof(Size), typeof(FlexibleImage), new FrameworkPropertyMetadata(Size.Empty, FrameworkPropertyMetadataOptions.Inherits));
			AutoRenderImageWhenResizingProperty = DependencyProperty.RegisterAttached("AutoRenderImageWhenResizing", typeof(bool), typeof(FlexibleImage), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));
			ViewboxProperty = TileBrush.ViewboxProperty.AddOwner(typeof(FlexibleImage), new FrameworkPropertyMetadata(new Rect(0.0, 0.0, 1.0, 1.0), FrameworkPropertyMetadataOptions.AffectsRender, (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
			{
				FlexibleImage flexibleImage = (FlexibleImage)o;
				flexibleImage.ClearRenderState();
				Rect rect = (Rect)args.NewValue;
				double val = (double.IsNaN(rect.X) ? 0.0 : Math.Max(rect.X, 0.0));
				double val2 = (double.IsNaN(rect.Y) ? 0.0 : Math.Max(rect.Y, 0.0));
				val = Math.Min(val, 1.0);
				val2 = Math.Min(val2, 1.0);
				double width = Math.Min(rect.Width, 1.0 - val);
				double height = Math.Min(rect.Height, 1.0 - val2);
				Rect viewbox = new Rect(val, val2, width, height);
				flexibleImage.Viewbox = viewbox;
				if (flexibleImage.IsLoaded)
				{
					flexibleImage._isRendering = true;
				}
			}));
			BitmapScalingModeSwitchProperty = DependencyProperty.RegisterAttached("BitmapScalingModeSwitch", typeof(bool), typeof(FlexibleImage), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));
			ReloadingEvent = EventManager.RegisterRoutedEvent("Reloading", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(FlexibleImage));
			ReloadedEvent = EventManager.RegisterRoutedEvent("Reloaded", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(FlexibleImage));
			ManuallyRenderEvent = EventManager.RegisterRoutedEvent("ManuallyRender", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(FlexibleImage));
			UIElement.SnapsToDevicePixelsProperty.OverrideMetadata(typeof(FlexibleImage), new FrameworkPropertyMetadata(true));
		}

		public FlexibleImage()
		{
			Reloading += OnReloading;
			Reloaded += OnReloaded;
			ManuallyRender += OnManuallyRender;
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			_timer = new DispatcherTimer(DispatcherPriority.ContextIdle, Dispatcher)
			{
				Interval = ReloadDelay
			};
			_timer.Tick += OnTick;
			Loaded += LoadedForWatchRenderBounds;
			Unloaded += UnloadedForWatchRenderBounds;
		}

		private async void LoadedForWatchRenderBounds(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(FileName) && new FileInfo(FileName).Exists && AutoRenderImageWhenResizing)
			{
				await WatchRenderBoundsAsync(this).ConfigureAwait(false);
			}
		}

		private void UnloadedForWatchRenderBounds(object sender, RoutedEventArgs e)
		{
			StopWatchRenderSize(this);
		}

		private void OnReloading(object sender, RoutedEventArgs e)
		{
			_isLoading = true;
			ReloadMode reloadMode = (e as ImageReloadingEventArgs)?.ReloadMode ?? ReloadMode.Display;
			InvalidateRender(true, reloadMode, (e as ImageReloadingEventArgs)?.Scale ?? 1.0);
		}

		private void OnReloaded(object sender, RoutedEventArgs e)
		{
			_isLoading = false;
			if (IsLoaded)
			{
				ClearRenderState();
				InvalidateRender();
			}
			else
			{
				InvalidateRender(true);
			}
		}

		private void OnManuallyRender(object sender, RoutedEventArgs e)
		{
			RequireRender();
		}

		private static async Task WatchRenderBoundsAsync(FlexibleImage newlyWatchingImage)
		{
			_003C_003Ec__DisplayClass15_0 CS_0024_003C_003E8__locals10 = new _003C_003Ec__DisplayClass15_0();
			CS_0024_003C_003E8__locals10.watchingImages = WatchingImages;
			if (_isWatchingBoundsChanging)
			{
				if (!CS_0024_003C_003E8__locals10.watchingImages.Contains(newlyWatchingImage))
				{
					CS_0024_003C_003E8__locals10.watchingImages.Add(newlyWatchingImage);
				}
				return;
			}
			_isWatchingBoundsChanging = true;
			CS_0024_003C_003E8__locals10.watchingImages.Add(newlyWatchingImage);
			try
			{
				while (CS_0024_003C_003E8__locals10.watchingImages.Any() && CS_0024_003C_003E8__locals10.watchingImages.Any((FlexibleImage x) => !x.IsSmallEnoughForNaturalRender))
				{
					CS_0024_003C_003E8__locals10.watchingImages.ForEach((FlexibleImage x) =>
					{
						x._renderingModeDuringLoad = x.RenderingMode;
					});
					await Task.Delay(CS_0024_003C_003E8__locals10.watchingImages.Min((FlexibleImage x) => x.ReloadDelay));
					CS_0024_003C_003E8__locals10._003CWatchRenderBoundsAsync_003Eg__WatchIfNeeded_007C1().ToList().ForEach((FlexibleImage x) =>
					{
						CS_0024_003C_003E8__locals10.watchingImages.Remove(x);
					});
				}
			}
			finally
			{
				_isWatchingBoundsChanging = false;
			}
		}

		private static void StopWatchRenderSize(FlexibleImage stoppingImage)
		{
			WatchingImages.Remove(stoppingImage);
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			_isLoading = false;
			_renderingModeDuringLoad = RenderingMode;
			if (RenderingMode != BitmapRenderingMode.WhenArranged)
			{
				_isRendering = true;
				_isDestroying = false;
				RequireRender(true);
			}
			_isLoaded = true;
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			_isLoaded = false;
			_isLoading = false;
			if (_renderingModeDuringLoad == BitmapRenderingMode.WhenArranged)
			{
				return;
			}
			_timer.Stop();
			_isDestroying = true;
			Dispatcher.InvokeAsync(() =>
			{
				if (_isDestroying)
				{
					_isDestroying = false;
					InvalidateRender(true);
					ClearRenderState();
				}
			}, DispatcherPriority.Input);
		}

		protected override void OnRender(DrawingContext dc)
		{
			if (!IsFileExists || RenderSize.Width <= 0.0 || RenderSize.Height <= 0.0)
			{
				return;
			}
			BitmapSource bitmapSource = _targetImage;
			if (bitmapSource == null)
			{
				WeakReference<BitmapSource> item = _cachedBitmapInfo.Item1;
				BitmapSource target = null;
				if (item != null && item.TryGetTarget(out target) && IsLoaded)
				{
					bitmapSource = target;
					UpdateCore(ScaledSize);
				}
			}
			if (_isRendering || bitmapSource == null)
			{
				_isRendering = false;
				if (IsLoaded || _renderingModeDuringLoad == BitmapRenderingMode.WhenArranged)
				{
					if (_targetImage == null)
					{
						UpdateCore();
					}
					RenderImage(dc, _targetImage, RenderSize);
				}
				else if (_isLoading)
				{
					_isLoading = false;
					if (_targetImage == null)
					{
						UpdateCore();
					}
					RenderImage(dc, _targetImage, RenderSize);
				}
				else if (_renderingModeDuringLoad == BitmapRenderingMode.LoadForClearer && OutOfTreeSize != Size.Empty)
				{
					UpdateCore(OutOfTreeSize);
					RenderImage(dc, _targetImage, RenderSize);
				}
			}
			else
			{
				RenderImage(dc, bitmapSource, RenderSize);
				if (IsLoaded)
				{
					RequireRender();
				}
			}
		}

		private void RenderImage(DrawingContext dc, BitmapSource bitmap, Size size)
		{
			Rect snappedBounds = GetSnappedBounds(size);
			GuidelineSet guidelines = new GuidelineSet(new double[2]
			{
				snappedBounds.X,
				snappedBounds.X + snappedBounds.Width
			}, new double[2]
			{
				snappedBounds.Y,
				snappedBounds.Y + snappedBounds.Height
			});
			dc.PushGuidelineSet(guidelines);
			dc.DrawImage(bitmap, snappedBounds);
			dc.Pop();
		}

		private Point GetSnapLocation()
		{
			UIElement uIElement = SnapTargets.FirstOrDefault();
			if (uIElement != null)
			{
				return TranslatePoint(default(Point), uIElement);
			}
			return default(Point);
		}

		private Rect GetSnappedBounds(Size size)
		{
			foreach (UIElement snapTarget in SnapTargets)
			{
				Rect snappedBoundsTo = GetSnappedBoundsTo(snapTarget, size);
				if (snappedBoundsTo.Width != 0.0 && snappedBoundsTo.Height != 0.0)
				{
					return snappedBoundsTo;
				}
			}
			return new Rect(size);
		}

		private Rect GetSnappedBoundsTo(UIElement element, Size size)
		{
			Point point = new Point(size.Width, size.Height);
			Point point2 = new Point(size.Width / 2.0, size.Height / 2.0);
			double item = this.GetTransformInfoToDevice().Item2;
			Point point3 = (default(Point) - point2).GetRotateVector(0.0 - item) + point2;
			Point point4 = (point - point2).GetRotateVector(0.0 - item) + point2;
			Point point5 = TranslatePoint(point3, element);
			Point point6 = TranslatePoint(point4, element);
			Rect rect = new Rect(point5, point6).MoveToRound(true);
			Point point7 = element.TranslatePoint(rect.TopLeft, this);
			Point point8 = element.TranslatePoint(rect.BottomRight, this);
			Point point9 = new Rect(point7, point8).Center();
			Point point10 = (point7 - point9).GetRotateVector(item) + point9;
			Point point11 = (point8 - point9).GetRotateVector(item) + point9;
			return new Rect(point10, point11);
		}

		private void RequireRender(bool immediately = false)
		{
			if (!IsFileExists)
			{
				return;
			}
			Size lastDesiredImageSize = _lastDesiredImageSize;
			Size desiredImageSize = DesiredImageSize;
			if (!IsValidSize(lastDesiredImageSize) || !IsValidSize(desiredImageSize) || Nearly.Equals(desiredImageSize, lastDesiredImageSize, 0.5))
			{
				return;
			}
			if (immediately || desiredImageSize.Width > lastDesiredImageSize.Width * 2.0 || desiredImageSize.Height > lastDesiredImageSize.Height * 2.0)
			{
				if (!_isUpdating)
				{
					UpdateAsync().ConfigureAwait(false);
					return;
				}
				_timer.Stop();
				_timer.Start();
			}
			else
			{
				_timer.Stop();
				_timer.Start();
			}
		}

		private void InvalidateRender(bool force = false, ReloadMode reloadMode = ReloadMode.Display, double scale = 0.0)
		{
			if (IsFileExists)
			{
				_cachedBitmapInfo.Item1 = new WeakReference<BitmapSource>(_targetImage);
				_cachedBitmapInfo.Item2 = _lastImageSize;
				_cachedBitmapInfo.Item3 = _naturalSize;
				ClearRenderState();
				if (force)
				{
					ForceRender(reloadMode, scale);
				}
				else
				{
					InvalidateVisual();
				}
			}
		}

		private async void OnTick(object sender, EventArgs e)
		{
			_timer.Stop();
			if (IsFileExists && IsLoaded && !Nearly.Equals(RenderSize, default(Size)))
			{
				await UpdateAsync().ConfigureAwait(false);
			}
		}

		private async Task UpdateAsync()
		{
			if (!IsFileExists)
			{
				return;
			}
			if (_isUpdating)
			{
				RequireRender();
				return;
			}
			_isUpdating = true;
			try
			{
				await UpdateCoreAsync();
			}
			finally
			{
				_isUpdating = false;
			}
		}

		private async Task UpdateCoreAsync()
		{
			if (!IsFileExists)
			{
				return;
			}
			FileInfo file = _file;
			Size size = ScaledSize;
			Rect viewbox = Viewbox;
			Size desiredSize = (IsLoaded ? DesiredImageSize : default(Size));
			if (IsValidSize(desiredSize))
			{
				ValueTuple<BitmapSource, Size, Size> valueTuple = await Task.Run( () => GenerateImage(file, size, viewbox));
				BitmapSource item = valueTuple.Item1;
				Size item2 = valueTuple.Item2;
				Size item3 = valueTuple.Item3;
				_targetImage = item;
				_naturalSize = item2;
				_lastImageSize = item3;
				_lastDesiredImageSize = desiredSize;
				UpdateInterpolation(_naturalSize, _lastImageSize);
				InvalidateVisual();
			}
		}

		private void UpdateCore()
		{
			UpdateCore(ScaledSize);
		}

		private void UpdateCore(Size limitedSize)
		{
			if (IsFileExists)
			{
				ValueTuple<BitmapSource, Size, Size> valueTuple = GenerateImage(_file, limitedSize, Viewbox);
				BitmapSource item = valueTuple.Item1;
				Size item2 = valueTuple.Item2;
				Size item3 = valueTuple.Item3;
				_targetImage = item;
				_naturalSize = item2;
				_lastImageSize = item3;
				_lastDesiredImageSize = (IsLoaded ? DesiredImageSize : default(Size));
				UpdateInterpolation(_naturalSize, _lastImageSize);
			}
		}

		private void ClearRenderState()
		{
			_targetImage = null;
			_lastImageSize = default(Size);
			_lastDesiredImageSize = default(Size);
			_naturalSize = default(Size);
		}

		private void ForceRender(ReloadMode reloadMode = ReloadMode.Display, double scale = 0.0)
		{
			if (!IsFileExists)
			{
				return;
			}
			if (IsLoaded && reloadMode == ReloadMode.Display)
			{
				InvalidateVisual();
				return;
			}
			using (DrawingContext drawingContext = RenderOpen())
			{
				if (scale > 0.0)
				{
					UpdateCore(RenderSize.ToScaledSize(scale, scale));
				}
				OnRender(drawingContext);
			}
		}

		internal Size GetImageSize(string fileName, Size? fallbackSize = null)
		{
			return HandleNotSupportException(() => Pool.GetImageSize(fileName), () => fallbackSize ?? _lastImageSize, () => fallbackSize ?? DesiredSize);
		}

		
		internal ValueTuple<BitmapSource, Size, Size> GenerateImage(FileSystemInfo file, Size limitedSize, Rect viewbox)
		{
			return HandleNotSupportException(() => Pool.GetImage(file, limitedSize, viewbox, WhenForceNatural), () => new ValueTuple<BitmapSource, Size, Size>(_targetImage, _naturalSize, _lastImageSize), () => new ValueTuple<BitmapSource, Size, Size>(null, limitedSize, limitedSize));
		}

		private bool WhenForceNatural(Size naturalSize, Size actualSize)
		{
			if (!IsDecodingScaleEnabled || actualSize.Width > naturalSize.Width / 2.0 || actualSize.Height > naturalSize.Height / 2.0 || IsSmallEnoughForNaturalRender)
			{
				return true;
			}
			return false;
		}

		private void UpdateInterpolation(Size naturalSize, Size actualSize)
		{
			if (naturalSize != default(Size) && actualSize != default(Size) && BitmapScalingModeSwitch && (actualSize.Width < naturalSize.Width || actualSize.Height < naturalSize.Height))
			{
				SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.NearestNeighbor);
			}
			else
			{
				ClearValue(RenderOptions.BitmapScalingModeProperty);
			}
		}

		private DrawingContext RenderOpen()
		{
			return (DrawingContext)_renderOpenLazy.Value.Invoke(this, null);
		}

		private static bool IsValidSize(Size size)
		{
			if (!double.IsNaN(size.Width) && !double.IsNaN(size.Height) && !double.IsInfinity(size.Width))
			{
				return !double.IsInfinity(size.Height);
			}
			return false;
		}

		private T HandleNotSupportException<T>(Func<T> func, Func<T> defaultValueForThisTimeFailed, Func<T> defaultValueForAlwaysFailed)
		{
			try
			{
				_imageGenerationCount++;
				return func();
			}
			catch (NotSupportedException ex)
			{
				NotSupportedException ex2 = ex;
				NotSupportedException ex3 = ex2;
				_imageGenerationFailedCount++;
				if (_imageGenerationCount == _imageGenerationFailedCount)
				{
					if (_imageGenerationCount <= 4)
					{
						Dispatcher.InvokeAsync(() =>
						{
							ExceptionDispatchInfo.Capture(ex3).Throw();
						});
					}
					return defaultValueForAlwaysFailed();
				}
				if (_imageGenerationCount / _imageGenerationFailedCount > 4)
				{
					Dispatcher.InvokeAsync(() =>
					{
						ExceptionDispatchInfo.Capture(ex3).Throw();
					});
				}
				return defaultValueForThisTimeFailed();
			}
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			return MeasureArrangeHelper(availableSize);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			return finalSize;
		}

		private Size MeasureArrangeHelper(Size inputSize)
		{
			if (!IsFileExists)
			{
				if (double.IsInfinity(inputSize.Width) || double.IsInfinity(inputSize.Height))
				{
					return new Size(30.0, 30.0);
				}
				return inputSize;
			}
			Size size = _naturalSize;
			if (size == default(Size))
			{
				size = (_naturalSize = GetImageSize(_file.FullName, inputSize));
			}
			return new Size(Math.Min(size.Width, inputSize.Width), Math.Min(size.Height, inputSize.Height));
		}

		public static void SetRenderingMode(DependencyObject element, BitmapRenderingMode value)
		{
			element.SetValue(RenderingModeProperty, value);
		}

		public static BitmapRenderingMode GetRenderingMode(DependencyObject element)
		{
			return (BitmapRenderingMode)element.GetValue(RenderingModeProperty);
		}

		public static void SetOutOfTreeSize(DependencyObject element, Size value)
		{
			element.SetValue(OutOfTreeSizeProperty, value);
		}

		public static Size GetOutOfTreeSize(DependencyObject element)
		{
			return (Size)element.GetValue(OutOfTreeSizeProperty);
		}

		public static bool GetAutoRenderImageWhenResizing(DependencyObject element)
		{
			return (bool)element.GetValue(AutoRenderImageWhenResizingProperty);
		}

		public static void SetAutoRenderImageWhenResizing(DependencyObject element, bool value)
		{
			element.SetValue(AutoRenderImageWhenResizingProperty, value);
		}

		[Conditional("DEBUG")]
		private static void Log(string text)
		{
		}

		public static IDestroyableImages ReloadAllTemporarily(Visual visualRoot, double scale = 1.0, ReloadMode reloadMode = ReloadMode.AncestorOutput)
		{
			Visual visual = visualRoot;
			if (visual == null)
			{
				throw new ArgumentNullException("visualRoot");
			}
			visualRoot = visual;
			ImageReloadingEventArgs e = new ImageReloadingEventArgs(ReloadingEvent)
			{
				ReloadMode = reloadMode,
				Scale = scale
			};
			foreach (UIElement item in IterateChildren(visualRoot, (DependencyObject x) => x is UIElement))
			{
				item.RaiseEvent(e);
			}
			return new FlexibleImageReloadingContext(() =>
			{
				RoutedEventArgs e2 = new RoutedEventArgs(ReloadedEvent);
				foreach (UIElement item2 in IterateChildren(visualRoot, (DependencyObject x) => x is UIElement))
				{
					item2.RaiseEvent(e2);
				}
			});
		}

		public static void RenderAllManually(Visual visualRoot)
		{
			Visual visual = visualRoot;
			if (visual == null)
			{
				throw new ArgumentNullException("visualRoot");
			}
			visualRoot = visual;
			RoutedEventArgs e = new RoutedEventArgs(ManuallyRenderEvent, visualRoot);
			foreach (UIElement item in IterateChildren(visualRoot, (DependencyObject x) => x is UIElement))
			{
				item.RaiseEvent(e);
			}
		}

		private static IEnumerable<DependencyObject> IterateChildren(DependencyObject d, Predicate<DependencyObject> predicate)
		{
			yield return d;
			IEnumerable children = LogicalTreeHelper.GetChildren(d);
			foreach (DependencyObject item in from x in children.OfType<DependencyObject>()
				where predicate?.Invoke(x) ?? true
				select x)
			{
				IEnumerable<DependencyObject> enumerable = IterateChildren(item, predicate);
				foreach (DependencyObject item2 in enumerable)
				{
					yield return item2;
				}
			}
		}

		[CompilerGenerated]
		internal static void _003CWatchRenderBoundsAsync_003Eg__WatchBounds_007C15_2(FlexibleImage x)
		{
			_003C_003Ec__DisplayClass15_1 obj = default(_003C_003Ec__DisplayClass15_1);
			obj.x = x;
			_003CWatchRenderBoundsAsync_003Eg__WatchLocation_007C15_6(ref obj);
			_003CWatchRenderBoundsAsync_003Eg__WatchSize_007C15_7(ref obj);
		}

		[CompilerGenerated]
		private static void _003CWatchRenderBoundsAsync_003Eg__WatchLocation_007C15_6(ref _003C_003Ec__DisplayClass15_1 P_0)
		{
			Point snapLocation = P_0.x.GetSnapLocation();
			if (!Nearly.Equals(P_0.x._lastWatchedLocation, snapLocation))
			{
				P_0.x._lastWatchedLocation = snapLocation;
				P_0.x.InvalidateVisual();
			}
		}

		[CompilerGenerated]
		private static void _003CWatchRenderBoundsAsync_003Eg__WatchSize_007C15_7(ref _003C_003Ec__DisplayClass15_1 P_0)
		{
			Size scaledSize = P_0.x.ScaledSize;
			if (!Nearly.Equals(P_0.x._lastWatchedSize, scaledSize, 0.5))
			{
				P_0.x._lastWatchedSize = scaledSize;
				P_0.x.ClearValue(RenderOptions.BitmapScalingModeProperty);
				P_0.x.RequireRender();
			}
		}
	}
}
