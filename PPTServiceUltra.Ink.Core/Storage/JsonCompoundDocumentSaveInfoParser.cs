using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public class JsonCompoundDocumentSaveInfoParser : CompoundDocumentSaveInfoParser
	{
		
		private sealed class StartObjectStorableNode : StorableNode
		{
		}

		public JsonCompoundDocumentSaveInfoParser(ISaveInfoManager saveInfoManager)
			: base(saveInfoManager)
		{
		}

		public JsonCompoundDocumentSaveInfoParser(ISaveInfoManager saveInfoManager, IReferenceManager referenceManager)
			: base(saveInfoManager, referenceManager)
		{
		}

		public override StorableNode ParseValue(string value)
		{
			JsonTextReader jsonTextReader = new JsonTextReader(new StringReader(value));
			Stack<StorableNode> stack = new Stack<StorableNode>();
			StorableNode storableNode = null;
			do
			{
				switch (jsonTextReader.TokenType)
				{
				case JsonToken.PropertyName:
					storableNode = new StorableNode(Convert.ToString(jsonTextReader.Value, CultureInfo.InvariantCulture));
					stack.Push(storableNode);
					break;
				case JsonToken.Integer:
				case JsonToken.Float:
				case JsonToken.String:
				case JsonToken.Boolean:
				case JsonToken.Date:
					if (storableNode != null)
					{
						storableNode.Value = Convert.ToString(jsonTextReader.Value, CultureInfo.InvariantCulture);
					}
					break;
				case JsonToken.StartObject:
					stack.Push(new StartObjectStorableNode());
					break;
				case JsonToken.EndObject:
				{
					IList<StorableNode> list = new List<StorableNode>();
					StorableNode item;
					while (!((item = stack.Pop()) is StartObjectStorableNode))
					{
						list.Insert(0, item);
					}
					if (stack.Count == 0)
					{
						return list.First();
					}
					foreach (StorableNode item2 in list)
					{
						stack.Peek()?.AddChild(item2);
					}
					break;
				}
				case JsonToken.StartArray:
					if (storableNode != null)
					{
						storableNode.IsArray = true;
					}
					break;
				}
			}
			while (jsonTextReader.Read());
			return null;
		}

		public override string DeparseValue(StorableNode node)
		{
			if (node.IsEmpty)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			JsonTextWriter jsonTextWriter = new JsonTextWriter(new StringWriter(stringBuilder));
			jsonTextWriter.WriteStartObject();
			RecursiveParseNode(jsonTextWriter, node);
			jsonTextWriter.WriteEndObject();
			return stringBuilder.ToString();
		}

		private static void RecursiveParseNode(JsonWriter jsonWriter, StorableNode node)
		{
			jsonWriter.WritePropertyName(node.Name);
			if (node.Value == null)
			{
				if (!node.HasChild)
				{
					jsonWriter.WriteStartObject();
					jsonWriter.WriteEndObject();
					return;
				}
				if (node.IsArray)
				{
					jsonWriter.WriteStartArray();
				}
				else
				{
					jsonWriter.WriteStartObject();
				}
				foreach (StorableNode child in node.Children)
				{
					if (node.IsArray)
					{
						jsonWriter.WriteStartObject();
					}
					RecursiveParseNode(jsonWriter, child);
					if (node.IsArray)
					{
						jsonWriter.WriteEndObject();
					}
				}
				if (node.IsArray)
				{
					jsonWriter.WriteEndArray();
				}
				else
				{
					jsonWriter.WriteEndObject();
				}
			}
			else
			{
				jsonWriter.WriteValue(node.Value);
			}
		}
	}
}
