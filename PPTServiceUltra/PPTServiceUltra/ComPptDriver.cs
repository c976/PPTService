using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using PPTServiceUltra.Collection;

namespace PPTServiceUltra
{
	/// <summary>
	/// COM 调用通道：原 PptOperator 的注册表 COM 实现，逻辑保持不动，
	/// 仅实现 IPptDriver 供 PptOperator 调度。
	/// </summary>
	public sealed class ComPptDriver : IPptDriver
	{
		private int _times;

		// COM 服务器持续不可用（RPC_E_SERVER_UNAVAILABLE 等）时，若一直只记日志重试，
		// 通道会永远卡死在失效的 _app 上。累计到阈值即断开绑定，让调度层的自动回退
		// 有机会切到 ROT 通道；任一次探测成功即清零。
		private const int MaxConsecutiveComFailures = 6;
		private int _consecutiveComFailures;

		private Microsoft.Office.Interop.PowerPoint.Application _app;

		private Stopwatch _stopwatch;

		private bool _isShowTypeSpeaker;

		// COM 事件与启动轮询可能在极短时间内同时命中同一场放映。IsShowing 要到
		// 一堆 COM 准备工作之后才置位，这段窗口期用进入标志挡住重复进入，
		// 否则会重复触发 ShowBegin，进而建出两个工具栏。
		private bool _isHandlingShowBegin;

		// 放映开始时 PPT COM 可能瞬时拒绝（RPC_E_CALL_REJECTED 0x80010001），典型场景是
		// CheckMainObject() 里读 SlideShowWindow.HWND。此时隔 ShowBeginRetryInterval 秒重试，
		// 累计 ShowBeginMaxRetryCount 次仍失败则放弃，避免无限重试；任一成功即清零计数。
		private const int ShowBeginRetryIntervalSeconds = 10;
		private const int ShowBeginMaxRetryCount = 6;
		private int _showBeginRetryCount;
		private DispatcherTimer _showBeginRetryTimer;

		private SlideShowWindow _slideShowWindow;

		private const int PptProcessCheckInterval = 2500;

		private static readonly int MinPageTurningTimerSpan = 30;

		// PPT 进程刚启动 / COM 忙时快速重试；进入放映后由 COM 事件接管。
		private static readonly int CheckPptRetryInterval = 500;

		private readonly DispatcherTimer _checkPowerPointTimer = new DispatcherTimer();

		private readonly DispatcherTimer _clearEmptyPowerPointTimer = new DispatcherTimer();

		// /debug 模拟放映状态（不接触任何 Office COM 对象）
		private int _debugPage = 1;
		private int _slidesCount;
		private string _pptFilePath;
		private List<Slide> _slides = new List<Slide>();
		private PpSlideShowRangeType _showRangeType;

		public bool IsShowing { get; set; }

		public Color PointerColor { get; set; }

		public int SlidesCount
		{
			get { return DebugMode.IsEnabled ? DebugMode.SlidesCount : _slidesCount; }
			private set { _slidesCount = value; }
		}

		public string PptFilePath
		{
			get { return DebugMode.IsEnabled ? "debug://simulated-slideshow" : _pptFilePath; }
			private set { _pptFilePath = value; }
		}

		public List<Slide> Slides
		{
			get { return _slides; }
			private set { _slides = value ?? new List<Slide>(); }
		}

		public Presentation Presentation { get; private set; }

		public IntPtr SlideShowWindowIntPtr { get; private set; }

		public PpSlideShowRangeType ShowRangeType
		{
			get { return DebugMode.IsEnabled ? PpSlideShowRangeType.ppShowAll : _showRangeType; }
			private set { _showRangeType = value; }
		}

		public PpSlideShowPointerType PointerType
		{
			get
			{
				PpSlideShowPointerType type = PpSlideShowPointerType.ppSlideShowPointerNone;
				SyntacticSugar.TryComAction(() =>
				{
					type = _slideShowWindow?.View.PointerType ?? PpSlideShowPointerType.ppSlideShowPointerNone;
				});
				return type;
			}
			set
			{
				SyntacticSugar.TryComAction(() =>
				{
					if (_slideShowWindow != null)
					{
						_slideShowWindow.View.PointerType = value;
					}
				});
			}
		}

