using System;
using System.Collections.Generic;
using System.Linq;
using Cvte.Diagnostics.Annotations;
using Cvte.Documents;

namespace Cvte.Paint.Framework.Operations
{
	public abstract class SlidesOperationBase : NotifyOperation, ISlidesOperationBase, IBoardOperationBase, IOperation
	{
		public IReadOnlyList<string> SlideIdList { get; }

		public Board Board { get; set; }

		protected SlidesOperationBase([NotNull] IReadOnlyList<string> slideIdList, [NotNull] Board board)
		{
			if (slideIdList == null)
			{
				throw new ArgumentNullException("slideIdList");
			}
			if (slideIdList.Count == 0)
			{
				throw new ArgumentException("不能传入空页面列表");
			}
			if (board == null)
			{
				throw new ArgumentNullException("board");
			}
			SlideIdList = slideIdList;
			Board = board;
		}

		protected SlidesOperationBase([NotNull] IReadOnlyList<Slide> slideList, [NotNull] Board board)
		{
			if (slideList == null)
			{
				throw new ArgumentNullException("slideList");
			}
			if (slideList.Count == 0)
			{
				throw new ArgumentException("不能传入空页面列表");
			}
			if (board == null)
			{
				throw new ArgumentNullException("board");
			}
			SlideIdList = slideList.Select((Slide slide) => slide.Id).ToList();
			Board = board;
		}

		protected abstract override void OnRedo();

		protected abstract override void OnUndo();
	}
}
