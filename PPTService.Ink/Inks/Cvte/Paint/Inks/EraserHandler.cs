using System.Collections.Generic;
using System.Windows;
using Cvte.Documents;

namespace Cvte.Paint.Inks
{
	public class EraserHandler
	{
		public bool CanActivateEventHelper { get; set; } = true;

		public Size EraserSize { get; set; }

		public virtual string SlideId { get; }

		public virtual void OnErasingStarted()
		{
		}

		public virtual void OnErasingSectionStarted(Point point)
		{
		}

		public virtual void OnErasingSectionCompleted()
		{
		}

		public virtual IOperation OnErasingCompleted()
		{
			return null;
		}

		public virtual void OnPreviewErasingCompleted()
		{
		}

		public virtual void OnErasingCanceled()
		{
		}

		public virtual void EraseCore(IList<Point> points)
		{
		}
	}
}
