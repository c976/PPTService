using System;
using System.Collections.Generic;
using System.Linq;
using Cvte.Diagnostics.Annotations;
using Cvte.Documents;

namespace Cvte.Paint.Framework.Operations
{
	public class SlidesCombinationOperation : CombinationOperation, ISlidesOperationBase, IBoardOperationBase, IOperation
	{
		public Board Board { get; }

		public IReadOnlyList<string> SlideIdList { get; }

		public SlidesCombinationOperation([NotNull] IReadOnlyList<string> slideIdList, [NotNull] Board board, IList<IOperation> operations)
			: base(operations)
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

		public SlidesCombinationOperation([NotNull] IReadOnlyList<Slide> slideList, [NotNull] Board board, IList<IOperation> operations)
			: base(operations)
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

		public SlidesCombinationOperation([NotNull] IReadOnlyList<string> slideIdList, [NotNull] Board board, params IOperation[] operations)
			: base(operations)
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

		public SlidesCombinationOperation([NotNull] IReadOnlyList<Slide> slideList, [NotNull] Board board, params IOperation[] operations)
			: base(operations)
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
	}
}
