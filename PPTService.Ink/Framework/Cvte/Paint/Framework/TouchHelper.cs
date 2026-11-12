using System;
using System.Windows.Input;

namespace Cvte.Paint.Framework
{
	public static class TouchHelper
	{
		private static readonly TouchPointCollection EmptyTouchPointCollection;

		private static TouchPointCollection _touchPointCollection;

		static TouchHelper()
		{
			Touch.FrameReported -= Touch_FrameReported;
			Touch.FrameReported += Touch_FrameReported;
			EmptyTouchPointCollection = new TouchPointCollection();
			_touchPointCollection = EmptyTouchPointCollection;
		}

		private static void Touch_FrameReported(object sender, TouchFrameEventArgs e)
		{
			_touchPointCollection = e.GetTouchPoints(null) ?? EmptyTouchPointCollection;
		}

		[Obsolete("请从你的控件事件获取触摸点集合，通过此方法获取的触摸点坐标预计不符合你预期")]
		public static TouchPointCollection GetTouchPoints()
		{
			return _touchPointCollection;
		}

		public static int GetTouchPointsCount()
		{
			return _touchPointCollection.Count;
		}
	}
}
