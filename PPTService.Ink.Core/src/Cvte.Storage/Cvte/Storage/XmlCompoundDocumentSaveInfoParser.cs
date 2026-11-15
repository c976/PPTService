using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using PPTService.Ink.Core.Storage.Log;

namespace PPTService.Ink.Core.Storage
{
	
	
	public class XmlCompoundDocumentSaveInfoParser : CompoundDocumentSaveInfoParser
	{
		public XmlCompoundDocumentSaveInfoParser(ISaveInfoManager saveInfoManager)
			: base(saveInfoManager)
		{
		}

		public XmlCompoundDocumentSaveInfoParser(ISaveInfoManager saveInfoManager, IReferenceManager referenceManager)
			: base(saveInfoManager, referenceManager)
		{
		}

		public override StorableNode ParseValue(string value)
		{
			return RecursiveParseNode(XDocument.Parse(value, LoadOptions.PreserveWhitespace).Root);
		}

		public override string DeparseValue(StorableNode node)
		{
			XElement xElement = RecursiveParseNode(node);
			XDocument xDocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), xElement);
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				XmlWriterSettings settings = new XmlWriterSettings
				{
					OmitXmlDeclaration = true,
					Indent = true,
					NewLineHandling = NewLineHandling.Entitize
				};
				using (XmlWriter writer = XmlWriter.Create(stringWriter, settings))
				{
					xDocument.WriteTo(writer);
				}
				return stringWriter.ToString();
			}
		}

		protected static StorableNode RecursiveParseNode(XElement currentElement)
		{
			StorableNode storableNode = new StorableNode(currentElement.Name.LocalName);
			try
			{
				storableNode.Value = currentElement.Value;
			}
			catch (OutOfMemoryException ex)
			{
				StorageStaticLoggerFactory.Logger?.Error("[RecursiveParseNode] " + currentElement.Name.LocalName + "; " + $"HasChildren:{currentElement.HasElements}", ex);
			}
			if (currentElement.HasElements)
			{
				foreach (XElement item in currentElement.Elements())
				{
					storableNode.AddChild(RecursiveParseNode(item));
				}
			}
			return storableNode;
		}

		protected static XElement RecursiveParseNode(StorableNode currentNode)
		{
			try
			{
				XElement xElement = new XElement(currentNode.Name);
				if (currentNode.HasChild)
				{
					foreach (StorableNode child in currentNode.Children)
					{
						xElement.Add(RecursiveParseNode(child));
					}
				}
				else
				{
					xElement.Value = currentNode.StringValue;
				}
				return xElement;
			}
			catch (Exception ex)
			{
				if (string.IsNullOrWhiteSpace(currentNode.Name))
				{
					throw new StorageNodeException("节点名称为空，请检查 SaveInfoContract 或 SaveInfoMember 标记。", ex);
				}
				throw new StorageNodeException(currentNode.Name + " 节点解析错误，" + ex.Message, ex);
			}
		}
	}
}
