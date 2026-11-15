using System.Collections.Generic;
using PPTService.Paint.Framework;

namespace PPTService.Paint.Inks
{
	internal interface ISplitErasableInk
	{
		IEnumerable<Element> CreateNewInkOnErased();
	}
}
