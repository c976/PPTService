using System;
using System.Collections.Generic;
using System.Windows.Media;
using Microsoft.Office.Interop.PowerPoint;

namespace PPTServiceUltra
{
	/// <summary>
	/// 演示程序调用通道的公共契约。COM 通道与 ROT 兼容通道各自独立实现，
	/// PptOperator 只负责按用户选择把 UI 调用转发给当前通道。
	/// </summary>
	internal interface IPptDriver
	{
		event EventHandler ShowBegin;
		event EventHandler ShowEnd;
		event EventHandler NextSlide;

		bool IsShowing { get; set; }
		IntPtr SlideShowWindowIntPtr { get; }
		int SlidesCount { get; }
		string PptFilePath { get; }
		List<Slide> Slides { get; }
		PpSlideShowRangeType ShowRangeType { get; }
		int StartingSlide { get; }
		int EndingSlide { get; }
		PpSlideShowType? PptShowType { get; }
		int CurrentShowPosition { get; }
		Color PointerColor { get; set; }
		PpSlideShowPointerType PointerType { get; set; }

		void Initialize();
		void Stop();
		void NextPage();
		void PreviousPage();
		void Exit();
		void GotoSlide(int index);
		void SetPointerColor(int color);
		void UpdatePointerColor();
		void ResetPointerColor();
		void SetDebugSlideWindowHandle(IntPtr hwnd);
	}
}
