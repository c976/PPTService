using System;

namespace PPTServiceUltra.Paint.Framework.Level
{
	public class LevelInfo
	{
		public string Id { get; }

		public LevelInfo()
		{
			Id = Guid.NewGuid().ToString();
		}
	}
}
