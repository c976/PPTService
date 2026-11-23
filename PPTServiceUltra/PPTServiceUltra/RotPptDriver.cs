using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Office.Interop.PowerPoint;

namespace PPTServiceUltra
{
	/// <summary>
	/// 独立的备用演示程序通道：不依赖注册表的 GetActiveObject/ProgID 绑定，
	/// 而是直接枚举 Windows 运行对象表（ROT）发现正在运行的 PowerPoint / WPS 实例。
	///
	/// 设计取舍（与 COM 通道的差异）：
	/// 1) COM 通道通过注册表把 ProgID 映射到 CLSID 再取活动对象；当 WPS 未勾选
	///    “兼容第三方软件”、或注册表 COM 注册损坏时该通道失效。ROT 通道绕开注册表，
	///    逐个取出 ROT 中的 COM 对象并“探测身份”，因此对 Office 与 WPS 一视同仁。
	/// 2) 身份探测全部走 IDispatch 反射（按属性/方法名），而不是强转 interop 接口：
	///    WPS 在非兼容模式下 ProgID/CLSID 不同，但其自动化接口仍沿用 PowerPoint 的
	///    DispId/名称，按名字访问兼容性最好。
	/// 3) 多目标时以“用户当前正在看的那一场”为准：比较放映窗口与前台窗口的
	///    根窗口/进程，取分数最高者；无法确定时宁可把 HWND 置 0，也不返回错误句柄。
	/// 4) 状态变化完全靠定时轮询（不订阅 COM 事件），所有 COM 访问都容错并写日志，
	///    绝不把异常抛到 UI 线程。
	/// </summary>
	internal sealed class RotPptDriver : IPptDriver
	{
		#region 事件

		public event EventHandler ShowBegin;
		public event EventHandler ShowEnd;
		public event EventHandler NextSlide;

		#endregion

		#region Win32 / OLE P-Invoke

		[DllImport("ole32.dll")]
		private static extern int GetRunningObjectTable(uint reserved, out IRunningObjectTable pprot);

		[DllImport("ole32.dll")]
		private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);

		private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

		[DllImport("user32.dll")]
		private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

		[DllImport("user32.dll")]
		private static extern bool IsWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		private static extern IntPtr GetAncestor(IntPtr hWnd, uint gaFlags);

		[DllImport("user32.dll")]
		private static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

		[DllImport("user32.dll")]
		private static extern int GetSystemMetrics(int nIndex);

		private const uint GA_ROOT = 2;
		private const int SM_CXSCREEN = 0;
		private const int SM_CYSCREEN = 1;

		[StructLayout(LayoutKind.Sequential)]
		private struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		#endregion

		#region 内部数据模型

		/// <summary>枚举 ROT 时收集到的一场候选放映（仅含选材所需的最小信息）。</summary>
		private sealed class ShowInfo
		{
			public object App;
			public object Win;
			public object View;
			public object Pres;
			public IntPtr Hwnd;
			public int Score;
			public bool Ambiguous;
		}

		/// <summary>最终锁定的放映目标：缓存标量值，并持有需要续命的 COM 引用。</summary>
		private sealed class Target
		{
			public object App;
			public object ShowWindow;
			public object View;
			public object Presentation;
			public IntPtr Hwnd;

			public string FilePath;
			public int SlidesCount;
			public PpSlideShowRangeType ShowRangeType;
			public int StartingSlide;
			public int EndingSlide;
			public PpSlideShowType? ShowType;
			public int CurrentPosition;
			public PpSlideShowPointerType PointerType;
			public Color PointerColor;

			/// <summary>本对象是否持有该 COM 引用（释放临时对象时要跳过）。</summary>
			public bool Owns(object o)
			{
				return ReferenceEquals(o, App) || ReferenceEquals(o, ShowWindow)
					|| ReferenceEquals(o, View) || ReferenceEquals(o, Presentation);
			}
		}

		#endregion

		#region 字段

		private readonly object _sync = new object();
		private DispatcherTimer _timer;
		private EventHandler _tickHandler;
		private bool _disposed;

		// 当前锁定的放映目标与派生状态。
		private Target _target;
		private int _lastPosition = -1;
		private bool _isShowing;
		private PpSlideShowPointerType _pointerTypeCache = PpSlideShowPointerType.ppSlideShowPointerArrow;
		private Color _pointerColorCache = PenSettings.DefaultColor;

