using System;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public class FontFamily : CompoundType
	{
		public string BaseUri { get; set; }

		public string Source { get; set; }

		public static implicit operator FontFamily(System.Windows.Media.FontFamily fontFamily)
		{
			return Convert(fontFamily);
		}

		public static implicit operator System.Windows.Media.FontFamily(FontFamily fontFamily)
		{
			return ConvertBack(fontFamily);
		}

		private static FontFamily Convert(System.Windows.Media.FontFamily fontFamily)
		{
			return new FontFamily
			{
				Source = (fontFamily?.Source ?? string.Empty),
				BaseUri = fontFamily?.BaseUri?.ToString()
			};
		}

		private static System.Windows.Media.FontFamily ConvertBack(FontFamily fontFamily)
		{
			if (string.IsNullOrEmpty(fontFamily.Source))
			{
				return new System.Windows.Media.FontFamily("Microsoft YaHei UI");
			}
			if (!string.IsNullOrEmpty(fontFamily.BaseUri))
			{
				return new System.Windows.Media.FontFamily(new System.Uri(fontFamily.BaseUri), fontFamily.Source);
			}
			return new System.Windows.Media.FontFamily(fontFamily.Source);
		}

		public override StorableNode Deparse()
		{
			StorableNode storableNode = new StorableNode(Name);
			storableNode.AddChild(new StorableNode("Source")
			{
				Value = Source
			});
			if (!string.IsNullOrEmpty(BaseUri))
			{
				storableNode.AddChild(new StorableNode("BaseUri")
				{
					Value = BaseUri
				});
			}
			return storableNode;
		}

		public override void Parse(StorableNode node)
		{
			if (!node.HasChild)
			{
				Source = node.StringValue;
				return;
			}
			Source = node.GetChildValue("Source");
			BaseUri = node.GetChildValue("BaseUri");
		}

		public override string ToString()
		{
			return Source;
		}
	}
}
