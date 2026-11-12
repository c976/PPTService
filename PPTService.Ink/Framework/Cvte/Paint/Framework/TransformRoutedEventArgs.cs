using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Cvte.Diagnostics.Annotations;
using Cvte.Linq;

namespace Cvte.Paint.Framework
{
	public class TransformRoutedEventArgs : RoutedEventArgs
	{
		private readonly List<Element> _suppressedElements = new List<Element>();

		public Slide Slide { get; }

		public Element[] TransformingElements { get; internal set; }

		public Element[] PrimaryElements { get; internal set; }

		public TransformBehavior PrimaryBehavior { get; internal set; }

		public TransformBehavior CurrentBehavior { get; set; }

		public IReadOnlyCollection<Element> ElementsThatOperationHasSuppressed { get; }

		[Obsolete("单词拼写错误，请使用 IsCanceled 属性替代。", true)]
		public bool IsCancled
		{
			get
			{
				return IsCanceled;
			}
			set
			{
				IsCanceled = value;
			}
		}

		[Obsolete("这并不是 Cvte.Paint.Framework 中的原生属性，因此众多核心功能并不支持它，如果不是需要处理网络冲突请求，请不要使用它。")]
		public bool IsCanceled { get; set; }

		[Obsolete("这并不是 Cvte.Paint.Framework 中的原生属性，因此众多核心功能并不支持它，如果不是需要处理网络冲突请求，请不要使用它。")]
		public CancelReason CancelReason { get; set; }

		public TransformRoutedEventArgs(RoutedEvent routedEvent, Slide slide, IEnumerable<Element> elements)
			: this(routedEvent, slide, elements, TransformBehavior.Manual, TransformBehavior.Manual)
		{
		}

		public TransformRoutedEventArgs(RoutedEvent routedEvent, Slide slide, IEnumerable<Element> elements, TransformBehavior primaryBehavior, TransformBehavior currentBehavior)
			: base(routedEvent)
		{
			Slide = slide;
			TransformingElements = (elements as Element[]) ?? elements.ToArray();
			PrimaryElements = TransformingElements;
			PrimaryBehavior = primaryBehavior;
			CurrentBehavior = currentBehavior;
			ElementsThatOperationHasSuppressed = new ReadOnlyCollection<Element>(_suppressedElements);
		}

		public void SuppressOperation([NotNull] Element element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			if (!_suppressedElements.Contains(element))
			{
				_suppressedElements.Add(element);
			}
		}

		public void SuppressOperation([NotNull] IEnumerable<Element> elements)
		{
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			elements.ForEach(SuppressOperation);
		}
	}
}