		// Slides 属性按需构造，缓存以降低 COM 往返。
		private List<Slide> _slidesCache;
		private readonly List<object> _slideRefs = new List<object>();

		// /debug 模拟放映模式由上层注入的窗口句柄。
		private IntPtr _debugHwnd;

		#endregion

		#region IPptDriver 属性

		public bool IsShowing
		{
			get { lock (_sync) { return _isShowing; } }
			set { lock (_sync) { _isShowing = value; } }
		}

		public IntPtr SlideShowWindowIntPtr
		{
			get
			{
				lock (_sync)
				{
					// 调试模式由上层注入真实窗口句柄，优先使用。
					if (_debugHwnd != IntPtr.Zero)
					{
						return _debugHwnd;
					}
					return _target == null ? IntPtr.Zero : _target.Hwnd;
				}
			}
		}

		public int SlidesCount
		{
			get { lock (_sync) { return _target == null ? 0 : _target.SlidesCount; } }
		}

		public string PptFilePath
		{
			get { lock (_sync) { return _target == null ? null : _target.FilePath; } }
		}

		public List<Slide> Slides
		{
			get
			{
				lock (_sync)
				{
					if (_slidesCache != null)
					{
						return _slidesCache;
					}

					List<Slide> result = new List<Slide>();
					if (_target == null || _target.Presentation == null)
					{
						_slidesCache = result;
						return result;
					}

					object slidesObj = TryGetProperty(_target.Presentation, "Slides");
					if (slidesObj != null)
					{
						try
						{
							IEnumerable enumerable = slidesObj as IEnumerable;
							if (enumerable != null)
							{
								foreach (object item in enumerable)
								{
									Slide slide = TryGetSlide(item);
									if (slide != null)
									{
										result.Add(slide);
										_slideRefs.Add(slide);
									}
								}
							}
						}
						catch (Exception ex)
						{
							Log.WriteError("RotPptDriver.Slides 读取失败：" + ex);
						}
						finally
						{
							SafeRelease(slidesObj);
						}
					}

					_slidesCache = result;
					return result;
				}
			}
		}

		public PpSlideShowRangeType ShowRangeType
		{
			get
			{
				lock (_sync)
				{
					return _target == null ? PpSlideShowRangeType.ppShowAll : _target.ShowRangeType;
				}
			}
		}

		public int StartingSlide
		{
			get { lock (_sync) { return _target == null ? 0 : _target.StartingSlide; } }
		}

		public int EndingSlide
		{
			get { lock (_sync) { return _target == null ? 0 : _target.EndingSlide; } }
		}

		public PpSlideShowType? PptShowType
		{
			get { lock (_sync) { return _target == null ? default(PpSlideShowType?) : _target.ShowType; } }
		}

		public int CurrentShowPosition
		{
			get { lock (_sync) { return _target == null ? 0 : _target.CurrentPosition; } }
		}

		public Color PointerColor
		{
			get { lock (_sync) { return _pointerColorCache; } }
			set
			{
				lock (_sync)
				{
					_pointerColorCache = value;
					WritePointerColorToCom(value);
				}
			}
		}

		public PpSlideShowPointerType PointerType
		{
			get { lock (_sync) { return _pointerTypeCache; } }
			set
			{
				lock (_sync)
				{
					_pointerTypeCache = value;
					SetComPropertyQuiet(GetCurrentView(), "PointerType", (int)value);
				}
			}
		}

		#endregion

		#region IPptDriver 方法

		public void Initialize()
		{
			if (_timer != null || _disposed)
			{
				return;
			}

			// DispatcherTimer 的回调运行在 UI 线程，事件订阅者无需自行切回。
			_tickHandler = delegate { Poll(); };
			_timer = new DispatcherTimer(DispatcherPriority.Background, Dispatcher.CurrentDispatcher);
			_timer.Interval = TimeSpan.FromMilliseconds(300.0);
			_timer.Tick += _tickHandler;
			_timer.Start();

			// 初始化时立即探测一次，避免首轮 300ms 空窗。
			Poll();
		}

		public void Stop()
		{
			if (_timer != null)
			{
				_timer.Stop();
				_timer.Tick -= _tickHandler;
				_timer = null;
				_tickHandler = null;
			}

			lock (_sync)
			{
				if (_target != null)
				{
					ReleaseTarget(_target);
					_target = null;
				}
				ClearSlidesCache();
				_isShowing = false;
				_lastPosition = -1;
				_disposed = true;
			}
		}

