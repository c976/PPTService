using System;
using System.Windows;

namespace Cvte.Paint.Framework
{
	public class BoardEventHelper
	{
		private readonly UIElement _element;

		private readonly BoardService _boardService;

		public BoardEventHelper(UIElement element, BoardService boardService)
		{
			_element = element;
			_boardService = boardService;
		}

		public void Register()
		{
			_element.AddHandler(Board.SlidesInsertedEvent, new EventHandler<SlidesRoutedEventArgs>(Board_OnSlideInserted));
			_element.AddHandler(Board.SlidesDeletingEvent, new EventHandler<SlidesRoutedEventArgs>(Board_OnSlideDeleting));
			_element.AddHandler(Board.SlidesDeletedEvent, new EventHandler<SlidesRoutedEventArgs>(Board_OnSlideDeleted));
			_element.AddHandler(Board.CurrentSlideChangedEvent, new RoutedPropertyChangedEventHandler<Slide>(Board_OnCurrentSlideChanged));
			_element.AddHandler(Board.CurrentSlideChangingEvent, new RoutedPropertyChangedEventHandler<Slide>(Board_OnCurrentSlideChanging));
			_element.AddHandler(Board.UndoRedoEnableChangedEvent, new RoutedEventHandler(Board_OnUndoRedoChanged));
			_element.AddHandler(Board.BoardClearingEvent, new RoutedEventHandler(Board_Clearing));
			_element.AddHandler(Board.BoardClearedEvent, new RoutedEventHandler(Board_Cleared));
			_element.AddHandler(Slide.ElementsAddedEvent, new RoutedCollectionChangedEventHandler<Element>(Slide_OnElementsAdded));
			_element.AddHandler(Slide.ElementsRemovedEvent, new RoutedCollectionChangedEventHandler<Element>(Slide_OnElementsRemoved));
			_element.AddHandler(Slide.ElementsChangedEvent, new RoutedElementsChangedEventHandler(Slide_OnElementsChanged));
			_element.AddHandler(GroupEvents.GroupingEvent, new ElementGroupChangedRoutedEventHandler(OnElementGrouping));
			_element.AddHandler(GroupEvents.GroupedEvent, new ElementGroupChangedRoutedEventHandler(OnElementGrouped));
			_element.AddHandler(GroupEvents.UngroupingEvent, new ElementGroupChangedRoutedEventHandler(OnElementUngrouping));
			_element.AddHandler(GroupEvents.UngroupedEvent, new ElementGroupChangedRoutedEventHandler(OnElementUngrouped));
			_element.AddHandler(Slide.SlideChangedEvent, new EventHandler<SlideChangeRoutedEventArgs>(Slide_OnSlideChanged));
			_element.AddHandler(Slide.SlideLoadingEvent, new EventHandler<SlideRoutedEventArgs>(Slide_SlideLoading));
			_element.AddHandler(Slide.SlideLoadedEvent, new EventHandler<SlideRoutedEventArgs>(Slide_SlideLoaded));
			_element.AddHandler(Slide.SelectionChangedEvent, new RoutedCollectionChangedEventHandler<Element>(Slide_SelectionChanged));
			_element.AddHandler(Slide.InnerSelectionChangedEvent, new RoutedInnerCollectionChangedEventHandler<object>(Slide_InnerSelectionChanged));
			_element.AddHandler(Board.ModeChangedEvent, new RoutedPropertyChangedEventHandler<Mode>(Board_OnModeChanged));
			_element.AddHandler(Board.ModeChangingEvent, new RoutedPropertyChangedEventHandler<Mode>(Board_OnModeChanging));
		}

