using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using PPTService.Ink.Core.Documents;
using PPTService.Paint.Framework.Settings;

namespace PPTService.Paint.Framework
{
	public interface IBoardService : IElementsService, IElementsEventSource, ISlideService, ISlideEventSource, INotifyPropertyChanged
	{
		Board Board { get; }

		Slide CurrentSlide { get; }

		int SlideCount { get; }

		Mode CurrentMode { get; set; }

		int CurrentSlideIndex { get; }

		BoardSettingsProvider BoardSettings { get; }

		IUndoRedoProvider UndoRedo { get; set; }

		event EventHandler AssociatedBoardChanged;

		event EventHandler UndoRedoEnableChanged;

		event EventHandler<ModeChangeEventArgs> ModeChanged;

		event EventHandler<ModeChangeEventArgs> ModeChanging;

		[Obsolete("此方法将会存在一次数组复制影响性能，建议使用 GetReadOnlySelectedElements代替")]
		Element[] GetSelectedElements();

		ReadOnlyCollection<Element> GetReadOnlySelectedElements();

		string AddResource(string resourcePath);
	}
}