		public void NextPage()
		{
			SyntacticSugar.TryCatchAction(delegate
			{
				object view = GetCurrentView();
				if (view != null)
				{
					InvokeComMethod(view, "Next");
				}
			});
		}

		public void PreviousPage()
		{
			SyntacticSugar.TryCatchAction(delegate
			{
				object view = GetCurrentView();
				if (view != null)
				{
					InvokeComMethod(view, "Previous");
				}
			});
		}

		public void GotoSlide(int index)
		{
			SyntacticSugar.TryCatchAction(delegate
			{
				object view = GetCurrentView();
				if (view != null)
				{
					// 可选参数 ResetSlide 交给 COM 默认值处理，只传页码。
					InvokeComMethod(view, "GotoSlide", index);
				}
			});
		}

		public void Exit()
		{
			SyntacticSugar.TryCatchAction(delegate
			{
				object view = GetCurrentView();
				if (view != null)
				{
					InvokeComMethod(view, "Exit");
				}
			});
		}

		public void SetPointerColor(int color)
		{
			lock (_sync)
			{
				_pointerColorCache = RgbToColor(color);
				WritePointerColorToCom(_pointerColorCache);
			}
		}

		public void UpdatePointerColor()
		{
			lock (_sync)
			{
				WritePointerColorToCom(_pointerColorCache);
			}
		}

		public void ResetPointerColor()
		{
			lock (_sync)
			{
				_pointerColorCache = PenSettings.DefaultColor;
				WritePointerColorToCom(_pointerColorCache);
			}
		}

		public void SetDebugSlideWindowHandle(IntPtr hwnd)
		{
			lock (_sync)
			{
				_debugHwnd = hwnd;
				if (hwnd != IntPtr.Zero)
				{
					_isShowing = true;
				}
			}
		}

		#endregion

		#region 轮询与选材

		/// <summary>
		/// 每 300ms 一次：校验/重新发现目标，检测放映开始、结束与翻页，并在锁外触发事件。
		/// </summary>
		private void Poll()
		{
			if (_disposed || DebugMode.IsEnabled)
			{
				return;
			}

			// 上一次 COM 探测被 PPT 瞬时拒绝时，短时间内跳过，避免刷屏与 UI 卡顿。
			if (SyntacticSugar.IsComBusy)
			{
				return;
			}

			bool raiseBegin = false;
			bool raiseEnd = false;
			bool raiseNext = false;

			SyntacticSugar.TryComAction(delegate
			{
				lock (_sync)
				{
					if (_target != null && !IsTargetAlive(_target))
					{
						ReleaseTarget(_target);
						_target = null;
						ClearSlidesCache();
					}

					if (_target == null)
					{
						Target found = Discover();
						if (found != null)
						{
							_target = found;
							_lastPosition = found.CurrentPosition;
							_pointerTypeCache = found.PointerType;
							_pointerColorCache = found.PointerColor;
							raiseBegin = true;
						}
					}
					else
					{
						RefreshTarget(_target);
						int pos = _target.CurrentPosition;
						if (pos != _lastPosition)
						{
							_lastPosition = pos;
							raiseNext = true;
						}
					}

					bool showing = _target != null;
					if (showing != _isShowing)
					{
						_isShowing = showing;
						if (!showing)
						{
							raiseEnd = true;
						}
					}
				}
			});

			// 事件放在锁外触发，避免订阅者回调期间持有锁。
			if (raiseEnd)
			{
				SafeRaise(ShowEnd);
			}
			if (raiseBegin)
			{
				SafeRaise(ShowBegin);
			}
			if (raiseNext)
			{
				SafeRaise(NextSlide);
			}
		}

