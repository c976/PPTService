using System;
using System.Collections.Generic;
using System.Windows;

namespace Cvte.Paint.Framework.SelectionBoxs
{
	public interface ISelectionOperator : ISlideLayeredOperator
	{
		IList<Element> SelectedElements { get; }

		bool IsTransforming { get; }

		ISelectionBox Box { get; }

		UIElement Attachment { get; set; }

		event EventHandler<EventArgs> SelectionUpdated;

		void NotifyTransformStarting(TransformRoutedEventArgs args);

		void NotifyTransformStarted(TransformRoutedEventArgs args);

		void NotifyTransformCompleted(TransformRoutedEventArgs args);

		void OnSelectionUpdated();
	}
}
