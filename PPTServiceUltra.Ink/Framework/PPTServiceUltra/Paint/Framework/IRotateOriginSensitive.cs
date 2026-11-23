using System;
using System.Windows;

namespace PPTServiceUltra.Paint.Framework
{
	public interface IRotateOriginSensitive
	{
		Tuple<bool, Point> TryGetSensitivePoint(Point point);
	}
}
