using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.ComponentModel;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	[ComplexType(typeof(Brush), "ImageBrush")]
	public class ImageBrush : Brush
	{
		public Uri Source { get; set; }

		public Stretch Stretch { get; set; }

		public TileMode TileMode { get; set; }

		public BrushMappingMode? ViewboxUnits { get; set; }

		public BrushMappingMode? ViewportUnits { get; set; }

		public Rect Viewbox { get; set; }

		public Rect Viewport { get; set; }

		public Matrix RelativeMatrixTransform { get; set; }

		public override StorableNode Deparse()
		{
			if (Source == null)
			{
				throw new InvalidOperationException("Source is null");
			}
			if (Source is FileUri)
			{
				Source = new IdUri(ReferenceManager.Add(Source.Value));
			}
			else if (Source is IdUri)
			{
				Source = new IdUri(ReferenceManager.AddReference(Source.Value));
			}
			if (DebuggingProperties.IsDebug && Source.Value.StartsWith("pack://application", StringComparison.InvariantCultureIgnoreCase))
			{
				throw new InvalidOperationException("不能将程序集资源作为存储资源保存，请使用 ImageBrush 等将资源转换为文件引用。" + Source.Value);
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>
			{
				{
					"Source",
					Source.Encode()
				},
				{
					"Stretch",
					Stretch.ToString()
				},
				{
					"TileMode",
					TileMode.ToString()
				},
				{
					"Opacity",
					Opacity.ToString(CultureInfo.InvariantCulture)
				}
			};
			if (ViewboxUnits.HasValue)
			{
				dictionary.Add("ViewboxUnits", ViewboxUnits.ToString());
			}
			if (ViewportUnits.HasValue)
			{
				dictionary.Add("ViewportUnits", ViewportUnits.ToString());
			}
			StorableNode storableNode = new StorableNode("ImageBrush", dictionary);
			if (Viewbox != null)
			{
				StorableNode storableNode2 = Viewbox.Deparse();
				storableNode2.Name = "Viewbox";
				storableNode.AddChild(storableNode2);
			}
			if (Viewport != null)
			{
				StorableNode storableNode3 = Viewport.Deparse();
				storableNode3.Name = "Viewport";
				storableNode.AddChild(storableNode3);
			}
			if (RelativeMatrixTransform != null)
			{
				StorableNode storableNode4 = RelativeMatrixTransform.Deparse();
				storableNode4.Name = "RelativeMatrixTransform";
				storableNode.AddChild(storableNode4);
			}
			return storableNode;
		}

		public override void Parse(StorableNode node)
		{
			string childValue = node.GetChildValue("Source");
			Source = Uri.Create(childValue);
			if (Source == null)
			{
				string absolutePath;
				if (!ReferenceManager.TryGet(childValue, out absolutePath))
				{
					Source = new AppUri(childValue);
				}
				else
				{
					Source = new FileUri(absolutePath);
				}
			}
			else if (Source is IdUri)
			{
				string absolutePath2;
				if (!ReferenceManager.TryGet(Source.Value, out absolutePath2))
				{
					throw new ReferenceMissingException(Source.Value)
					{
						ExtendMessage = "Source StringValue:" + childValue
					};
				}
				Source = new FileUri(absolutePath2);
			}
			Stretch = (Stretch)Enum.Parse(typeof(Stretch), node.GetChildValue("Stretch"));
			TileMode = (TileMode)Enum.Parse(typeof(TileMode), node.GetChildValue("TileMode"));
			double result;
			if (double.TryParse(node.GetChildValue("Opacity"), out result))
			{
				Opacity = result;
			}
			string childValue2 = node.GetChildValue("ViewboxUnits");
			if (!string.IsNullOrEmpty(childValue2))
			{
				ViewboxUnits = (BrushMappingMode)Enum.Parse(typeof(BrushMappingMode), childValue2);
			}
			string childValue3 = node.GetChildValue("ViewportUnits");
			if (!string.IsNullOrEmpty(childValue3))
			{
				ViewportUnits = (BrushMappingMode)Enum.Parse(typeof(BrushMappingMode), childValue3);
			}
			StorableNode node2 = node.GetNode("Viewbox");
			if (node2 != null)
			{
				Viewbox = new Rect();
				Viewbox.Parse(node2);
			}
			StorableNode node3 = node.GetNode("Viewport");
			if (node3 != null)
			{
				Viewport = new Rect();
				Viewport.Parse(node3);
			}
			StorableNode node4 = node.GetNode("RelativeMatrixTransform");
			if (node4 != null)
			{
				RelativeMatrixTransform = new Matrix();
				RelativeMatrixTransform.Parse(node4);
			}
		}
	}
}
