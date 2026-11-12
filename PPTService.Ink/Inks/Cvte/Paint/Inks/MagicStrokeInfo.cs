using System;

namespace Cvte.Paint.Inks
{
	public class MagicStrokeInfo
	{
		public double Scale { get; }

		public Type Type { get; }

		public InkType StrokeType { get; }

		public MagicStrokeInfo(InkType strokeType, Type type, double scale = 1.0)
		{
			if (scale <= 0.0)
			{
				throw new ArgumentException($"{scale}必须为正");
			}
			StrokeType = strokeType;
			Type = type;
			Scale = scale;
		}
	}
}
