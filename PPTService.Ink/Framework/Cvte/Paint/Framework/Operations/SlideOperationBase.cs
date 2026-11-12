using System;
using Cvte.Diagnostics.Annotations;
using Cvte.Documents;

namespace Cvte.Paint.Framework.Operations
{
	public abstract class SlideOperationBase : NotifyOperation, IInjectBoardOperation, IOperation, ISlideOperationBase
	{
		private string _slideId;

		public string SlideId
		{
			get
			{
				return _slideId;
			}
			protected set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				_slideId = value;
			}
		}

		Board IInjectBoardOperation.Board { get; set; }

		protected SlideOperationBase(string slideId)
		{
			SlideId = slideId;
		}

		protected abstract override void OnRedo();

		protected abstract override void OnUndo();

		protected SlideOperationBase([NotNull] Slide slide)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			SlideId = slide.Id;
		}

		public Slide GetSlide()
		{
			return this.GetOperationSlide();
		}

		protected Board GetBoard()
		{
			return this.GetOperationBoard();
		}
	}
}
