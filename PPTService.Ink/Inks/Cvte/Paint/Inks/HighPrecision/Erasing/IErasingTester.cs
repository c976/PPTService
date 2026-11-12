using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Ink;
using System.Windows.Media;

namespace Cvte.Paint.Inks.HighPrecision.Erasing
{
	public interface IErasingTester
	{
		ValueTuple<bool, IEnumerable<Stroke>> EraseTest(Stroke stroke);

		ValueTuple<bool, Geometry> EraseTest(Geometry elementGeometry, Transform slideToElementTransfrom);

		void Move([In]ref EraserTouchEventArgs eraserTouchEventArgs);
	}
}
