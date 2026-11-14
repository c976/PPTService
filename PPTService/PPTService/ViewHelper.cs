using System;
using PPTService.Core.Windows;

namespace PPTService
{
	public class ViewHelper
	{
		private static InnerBoardWindow _innerBoardWindow;

		public static Magnifier Magnifier { get; set; }

		public static LeftArea LeftArea { get; private set; }

		public static RightArea RightArea { get; private set; }

		public static BottomArea BottomArea { get; private set; }

		public static LeftBottomArea LeftBottomArea { get; private set; }

		public static RightBottomArea RightBottomArea { get; private set; }

		public static MainWindow MainWindow { get; private set; }

		public static InnerBoardWindow InnerBoardWindow
		{
			get
			{
				if (_innerBoardWindow != null)
				{
					return _innerBoardWindow;
				}
				_innerBoardWindow = new InnerBoardWindow
				{
					Owner = MainWindow
				};
				ControlHelper.SetAltF4Enabled(InnerBoardWindow, false);
				return _innerBoardWindow;
			}
		}

		/// <summary>放映结束时复位板中板笔色 / 粗细；窗口尚未创建则不主动创建（避免建一整套白板）。</summary>
		public static void ResetInnerBoardPenSettingsIfCreated()
		{
			if (_innerBoardWindow != null)
			{
				_innerBoardWindow.ResetPenSettings();
			}
		}

		public void Initialize()
		{
			PptOperator.Instance.ShowBegin += Instance_ShowBegin;
		}

		private void Instance_ShowBegin(object sender, EventArgs e)
		{
			SyntacticSugar.CurrentDispatcherInvoke(() =>
			{
				LoadingScreen<LoadingView>.Current = new LoadingScreen<LoadingView>();
				LeftArea = new LeftArea();
				RightArea = new RightArea();
				LeftBottomArea = new LeftBottomArea();
				RightBottomArea = new RightBottomArea();
				BottomArea = new BottomArea();
				MainWindow = new MainWindow();
				ControlHelper.SetAltF4Enabled(MainWindow, false);
			});
			MainWindow._pptOperatorShowBegin(null, null);
			PptOperator.Instance.ShowBegin -= Instance_ShowBegin;
		}

		public static void StopMainWindowUpdateTimerAndMouseGesture()
		{
			MainWindow.IsNeedUpdateTimer = false;
			MainWindow.UpdateTimer.Stop();
			MouseHelper.Instance.Stop();
		}

		public static void BeginMainWindowUpdateTimerAndMouseGesture()
		{
			MainWindow.IsNeedUpdateTimer = true;
			MainWindow.UpdateTimer.Start();
			MouseHelper.Instance.Start();
		}
	}
}
