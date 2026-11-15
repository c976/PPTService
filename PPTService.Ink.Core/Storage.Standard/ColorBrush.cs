using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	[ComplexType(typeof(Brush), "ColorBrush")]
	public class ColorBrush : Brush
	{
		public Color Color { get; set; }

		public override StorableNode Deparse()
		{
			return new StorableNode("ColorBrush")
			{
				Value = Color.Encode()
			};
		}

		public override void Parse(StorableNode node)
		{
			Color = Color.Parse(node.StringValue);
		}
	}
}
