using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public class DashStyle : CompoundType
	{
		public List<double> Dashes { get; set; }

		public double Offset { get; set; }

		public static implicit operator DashStyle(System.Windows.Media.DashStyle dashStyle)
		{
			return Convert(dashStyle);
		}

		public static implicit operator System.Windows.Media.DashStyle(DashStyle dashStyle)
		{
			return ConvertBack(dashStyle);
		}

		private static DashStyle Convert(System.Windows.Media.DashStyle dashStyle)
		{
			if (dashStyle?.Dashes == null)
			{
				return null;
			}
			return new DashStyle
			{
				Dashes = dashStyle.Dashes.ToList(),
				Offset = dashStyle.Offset
			};
		}

		private static System.Windows.Media.DashStyle ConvertBack(DashStyle dashStyle)
		{
			if (dashStyle == null)
			{
				return null;
			}
			return new System.Windows.Media.DashStyle(dashStyle.Dashes, dashStyle.Offset);
		}

		public override StorableNode Deparse()
		{
			StorableNode storableNode = new StorableNode(Name);
			storableNode.AddChild(new StorableNode("Offset")
			{
				Value = Offset
			});
			StorableNode storableNode2 = new StorableNode("Dashes");
			foreach (double dash in Dashes)
			{
				storableNode2.AddChild(new StorableNode("Item")
				{
					Value = dash
				});
			}
			storableNode.AddChild(storableNode2);
			return storableNode;
		}

		public override void Parse(StorableNode node)
		{
			if (!node.HasChild)
			{
				return;
			}
			double result;
			if (double.TryParse(node.GetChildValue("Offset"), out result))
			{
				Offset = result;
			}
			StorableNode node2 = node.GetNode("Dashes");
			Dashes = new List<double>();
			foreach (StorableNode child in node2.Children)
			{
				double result2;
				if (!double.TryParse(child.StringValue, out result2))
				{
					result2 = 1.0;
				}
				Dashes.Add(result2);
			}
		}
	}
}
