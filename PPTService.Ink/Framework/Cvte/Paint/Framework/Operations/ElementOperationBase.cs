using System;
using System.Windows;
using Cvte.ComponentModel;
using Cvte.Diagnostics.Annotations;
using Cvte.Documents;

namespace Cvte.Paint.Framework.Operations
{
	public abstract class ElementOperationBase : NotifyOperation, IInjectBoardOperation, IOperation, IElementOperationBase, ISlideOperationBase
	{
		[CanBeNull]
		public string SlideId { get; protected set; }

		Board IInjectBoardOperation.Board { get; set; }

		protected ElementOperationBase(string slideId)
		{
			SlideId = slideId;
		}

		protected ElementOperationBase([NotNull] Slide slide)
			: this(slide.Id)
		{
		}

		protected ElementOperationBase(Element element)
		{
			Slide slide = Slide.GetSlide(element);
			if (slide == null)
			{
				if (Window.GetWindow(element) == null && DebuggingProperties.IsDebug)
				{
					throw new ArgumentException("元素不在页面上，同时不是任何一个已知窗口上，是否创建对象的时机出错。");
				}
			}
			else
			{
				SlideId = slide.Id;
			}
		}

		protected abstract override void OnRedo();

		protected abstract override void OnUndo();

		public Slide GetSlide()
		{
			return this.GetOperationSlide();
		}
	}
}