		public int CurrentShowPosition
		{
			get
			{
				if (DebugMode.IsEnabled)
				{
					return _debugPage;
				}
				try
				{
					return _slideShowWindow.View.CurrentShowPosition;
				}
				catch
				{
					return 1;
				}
			}
		}

		public int EndingSlide => DebugMode.IsEnabled ? DebugMode.SlidesCount : Presentation.SlideShowSettings.EndingSlide;

		public int StartingSlide => DebugMode.IsEnabled ? 1 : Presentation.SlideShowSettings.StartingSlide;

		public PpSlideShowType? PptShowType => DebugMode.IsEnabled
			? (PpSlideShowType?)PpSlideShowType.ppShowTypeSpeaker
			: Presentation.SlideShowSettings.ShowType;

		public event EventHandler ShowBegin;

		public event EventHandler ShowEnd;

		public event EventHandler NextSlide;

		private void SetSlideAdvanceOnClickProperty(MsoTriState value)
		{
			SyntacticSugar.TryCatchAction(() =>
			{
				if (Slides == null || !Slides.Any())
				{
					return;
				}
				foreach (Slide slide in Slides)
				{
					slide.SlideShowTransition.AdvanceOnClick = value;
				}
			});
		}

		public void PreviousPage()
		{
			if (DebugMode.IsEnabled)
			{
				DebugSetPage(_debugPage - 1);
				return;
			}
			if (_stopwatch.ElapsedMilliseconds < MinPageTurningTimerSpan)
			{
				return;
			}
			SyntacticSugar.TryCatchAction(() =>
			{
				Task.Run(() =>
				{
					_slideShowWindow.View.Previous();
				});
			});
		}

		public void NextPage()
		{
			if (DebugMode.IsEnabled)
			{
				DebugSetPage(_debugPage + 1);
				return;
			}
			if (_stopwatch.ElapsedMilliseconds < MinPageTurningTimerSpan)
			{
				return;
			}
			SyntacticSugar.TryCatchAction(() =>
			{
				Task.Run(() =>
				{
					_slideShowWindow.View.Next();
				});
			});
		}

		public void Exit()
		{
			if (DebugMode.IsEnabled)
			{
				IsShowing = false;
				// 调试模式直接终止进程，绕过主窗口关闭保护 / 模拟放映窗口等所有 Closing 拦截，
				// 保证「结束放映」和 ESC 都能立即干净退出，不留后台进程。
				// 调试模式不启动 DefenderGuardian，无需 OnExit 里的收尾。
				System.Environment.Exit(0);
				return;
			}
			SetActive((IntPtr)_app.HWND);
			SyntacticSugar.TryCatchAction(() =>
			{
				Task.Run(() =>
				{
					PptAnalyzer.StopAnalyzeAllAsyn();
					_slideShowWindow.View.Exit();
				});
			});
		}

		public void GotoSlide(int index)
		{
			if (DebugMode.IsEnabled)
			{
				DebugSetPage(index);
				return;
			}
			new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowNextSlide").RemoveEventHandler(_app, new EApplication_SlideShowNextSlideEventHandler(OnSlideShowNextSlide));
			_slideShowWindow.View.GotoSlide(index, MsoTriState.msoFalse);
			new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowNextSlide").AddEventHandler(_app, new EApplication_SlideShowNextSlideEventHandler(OnSlideShowNextSlide));
			_slideShowWindow.Activate();
		}

		/// <summary>把模拟放映窗口的 HWND 交给本类，供窗口定位 / 置顶 / 批注层对齐使用。</summary>
		public void SetDebugSlideWindowHandle(IntPtr hwnd)
		{
			SlideShowWindowIntPtr = hwnd;
		}

