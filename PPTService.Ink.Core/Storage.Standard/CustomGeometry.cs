using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	[ComplexType(typeof(Geometry), "CustomGeometry")]
	public class CustomGeometry : Geometry
	{
		public Path Path { get; set; }

		public override StorableNode Deparse()
		{
			StorableNode storableNode = new StorableNode("CustomGeometry");
			storableNode.AddChild(new StorableNode("GeometryType")
			{
				Value = GeometryType
			});
			storableNode.AddChild(new StorableNode("Path")
			{
				Value = Path.Encode()
			});
			return storableNode;
		}

		public override void Parse(StorableNode node)
		{
			GeometryType = node.GetChildValue("GeometryType");
			Path = new Path();
			Path.TryDecode(node.GetChildValue("Path"));
		}
	}
}
