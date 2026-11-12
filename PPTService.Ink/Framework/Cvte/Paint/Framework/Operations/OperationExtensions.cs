using System;
using System.Linq;
using Cvte.ComponentModel;
using Cvte.Contracts;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework.Operations
{
	public static class OperationExtensions
	{
		public static Board GetBoard(this SlideOperationBase operation)
		{
			return operation.GetOperationBoard();
		}

		public static Board GetBoard(this ElementOperationBase operation)
		{
			return operation.GetOperationBoard();
		}

		public static Board GetBoard(this ElementCombinationOperation operation)
		{
			return operation.GetOperationBoard();
		}

		internal static Board GetOperationBoard(this IInjectBoardOperation boardOperation)
		{
			Board board = boardOperation.Board;
			if (board == null)
			{
				throw new MemberNullException("找不到 Board，请检查是否注入Board，是否使用 BoardUndoRedoProvider ");
			}
			return board;
		}

		internal static Slide GetOperationSlide([NotNull] this ISlideOperationBase slideOperation)
		{
			if (slideOperation == null)
			{
				throw new ArgumentNullException("slideOperation");
			}
			string slideId = slideOperation.SlideId;
			if (slideId == null)
			{
				throw new MemberNullException("SlideId", "调用GetSlide时，SlideId不能为空，不规范的类型是 " + slideOperation.GetType().FullName);
			}
			Board board = slideOperation.Board;
			if (board == null)
			{
				throw new MemberNullException("找不到 Board，请检查是否注入Board，是否使用 BoardUndoRedoProvider ");
			}
			Slide slideById = board.GetSlideById(slideId);
			if (slideById == null && DebuggingProperties.IsDebug && !(slideOperation is SlideOperationBase))
			{
				throw new MemberNullException("slide", $"在 {board} 找不到 id={slideId} 的页面，请检查页面是否存在")
				{
					Data = 
					{
						{
							(object)"slideOperation",
							(object)slideOperation.GetType().FullName
						},
						{
							(object)"Board 的所有页面",
							(object)string.Join(" ; ", board.Slides.Select((Slide temp) => temp.Id))
						}
					}
				};
			}
			return slideById;
		}
	}
}