		/// <summary>COM 通道缩略图导出：返回 false，沿用 PptAnalyzer 原有的强类型 Slide 路径。</summary>
		public bool ExportThumbnail(int slideIndex, string filePath, int width, int height)
		{
			return false;
		}

		private void DebugSetPage(int index)
		{
			if (index < 1)
			{
				index = 1;
			}
			if (index > DebugMode.SlidesCount)
			{
				index = DebugMode.SlidesCount;
			}
			if (index == _debugPage)
			{
				return;
			}
			_debugPage = index;
			OnNextSlide();
		}

		public void UpdatePointerColor()
		{
			// 放映未开始 / 已结束时不碰 COM：放映窗口的 View 此时常处于销毁中，
			// 读 PointerColor 会得到 0x80048010 / RPC_E_CALL_REJECTED。
			if (!IsShowing || _slideShowWindow == null)
			{
				return;
			}
			SyntacticSugar.TryComAction(() =>
			{
				Color red = Colors.Red;
				string text = (_slideShowWindow?.View.PointerColor.RGB ?? 0).ToString("X2").PadLeft(6, '0');
				red.R = Convert.ToByte(text.Substring(4, 2), 16);
				red.G = Convert.ToByte(text.Substring(2, 2), 16);
				red.B = Convert.ToByte(text.Substring(0, 2), 16);
				PointerColor = red;
			});
		}

		public void SetPointerColor(int color)
		{
			SyntacticSugar.TryComAction(() =>
			{
				PointerType = PpSlideShowPointerType.ppSlideShowPointerPen;
				_slideShowWindow.View.PointerColor.RGB = color;
			});
		}

		/// <summary>
		/// 放映结束：只把 Office 原生画笔颜色复位为默认色，不改指针类型
		/// （SetPointerColor 会强制切到画笔，放映结束时调用会把鼠标态弄乱）。
		/// 注意：本方法由 ShowEnd 调用，此时放映已经结束，_slideShowWindow 指向的
		/// COM 对象正在/已被 PowerPoint 释放。继续往它的 View.PointerColor 写值必然
		/// 抛 0x80048010（对象不存在）/ 0x80010001（拒绝），旧实现靠 TryComAction
		/// 吞掉异常，但每场放映结束都会产生一轮 COM 异常，且污染 RCW 引用计数。
		/// 因此放映已结束时只复位内部缓存值——下次放映 UpdatePointerColor 会重新
		/// 从COM读取，颜色不会错；只有放映仍在进行时才真正写 COM。
		/// </summary>
		public void ResetPointerColor()
		{
			PointerColor = PenSettings.DefaultColor;
			if (!IsShowing || _slideShowWindow == null)
			{
				return;
			}
			SyntacticSugar.TryComAction(() =>
			{
				Color c = PenSettings.DefaultColor;
				_slideShowWindow.View.PointerColor.RGB = (c.B << 16) | (c.G << 8) | c.R;
			});
		}

		private bool CheckMainObject()
		{
			if (_slideShowWindow == null)
			{
				Log.WriteError("SlideShowWindowIsNull");
				return false;
			}
			SlideShowWindowIntPtr = (IntPtr)(_slideShowWindow?.HWND).Value;
			Presentation = _slideShowWindow?.Presentation;
			if (Presentation == null)
			{
				Log.WriteError("PresentationIsNull");
				return false;
			}
			return true;
		}

		private void OnShowBegin()
		{
			ShowBegin?.Invoke(null, EventArgs.Empty);
		}

		private void OnShowEnd()
		{
			ShowEnd?.Invoke(null, EventArgs.Empty);
		}

		private void OnNextSlide()
		{
			NextSlide?.Invoke(this, EventArgs.Empty);
		}

