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
using PPTService.Collection;

namespace PPTService
{
	public class PptOperator
	{
		private int _times;

		private Microsoft.Office.Interop.PowerPoint.Application _app;

		private Stopwatch _stopwatch;

		private bool _isShowTypeSpeaker;

		private SlideShowWindow _slideShowWindow;

		private const int PptProcessCheckInterval = 2500;

		private static readonly int MinPageTurningTimerSpan = 30;

		private bool _isCheckPptStateFirstly = true;

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

		public static PptOperator Instance { get; set; }

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
				SyntacticSugar.TryCatchAction(() =>
				{
					type = _slideShowWindow?.View.PointerType ?? PpSlideShowPointerType.ppSlideShowPointerNone;
				});
				return type;
			}
			set
			{
				SyntacticSugar.TryCatchAction(() =>
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
				// 主窗口启用了关闭保护，程序内部主动退出时显式放行。
				ViewHelper.MainWindow?.AllowClose();
				System.Windows.Application.Current.Shutdown();
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
			SyntacticSugar.TryCatchAction(() =>
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
			SyntacticSugar.TryCatchAction(() =>
			{
				PointerType = PpSlideShowPointerType.ppSlideShowPointerPen;
				_slideShowWindow.View.PointerColor.RGB = color;
			});
		}

		/// <summary>
		/// 放映结束：只把 Office 原生画笔颜色复位为默认色，不改指针类型
		/// （SetPointerColor 会强制切到画笔，放映结束时调用会把鼠标态弄乱）。
		/// 放映窗口可能已经释放，COM 异常由 TryCatchAction 吞掉。
		/// </summary>
		public void ResetPointerColor()
		{
			PointerColor = PenSettings.DefaultColor;
			SyntacticSugar.TryCatchAction(() =>
			{
				if (_slideShowWindow != null)
				{
					Color c = PenSettings.DefaultColor;
					_slideShowWindow.View.PointerColor.RGB = (c.B << 16) | (c.G << 8) | c.R;
				}
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
			_checkPowerPointTimer.Interval = TimeSpan.FromMilliseconds(2500.0);
			_checkPowerPointTimer.Tick += CheckPowerPointProcess;
			_checkPowerPointTimer.Start();
			_clearEmptyPowerPointTimer.Interval = TimeSpan.FromMilliseconds(2500.0);
			_clearEmptyPowerPointTimer.Tick += ClearEmptyPowerPointProcess;
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
			if (ProcessHelper.HasPowerPointProcess())
			{
				try
				{
					CreatePowerPointApplication();
					InitializeControl();
					SetTimerEnable(true);
				}
				catch (Exception)
				{
					_app = null;
					ConfigHelper.Instance.IsInitApplicationSuccessful = false;
				}
			}
			_isCheckPptStateFirstly = false;
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
				if (ex.HResult == -2147023174)
				{
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
				_checkPowerPointTimer.Start();
			}
		}

		private void InitializeControl()
		{
			if (_app == null || IsShowing)
			{
				return;
			}
			SlideShowWindows slideShowWindows = _app.SlideShowWindows;
			if (slideShowWindows.Count <= 0)
			{
				return;
			}
			if (_isCheckPptStateFirstly)
			{
				try
				{
					if (_app.ActivePresentation.SlideShowSettings.ShowType != PpSlideShowType.ppShowTypeSpeaker)
					{
						AppSlideShowBegin(slideShowWindows[1]);
					}
					return;
				}
				catch (Exception arg)
				{
					Log.WriteError($"InitializeControl：{arg}");
					return;
				}
			}
			AppSlideShowBegin(slideShowWindows[1]);
		}

		private void CreatePowerPointApplication()
		{
			if (_app != null)
			{
				new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowBegin").RemoveEventHandler(_app, new EApplication_SlideShowBeginEventHandler(AppSlideShowBegin));
				new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowEnd").RemoveEventHandler(_app, new EApplication_SlideShowEndEventHandler(OnAppSlideShowEnd));
				new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowNextSlide").RemoveEventHandler(_app, new EApplication_SlideShowNextSlideEventHandler(OnSlideShowNextSlide));
			}
			_app = (Microsoft.Office.Interop.PowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");
			PptVersionManager.UpdatePptApplicationInfo(_app.Path, _app.Version);
			new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowBegin").AddEventHandler(_app, new EApplication_SlideShowBeginEventHandler(AppSlideShowBegin));
			new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowEnd").AddEventHandler(_app, new EApplication_SlideShowEndEventHandler(OnAppSlideShowEnd));
			new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowNextSlide").AddEventHandler(_app, new EApplication_SlideShowNextSlideEventHandler(OnSlideShowNextSlide));
			new ComAwareEventInfo(typeof(EApplication_Event), "PresentationCloseFinal").AddEventHandler(_app, new EApplication_PresentationCloseFinalEventHandler(AppOnPresentationCloseFinal));
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
			try
			{
				if (_app != null)
				{
					GC.Collect();
					GC.WaitForPendingFinalizers();
					new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowBegin").RemoveEventHandler(_app, new EApplication_SlideShowBeginEventHandler(AppSlideShowBegin));
					new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowEnd").RemoveEventHandler(_app, new EApplication_SlideShowEndEventHandler(OnAppSlideShowEnd));
					new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowNextSlide").RemoveEventHandler(_app, new EApplication_SlideShowNextSlideEventHandler(OnSlideShowNextSlide));
					Marshal.ReleaseComObject(_app);
					Marshal.ReleaseComObject(_app);
					_app = null;
				}
			}
			catch (Exception ex)
			{
				Log.WriteError(ex.Message);
			}
		}

		private void AppSlideShowBegin(SlideShowWindow wn)
		{
			try
			{
				if (!ConfigHelper.Instance.IsNeedStart())
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
				Log.WriteError($"ShowBeginError：{arg}");
			}
		}

		private void OnAppSlideShowEnd(Presentation pres)
		{
			SetSlideAdvanceOnClickProperty(MsoTriState.msoTrue);
			SyntacticSugar.TryCatchAction(() =>
			{
				if (_isShowTypeSpeaker)
				{
					IsShowing = false;
					PptAnalyzer.StopAnalyzeAllAsyn();
					SyntacticSugar.CurrentDispatcherBeginInvoke(() =>
					{
						_stopwatch.Stop();
					});
					ConfigHelper.Instance.IsShowing = IsShowing;
					_isShowTypeSpeaker = false;
					OnShowEnd();
					Log.WriteDebug("PPTShowEnd");
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
