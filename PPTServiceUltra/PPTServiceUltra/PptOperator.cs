using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Office.Interop.PowerPoint;

namespace PPTServiceUltra
{
	/// <summary>
	/// 演示程序调用门面：自身不实现任何具体联动逻辑，按用户选择的调用方式把
	/// 属性 / 方法 / 事件转发给当前通道。两套通道彼此独立：
	/// - ComPptDriver：注册表 COM（GetActiveObject + 事件）。
	/// - RotPptDriver：ROT 枚举 + 后期绑定（兼容 COM 注册损坏 / WPS 多开）。
	/// "自动"模式优先 COM，绑定不到 Application 时回退到 ROT。
	/// </summary>
	public class PptOperator : IPptDriver
	{
		private readonly ComPptDriver _com = new ComPptDriver();
		private RotPptDriver _rot;
		private IPptDriver _active;
		private string _mode = UserConfig.InvokeModeAuto;
		private bool _initialized;
		private DispatcherTimer _fallbackTimer;
		private DateTime _comBindDeadline;

		// /debug 下完全不启动任何 COM/ROT 通道，只维持模拟放映状态。
		private int _debugPage = 1;
		private bool _debugShowing;

		public static PptOperator Instance { get; set; }

		private IPptDriver Active
		{
			get { return _active ?? _com; }
		}

		public event EventHandler ShowBegin;
		public event EventHandler ShowEnd;
		public event EventHandler NextSlide;

		public bool IsShowing
		{
			get { return DebugMode.IsEnabled ? _debugShowing : Active.IsShowing; }
			set
			{
				if (DebugMode.IsEnabled)
				{
					_debugShowing = value;
				}
				else
				{
					Active.IsShowing = value;
				}
			}
		}

		public IntPtr SlideShowWindowIntPtr
		{
			get { return Active.SlideShowWindowIntPtr; }
		}

		public int SlidesCount
		{
			get { return DebugMode.IsEnabled ? DebugMode.SlidesCount : Active.SlidesCount; }
		}

		public string PptFilePath
		{
			get { return DebugMode.IsEnabled ? "debug://simulated-slideshow" : Active.PptFilePath; }
		}

		public List<Slide> Slides
		{
			get { return Active.Slides; }
		}

		public Presentation Presentation { get; private set; }

		public PpSlideShowRangeType ShowRangeType
		{
			get { return DebugMode.IsEnabled ? PpSlideShowRangeType.ppShowAll : Active.ShowRangeType; }
		}

		public int StartingSlide
		{
			get { return DebugMode.IsEnabled ? 1 : Active.StartingSlide; }
		}

		public int EndingSlide
		{
			get { return DebugMode.IsEnabled ? DebugMode.SlidesCount : Active.EndingSlide; }
		}

		public PpSlideShowType? PptShowType
		{
			get { return DebugMode.IsEnabled ? PpSlideShowType.ppShowTypeSpeaker : Active.PptShowType; }
		}

		public int CurrentShowPosition
		{
			get { return DebugMode.IsEnabled ? _debugPage : Active.CurrentShowPosition; }
		}

		public Color PointerColor
		{
			get { return Active.PointerColor; }
			set { Active.PointerColor = value; }
		}

		public PpSlideShowPointerType PointerType
		{
			get { return Active.PointerType; }
			set { Active.PointerType = value; }
		}

		private void OnShowBegin(object sender, EventArgs e)
		{
			ShowBegin?.Invoke(this, e);
		}

		private void OnShowEnd(object sender, EventArgs e)
		{
			ShowEnd?.Invoke(this, e);
		}

		private void OnNextSlide(object sender, EventArgs e)
		{
			NextSlide?.Invoke(this, e);
		}

		private void Hook(IPptDriver driver)
		{
			driver.ShowBegin += OnShowBegin;
			driver.ShowEnd += OnShowEnd;
			driver.NextSlide += OnNextSlide;
		}

		private void Unhook(IPptDriver driver)
		{
			driver.ShowBegin -= OnShowBegin;
			driver.ShowEnd -= OnShowEnd;
			driver.NextSlide -= OnNextSlide;
		}

		/// <summary>按配置选择通道；重复调用时先停掉旧通道，避免两条通道同时驱动界面。</summary>
		public void ApplyInvokeMode(string mode)
		{
			string normalized = UserConfig.NormalizeInvokeMode(mode);
			if (_mode == normalized && _initialized)
			{
				return;
			}
			_mode = normalized;
			if (!_initialized || DebugMode.IsEnabled)
			{
				return;
			}
			// 放映中改调用方式不打断当前通道；配置已落盘，下一场放映/下次启动生效。
			if (IsShowing)
			{
				StopFallbackWatchdog();
				return;
			}
			SwitchChannel(ResolveChannel());
			StartFallbackWatchdog();
		}

		private string ResolveChannel()
		{
			// 自动模式先走 COM；绑定不到由看门狗在宽限期后切 ROT。
			return _mode == UserConfig.InvokeModeRot ? UserConfig.InvokeModeRot : UserConfig.InvokeModeCom;
		}

