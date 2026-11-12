using System.Linq;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework
{
	public static class BoardServiceManager
	{
		private static WeakCollection<BoardService> BoardServices { get; } = new WeakCollection<BoardService>();

		[CanBeNull]
		public static BoardService GetBoardService([NotNull] Board board)
		{
			return BoardServices.TryGetItems((BoardService service) => object.Equals(service.Board, board)).FirstOrDefault();
		}

		internal static void AddBoardService(BoardService boardService)
		{
			BoardServices.Add(boardService);
		}
	}
}
