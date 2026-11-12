using System.Windows;
using System.Windows.Controls;
using System.Windows.Input.StylusPlugIns;

namespace Cvte.Paint.Framework
{
	public class BoardEventSource : Grid, IStylusPlugInSource, IInputElement
	{
		public void AddStylusPlugIns(StylusPlugIn stylusPlugIns)
		{
			StylusPlugIns.Add(stylusPlugIns);
		}

		public void RemoveStylusPlugIns(StylusPlugIn stylusPlugIns)
		{
			StylusPlugIns.Remove(stylusPlugIns);
		}

		public bool ContainsStylusPlugIns(StylusPlugIn stylusPlugIns)
		{
			return StylusPlugIns.Contains(stylusPlugIns);
		}
	}
}