		private void SwitchChannel(string channel)
		{
			if (_active != null)
			{
				Unhook(_active);
				_active.Stop();
				_active = null;
			}
			if (channel == UserConfig.InvokeModeRot)
			{
				if (_rot == null)
				{
					_rot = new RotPptDriver();
				}
				Hook(_rot);
				_active = _rot;
			}
			else
			{
				Hook(_com);
				_active = _com;
				_comBindDeadline = DateTime.Now.AddSeconds(8.0);
			}
			_active.Initialize();
		}

		/// <summary>
		/// 自动模式看门狗：给 COM 一段绑定宽限期，超时仍拿不到 Application 就切 ROT；
		/// 已经切过去后不再来回抖。只在没有放映时切换。
		/// </summary>
		private void StartFallbackWatchdog()
		{
			StopFallbackWatchdog();
			if (_mode != UserConfig.InvokeModeAuto || DebugMode.IsEnabled)
			{
				return;
			}
			_fallbackTimer = new DispatcherTimer();
			_fallbackTimer.Interval = TimeSpan.FromMilliseconds(1000.0);
			_fallbackTimer.Tick += (object sender, EventArgs e) => FallbackToRotIfNeeded();
			_fallbackTimer.Start();
		}

		private void StopFallbackWatchdog()
		{
			if (_fallbackTimer != null)
			{
				_fallbackTimer.Stop();
				_fallbackTimer = null;
			}
		}

		public void Initialize()
		{
			_initialized = true;
			if (DebugMode.IsEnabled)
			{
				// 调试模式不接触 COM / ROT，直接置为放映中，延后触发让 ViewHelper 完成订阅。
				_debugShowing = true;
				ConfigHelper.Instance.IsShowing = true;
				ConfigHelper.Instance.IsPptShowTypeSpeaker = true;
				ConfigHelper.Instance.IsInitApplicationSuccessful = true;
				ConfigHelper.Instance.IsComOk = false;
				System.Windows.Application.Current.Dispatcher.BeginInvoke(
					new Action(() => OnShowBegin(this, EventArgs.Empty)), DispatcherPriority.Background);
				return;
			}
			SwitchChannel(ResolveChannel());
			StartFallbackWatchdog();
		}

		/// <summary>停止当前通道；同时在自动模式下，COM 绑定失败时切到 ROT 再试一次。</summary>
		public void Stop()
		{
			StopFallbackWatchdog();
			if (_active != null)
			{
				Unhook(_active);
				_active.Stop();
				_active = null;
			}
			_com.Stop();
			if (_rot != null)
			{
				_rot.Stop();
			}
			_initialized = false;
			_debugShowing = false;
		}

		/// <summary>
		/// COM 通道在自动模式下绑定失败时由调度层调用：切到 ROT 通道重试。
		/// 只在尚未进入放映时切换，避免放映中途更换驱动导致工具栏状态错乱。
		/// </summary>
		private void FallbackToRotIfNeeded()
		{
			if (_mode != UserConfig.InvokeModeAuto || DebugMode.IsEnabled || _active != _com)
			{
				return;
			}
			// 两种 COM 损坏表现都要回退：① 根本绑定不到 Application；
			// ② 能绑定并收到放映事件，但 HWND 为 0（本机现象），此时工具栏无法定位/调用。
			bool bindFailed = !_com.IsApplicationBound && DateTime.Now >= _comBindDeadline;
			bool hwndMissing = _com.IsShowingWithoutWindow;
			if (!bindFailed && !hwndMissing)
			{
				return;
			}
			Log.WriteDebug("PPT COM unavailable (" + (hwndMissing ? "show HWND is zero" : "bind timeout") + "), switching to ROT channel.");
			SwitchChannel(UserConfig.InvokeModeRot);
		}

		public void NextPage()
		{
			if (DebugMode.IsEnabled)
			{
				_debugPage = Math.Min(DebugMode.SlidesCount, _debugPage + 1);
				OnNextSlide(this, EventArgs.Empty);
				return;
			}
			Active.NextPage();
		}

		public void PreviousPage()
		{
			if (DebugMode.IsEnabled)
			{
				_debugPage = Math.Max(1, _debugPage - 1);
				OnNextSlide(this, EventArgs.Empty);
				return;
			}
			Active.PreviousPage();
		}

		public void Exit()
		{
			if (DebugMode.IsEnabled)
			{
				_debugShowing = false;
				IsShowing = false;
				Environment.Exit(0);
				return;
			}
			Active.Exit();
		}

		public void GotoSlide(int index)
		{
			if (DebugMode.IsEnabled)
			{
				_debugPage = Math.Max(1, Math.Min(DebugMode.SlidesCount, index));
				OnNextSlide(this, EventArgs.Empty);
				return;
			}
			Active.GotoSlide(index);
		}

		public void SetPointerColor(int color)
		{
			Active.SetPointerColor(color);
		}

		public void UpdatePointerColor()
		{
			Active.UpdatePointerColor();
		}

		public void ResetPointerColor()
		{
			Active.ResetPointerColor();
		}

		public void SetDebugSlideWindowHandle(IntPtr hwnd)
		{
			_com.SetDebugSlideWindowHandle(hwnd);
		}

		/// <summary>缩略图导出交给当前通道；返回 false 时调用方回退到原 Slide 列表路径。</summary>
		public bool ExportThumbnail(int slideIndex, string filePath, int width, int height)
		{
			if (DebugMode.IsEnabled)
			{
				return false;
			}
			return Active.ExportThumbnail(slideIndex, filePath, width, height);
		}
	}
}
