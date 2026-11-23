using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace PPTServiceUltra.Paint.Inks
{
	internal interface IRoadStroke
	{
		double StepLength { get; }

		int StepAmount { get; }

		IList<Action<DrawingContext, Point>> DrawActions { get; }
	}
}