		[DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
		private static extern bool SetActive(IntPtr hWnd);

		/// <summary>COM 通道是否已绑定可用的 Application；"自动"模式据此决定是否回退到 ROT 通道。</summary>
		public bool IsApplicationBound
		{
			get { return _app != null; }
		}

		/// <summary>
		/// 已经进入放映却拿不到放映窗口句柄——这正是本机 COM 损坏时的表现。
		/// 调度层据此在"自动"模式下切到 ROT 通道。
		/// </summary>
		public bool IsShowingWithoutWindow
		{
			get { return IsShowing && SlideShowWindowIntPtr == IntPtr.Zero; }
		}

		/// <summary>关闭本通道的定时器并释放 COM 引用，由调度层在切换通道/退出时调用。</summary>
		public void Stop()
		{
			bool wasShowing = IsShowing;
			_checkPowerPointTimer.Stop();
			_clearEmptyPowerPointTimer.Stop();
			if (_showBeginRetryTimer != null)
			{
				_showBeginRetryTimer.Stop();
			}
			TryCleanComObject();
			IsShowing = false;
			ConfigHelper.Instance.IsShowing = false;
			if (wasShowing)
			{
				OnShowEnd();
			}
		}

		public void Initialize()
		{
			_stopwatch = new Stopwatch();
			if (DebugMode.IsEnabled)
			{
				// 不启动任何 PowerPoint 进程轮询 / COM 调用，直接置为「正在放映」。
				_stopwatch.Start();
				IsShowing = true;
				ConfigHelper.Instance.IsShowing = true;
				ConfigHelper.Instance.IsPptShowTypeSpeaker = true;
				ConfigHelper.Instance.IsInitApplicationSuccessful = true;
				// 关闭上报，避免无网络环境下尝试联网。
				ConfigHelper.Instance.IsComOk = false;
				// 延后到 OnStartup 返回之后再触发，确保 ViewHelper 已订阅 ShowBegin。
				System.Windows.Application.Current.Dispatcher.BeginInvoke(
					new Action(OnShowBegin), DispatcherPriority.Background);
				return;
			}
			// 自动模式下每次放映结束会切回 COM 重新 Initialize；Tick 必须先退再挂，
			// 否则处理器被重复订阅，每轮探测执行多次、日志刷屏且状态机错乱。
			_checkPowerPointTimer.Tick -= CheckPowerPointProcess;
			_checkPowerPointTimer.Tick += CheckPowerPointProcess;
			_checkPowerPointTimer.Interval = TimeSpan.FromMilliseconds(CheckPptRetryInterval);
			_checkPowerPointTimer.Start();
			_clearEmptyPowerPointTimer.Tick -= ClearEmptyPowerPointProcess;
			_clearEmptyPowerPointTimer.Tick += ClearEmptyPowerPointProcess;
			_clearEmptyPowerPointTimer.Interval = TimeSpan.FromMilliseconds(2500.0);
			CheckPowerPointProcess();
		}

		private void CheckPowerPointProcess(object sender, EventArgs e)
		{
			CheckPowerPointProcess();
			if (_times++ > 10)
			{
				_times = 0;
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
		}

		private void ClearEmptyPowerPointProcess(object sender, EventArgs e)
		{
			ClearEmptyPowerPointProcess();
		}

		private void CheckPowerPointProcess()
		{
			if (!ProcessHelper.HasPowerPointProcess())
			{
				return;
			}
			try
			{
				// 已挂上 COM 就不要每轮都反注册/重注册事件（GetActiveObject 在 PPT 忙时会
				// 抛 RPC_E_CALL_REJECTED，旧逻辑此时把 _app 置空，下一次探测要再等 2.5s，
				// 累积起来就是「启动后 6~14s 才看到工具栏」）。
				if (_app == null)
				{
					CreatePowerPointApplication();
				}
				// 走到这里说明本轮 COM 探测成功，清零连续失败计数。
				_consecutiveComFailures = 0;
				InitializeControl();
				if (IsShowing)
				{
					_checkPowerPointTimer.Stop();
					_clearEmptyPowerPointTimer.Start();
				}
				else
				{
					// 放映尚未开始（或启动时已经在放映、ShowBegin 事件被错过）：
					// 保持短间隔轮询，按当前 SlideShowWindows 补一次检测。
					_checkPowerPointTimer.Interval = TimeSpan.FromMilliseconds(CheckPptRetryInterval);
					if (!_checkPowerPointTimer.IsEnabled)
					{
						_checkPowerPointTimer.Start();
					}
					_clearEmptyPowerPointTimer.Stop();
				}
			}
			catch (COMException ex) when (ex.HResult == unchecked((int)0x80048010))
			{
				// 0x80048010「对象不存在」是确定性通道失效（放映结束关闭文件后，
				// 原 Application RCW 变僵尸），不是"忙"，重试再多也不会恢复。
				// 立即断开绑定，让看门狗切 ROT 或下一轮重新 GetActiveObject。
				Log.WriteError("PPT COM object no longer exists, dropping binding: " + ex.Message);
				TryCleanComObject();
				_consecutiveComFailures = 0;
			}
			catch (COMException ex) when (SyntacticSugar.IsTransientComException(ex))
			{
				// PPT 正忙（RPC_E_CALL_REJECTED 等）：下轮快速重试。
				// 但如果是「RPC 服务器不可用」这类通道级失效，连续多次后必须断开绑定，
				// 否则 _app 一直非空、自动回退看门狗永远不触发，工具栏永久消失只能重启。
				_consecutiveComFailures++;
				Log.WriteError("PPT COM busy (transient), will retry: " + ex.Message);
				if (_consecutiveComFailures >= MaxConsecutiveComFailures)
				{
					Log.WriteError("PPT COM unavailable for " + _consecutiveComFailures +
						" consecutive checks, dropping binding for fallback.");
					_app = null;
					_consecutiveComFailures = 0;
					ConfigHelper.Instance.IsInitApplicationSuccessful = false;
				}
			}
			catch (Exception)
			{
				_app = null;
				_consecutiveComFailures = 0;
				ConfigHelper.Instance.IsInitApplicationSuccessful = false;
			}
		}

		private void ClearEmptyPowerPointProcess()
		{
			try
			{
				if (!ProcessHelper.HasPowerPointProcess())
				{
					CleanAndListen();
				}
				else if (_app != null && _app.Presentations.Count == 0)
				{
					CleanAndListen();
					if (_app.Caption.ToLower().Contains("wps") || _app.Path.ToLower().Contains("wps") || _app.Path.ToLower().Contains("wpp"))
					{
						ProcessHelper.TryKillWppProcess();
					}
				}
			}
			catch (COMException ex)
			{
				// -2147023174=RPC 服务器不可用；-2146827280=0x80048010 对象不存在。
				// 两者都说明 COM 通道已经失效（放映结束关闭文件后常见），必须断开重监听，
				// 否则僵尸 _app 会让看门狗误判"仍绑定"，既不重连也不切 ROT。
				if (ex.HResult == -2147023174 || ex.HResult == unchecked((int)0x80048010))
				{
					Log.WriteError("PPT COM object gone during idle check, resetting binding: 0x" +
						unchecked((uint)ex.HResult).ToString("X8"));
					CleanAndListen();
				}
			}
			catch (Exception ex2)
			{
				Log.WriteError(ex2.Message);
			}
		}

		private void SetTimerEnable(bool isPowerPointProcessStart)
		{
			if (isPowerPointProcessStart)
			{
				_checkPowerPointTimer.Stop();
				_clearEmptyPowerPointTimer.Start();
			}
			else
			{
				_clearEmptyPowerPointTimer.Stop();
				_checkPowerPointTimer.Interval = TimeSpan.FromMilliseconds(CheckPptRetryInterval);
				if (!_checkPowerPointTimer.IsEnabled)
				{
					_checkPowerPointTimer.Start();
				}
			}
		}

		private void InitializeControl()
		{
			if (_app == null || IsShowing)
			{
				return;
			}
			SlideShowWindows slideShowWindows = _app.SlideShowWindows;
			if (slideShowWindows == null || slideShowWindows.Count <= 0)
			{
				return;
			}
			// 启动时放映可能已经开始，SlideShowBegin 事件已经错过。
			// 旧逻辑首轮对演讲者放映直接 return，只能等用户再开一场；
			// AppSlideShowBegin 内部已按放映类型判断，非演讲者放映不会触发 ShowBegin，
			// 因此这里一律按当前放映窗口补一次初始化。
			AppSlideShowBegin(slideShowWindows[1]);
		}

		private void CreatePowerPointApplication()
		{
			if (_app != null)
			{
				new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowBegin").RemoveEventHandler(_app, new EApplication_SlideShowBeginEventHandler(AppSlideShowBegin));
				new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowEnd").RemoveEventHandler(_app, new EApplication_SlideShowEndEventHandler(OnAppSlideShowEnd));
				new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowNextSlide").RemoveEventHandler(_app, new EApplication_SlideShowNextSlideEventHandler(OnSlideShowNextSlide));
				new ComAwareEventInfo(typeof(EApplication_Event), "PresentationCloseFinal").RemoveEventHandler(_app, new EApplication_PresentationCloseFinalEventHandler(AppOnPresentationCloseFinal));
			}
			// 先挂到局部变量：任何一个 AddEventHandler 被 PPT 忙拒绝时，
			// 字段 _app 必须保持 null，下一轮探测才能重新挂全部事件。
			// 旧逻辑先赋 _app 再逐个订阅，中途失败会永久留下「部分订阅」状态，
			// 之后放映事件再也收不到、且被空 catch 吞掉无任何日志。
			Microsoft.Office.Interop.PowerPoint.Application app =
				(Microsoft.Office.Interop.PowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");
			PptVersionManager.UpdatePptApplicationInfo(app.Path, app.Version);
			new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowBegin").AddEventHandler(app, new EApplication_SlideShowBeginEventHandler(AppSlideShowBegin));
			new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowEnd").AddEventHandler(app, new EApplication_SlideShowEndEventHandler(OnAppSlideShowEnd));
			new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowNextSlide").AddEventHandler(app, new EApplication_SlideShowNextSlideEventHandler(OnSlideShowNextSlide));
			new ComAwareEventInfo(typeof(EApplication_Event), "PresentationCloseFinal").AddEventHandler(app, new EApplication_PresentationCloseFinalEventHandler(AppOnPresentationCloseFinal));
			_app = app;
			ConfigHelper.Instance.IsInitApplicationSuccessful = true;
			Log.WriteDebug("PPTApplicationInitialized");
		}

		private void AppOnPresentationCloseFinal(Presentation pres)
		{
			if (_app.Presentations.Count != 0 && _app.Presentations.Count != 1)
			{
				return;
			}
			try
			{
				if (_app.ActivePresentation == pres)
				{
					System.Windows.Application.Current?.Dispatcher.InvokeAsync(CleanAndListen);
				}
			}
			catch (Exception ex)
			{
				Log.WriteDebug(ex.Message);
			}
		}

		private void CleanAndListen()
		{
			TryCleanComObject();
			SetTimerEnable(false);
		}

		private void TryCleanComObject()
		{
			Microsoft.Office.Interop.PowerPoint.Application doomed = _app;
			if (doomed == null)
			{
				return;
			}
			// 先把字段置空再逐个退订/释放：僵尸 COM 对象在 RemoveEventHandler 或
			// ReleaseComObject 时抛「对象不存在」(0x80048010)，旧逻辑在 finally 之前
			// 置 _app=null，异常一抛就永远执行不到，导致失效对象被永久当成"已绑定"，
			// 看门狗不切换、工具栏消失只能重启。这里用 finally 兜底保证字段一定清空。
			_app = null;
			try
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();
				// 逐个退订并单独吞异常：僵尸对象上任何一个 RemoveEventHandler 抛
				// 「对象不存在」都不应阻断其余退订和 Release。
				UnhookOne(doomed, "SlideShowBegin", new EApplication_SlideShowBeginEventHandler(AppSlideShowBegin));
				UnhookOne(doomed, "SlideShowEnd", new EApplication_SlideShowEndEventHandler(OnAppSlideShowEnd));
				UnhookOne(doomed, "SlideShowNextSlide", new EApplication_SlideShowNextSlideEventHandler(OnSlideShowNextSlide));
				UnhookOne(doomed, "PresentationCloseFinal", new EApplication_PresentationCloseFinalEventHandler(AppOnPresentationCloseFinal));
				try { Marshal.ReleaseComObject(doomed); } catch { }
				try { Marshal.ReleaseComObject(doomed); } catch { }
			}
			catch (Exception ex)
			{
				Log.WriteError("TryCleanComObject: " + ex.Message);
			}
			finally
			{
				// 无论退订/释放是否抛异常，都必须清空字段和放映态，
				// 否则看门狗会一直认为 COM 已绑定，永不切 ROT。
				_app = null;
				_slideShowWindow = null;
				ConfigHelper.Instance.IsInitApplicationSuccessful = false;
			}
		}

		private static void UnhookOne(Microsoft.Office.Interop.PowerPoint.Application app, string name, Delegate handler)
		{
			try
			{
				new ComAwareEventInfo(typeof(EApplication_Event), name).RemoveEventHandler(app, handler);
			}
			catch
			{
				// 僵尸对象退订必然抛异常，逐个吞掉以保证其余退订继续。
			}
		}

		private void AppSlideShowBegin(SlideShowWindow wn)
		{
			// COM 事件与启动轮询可能并发/重入；IsShowing 之前的准备工作也要挡住。
			// 自愈：若标记显示仍在放映、但当前已无放映窗口（上一场 ShowEnd 事件丢失，
			// 或初始化异常导致标记卡住），必须清掉，否则此后每场放映都会在这里静默 return，
			// 工具栏永久不出现且无任何日志。
			if (IsShowing && wn == null)
			{
				Log.WriteError("ShowBegin self-heal: IsShowing stuck true without live window, resetting.");
				IsShowing = false;
				ConfigHelper.Instance.IsShowing = false;
				_isShowTypeSpeaker = false;
			}
			if (IsShowing || _isHandlingShowBegin)
			{
				return;
			}
			_isHandlingShowBegin = true;
			try
			{
				// COM 事件与启动轮询可能同时命中同一场放映，避免重复触发 ShowBegin。
				if (wn == null || !ConfigHelper.Instance.IsNeedStart())
				{
					return;
				}
				_slideShowWindow = wn;
				if (!CheckMainObject())
				{
					return;
				}
				_isShowTypeSpeaker = PptShowType == PpSlideShowType.ppShowTypeSpeaker;
				ConfigHelper.Instance.IsPptShowTypeSpeaker = _isShowTypeSpeaker;
				if (_isShowTypeSpeaker)
				{
					ShowRangeType = Presentation.SlideShowSettings.RangeType;
					SlidesCount = Presentation.Slides.Count;
					PptFilePath = Presentation.FullName;
					Slides = Presentation.Slides.Cast<Slide>().ToList();
					PptAnalyzer.InitPptAnalyzer(PptFilePath);
					IsShowing = true;
					// 放映终于初始化成功：清零瞬时错误重试计数、停掉重试定时器。
					ResetShowBeginRetry();
					OnShowBegin();
					SetSlideAdvanceOnClickProperty(MsoTriState.msoFalse);
					ConfigHelper.Instance.IsShowing = IsShowing;
					SyntacticSugar.CurrentDispatcherBeginInvoke(() =>
					{
						_stopwatch.Restart();
					});
					ConfigHelper.Instance.IsComOk = true;
					Log.WriteDebug("PPTShowBegin");
				}
			}
			catch (Exception arg)
			{
				ConfigHelper.Instance.IsComOk = false;
				// 初始化失败必须把放映态收回，否则下一场放映会被误判为「已放映」。
				IsShowing = false;
				Log.WriteError($"ShowBeginError: {arg}");
				// 瞬时 COM 拒绝（PPT 忙，如读 HWND 时的 0x80010001）：隔 10s 重试一次，
				// 累计到上限仍失败则放弃，避免无限重试；任一次成功会清零计数（见成功分支）。
				// 这样偶发一次 RPC_E_CALL_REJECTED 不会再让整场放映的工具栏永久消失。
				if (SyntacticSugar.IsTransientComException(arg) && wn != null)
				{
					ScheduleShowBeginRetry(wn);
				}
			}
			finally
			{
				_isHandlingShowBegin = false;
			}
		}

		/// <summary>安排一次延时重试放映初始化；超过上限则放弃。DispatcherTimer 只能在 UI 线程操作。</summary>
		private void ScheduleShowBeginRetry(SlideShowWindow wn)
		{
			Dispatcher dispatcher = System.Windows.Application.Current != null
				? System.Windows.Application.Current.Dispatcher
				: Dispatcher.CurrentDispatcher;
			if (!dispatcher.CheckAccess())
			{
				dispatcher.BeginInvoke(new Action(() => ScheduleShowBeginRetry(wn)));
				return;
			}
			if (_showBeginRetryCount >= ShowBeginMaxRetryCount)
			{
				Log.WriteError("ShowBegin retry limit reached (" + ShowBeginMaxRetryCount + "), giving up this show.");
				return;
			}
			_showBeginRetryCount++;
			if (_showBeginRetryTimer == null)
			{
				_showBeginRetryTimer = new DispatcherTimer();
				_showBeginRetryTimer.Tick += (object s, EventArgs e) =>
				{
					_showBeginRetryTimer.Stop();
					SyntacticSugar.TryCatchAction(() => AppSlideShowBegin(wn));
				};
			}
			_showBeginRetryTimer.Interval = TimeSpan.FromSeconds(ShowBeginRetryIntervalSeconds);
			_showBeginRetryTimer.Start();
		}

		/// <summary>放映初始化成功后清零重试计数并停止重试定时器。</summary>
		private void ResetShowBeginRetry()
		{
			Dispatcher dispatcher = System.Windows.Application.Current != null
				? System.Windows.Application.Current.Dispatcher
				: Dispatcher.CurrentDispatcher;
			if (!dispatcher.CheckAccess())
			{
				dispatcher.BeginInvoke(new Action(ResetShowBeginRetry));
				return;
			}
			_showBeginRetryCount = 0;
			if (_showBeginRetryTimer != null)
			{
				_showBeginRetryTimer.Stop();
			}
		}

		private void OnAppSlideShowEnd(Presentation pres)
		{
			SetSlideAdvanceOnClickProperty(MsoTriState.msoTrue);
			SyntacticSugar.TryCatchAction(() =>
			{
				// IsShowing 必须无条件复位：只按 _isShowTypeSpeaker 判断会在放映类型
				// 误判时留下卡死的 true，导致后续放映全被 AppSlideShowBegin 静默挡掉。
				bool wasShowing = IsShowing;
				IsShowing = false;
				ConfigHelper.Instance.IsShowing = false;
				if (_isShowTypeSpeaker)
				{
					PptAnalyzer.StopAnalyzeAllAsyn();
					SyntacticSugar.CurrentDispatcherBeginInvoke(() =>
					{
						_stopwatch.Stop();
					});
					_isShowTypeSpeaker = false;
					OnShowEnd();
					Log.WriteDebug("PPTShowEnd");
				}
				else if (wasShowing)
				{
					// 非演讲者放映也要通知 UI 收起工具栏，避免状态残留。
					OnShowEnd();
					Log.WriteDebug("PPTShowEnd (non-speaker)");
				}
			});
		}

		private void OnSlideShowNextSlide(SlideShowWindow wn)
		{
			SyntacticSugar.TryCatchAction(() =>
			{
				if (_isShowTypeSpeaker)
				{
					SyntacticSugar.CurrentDispatcherBeginInvoke(() =>
					{
						_stopwatch.Restart();
					});
					OnNextSlide();
				}
			});
		}
	}
}