		/// <summary>通过 ROT 枚举所有运行中的演示程序，挑出当前正在放映的那一场。</summary>
		private static Target Discover()
		{
			IRunningObjectTable rot = null;
			IBindCtx bindCtx = null;
			IEnumMoniker enumMoniker = null;
			List<object> temp = new List<object>();
			List<ShowInfo> shows = new List<ShowInfo>();
			Target result = null;

			try
			{
				if (GetRunningObjectTable(0, out rot) != 0 || rot == null)
				{
					return null;
				}
				if (CreateBindCtx(0, out bindCtx) != 0)
				{
					return null;
				}

				rot.EnumRunning(out enumMoniker);
				if (enumMoniker == null)
				{
					return null;
				}

				enumMoniker.Reset();
				IMoniker[] monikers = new IMoniker[1];
				IntPtr fetched = IntPtr.Zero;

				while (enumMoniker.Next(1, monikers, fetched) == 0)
				{
					IMoniker moniker = monikers[0];
					monikers[0] = null;
					if (moniker == null)
					{
						continue;
					}

					string displayName = null;
					try
					{
						moniker.GetDisplayName(bindCtx, null, out displayName);
					}
					catch (Exception)
					{
						// 个别 moniker 取不到显示名，不影响随后按对象身份探测。
					}

					object obj = null;
					try
					{
						rot.GetObject(moniker, out obj);
					}
					catch (Exception)
					{
						// ROT 中可能残留已失效条目，忽略。
					}
					finally
					{
						SafeRelease(moniker);
					}

					if (obj == null)
					{
						continue;
					}
					temp.Add(obj);

					object app = TryGetAppFromRotObject(obj, temp, displayName);
					if (app == null)
					{
						continue;
					}

					CollectShows(app, shows, temp);
				}

				ShowInfo winner = SelectShow(shows);
				if (winner != null)
				{
					result = BuildTarget(winner);
				}
			}
			catch (Exception ex)
			{
				Log.WriteError("RotPptDriver.Discover 失败：" + ex);
			}
			finally
			{
				foreach (object o in temp)
				{
					if (result != null && result.Owns(o))
					{
						continue;
					}
					SafeRelease(o);
				}
				SafeRelease(enumMoniker);
				SafeRelease(bindCtx);
				SafeRelease(rot);
			}

			return result;
		}

		/// <summary>
		/// 判断一个 ROT 对象是不是应用对象；如果是文稿对象，则取其 Application 属性。
		/// 全程按自动化名称探测，兼容 PowerPoint 与 WPS。
		/// </summary>
		private static object TryGetAppFromRotObject(object obj, List<object> temp, string displayName)
		{
			// 应用对象：拥有 SlideShowWindows 集合。
			if (PeekProperty(obj, "SlideShowWindows"))
			{
				return obj;
			}

			// 文稿对象：拥有 Slides 集合与 FullName；Application 属性可拿到所属应用。
			if (PeekProperty(obj, "Slides") && PeekProperty(obj, "FullName"))
			{
				object app = TryGetProperty(obj, "Application");
				if (app != null)
				{
					temp.Add(app);
					return app;
				}
			}

			// 保留 displayName 形参以便将来按需扩展按显示名识别；此处仅用于调试轨迹。
			if (!string.IsNullOrEmpty(displayName))
			{
				Log.WriteDebug("ROT 条目：" + displayName);
			}
			return null;
		}

		/// <summary>收集某应用下所有正在进行的放映窗口。</summary>
		private static void CollectShows(object app, List<ShowInfo> shows, List<object> temp)
		{
			object windows = TryGetProperty(app, "SlideShowWindows");
			if (windows == null)
			{
				return;
			}
			temp.Add(windows);

			try
			{
				IEnumerable enumerable = windows as IEnumerable;
				if (enumerable == null)
				{
					return;
				}

				foreach (object win in enumerable)
				{
					if (win == null)
					{
						continue;
					}
					temp.Add(win);

					int hwndValue = ReadInt(win, "HWND", 0);
					IntPtr hwnd = hwndValue > 0 ? new IntPtr(hwndValue) : IntPtr.Zero;

					object view = TryGetProperty(win, "View");
					if (view != null)
					{
						temp.Add(view);
					}
					object pres = TryGetProperty(win, "Presentation");
					if (pres != null)
					{
						temp.Add(pres);
					}

					if (view == null && pres == null)
					{
						continue;
					}

					// 部分环境下 COM 报告放映窗口句柄恒为 0，用进程内唯一全屏窗口兜底。
					if (hwnd == IntPtr.Zero || !IsWindow(hwnd))
					{
						hwnd = FindFullscreenWindowOfApp(app);
					}

					ShowInfo info = new ShowInfo();
					info.App = app;
					info.Win = win;
					info.View = view;
					info.Pres = pres;
					info.Hwnd = hwnd;
					info.Score = ScoreShow(hwnd);
					shows.Add(info);
				}
			}
			catch (Exception ex)
			{
				Log.WriteError("RotPptDriver.CollectShows 失败：" + ex);
			}
		}

