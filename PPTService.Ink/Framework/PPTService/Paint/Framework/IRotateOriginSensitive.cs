using System;
using System.Windows;

namespace PPTService.Paint.Framework
{
	public interface IRotateOriginSensitive
	{
		Tuple<bool, Point> TryGetSensitivePoint(Point point);
	}
}
