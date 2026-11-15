using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	[ComplexType(typeof(Brush), "LinearGradientBrush")]
	public class LinearGradientBrush : Brush
	{
		public Point StartPoint { get; set; }

		public Point EndPoint { get; set; }

		public string GradientStops { get; set; }

		public override StorableNode Deparse()
		{
			return new StorableNode("LinearGradientBrush", new Dictionary<string, object>
			{
				{
					"StartPoint",
					StartPoint.Encode()
				},
				{
					"EndPoint",
					EndPoint.Encode()
				},
				{ "GradientStops", GradientStops },
				{ "Opacity", Opacity }
			});
		}

		public override void Parse(StorableNode node)
		{
			GradientStops = node.GetChildValue("GradientStops");
			StartPoint = new Point();
			StartPoint.TryDecode(node.GetChildValue("StartPoint"));
			EndPoint = new Point();
			EndPoint.TryDecode(node.GetChildValue("EndPoint"));
			double result;
			if (double.TryParse(node.GetChildValue("Opacity"), out result))
			{
				Opacity = result;
			}
		}
	}
}