		/// <summary>
		/// 打分规则：放映窗口的根窗口就是前台根窗口（用户正在看）得分最高；
		/// 其次是同进程；可见窗口额外加分。分数用于多放映时的取舍。
		/// </summary>
		private static int ScoreShow(IntPtr hwnd)
		{
			if (hwnd == IntPtr.Zero || !IsWindow(hwnd))
			{
				return 0;
			}

			int score = 0;
			IntPtr foreground = GetForegroundWindow();
			uint showPid = 0;
			uint fgPid = 0;
			GetWindowThreadProcessId(hwnd, out showPid);
			GetWindowThreadProcessId(foreground, out fgPid);

			IntPtr showRoot = GetAncestor(hwnd, GA_ROOT);
			IntPtr fgRoot = GetAncestor(foreground, GA_ROOT);
			if (showRoot != IntPtr.Zero && showRoot == fgRoot)
			{
				score += 100;
			}
			else if (showPid != 0 && showPid == fgPid)
			{
				score += 40;
			}

			if (IsWindowVisible(hwnd))
			{
				score += 10;
			}
			return score;
		}

		/// <summary>取分数最高者；并列且无法区分时标记 Ambiguous（HWND 不予返回）。</summary>
		private static ShowInfo SelectShow(List<ShowInfo> shows)
		{
			ShowInfo winner = null;
			int top = int.MinValue;
			int topCount = 0;

			for (int i = 0; i < shows.Count; i++)
			{
				ShowInfo s = shows[i];
				if (s.Score > top)
				{
					top = s.Score;
					winner = s;
					topCount = 1;
				}
				else if (s.Score == top)
				{
					topCount++;
				}
			}

			if (winner != null && topCount > 1)
			{
				winner.Ambiguous = true;
			}
			return winner;
		}

		/// <summary>为胜出的放映读取完整信息，构造缓存目标。</summary>
		private static Target BuildTarget(ShowInfo info)
		{
			Target target = new Target();
			target.App = info.App;
			target.ShowWindow = info.Win;
			target.View = info.View;
			target.Presentation = info.Pres;
			// 多目标无法唯一确定、或窗口句柄无效时，宁可不返回。
			target.Hwnd = info.Ambiguous ? IntPtr.Zero : info.Hwnd;

			// 翻页按钮的默认布局（Office 2016+ 底部 / 其余两侧）依赖版本管理器里的
			// IsOffice2016OrLater。COM 通道在绑定时会更新它，兼容通道也必须更新，
			// 否则默认positional会一直落到"两侧"。
			string appPath = TryGetString(info.App, "Path");
			string appVersion = TryGetString(info.App, "Version");
			if (!string.IsNullOrWhiteSpace(appPath))
			{
				PPTServiceUltra.Collection.PptVersionManager.UpdatePptApplicationInfo(appPath, appVersion);
			}

			target.CurrentPosition = ReadInt(info.View, "CurrentShowPosition", 0);
			target.PointerType = (PpSlideShowPointerType)ReadInt(info.View, "PointerType",
				(int)PpSlideShowPointerType.ppSlideShowPointerArrow);

			object colorFormat = TryGetProperty(info.View, "PointerColor");
			if (colorFormat != null)
			{
				try
				{
					target.PointerColor = RgbToColor(ReadInt(colorFormat, "RGB", 0));
				}
				finally
				{
					SafeRelease(colorFormat);
				}
			}
			else
			{
				target.PointerColor = PenSettings.DefaultColor;
			}

			target.FilePath = TryGetString(info.Pres, "FullName");

			object slides = TryGetProperty(info.Pres, "Slides");
			if (slides != null)
			{
				try
				{
					target.SlidesCount = ReadInt(slides, "Count", 0);
				}
				finally
				{
					SafeRelease(slides);
				}
			}

			object settings = TryGetProperty(info.Pres, "SlideShowSettings");
			if (settings != null)
			{
				try
				{
					int range = ReadInt(settings, "RangeType", (int)PpSlideShowRangeType.ppShowAll);
					target.ShowRangeType = (PpSlideShowRangeType)range;
					target.StartingSlide = ReadInt(settings, "StartingSlide", 0);
					target.EndingSlide = ReadInt(settings, "EndingSlide", 0);
					target.ShowType = (PpSlideShowType)ReadInt(settings, "ShowType",
						(int)PpSlideShowType.ppShowTypeSpeaker);
				}
				finally
				{
					SafeRelease(settings);
				}
			}

			return target;
		}

