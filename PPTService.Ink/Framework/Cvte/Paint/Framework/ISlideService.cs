using System.Collections.Generic;

namespace Cvte.Paint.Framework
{
	public interface ISlideService
	{
		SlideCollection Slides { get; }

		void InsertSlides(List<Slide> slides);

		void InsertSlides(List<Slide> slides, int startIndex);

		void InsertSlides(List<Slide> slides, List<int> indexes);

		void InsertSlide(Slide slide);

		void InsertSlide(Slide slide, int index);

		void InsertAndGoToNewSlide(Slide slide);

		void DeleteSlide(Slide slide);

		void DeleteSlides(List<Slide> slides);

		void DeleteSlidesAt(List<int> indexes);

		void GoToNextSlide();

		void GoToPreviousSlide();

		void GoToSlide(int slideIndex);

		void GoToSlide(Slide slide);

		Slide GetSlideById(string slideId);

		void SortSlides(IList<int> sortList);
	}
}