		public void UnRegister()
		{
			_element.RemoveHandler(Board.SlidesInsertedEvent, new EventHandler<SlidesRoutedEventArgs>(Board_OnSlideInserted));
			_element.RemoveHandler(Board.SlidesDeletingEvent, new EventHandler<SlidesRoutedEventArgs>(Board_OnSlideDeleting));
			_element.RemoveHandler(Board.SlidesDeletedEvent, new EventHandler<SlidesRoutedEventArgs>(Board_OnSlideDeleted));
			_element.RemoveHandler(Board.CurrentSlideChangedEvent, new RoutedPropertyChangedEventHandler<Slide>(Board_OnCurrentSlideChanged));
			_element.RemoveHandler(Board.CurrentSlideChangingEvent, new RoutedPropertyChangedEventHandler<Slide>(Board_OnCurrentSlideChanging));
			_element.RemoveHandler(Board.UndoRedoEnableChangedEvent, new RoutedEventHandler(Board_OnUndoRedoChanged));
			_element.RemoveHandler(Board.BoardClearingEvent, new RoutedEventHandler(Board_Clearing));
			_element.RemoveHandler(Board.BoardClearedEvent, new RoutedEventHandler(Board_Cleared));
			_element.RemoveHandler(Slide.ElementsAddedEvent, new RoutedCollectionChangedEventHandler<Element>(Slide_OnElementsAdded));
			_element.RemoveHandler(Slide.ElementsRemovedEvent, new RoutedCollectionChangedEventHandler<Element>(Slide_OnElementsRemoved));
			_element.RemoveHandler(Slide.ElementsChangedEvent, new RoutedElementsChangedEventHandler(Slide_OnElementsChanged));
			_element.RemoveHandler(GroupEvents.GroupingEvent, new ElementGroupChangedRoutedEventHandler(OnElementGrouping));
			_element.RemoveHandler(GroupEvents.GroupedEvent, new ElementGroupChangedRoutedEventHandler(OnElementGrouped));
			_element.RemoveHandler(GroupEvents.UngroupingEvent, new ElementGroupChangedRoutedEventHandler(OnElementUngrouping));
			_element.RemoveHandler(GroupEvents.UngroupedEvent, new ElementGroupChangedRoutedEventHandler(OnElementUngrouped));
			_element.RemoveHandler(Slide.SlideChangedEvent, new EventHandler<SlideChangeRoutedEventArgs>(Slide_OnSlideChanged));
			_element.RemoveHandler(Slide.SlideLoadingEvent, new EventHandler<SlideRoutedEventArgs>(Slide_SlideLoading));
			_element.RemoveHandler(Slide.SlideLoadedEvent, new EventHandler<SlideRoutedEventArgs>(Slide_SlideLoaded));
			_element.RemoveHandler(Slide.SelectionChangedEvent, new RoutedCollectionChangedEventHandler<Element>(Slide_SelectionChanged));
			_element.RemoveHandler(Slide.InnerSelectionChangedEvent, new RoutedInnerCollectionChangedEventHandler<object>(Slide_InnerSelectionChanged));
			_element.RemoveHandler(Board.ModeChangedEvent, new RoutedPropertyChangedEventHandler<Mode>(Board_OnModeChanged));
			_element.RemoveHandler(Board.ModeChangingEvent, new RoutedPropertyChangedEventHandler<Mode>(Board_OnModeChanging));
		}

		private void Board_OnSlideInserted(object sender, SlidesRoutedEventArgs e)
		{
			_boardService.RaiseSlidesInserted(new SlidesEventArgs(e.Slides));
		}

		private void Board_OnSlideDeleting(object sender, SlidesRoutedEventArgs e)
		{
			_boardService.RaiseSlidesDeleting(new SlidesEventArgs(e.Slides));
		}

		private void Board_OnSlideDeleted(object sender, SlidesRoutedEventArgs e)
		{
			_boardService.RaiseSlidesDeleted(new SlidesEventArgs(e.Slides));
		}

		private void Board_OnCurrentSlideChanging(object sender, RoutedPropertyChangedEventArgs<Slide> e)
		{
			_boardService.RaiseCurrentSlideChanging(new CurrentSlideChangeEventArgs(e.OldValue, e.NewValue));
		}

