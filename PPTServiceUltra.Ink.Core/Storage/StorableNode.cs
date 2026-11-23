using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public class StorableNode
	{
		private string _internalKey;

		public string Name { get; set; }

		public object Value { get; set; }

		public string StringValue => GetStringValue();

		public bool IsArray { get; set; }

		public bool IsEmpty
		{
			get
			{
				if (Value == null)
				{
					return !HasChild;
				}
				return false;
			}
		}

		public IList<StorableNode> Children { get; private set; }

		public StorableNode FirstNode => Children?.FirstOrDefault();

		public bool HasChild
		{
			get
			{
				if (Children != null)
				{
					return Children.Any();
				}
				return false;
			}
		}

		internal string InternalKey
		{
			get
			{
				return _internalKey ?? Name;
			}
			set
			{
				_internalKey = value;
			}
		}

		internal bool DeltaSaveTerminalNode { get; set; }

		internal string ConsistentName { get; set; }

		internal string RelativePath { get; set; }

		internal string AbsolutePath { get; set; }

		internal bool IsDeltaSupported { get; set; } = true;

		private string GetStringValue()
		{
			if (Value == null)
			{
				return string.Empty;
			}
			return TextFileWriter.SafetyContent(Convert.ToString(Value, CultureInfo.InvariantCulture));
		}

		public StorableNode()
		{
			Children = new List<StorableNode>();
		}

		public StorableNode(string name)
		{
			Name = name;
			Children = new List<StorableNode>();
		}

		public StorableNode(StorableNode node)
		{
			Children = new List<StorableNode> { node };
		}

		public StorableNode(string name,  IEnumerable<KeyValuePair<string, object>> children)
		{
			Name = name;
			Children = children.Select( (KeyValuePair<string, object> p) => new StorableNode(p.Key)
			{
				Value = p.Value
			}).ToList();
		}

		public StorableNode(string name, IList<StorableNode> children)
		{
			Name = name;
			Children = children;
		}

		public void AddChild( IEnumerable<KeyValuePair<string, object>> children)
		{
			Children = children.Select( (KeyValuePair<string, object> p) => new StorableNode(p.Key)
			{
				Value = p.Value
			}).ToList();
		}

		public void AddChild(StorableNode node)
		{
			if (Children == null)
			{
				Children = new List<StorableNode>();
			}
			Children.Add(node);
		}

		public void AddChild(IList<StorableNode> children)
		{
			if (children == null || children.Count == 0)
			{
				return;
			}
			foreach (StorableNode child in children)
			{
				AddChild(child);
			}
		}

		public string GetChildValue(string name, string defaultValue = "")
		{
			if (Children == null)
			{
				return defaultValue;
			}
			StorableNode storableNode = Children.FirstOrDefault( (StorableNode c) => c.Name == name);
			if (storableNode != null)
			{
				return storableNode.StringValue;
			}
			return defaultValue;
		}
	}
}
