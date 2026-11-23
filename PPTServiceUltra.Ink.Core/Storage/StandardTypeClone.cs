using System;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Storage.Standard;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public static class StandardTypeClone
	{
		
		[SaveInfoContract("TypeCloneItem")]
		public class CloneItemSaveInfo : SaveInfo
		{
			[SaveInfoMember("Item")]
			public object Item { get; set; }
		}

		
		private class CloneXmlCompoundDocumentSaveInfoParser : XmlCompoundDocumentSaveInfoParser
		{
			public CloneXmlCompoundDocumentSaveInfoParser(ISaveInfoManager saveInfoManager)
				: base(saveInfoManager)
			{
			}

			public CloneXmlCompoundDocumentSaveInfoParser(ISaveInfoManager saveInfoManager, IReferenceManager referenceManager)
				: base(saveInfoManager, referenceManager)
			{
			}

			public SaveInfo ParseStorableNode(StorableNode node)
			{
				return ParseNode(node);
			}

			public SaveInfo ParseStorableNode(StorableNode node, Type givenPropertyType)
			{
				return ParseNode(node, givenPropertyType);
			}

			public StorableNode DeparseStorableNode(SaveInfo saveInfo)
			{
				return DeparseNode(saveInfo);
			}
		}

		private static readonly CloneXmlCompoundDocumentSaveInfoParser Parser;

		static StandardTypeClone()
		{
			SaveInfoManager saveInfoManager = new SaveInfoManager();
			saveInfoManager.Load(typeof(StorableNode).Assembly);
			Parser = new CloneXmlCompoundDocumentSaveInfoParser(saveInfoManager, new CompoundDocumentReferenceManager());
		}

		public static T DeepClone< T>(this T obj) where T : IStorableType
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			CloneItemSaveInfo saveInfo = new CloneItemSaveInfo
			{
				Item = obj
			};
			StorableNode storableNode = Parser.DeparseStorableNode(saveInfo);
			Type typeFromHandle = typeof(T);
			if (typeFromHandle.IsSubclassOf(typeof(Brush)))
			{
				typeFromHandle = typeof(Brush);
			}
			else if (typeFromHandle.IsSubclassOf(typeof(Geometry)))
			{
				typeFromHandle = typeof(Geometry);
			}
			else if (typeFromHandle.IsSubclassOf(typeof(Region)))
			{
				typeFromHandle = typeof(Region);
			}
			SaveInfo saveInfo2 = Parser.ParseStorableNode(storableNode, typeFromHandle);
			if (saveInfo2 == null)
			{
				throw new NotSupportedException($"Can not parse {typeof(T)}({storableNode.Name});");
			}
			return (T)((CloneItemSaveInfo)saveInfo2).Item;
		}
	}
}
