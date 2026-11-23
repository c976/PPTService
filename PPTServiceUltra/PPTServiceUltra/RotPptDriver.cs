using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;

namespace PPTServiceUltra
{
	/// <summary>
	/// 兼容调用通道：不依赖 Register/GetActiveObject 那套注册表绑定，
	/// 而是从 Running Object Table 里枚举正在运行的演示程序对象，
	/// 用后期绑定读写属性并驱动放映；放映窗口句柄在 COM 属性取不到时
	/// 退回按窗口标题匹配。用于 COM 注册损坏、或同时装了 WPS 与 Office
	/// 且未勾选兼容选项、导致常规 COM 通道拿不到放映窗口的环境。
	///
	/// 本文件为独立实现，与 COM 通道（ComPptDriver / 原 PptOperator 逻辑）
	/// 不共享状态；两条通道由 PptOperator 按用户选择调度。
	/// </summary>
	internal sealed class RotPptDriver : IPptDriver
	{
		/// <summary>候选应用的可绑定程度，值越大越接近"用户正在观看的放映"。</summary>
		private enum BindRank
		{
			Unusable = 0,
			HasDocument = 1,
			ShowWindowOpen = 2,
			ShowWindowActive = 3
		}

		private const int PollIntervalMs = 500;
		private const int BusyRetryDelayMs = 200;

		// ROT 中演示程序对象可能登记的自动化标识；大小写不敏感，按集合去重。
		private static readonly HashSet<string> AutomationIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"KWPP.Application", "Wpp.Application", "WPP.Application", "PowerPoint.Application"
		};

