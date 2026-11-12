using System;

namespace Cvte.Paint.Framework
{
	public abstract class PaintException : Exception
	{
		protected PaintException()
		{
		}

		protected PaintException(string message)
			: base(message)
		{
		}

		protected PaintException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
