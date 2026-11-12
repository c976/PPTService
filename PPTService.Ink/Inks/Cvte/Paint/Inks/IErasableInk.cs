using System.Windows.Ink;

namespace Cvte.Paint.Inks
{
	internal interface IErasableInk : IInk, IEraseSensitive, ISplitErasableInk
	{
		bool IsErasable { get; }

		StrokeCollection TempInkContainer { get; }

		Stroke InnerStroke { get; }

		bool IsErasing { get; set; }
	}
}
