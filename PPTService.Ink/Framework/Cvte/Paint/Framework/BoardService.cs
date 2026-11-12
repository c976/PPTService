using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Cvte.Diagnostics.Annotations;
using Cvte.Documents;
using Cvte.Paint.Framework.Settings;

namespace Cvte.Paint.Framework
{
	public abstract class BoardService : IBoardService, IElementsService, IElementsEventSource, ISlideService, ISlideEventSource, INotifyPropertyChanged
	{
		private Board _associatedBoard;

		public virtual Board Board
		{
			get
			{
				return _associatedBoard;
			}
			set
			{
				if (value != null)
				{
					_associatedBoard = value;
					OnAssociatedBoardChanged();
				}
			}
		}

		public BoardSettingsProvider BoardSettings => Board.BoardSettings;

		public Mode CurrentMode
		{
			get
			{
				return Board.Mode;
			}
			set
			{
				Board.Mode = value;
			}
		}

		public Slide CurrentSlide => Board.CurrentSlide;

		public SlideCollection Slides => Board.Slides;

		public int SlideCount => Board.Slides.Count;

		public int CurrentSlideIndex => Board.Slides.CurrentIndex;

		public IUndoRedoProvider UndoRedo
		{
			get
			{
				return Board.UndoRedoProvider;
			}
			set
			{
				Board.UndoRedoProvider = value;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public event EventHandler AssociatedBoardChanged;

		public event EventHandler<SlidesEventArgs> SlidesInserting;

		public event EventHandler<SlidesEventArgs> SlidesInserted;

		public event EventHandler<SlidesEventArgs> SlidesDeleting;

		public event EventHandler<SlidesEventArgs> SlidesDeleted;

		public event EventHandler<SlideChangeEventArgs> SlideChanged;

		public event EventHandler<SlideEventArgs> SlideLoading;

		public event EventHandler<SlideEventArgs> SlideLoaded;

		public event EventHandler<EventArgs> BoardClearing;

		public event EventHandler<EventArgs> BoardCleared;

		public event EventHandler<ModeChangeEventArgs> ModeChanged;

		public event EventHandler<ModeChangeEventArgs> ModeChanging;

		public event EventHandler<CurrentSlideChangeEventArgs> CurrentSlideChanging;

		public event EventHandler<CurrentSlideChangeEventArgs> CurrentSlideChanged;

		public event EventHandler<ElementsEventArgs> ElementsAdded;

		public event EventHandler<ElementsEventArgs> ElementsRemoved;

		public event EventHandler<ElementsEventArgs> SelectionChanged;

		public event EventHandler<ElementsChangedEventArgs> ElementsChanged;

		public event EventHandler<ElementInnerSelectionChangedEventArgs> ElementInnerSelectionChanged;

		public event ElementGroupChangedEventHandler ElementGrouping;

		public event ElementGroupChangedEventHandler ElementGrouped;

		public event ElementGroupChangedEventHandler ElementUngrouping;

		public event ElementGroupChangedEventHandler ElementUngrouped;

		public event EventHandler UndoRedoEnableChanged;

		protected BoardService()
		{
			BoardServiceManager.AddBoardService(this);
		}

		public string AddResource(string resourcePath)
		{
			return resourcePath;
		}

		public void SortSlides(IList<int> sortList)
		{
			Board.SortSlides(sortList);
		}

		public void InsertSlides(List<Slide> slides)
		{
			InsertSlides(slides, SlideCount);
		}

		public void InsertSlides(List<Slide> slides, int startIndex)
		{
			List<int> list = new List<int>(slides.Count);
			for (int i = 0; i < slides.Count; i++)
			{
				list.Add(startIndex + i);
			}
			InsertSlides(slides, list);
		}

		public void InsertSlides(List<Slide> slides, List<int> indexes)
		{
			Board.InsertAt(slides, indexes);
		}

		public void DeleteSlides(List<Slide> slides)
		{
			Board.Delete(slides);
		}

		public void DeleteSlidesAt(List<int> indexes)
		{
			Board.DeleteAt(indexes);
		}

		public void InsertSlide(Slide slide)
		{
			Board.Insert(slide);
		}

		public void DeleteSlide(Slide slide)
		{
			Board.Delete(slide);
		}

		public void InsertSlide(Slide slide, int index)
		{
			Board.InsertAt(slide, index);
		}

		public void GoToNextSlide()
		{
			Board.GoToNextSlide();
		}

		public void GoToPreviousSlide()
		{
			Board.GoToPreviousSlide();
		}

		public void GoToSlide(int pageIndex)
		{
			Board.GoTo(pageIndex);
		}

		public void GoToSlide(Slide slide)
		{
			Board.GoTo(slide);
		}

		public void InsertAndGoToNewSlide(Slide slide)
		{
			InsertSlide(slide);
			GoToSlide(slide);
		}

		public Slide GetSlideById(string slideId)
		{
			return Board.GetSlideById(slideId);
		}

		public Slide GetSlideByIndex(int index)
		{
			if (index < 0 || index >= SlideCount)
			{
				return null;
			}
			return Board.GetSlide(index);
		}

		public void AddElement(Element element)
		{
			CurrentSlide.Add(element);
		}

		public void AddElements(IEnumerable<Element> elements)
		{
			CurrentSlide.Add(elements);
		}

		public void RemoveElement(Element element)
		{
			CurrentSlide.Remove(element);
		}

		public void RemoveElements(IEnumerable<Element> elements)
		{
			CurrentSlide.Remove(elements);
		}

		public void SelectElements(IEnumerable<Element> elements)
		{
			CurrentSlide.Select(elements);
		}

		public Element GetElementById(string id)
		{
			return CurrentSlide.GetElementById(id);
		}

		public void Unselect()
		{
			CurrentSlide.Unselect();
		}

		public bool ContainsElement(Element element)
		{
			return CurrentSlide.Elements.Contains(element);
		}

		public Element[] GetSelectedElements()
		{
			return CurrentSlide.GetSelectedElements().ToArray();
		}

		public ReadOnlyCollection<Element> GetReadOnlySelectedElements()
		{
			return CurrentSlide.GetSelectedElements();
		}

		[CanBeNull]
		public static BoardService GetBoardService([NotNull] DependencyObject element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			Slide slide = Slide.GetSlide(element);
			if (slide != null)
			{
				return GetBoardService(slide);
			}
			return null;
		}

		[CanBeNull]
		public static BoardService GetBoardService([NotNull] Slide slide)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			Board board = slide.Parent as Board;
			if (board != null)
			{
				return BoardServiceManager.GetBoardService(board);
			}
			return null;
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected virtual void OnAssociatedBoardChanged()
		{
			AssociatedBoardChanged?.Invoke(this, EventArgs.Empty);
		}

		public void RaiseUndoRedoEnableChanged()
		{
			UndoRedoEnableChanged?.Invoke(this, EventArgs.Empty);
		}

		public void RaiseElementsAdded(ElementsEventArgs e)
		{
			Raise(ElementsAdded, e);
		}

		public void RaiseElementsRemoved(ElementsEventArgs e)
		{
			Raise(ElementsRemoved, e);
		}

		public void RaiseSelectionChanged(ElementsEventArgs e)
		{
			Raise(SelectionChanged, e);
		}

		public void RaiseInnerSelectionChanged(ElementInnerSelectionChangedEventArgs e)
		{
			Raise(ElementInnerSelectionChanged, e);
		}

		public void RaiseElementsChanged(ElementsChangedEventArgs e)
		{
			Raise(ElementsChanged, e);
		}

		public void RaiseElementGrouping(ElementGroupChangedEventArgs e)
		{
			ElementGrouping?.Invoke(this, e);
		}

		public void RaiseElementGrouped(ElementGroupChangedEventArgs e)
		{
			ElementGrouped?.Invoke(this, e);
		}

		public void RaiseElementUngrouping(ElementGroupChangedEventArgs e)
		{
			ElementUngrouping?.Invoke(this, e);
		}

		public void RaiseElementUngrouped(ElementGroupChangedEventArgs e)
		{
			ElementUngrouped?.Invoke(this, e);
		}

		public void RaiseCurrentSlideChanging(CurrentSlideChangeEventArgs e)
		{
			Raise(CurrentSlideChanging, e);
		}

		public void RaiseCurrentSlideChanged(CurrentSlideChangeEventArgs e)
		{
			Raise(CurrentSlideChanged, e);
		}

		public void RaiseSlidesInserting(SlidesEventArgs e)
		{
			Raise(SlidesInserting, e);
		}

		public void RaiseSlidesInserted(SlidesEventArgs e)
		{
			Raise(SlidesInserted, e);
		}

		public void RaiseSlidesDeleting(SlidesEventArgs e)
		{
			Raise(SlidesDeleting, e);
		}

		public void RaiseSlidesDeleted(SlidesEventArgs e)
		{
			Raise(SlidesDeleted, e);
		}

		public void RaiseSlideChanged(SlideChangeEventArgs e)
		{
			Raise(SlideChanged, e);
		}

		public void RaiseSlideLoading(SlideEventArgs e)
		{
			Raise(SlideLoading, e);
		}

		public void RaiseSlideLoaded(SlideEventArgs e)
		{
			Raise(SlideLoaded, e);
		}

		public void RaiseBoardClearing(EventArgs e)
		{
			Raise(BoardClearing, e);
		}

		public void RaiseBoardCleared(EventArgs e)
		{
			Raise(BoardCleared, e);
		}

		public void RaiseBoardModeChanged(ModeChangeEventArgs e)
		{
			Raise(ModeChanged, e);
		}

		public void RaiseBoardModeChanging(ModeChangeEventArgs e)
		{
			Raise(ModeChanging, e);
		}

		private void Raise<T>(EventHandler<T> eventDelegate, T e) where T : EventArgs
		{
			eventDelegate?.Invoke(this, e);
		}
	}
}
