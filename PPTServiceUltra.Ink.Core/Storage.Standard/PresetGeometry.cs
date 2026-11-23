using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	[ComplexType(typeof(Geometry), "PresetGeometry")]
	public class PresetGeometry : Geometry
	{
		public IList<AdjustSaveInfo> Adjusts { get; set; }

		public override StorableNode Deparse()
		{
			StorableNode storableNode = new StorableNode("PresetGeometry");
			storableNode.AddChild(new StorableNode("GeometryType")
			{
				Value = GeometryType
			});
			if (Adjusts == null)
			{
				return storableNode;
			}
			StorableNode storableNode2 = new StorableNode("Adjusts")
			{
				IsArray = true
			};
			foreach (AdjustSaveInfo adjust in Adjusts)
			{
				StorableNode storableNode3 = new StorableNode("Adjust");
				storableNode3.AddChild(new StorableNode("Id")
				{
					Value = adjust.Id
				});
				storableNode3.AddChild(new StorableNode("ScaleX")
				{
					Value = adjust.ScaleX.ToString(CultureInfo.InvariantCulture)
				});
				storableNode3.AddChild(new StorableNode("ScaleY")
				{
					Value = adjust.ScaleY.ToString(CultureInfo.InvariantCulture)
				});
				storableNode2.AddChild(storableNode3);
			}
			storableNode.AddChild(storableNode2);
			return storableNode;
		}

		public override void Parse(StorableNode node)
		{
			GeometryType = node.GetChildValue("GeometryType");
			Adjusts = null;
			StorableNode storableNode = node.Children.FirstOrDefault( (StorableNode c) => c.Name == "Adjusts");
			if (storableNode == null)
			{
				return;
			}
			Adjusts = new List<AdjustSaveInfo>();
			foreach (StorableNode child in storableNode.Children)
			{
				AdjustSaveInfo adjustSaveInfo = new AdjustSaveInfo();
				adjustSaveInfo.Id = child.GetChildValue("Id");
				adjustSaveInfo.ScaleX = double.Parse(child.GetChildValue("ScaleX"), CultureInfo.InvariantCulture);
				adjustSaveInfo.ScaleY = double.Parse(child.GetChildValue("ScaleY"), CultureInfo.InvariantCulture);
				Adjusts.Add(adjustSaveInfo);
			}
		}
	}
}
