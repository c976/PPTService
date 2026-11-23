namespace PPTServiceUltra.Ink.Core.Windows.Media
{
	public struct FloodFillRange
	{
		public int StartX;

		public int EndX;

		public int Y;

		public FloodFillRange(int startX, int endX, int y)
		{
			StartX = startX;
			EndX = endX;
			Y = y;
		}
	}
}