		/// <summary>校验当前目标是否仍在放映；放映窗口集合清空即视为结束。</summary>
		private static bool IsTargetAlive(Target target)
		{
			object windows = TryGetProperty(target.App, "SlideShowWindows");
			if (windows == null)
			{
				// 取不到集合时，若句柄仍有效则暂时认为存活，避免误报结束。
				return target.Hwnd != IntPtr.Zero && IsWindow(target.Hwnd);
			}
			try
			{
				return ReadInt(windows, "Count", 0) > 0;
			}
			finally
			{
				SafeRelease(windows);
			}
		}

		/// <summary>轮询中刷新会变化的量：页码、笔型、笔色、句柄。</summary>
		private static void RefreshTarget(Target target)
		{
			target.CurrentPosition = ReadInt(target.View, "CurrentShowPosition", target.CurrentPosition);
			target.PointerType = (PpSlideShowPointerType)ReadInt(target.View, "PointerType",
				(int)target.PointerType);

			object colorFormat = TryGetProperty(target.View, "PointerColor");
			if (colorFormat != null)
			{
				try
				{
					target.PointerColor = RgbToColor(ReadInt(colorFormat, "RGB", ColorToRgb(target.PointerColor)));
				}
				finally
				{
					SafeRelease(colorFormat);
				}
			}

			int hwndValue = ReadInt(target.ShowWindow, "HWND", 0);
			if (hwndValue > 0 && IsWindow(new IntPtr(hwndValue)))
			{
				target.Hwnd = new IntPtr(hwndValue);
			}
		}

		#endregion

		#region Win32 辅助：进程内全屏窗口兜底

		/// <summary>
		/// 当 COM 报告的放映窗口句柄无效时，寻找该应用进程内唯一的可见全屏窗口。
		/// 只在“唯一”时返回，否则返回 0，避免把编辑器窗口误当放映窗口。
		/// </summary>
		private static IntPtr FindFullscreenWindowOfApp(object app)
		{
			int appHwndValue = ReadInt(app, "HWND", 0);
			if (appHwndValue <= 0)
			{
				return IntPtr.Zero;
			}

			uint pid = 0;
			GetWindowThreadProcessId(new IntPtr(appHwndValue), out pid);
			if (pid == 0)
			{
				return IntPtr.Zero;
			}
			return FindFullscreenWindow(pid);
		}

		private static IntPtr FindFullscreenWindow(uint pid)
		{
			IntPtr found = IntPtr.Zero;
			int screenW = GetSystemMetrics(SM_CXSCREEN);
			int screenH = GetSystemMetrics(SM_CYSCREEN);

			EnumWindows(delegate(IntPtr hWnd, IntPtr lParam)
			{
				if (!IsWindowVisible(hWnd))
				{
					return true;
				}

				uint windowPid = 0;
				GetWindowThreadProcessId(hWnd, out windowPid);
				if (windowPid != pid)
				{
					return true;
				}

				RECT rect;
				if (!GetWindowRect(hWnd, out rect))
				{
					return true;
				}

				int width = rect.Right - rect.Left;
				int height = rect.Bottom - rect.Top;
				if (Math.Abs(width - screenW) <= 2 && Math.Abs(height - screenH) <= 2)
				{
					if (found != IntPtr.Zero)
					{
						// 出现第二个候选，无法唯一确定，放弃。
						found = IntPtr.Zero;
						return false;
					}
					found = hWnd;
				}
				return true;
			}, IntPtr.Zero);

			return found;
		}

		#endregion

		#region COM 反射与工具

