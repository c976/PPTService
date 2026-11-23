using System.Collections.Generic;
using PPTServiceUltra.Paint.Framework;

namespace PPTServiceUltra.Paint.Inks
{
	internal interface ISplitErasableInk
	{
		IEnumerable<Element> CreateNewInkOnErased();
	}
}
