using System;
using System.Collections.Generic;
using Cvte.ComponentModel;
using Cvte.Documents;
using Linq;

namespace Cvte.Paint.Framework.Operations
{
	public class ElementCombinationOperation : CombinationOperation, IElementOperationBase, ISlideOperationBase, IInjectBoardOperation, IOperation
	{
		Board IInjectBoardOperation.Board { get; set; }

		public string SlideId { get; }

		public ElementCombinationOperation(IList<IOperation> operations, string slideId)
			: base(operations)
		{
			SlideId = slideId;
			VerifyOperationList(operations);
		}

		public ElementCombinationOperation(Slide slide, IList<IOperation> operations)
			: base(operations)
		{
			SlideId = slide.Id;
			VerifyOperationList(operations);
		}

		public ElementCombinationOperation(string slideId, IList<IOperation> operations)
			: base(operations)
		{
			SlideId = slideId;
			VerifyOperationList(operations);
		}

		public ElementCombinationOperation(string slideId, params IOperation[] operations)
			: base(operations)
		{
			SlideId = slideId;
			VerifyOperationList(operations);
		}

		public ElementCombinationOperation(Slide slide, params IOperation[] operations)
			: base(operations)
		{
			SlideId = slide.Id;
			VerifyOperationList(operations);
		}

		private void VerifyOperationList(IEnumerable<IOperation> operations)
		{
			if (DebuggingProperties.IsDebug && operations.AllAny((IOperation operation) => operation is SlideOperationBase))
			{
				throw new ArgumentException("传入的撤销重做全部是页面撤销重做，请将页面撤销重做放在 SlidesCombinationOperation 类");
			}
		}

		public Slide GetSlide()
		{
			return this.GetOperationSlide();
		}
	}
}