		private void Board_OnCurrentSlideChanged(object sender, RoutedPropertyChangedEventArgs<Slide> e)
		{
			_boardService.RaiseCurrentSlideChanged(new CurrentSlideChangeEventArgs(e.OldValue, e.NewValue));
		}

		private void Board_OnUndoRedoChanged(object sender, RoutedEventArgs e)
		{
			_boardService.RaiseUndoRedoEnableChanged();
		}

		private void Board_Clearing(object sender, RoutedEventArgs e)
		{
			_boardService.RaiseBoardClearing(e);
		}

		private void Board_Cleared(object sender, RoutedEventArgs e)
		{
			_boardService.RaiseBoardCleared(EventArgs.Empty);
		}

		private void Slide_OnElementsAdded(object sender, RoutedCollectionChangedEventArgs<Element> e)
		{
			_boardService.RaiseElementsAdded(new ElementsEventArgs(e.Items, (Slide)e.OriginalSource));
		}

		private void Slide_OnElementsRemoved(object sender, RoutedCollectionChangedEventArgs<Element> e)
		{
			_boardService.RaiseElementsRemoved(new ElementsEventArgs(e.Items, (Slide)e.OriginalSource));
		}

		private void Slide_OnElementsChanged(object sender, RoutedElementsChangedEventArgs e)
		{
			_boardService.RaiseElementsChanged(new ElementsChangedEventArgs(e.Items, (Slide)e.OriginalSource, e.IsFinalState));
		}

		private void OnElementGrouping(object sender, ElementGroupChangedRoutedEventArgs e)
		{
			_boardService.RaiseElementGrouping(ElementGroupChangedEventArgs.FromRoutedEventArgs(e));
		}

		private void OnElementGrouped(object sender, ElementGroupChangedRoutedEventArgs e)
		{
			_boardService.RaiseElementGrouped(ElementGroupChangedEventArgs.FromRoutedEventArgs(e));
		}

		private void OnElementUngrouping(object sender, ElementGroupChangedRoutedEventArgs e)
		{
			_boardService.RaiseElementUngrouping(ElementGroupChangedEventArgs.FromRoutedEventArgs(e));
		}

		private void OnElementUngrouped(object sender, ElementGroupChangedRoutedEventArgs e)
		{
			_boardService.RaiseElementUngrouped(ElementGroupChangedEventArgs.FromRoutedEventArgs(e));
		}

		private void Slide_OnSlideChanged(object sender, SlideChangeRoutedEventArgs e)
		{
			_boardService.RaiseSlideChanged(new SlideChangeEventArgs(e.Slide, e.Property));
		}

		private void Slide_SlideLoading(object sender, SlideRoutedEventArgs e)
		{
			_boardService.RaiseSlideLoading(new SlideEventArgs(e.Slide));
		}

		private void Slide_SlideLoaded(object sender, SlideRoutedEventArgs e)
		{
			_boardService.RaiseSlideLoaded(new SlideEventArgs(e.Slide));
		}

		private void Slide_SelectionChanged(object sender, RoutedCollectionChangedEventArgs<Element> e)
		{
			_boardService.RaiseSelectionChanged(new ElementsEventArgs(e.Items, (Slide)e.OriginalSource));
		}

		private void Slide_InnerSelectionChanged(object sender, RoutedInnerCollectionChangedEventArgs<object> e)
		{
			_boardService.RaiseInnerSelectionChanged(new ElementInnerSelectionChangedEventArgs(e.Root, e.Parent, e.Items));
		}

		private void Board_OnModeChanged(object sender, RoutedPropertyChangedEventArgs<Mode> e)
		{
			_boardService.RaiseBoardModeChanged(new ModeChangeEventArgs(e.OldValue, e.NewValue));
		}

		private void Board_OnModeChanging(object sender, RoutedPropertyChangedEventArgs<Mode> e)
		{
			_boardService.RaiseBoardModeChanging(new ModeChangeEventArgs(e.OldValue, e.NewValue));
		}
	}
}
