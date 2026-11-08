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

		public bool IsShowing { get; set; }

		public Color PointerColor { get; set; }

		public int SlidesCount { get; private set; }

		public string PptFilePath { get; private set; }

		public List<Slide> Slides { get; private set; }

		public static PptOperator Instance { get; set; }

		public Presentation Presentation { get; private set; }

		public IntPtr SlideShowWindowIntPtr { get; private set; }

		public PpSlideShowRangeType ShowRangeType { get; private set; }

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

		public int EndingSlide => Presentation.SlideShowSettings.EndingSlide;

		public int StartingSlide => Presentation.SlideShowSettings.StartingSlide;

		public PpSlideShowType? PptShowType => Presentation.SlideShowSettings.ShowType;

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
			new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowNextSlide").RemoveEventHandler(_app, new EApplication_SlideShowNextSlideEventHandler(OnSlideShowNextSlide));
			_slideShowWindow.View.GotoSlide(index, MsoTriState.msoFalse);
			new ComAwareEventInfo(typeof(EApplication_Event), "SlideShowNextSlide").AddEventHandler(_app, new EApplication_SlideShowNextSlideEventHandler(OnSlideShowNextSlide));
			_slideShowWindow.Activate();
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
