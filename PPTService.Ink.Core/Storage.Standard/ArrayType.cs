using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	public abstract class ArrayType : IStorableType
	{
		
		public abstract StorableNode Deparse();

		
		public abstract void Parse(StorableNode node);
	}
	
	
	public class ArrayType< T> : ArrayType where T : IStorableType, new()
	{
		public IList<T> Items { get; }

		public int Count => Items.Count;

		public T this[int index] => Items[index];

		public ArrayType()
		{
			Items = new List<T>();
		}

		public ArrayType(IEnumerable<T> items)
		{
			Items = items.Select( (T i) => i).ToList();
		}

		public override StorableNode Deparse()
		{
			StorableNode storableNode = new StorableNode(string.Empty);
			foreach (T item in Items)
			{
				StorableNode storableNode2 = item.Deparse();
				storableNode2.Name = typeof(T).Name;
				storableNode.AddChild(storableNode2);
			}
			return storableNode;
		}

		public override void Parse(StorableNode node)
		{
			foreach (StorableNode child in node.Children)
			{
				T item = new T();
				item.Parse(child);
				Items.Add(item);
			}
		}
	}
}
