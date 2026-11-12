using System;
using System.Windows;
using Cvte.ComponentModel;
using Cvte.Diagnostics.Annotations;
using Cvte.Documents;

namespace Cvte.Paint.Framework.Operations
{
	[Obsolete("无法说明这是给 Paint 提供的基类，请使用 ElementOperationBase 代替")]
	public abstract class OperationBase : NotifyOperation, IBoardOperation, IOperation
	{
		[CanBeNull]
		public string SlideId { get; protected set; }

		public Board Board { get; set; }

		protected OperationBase(string slideId)
		{
			SlideId = slideId;
		}

		protected OperationBase([NotNull] Slide slide)
			: this(slide.Id)
		{
		}

		protected OperationBase(Element element)
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

		protected Slide GetSlide()
		{
			if (SlideId == null)
			{
				throw new InvalidOperationException("调用GetSlide时，SlideId不能为空");
			}
			if (Board == null)
			{
				throw new InvalidOperationException("找不到 Board，请检查是否注入Board，是否使用 BoardUndoRedoProvider ");
			}
			return Board.GetSlideById(SlideId);
		}
	}
}
