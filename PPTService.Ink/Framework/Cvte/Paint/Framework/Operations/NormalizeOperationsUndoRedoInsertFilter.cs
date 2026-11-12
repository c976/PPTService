using System;
using System.Linq;
using Cvte.Documents;

namespace Cvte.Paint.Framework.Operations
{
	internal class NormalizeOperationsUndoRedoInsertFilter : IBoardUndoRedoInsertFilter
	{
		public void FilterInsert(IOperation operation)
		{
			if (!CheckOperationNormative(operation))
			{
				throw new ArgumentException("传入的" + operation.GetType().FullName + "不符合规范，要求所有撤销重做必须是属于 ApplicationOperationBase BoardOperationBase SlideOperationBase ElementOperationBase 的任意一级");
			}
		}

		private bool CheckOperationNormative(IOperation operation)
		{
			if (operation is ISlideOperationBase || operation is IBoardOperationBase)
			{
				return true;
			}
			if (operation is IApplicationOperationBase)
			{
				return true;
			}
			return (operation as CombinationOperation)?.Operations.All(CheckOperationNormative) ?? false;
		}
	}
}
