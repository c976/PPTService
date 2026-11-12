using System;

namespace Cvte.Paint.Framework
{
	public interface ISlideEventSource
	{
		event EventHandler<CurrentSlideChangeEventArgs> CurrentSlideChanging;

		event EventHandler<CurrentSlideChangeEventArgs> CurrentSlideChanged;

		event EventHandler<SlidesEventArgs> SlidesInserting;

		event EventHandler<SlidesEventArgs> SlidesInserted;

		event EventHandler<SlidesEventArgs> SlidesDeleting;

		event EventHandler<SlidesEventArgs> SlidesDeleted;

		event EventHandler<SlideChangeEventArgs> SlideChanged;

		event EventHandler<SlideEventArgs> SlideLoading;

		event EventHandler<SlideEventArgs> SlideLoaded;

		event EventHandler<EventArgs> BoardClearing;

		event EventHandler<EventArgs> BoardCleared;
	}
}
