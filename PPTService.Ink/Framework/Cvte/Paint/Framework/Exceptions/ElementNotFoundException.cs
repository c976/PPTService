using System;

namespace Cvte.Paint.Framework.Exceptions
{
	public class ElementNotFoundException : ArgumentException
	{
		public Slide CurrentSlide { get; }

		public ElementNotFoundException(Slide currentSlide)
		{
			CurrentSlide = currentSlide;
		}

		public ElementNotFoundException(string message, Slide currentSlide)
			: base(message)
		{
			CurrentSlide = currentSlide;
		}

		public ElementNotFoundException(string message, Exception innerException, Slide currentSlide)
			: base(message, innerException)
		{
			CurrentSlide = currentSlide;
		}

		public ElementNotFoundException(string message, string paramName, Exception innerException, Slide currentSlide)
			: base(message, paramName, innerException)
		{
			CurrentSlide = currentSlide;
		}

		public ElementNotFoundException(string message, string paramName, Slide currentSlide)
			: base(message, paramName)
		{
			CurrentSlide = currentSlide;
		}
	}
}
