using System;
using System.Windows;

namespace Cvte.Paint.Framework
{
	public interface IRotateOriginSensitive
	{
		Tuple<bool, Point> TryGetSensitivePoint(Point point);
	}
}