		/// <summary>静默读属性：身份探测时绝大多数 ROT 对象都不是演示程序，失败属正常，不写日志。</summary>
		private static object TryGetPropertySilent(object obj, string name)
		{
			if (obj == null)
			{
				return null;
			}
			try
			{
				return obj.GetType().InvokeMember(name,
					BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance,
					null, obj, null);
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>按名称读属性；不写日志，用于身份探测等高频/可失败场景。</summary>
		private static bool PeekProperty(object obj, string name)
		{
			return TryGetPropertySilent(obj, name) != null;
		}

		/// <summary>按名称读属性，失败写日志。</summary>
		private static object TryGetProperty(object obj, string name)
		{
			if (obj == null)
			{
				return null;
			}
			try
			{
				return obj.GetType().InvokeMember(name,
					BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance,
					null, obj, null);
			}
			catch (Exception ex)
			{
				Log.WriteError("RotPptDriver 读取属性 " + name + " 失败：" + ex);
				return null;
			}
		}

		private static int ReadInt(object obj, string name, int defaultValue)
		{
			if (obj == null)
			{
				return defaultValue;
			}
			object value = TryGetProperty(obj, name);
			if (value == null)
			{
				return defaultValue;
			}
			try
			{
				return Convert.ToInt32(value);
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}

		private static string TryGetString(object obj, string name)
		{
			if (obj == null)
			{
				return null;
			}
			object value = TryGetProperty(obj, name);
			return value == null ? null : Convert.ToString(value);
		}

		private static void SetComPropertyQuiet(object obj, string name, object value)
		{
			if (obj == null)
			{
				return;
			}
			try
			{
				obj.GetType().InvokeMember(name,
					BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance,
					null, obj, new object[] { value });
			}
			catch (Exception ex)
			{
				Log.WriteError("RotPptDriver 写入属性 " + name + " 失败：" + ex);
			}
		}

		private static void InvokeComMethod(object obj, string name, params object[] args)
		{
			if (obj == null)
			{
				return;
			}
			try
			{
				obj.GetType().InvokeMember(name,
					BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
					null, obj, args);
			}
			catch (Exception ex)
			{
				Log.WriteError("RotPptDriver 调用方法 " + name + " 失败：" + ex);
			}
		}

		/// <summary>把 ROT 取出的对象尽量包装成 interop Slide，供上层作为 List&lt;Slide&gt; 使用。</summary>
		private static Slide TryGetSlide(object item)
		{
			if (item == null)
			{
				return null;
			}

			Slide slide = item as Slide;
			if (slide != null)
			{
				return slide;
			}

			try
			{
				IntPtr punk = Marshal.GetIUnknownForObject(item);
				if (punk == IntPtr.Zero)
				{
					return null;
				}
				try
				{
					object typed = Marshal.GetObjectForIUnknown(punk);
					return typed as Slide;
				}
				finally
				{
					Marshal.Release(punk);
				}
			}
			catch (Exception ex)
			{
				Log.WriteError("RotPptDriver 转换 Slide 失败：" + ex);
				return null;
			}
		}

		private object GetCurrentView()
		{
			lock (_sync)
			{
				return _target == null ? null : _target.View;
			}
		}

		private void WritePointerColorToCom(Color color)
		{
			object view = GetCurrentView();
			if (view == null)
			{
				return;
			}
			object colorFormat = TryGetProperty(view, "PointerColor");
			if (colorFormat == null)
			{
				return;
			}
			try
			{
				SetComPropertyQuiet(colorFormat, "RGB", ColorToRgb(color));
			}
			finally
			{
				SafeRelease(colorFormat);
			}
		}

		/// <summary>Office 的 RGB 为 0x00BBGGRR（红在低字节）。</summary>
		private static int ColorToRgb(Color color)
		{
			return color.R | (color.G << 8) | (color.B << 16);
		}

		private static Color RgbToColor(int rgb)
		{
			return Color.FromRgb(
				(byte)(rgb & 0xFF),
				(byte)((rgb >> 8) & 0xFF),
				(byte)((rgb >> 16) & 0xFF));
		}

		private static void SafeRelease(object com)
		{
			if (com == null)
			{
				return;
			}
			try
			{
				Marshal.ReleaseComObject(com);
			}
			catch (Exception ex)
			{
				Log.WriteError("RotPptDriver 释放 COM 对象失败：" + ex);
			}
		}

		private static void ReleaseTarget(Target target)
		{
			SafeRelease(target.View);
			SafeRelease(target.ShowWindow);
			SafeRelease(target.Presentation);
			SafeRelease(target.App);
			target.View = null;
			target.ShowWindow = null;
			target.Presentation = null;
			target.App = null;
		}

		private void ClearSlidesCache()
		{
			for (int i = 0; i < _slideRefs.Count; i++)
			{
				SafeRelease(_slideRefs[i]);
			}
			_slideRefs.Clear();
			_slidesCache = null;
		}

		private void SafeRaise(EventHandler handler)
		{
			if (handler == null)
			{
				return;
			}
			try
			{
				handler(this, EventArgs.Empty);
			}
			catch (Exception ex)
			{
				Log.WriteError("RotPptDriver 事件处理失败：" + ex);
			}
		}

		#endregion
	}
}