		// 注册表项不完整时用于识别应用对象的两个已知类标识。
		private static readonly HashSet<string> KnownAppClsids = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"!{44720441-94BF-4940-926D-4F38FECF2A48}",
			"!{91493441-5A91-11CF-8700-00AA0060263B}"
		};

		// 演示文稿文件的扩展名，按字母序排列便于核对。
		private static readonly HashSet<string> SlideDocumentSuffixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			".dps", ".dpt", ".pot", ".potm", ".potx", ".pps", ".ppsm", ".ppsx", ".ppt", ".pptm", ".pptx"
		};

		private object _app;
		private object _presentation;
		private object _slideShowWindow;
		private List<Slide> _slides = new List<Slide>();
		private IntPtr _hwnd;
		private int _slidesCount;
		private int _currentPage;
		private string _pptFilePath;
		private bool _isShowing;
		private bool _started;
		private bool _needRebind;
		private DateTime _lastActivity = DateTime.MinValue;
		private DispatcherTimer _pollTimer;

		public event EventHandler ShowBegin;
		public event EventHandler ShowEnd;
		public event EventHandler NextSlide;

		public bool IsShowing
		{
			get { return _isShowing; }
			set { _isShowing = value; }
		}

		public IntPtr SlideShowWindowIntPtr
		{
			get { return _hwnd; }
		}

		public int SlidesCount
		{
			get { return _slidesCount; }
		}

		public string PptFilePath
		{
			get { return _pptFilePath; }
		}

		public List<Slide> Slides
		{
			get { return _slides; }
		}

		public PpSlideShowRangeType ShowRangeType
		{
			get { return PpSlideShowRangeType.ppShowAll; }
		}

		public int StartingSlide
		{
			get { return 1; }
		}

		public int EndingSlide
		{
			get { return _slidesCount; }
		}

		public PpSlideShowType? PptShowType
		{
			get { return PpSlideShowType.ppShowTypeSpeaker; }
		}

		public int CurrentShowPosition
		{
			get { return _currentPage < 1 ? 1 : _currentPage; }
		}

		public Color PointerColor { get; set; }

		public PpSlideShowPointerType PointerType
		{
			get
			{
				object view = GetProperty(_slideShowWindow, "View");
				try
				{
					int value = Convert.ToInt32(GetProperty(view, "PointerType"), CultureInfo.InvariantCulture);
					return (PpSlideShowPointerType)value;
				}
				catch
				{
					return PpSlideShowPointerType.ppSlideShowPointerNone;
				}
				finally
				{
					SafeRelease(view);
				}
			}
			set
			{
				object view = GetProperty(_slideShowWindow, "View");
				try
				{
					SetProperty(view, "PointerType", (int)value);
				}
				catch (Exception ex)
				{
					Log.WriteDebug("RotPptDriver set PointerType failed: " + ex.Message);
				}
				finally
				{
					SafeRelease(view);
				}
			}
		}

		public void Initialize()
		{
			if (_started)
			{
				return;
			}
			_started = true;
			_pollTimer = new DispatcherTimer();
			_pollTimer.Interval = TimeSpan.FromMilliseconds(PollIntervalMs);
			_pollTimer.Tick += PollTick;
			_pollTimer.Start();
			Log.WriteDebug("RotPptDriver started");
			PollOnce();
		}

		public void Stop()
		{
			if (!_started)
			{
				return;
			}
			_started = false;
			if (_pollTimer != null)
			{
				_pollTimer.Stop();
				_pollTimer.Tick -= PollTick;
				_pollTimer = null;
			}
			RaiseShowEndIfNeeded();
			ReleaseBinding(true);
			Log.WriteDebug("RotPptDriver stopped");
		}

		private void PollTick(object sender, EventArgs e)
		{
			SyntacticSugar.TryCatchAction(PollOnce);
		}

		private void PollOnce()
		{
			if (!_started || DebugMode.IsEnabled)
			{
				return;
			}

			if (_app == null || _needRebind)
			{
				TryBindBestApplication();
				return;
			}

			// 当前演示文稿被换掉（打开其它文件或关闭）时先收尾，再重新绑定。
			object active = GetProperty(_app, "ActivePresentation");
			try
			{
				if (active == null || !AreSameComObject(active, _presentation))
				{
					RaiseShowEndIfNeeded();
					ReleaseBinding(false);
					_needRebind = true;
					return;
				}
			}
			finally
			{
				SafeRelease(active);
			}

			int count = GetSlideShowWindowCount(_app);
			if (count <= 0)
			{
				RaiseShowEndIfNeeded();
				_slideShowWindow = SafeReplace(_slideShowWindow, null);
				return;
			}

			object window = GetProperty(_presentation, "SlideShowWindow");
			if (window == null)
			{
				RaiseShowEndIfNeeded();
				return;
			}
			if (!AreSameComObject(window, _slideShowWindow))
			{
				SafeRelease(_slideShowWindow);
				_slideShowWindow = window;
				window = null;
			}
			else
			{
				SafeRelease(window);
			}

			int total = GetSlidesCount(_presentation);
			int page = GetCurrentPage(_slideShowWindow);
			if (total > 0)
			{
				_slidesCount = total;
			}
			if (page > 0)
			{
				if (_isShowing && page != _currentPage)
				{
					NextSlide?.Invoke(this, EventArgs.Empty);
				}
				_currentPage = page;
			}

			IntPtr hwnd = ResolveShowWindowHandle();
			if (hwnd != IntPtr.Zero)
			{
				_hwnd = hwnd;
			}

			if (!_isShowing)
			{
				BeginShow();
			}
			else
			{
				_lastActivity = DateTime.Now;
			}
		}

		private void BeginShow()
		{
			_pptFilePath = GetStringProperty(_presentation, "FullName");
			_slides = LoadSlides(_presentation);
			_slidesCount = GetSlidesCount(_presentation);
			_currentPage = GetCurrentPage(_slideShowWindow);
			_hwnd = ResolveShowWindowHandle();
			if (_slidesCount <= 0 || _hwnd == IntPtr.Zero)
			{
				// 页数或窗口都拿不到时先不认这场放映，下个 tick 再试，
				// 避免弹一个残缺的工具栏。
				Log.WriteDebug("RotPptDriver BeginShow deferred: pages=" + _slidesCount + " hwnd=" + _hwnd);
				return;
			}
			_isShowing = true;
			_lastActivity = DateTime.Now;
			ShowBegin?.Invoke(this, EventArgs.Empty);
			Log.WriteDebug("RotPptDriver ShowBegin pages=" + _slidesCount);
		}

		private void RaiseShowEndIfNeeded()
		{
			if (!_isShowing)
			{
				return;
			}
			_isShowing = false;
			_currentPage = 0;
			_hwnd = IntPtr.Zero;
			ShowEnd?.Invoke(this, EventArgs.Empty);
			Log.WriteDebug("RotPptDriver ShowEnd");
		}

		private void TryBindBestApplication()
		{
			BindRank bestRank;
			BindRank boundRank;
			object best = FindBestApplication(out bestRank, out boundRank);
			if (best == null)
			{
				return;
			}

			if (_app != null)
			{
				if (AreSameComObject(best, _app))
				{
					SafeRelease(best);
					_needRebind = false;
					return;
				}
				// 绑定对象相同优先级但 ROT 包装不同：保留旧绑定，避免无谓重建。
				if (bestRank <= boundRank && IsCurrentPresentationReadable())
				{
					SafeRelease(best);
					_needRebind = false;
					return;
				}
				RaiseShowEndIfNeeded();
				ReleaseBinding(false);
			}

			_app = best;
			_presentation = GetProperty(_app, "ActivePresentation");
			_needRebind = false;

			// 翻页按钮的默认布局取决于版本管理器里的 IsOffice2016OrLater（Office 2016+ 底部，
			// 其余两侧）。COM 通道绑定时会更新它，本通道也要更新，否则 Office 会错落到两侧。
			string appPath = GetStringProperty(_app, "Path");
			string appVersion = GetStringProperty(_app, "Version");
			if (!string.IsNullOrWhiteSpace(appPath))
			{
				Collection.PptVersionManager.UpdatePptApplicationInfo(appPath, appVersion);
			}
			Log.WriteDebug("RotPptDriver bound application, rank=" + bestRank);
		}

		private bool IsCurrentPresentationReadable()
		{
			try
			{
				return _presentation != null && GetStringProperty(_presentation, "FullName").Length > 0;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 枚举 ROT，挑优先级最高的演示程序对象：有演示文稿=1，有放映窗口=2，
		/// 放映窗口处于活动/前台=3。多开时按当前演示文稿区分，优先级低者不抢占。
		/// </summary>
		private object FindBestApplication(out BindRank bestRank, out BindRank boundRank)
		{
			bestRank = BindRank.Unusable;
			boundRank = BindRank.Unusable;
			IRunningObjectTable table = null;
			IEnumMoniker enumerator = null;
			object best = null;
			List<object> scanned = new List<object>();
			HashSet<string> appMonikers = BuildApplicationMonikers();

			try
			{
				if (GetRunningObjectTable(0, out table) != 0 || table == null)
				{
					return null;
				}
				table.EnumRunning(out enumerator);
				if (enumerator == null)
				{
					return null;
				}

				IMoniker[] batch = new IMoniker[1];
				IntPtr fetched = IntPtr.Zero;
				while (enumerator.Next(1, batch, fetched) == 0 && batch[0] != null)
				{
					IBindCtx context = null;
					object comObject = null;
					object candidate = null;
					bool ownedByList = false;
					string displayName = string.Empty;

					try
					{
						CreateBindCtx(0, out context);
						batch[0].GetDisplayName(context, null, out displayName);

						bool isApplication = appMonikers.Contains(displayName) ||
							KnownAppClsids.Contains(displayName);
						bool isPresentation = IsPresentationMoniker(displayName);
						if (!isApplication && !isPresentation)
						{
							continue;
						}

						table.GetObject(batch[0], out comObject);
						if (comObject == null)
						{
							continue;
						}

						if (isApplication)
						{
							candidate = comObject;
							comObject = null;
						}
						else
						{
							candidate = GetProperty(comObject, "Application");
						}
						if (candidate == null)
						{
							continue;
						}

						bool duplicate = false;
						foreach (object seen in scanned)
						{
							if (AreSameComObject(seen, candidate))
							{
								duplicate = true;
								break;
							}
						}
						if (duplicate)
						{
							continue;
						}
						scanned.Add(candidate);
						ownedByList = true;

						BindRank rank = RankApplication(candidate);
						if (_app != null && AreSameComObject(candidate, _app))
						{
							boundRank = rank;
						}
						if (rank > bestRank)
						{
							SafeRelease(best);
							best = candidate;
							candidate = null;
							bestRank = rank;
						}
					}
					catch (Exception ex)
					{
						Log.WriteDebug("RotPptDriver ROT entry skipped: " + ex.Message);
					}
					finally
					{
						if (!ownedByList)
						{
							SafeRelease(candidate);
						}
						SafeRelease(comObject);
						SafeRelease(context);
						SafeRelease(batch[0]);
					}
				}
			}
			catch (Exception ex)
			{
				Log.WriteError("RotPptDriver ROT scan failed: " + ex.Message);
			}
			finally
			{
				foreach (object seen in scanned)
				{
					if (!ReferenceEquals(seen, best))
					{
						SafeRelease(seen);
					}
				}
				scanned.Clear();
				SafeRelease(enumerator);
				SafeRelease(table);
			}
			return best;
		}

		private static HashSet<string> BuildApplicationMonikers()
		{
			HashSet<string> monikers = new HashSet<string>(KnownAppClsids, StringComparer.OrdinalIgnoreCase);
			foreach (string automationId in AutomationIds)
			{
				Guid clsid;
				if (CLSIDFromProgID(automationId, out clsid) == 0 && clsid != Guid.Empty)
				{
					monikers.Add("!" + clsid.ToString("B").ToUpperInvariant());
				}
			}
			return monikers;
		}

		private static bool IsPresentationMoniker(string displayName)
		{
			if (string.IsNullOrEmpty(displayName))
			{
				return false;
			}
			string lower = displayName.ToLowerInvariant();
			foreach (string suffix in SlideDocumentSuffixes)
			{
				if (lower.Contains(suffix))
				{
					return true;
				}
			}
			return false;
		}

		private BindRank RankApplication(object application)
		{
			if (application == null)
			{
				return BindRank.Unusable;
			}
			object presentation = GetProperty(application, "ActivePresentation");
			if (presentation == null)
			{
				return BindRank.Unusable;
			}
			try
			{
				object window = GetProperty(presentation, "SlideShowWindow");
				if (window != null)
				{
					try
					{
						object active = GetProperty(window, "Active");
						if (IsTriStateTrue(active) || IsWindowOnForeground(window))
						{
							return BindRank.ShowWindowActive;
						}
					}
					finally
					{
						SafeRelease(window);
					}
					return BindRank.ShowWindowOpen;
				}

				// 部分 WPS 版本不返回 SlideShowWindow 的属性，但 SlideShowWindows 有计数。
				object windows = GetProperty(application, "SlideShowWindows");
				try
				{
					int count = Convert.ToInt32(GetProperty(windows, "Count"), CultureInfo.InvariantCulture);
					return count > 0 ? BindRank.ShowWindowOpen : BindRank.HasDocument;
				}
				catch
				{
					return BindRank.HasDocument;
				}
				finally
				{
					SafeRelease(windows);
				}
			}
			catch
			{
				return BindRank.HasDocument;
			}
			finally
			{
				SafeRelease(presentation);
			}
		}

		private static bool IsTriStateTrue(object value)
		{
			if (value == null)
			{
				return false;
			}
			if (value is bool)
			{
				return (bool)value;
			}
			try
			{
				return Convert.ToInt32(value, CultureInfo.InvariantCulture) == -1;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 判断两个进程是否属于同一套 WPS 演示产品。WPS 的主程序与放映进程是分开的，
		/// 非全屏放映时前台焦点落在主程序上，需要把它们视为同一个应用。
		/// </summary>
		private static bool SameWpsSuite(string first, string second)
		{
			bool firstMain = first.StartsWith("wps", StringComparison.Ordinal);
			bool firstShow = first.StartsWith("wpp", StringComparison.Ordinal);
			bool secondMain = second.StartsWith("wps", StringComparison.Ordinal);
			bool secondShow = second.StartsWith("wpp", StringComparison.Ordinal);
			return (firstMain && secondShow) || (firstShow && secondMain);
		}

		/// <summary>
		/// 前台窗口是否属于这场放映：同进程直接成立；WPS 则放宽到同套产品进程。
		/// </summary>
		private static bool IsWindowOnForeground(object window)
		{
			IntPtr hwnd = ReadWindowHandle(window);
			if (hwnd == IntPtr.Zero)
			{
				return false;
			}
			IntPtr foreground = GetForegroundWindow();
			if (foreground == IntPtr.Zero)
			{
				return false;
			}
			uint foregroundPid;
			uint windowPid;
			GetWindowThreadProcessId(foreground, out foregroundPid);
			GetWindowThreadProcessId(hwnd, out windowPid);
			if (foregroundPid == 0 || windowPid == 0)
			{
				return false;
			}
			if (foregroundPid == windowPid)
			{
				return true;
			}
			try
			{
				using (Process front = Process.GetProcessById((int)foregroundPid))
				using (Process owner = Process.GetProcessById((int)windowPid))
				{
					return SameWpsSuite(front.ProcessName.ToLowerInvariant(),
						owner.ProcessName.ToLowerInvariant());
				}
			}
			catch
			{
				return false;
			}
		}

		private static int GetSlideShowWindowCount(object application)
		{
			object windows = GetProperty(application, "SlideShowWindows");
			try
			{
				return Convert.ToInt32(GetProperty(windows, "Count"), CultureInfo.InvariantCulture);
			}
			catch
			{
				return 0;
			}
			finally
			{
				SafeRelease(windows);
			}
		}

		private static int GetSlidesCount(object presentation)
		{
			object slides = GetProperty(presentation, "Slides");
			try
			{
				return Convert.ToInt32(GetProperty(slides, "Count"), CultureInfo.InvariantCulture);
			}
			catch
			{
				return 0;
			}
			finally
			{
				SafeRelease(slides);
			}
		}

		private static int GetCurrentPage(object window)
		{
			if (window == null)
			{
				return 0;
			}
			object view = GetProperty(window, "View");
			object slide = null;
			try
			{
				slide = GetProperty(view, "Slide");
				return Convert.ToInt32(GetProperty(slide, "SlideIndex"), CultureInfo.InvariantCulture);
			}
			catch
			{
				return 0;
			}
			finally
			{
				SafeRelease(slide);
				SafeRelease(view);
			}
		}

		private static List<Slide> LoadSlides(object presentation)
		{
			List<Slide> result = new List<Slide>();
			object slides = GetProperty(presentation, "Slides");
			if (slides == null)
			{
				return result;
			}
			try
			{
				int count = Convert.ToInt32(GetProperty(slides, "Count"), CultureInfo.InvariantCulture);
				// 不依赖 as Slides 早期绑定：ROT 取到的集合在混装环境强转会失败，
				// 用 1 基索引逐个取 Item，再转成 Slide 供缩略图导出使用。
				for (int index = 1; index <= count; index++)
				{
					object item = GetIndexedProperty(slides, "Item", index);
					if (item == null)
					{
						continue;
					}
					Slide slide = item as Slide;
					if (slide != null)
					{
						result.Add(slide);
					}
					else
					{
						SafeRelease(item);
					}
				}
			}
			catch (Exception ex)
			{
				Log.WriteDebug("RotPptDriver load slides failed: " + ex.Message);
			}
			finally
			{
				SafeRelease(slides);
			}
			return result;
		}

		/// <summary>读取带参数的索引属性（如 Slides.Item(index)），避免依赖早期绑定接口。</summary>
		private static object GetIndexedProperty(object target, string name, int index)
		{
			if (target == null)
			{
				return null;
			}
			try
			{
				return target.GetType().InvokeMember(name, BindingFlags.GetProperty, null, target,
					new object[] { index });
			}
			catch
			{
				return null;
			}
		}

		private IntPtr ResolveShowWindowHandle()
		{
			IntPtr hwnd = ReadWindowHandle(_slideShowWindow);
			if (hwnd != IntPtr.Zero)
			{
				return hwnd;
			}
			return FindWindowByTitle(GetStringProperty(_presentation, "FullName"),
				GetStringProperty(_app, "Name"));
		}

		private static IntPtr ReadWindowHandle(object window)
		{
			if (window == null)
			{
				return IntPtr.Zero;
			}
			try
			{
				SlideShowWindow typed = (SlideShowWindow)window;
				return new IntPtr(typed.HWND);
			}
			catch
			{
				return IntPtr.Zero;
			}
		}

		/// <summary>
		/// COM 的 HWND 属性被保护或返回 0 时的兜底：按「文件名 + 程序关键字」枚举顶层窗口，
		/// 只有唯一命中才返回，宁可不设置也不误绑到别的窗口。
		/// </summary>
		private static IntPtr FindWindowByTitle(string presentationFullName, string applicationName)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(presentationFullName) || string.IsNullOrWhiteSpace(applicationName))
				{
					return IntPtr.Zero;
				}
				string fileName = Path.GetFileName(presentationFullName);
				if (string.IsNullOrWhiteSpace(fileName))
				{
					return IntPtr.Zero;
				}
				string keyword = applicationName.IndexOf("WPS", StringComparison.OrdinalIgnoreCase) >= 0
					? "WPS"
					: (applicationName.IndexOf("PowerPoint", StringComparison.OrdinalIgnoreCase) >= 0
						? "PowerPoint"
						: string.Empty);
				if (keyword.Length == 0)
				{
					// WPS 老版本把 Application.Name 报成 Microsoft PowerPoint，两种关键字都试。
					keyword = "PowerPoint";
				}

				IntPtr candidate = IntPtr.Zero;
				bool conflicting = false;
				EnumWindows(delegate(IntPtr handle, IntPtr parameter)
				{
					try
					{
						if (!IsWindowVisible(handle))
						{
							return true;
						}
						int length = GetWindowTextLength(handle);
						if (length <= 0)
						{
							return true;
						}
						StringBuilder text = new StringBuilder(length + 1);
						GetWindowText(handle, text, text.Capacity);
						string title = text.ToString();
						if (title.IndexOf(fileName, StringComparison.OrdinalIgnoreCase) >= 0 &&
							(title.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
							 title.IndexOf("WPS", StringComparison.OrdinalIgnoreCase) >= 0))
						{
							if (candidate == IntPtr.Zero)
							{
								candidate = handle;
							}
							else
							{
								conflicting = true;
							}
						}
					}
					catch
					{
						// 单个窗口枚举失败不影响其它窗口。
					}
					return true;
				}, IntPtr.Zero);

				return conflicting ? IntPtr.Zero : candidate;
			}
			catch (Exception ex)
			{
				Log.WriteDebug("RotPptDriver window title match failed: " + ex.Message);
				return IntPtr.Zero;
			}
		}

		public void NextPage()
		{
			InvokeView("Next");
		}

		public void PreviousPage()
		{
			InvokeView("Previous");
		}

		public void Exit()
		{
			InvokeView("Exit");
		}

		public void GotoSlide(int index)
		{
			object view = GetProperty(_slideShowWindow, "View");
			try
			{
				view.GetType().InvokeMember("GotoSlide", BindingFlags.InvokeMethod, null, view,
					new object[] { index, (int)MsoTriState.msoFalse });
			}
			catch (Exception ex)
			{
				Log.WriteDebug("RotPptDriver GotoSlide failed: " + ex.Message);
			}
			finally
			{
				SafeRelease(view);
			}
		}

		public void SetPointerColor(int color)
		{
			PointerType = PpSlideShowPointerType.ppSlideShowPointerPen;
			object view = GetProperty(_slideShowWindow, "View");
			try
			{
				object pointerColor = GetProperty(view, "PointerColor");
				try
				{
					SetProperty(pointerColor, "RGB", color);
				}
				finally
				{
					SafeRelease(pointerColor);
				}
			}
			catch (Exception ex)
			{
				Log.WriteDebug("RotPptDriver SetPointerColor failed: " + ex.Message);
			}
			finally
			{
				SafeRelease(view);
			}
		}

		public void UpdatePointerColor()
		{
			object view = GetProperty(_slideShowWindow, "View");
			object pointerColor = null;
			try
			{
				pointerColor = GetProperty(view, "PointerColor");
				int rgb = Convert.ToInt32(GetProperty(pointerColor, "RGB"), CultureInfo.InvariantCulture);
				string text = (rgb & 0xFFFFFF).ToString("X6", CultureInfo.InvariantCulture);
				Color color = Colors.Red;
				color.R = Convert.ToByte(text.Substring(4, 2), 16);
				color.G = Convert.ToByte(text.Substring(2, 2), 16);
				color.B = Convert.ToByte(text.Substring(0, 2), 16);
				PointerColor = color;
			}
			catch
			{
				// 放映窗口销毁途中取不到颜色属正常情况。
			}
			finally
			{
				SafeRelease(pointerColor);
				SafeRelease(view);
			}
		}

		public void ResetPointerColor()
		{
			Color color = PenSettings.DefaultColor;
			PointerColor = color;
			SetPointerColor((color.B << 16) | (color.G << 8) | color.R);
		}

		public void SetDebugSlideWindowHandle(IntPtr hwnd)
		{
			// 兼容调用通道不参与 /debug 模拟放映。
		}

		private void InvokeView(string methodName)
		{
			object view = GetProperty(_slideShowWindow, "View");
			try
			{
				view.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, view, null);
			}
			catch (Exception ex)
			{
				if (IsBusyException(ex))
				{
					System.Threading.Thread.Sleep(BusyRetryDelayMs);
				}
				Log.WriteDebug("RotPptDriver " + methodName + " failed: " + ex.Message);
			}
			finally
			{
				SafeRelease(view);
			}
		}

		private static bool IsBusyException(Exception ex)
		{
			Exception current = ex;
			while (current != null)
			{
				if (SyntacticSugar.IsTransientComException(current))
				{
					return true;
				}
				current = current.InnerException;
			}
			return false;
		}

		private void ReleaseBinding(bool final)
		{
			if (final)
			{
				SafeFinalRelease(_slideShowWindow);
				SafeFinalRelease(_presentation);
				SafeFinalRelease(_app);
			}
			else
			{
				SafeRelease(_slideShowWindow);
				SafeRelease(_presentation);
				SafeRelease(_app);
			}
			_slideShowWindow = null;
			_presentation = null;
			_app = null;
			_slides = new List<Slide>();
			_hwnd = IntPtr.Zero;
			_slidesCount = 0;
			_currentPage = 0;
			_lastActivity = DateTime.MinValue;

			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}

		private static object SafeReplace(object oldValue, object newValue)
		{
			SafeRelease(oldValue);
			return newValue;
		}

		private static object GetProperty(object target, string name)
		{
			if (target == null)
			{
				return null;
			}
			try
			{
				return target.GetType().InvokeMember(name, BindingFlags.GetProperty, null, target, null);
			}
			catch
			{
				return null;
			}
		}

		private static void SetProperty(object target, string name, object value)
		{
			if (target == null)
			{
				return;
			}
			target.GetType().InvokeMember(name, BindingFlags.SetProperty, null, target, new[] { value });
		}

		private static string GetStringProperty(object target, string name)
		{
			object value = GetProperty(target, name);
			return value == null ? string.Empty : Convert.ToString(value, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// 比较两个 COM 包装是否指向同一个底层对象。取各自的规范 IUnknown 指针值比较，
		/// 指针用完立即释放，避免 RCW 引用计数泄漏导致演示程序进程残留。
		/// </summary>
		private static bool AreSameComObject(object left, object right)
		{
			if (ReferenceEquals(left, right))
			{
				return true;
			}
			if (left == null || right == null)
			{
				return false;
			}
			return IdentityOf(left) == IdentityOf(right);
		}

		private static long IdentityOf(object comObject)
		{
			IntPtr unknown = IntPtr.Zero;
			try
			{
				unknown = Marshal.GetIUnknownForObject(comObject);
				return unknown.ToInt64();
			}
			catch
			{
				return 0L;
			}
			finally
			{
				if (unknown != IntPtr.Zero)
				{
					Marshal.Release(unknown);
				}
			}
		}

		private static void SafeRelease(object value)
		{
			if (value == null || !Marshal.IsComObject(value))
			{
				return;
			}
			try
			{
				Marshal.ReleaseComObject(value);
			}
			catch
			{
				// 对象已被释放时忽略。
			}
		}

		private static void SafeFinalRelease(object value)
		{
			if (value == null || !Marshal.IsComObject(value))
			{
				return;
			}
			try
			{
				Marshal.FinalReleaseComObject(value);
			}
			catch
			{
				// 对象已被释放时忽略。
			}
		}

		private delegate bool WindowEnumerator(IntPtr handle, IntPtr parameter);

		[DllImport("ole32.dll")]
		private static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable table);

		[DllImport("ole32.dll")]
		private static extern int CreateBindCtx(int reserved, out IBindCtx context);

		[DllImport("ole32.dll", CharSet = CharSet.Unicode)]
		private static extern int CLSIDFromProgID(string progId, out Guid clsid);

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		private static extern uint GetWindowThreadProcessId(IntPtr handle, out uint processId);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool EnumWindows(WindowEnumerator callback, IntPtr parameter);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsWindowVisible(IntPtr handle);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		private static extern int GetWindowText(IntPtr handle, StringBuilder text, int maxCount);

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		private static extern int GetWindowTextLength(IntPtr handle);
	}
}
