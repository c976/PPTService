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

		/// <summary>
		/// 把指定页导出为缩略图。让各通道用自己的 COM 对象后期绑定完成，
		/// 避免 ROT 对象无法强转成早期绑定 Slide 接口时导出失败。
		/// 返回成功与否；不支持的通道可返回 false 让调用方走原路径。
		/// </summary>
		bool ExportThumbnail(int slideIndex, string filePath, int width, int height);
	}
}
