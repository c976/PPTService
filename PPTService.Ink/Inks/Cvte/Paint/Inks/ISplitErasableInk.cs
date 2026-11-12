using System.Collections.Generic;
using Cvte.Paint.Framework;

namespace Cvte.Paint.Inks
{
	internal interface ISplitErasableInk
	{
		IEnumerable<Element> CreateNewInkOnErased();
	}
}
