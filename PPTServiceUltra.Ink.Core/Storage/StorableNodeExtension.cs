using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public static class StorableNodeExtension
	{
		public static bool AreEquals(this StorableNode node, StorableNode otherNode)
		{
			if (otherNode == null)
			{
				return false;
			}
			if (node.Children.Count != otherNode.Children.Count)
			{
				return false;
			}
			if (node.StringValue != otherNode.StringValue)
			{
				return false;
			}
			for (int i = 0; i < node.Children.Count; i++)
			{
				if (!node.Children[i].AreEquals(otherNode.Children[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static T GetValue< T>(this StorableNode node, params string[] names)
		{
			if (!node.HasChild)
			{
				return default(T);
			}
			foreach (StorableNode child in node.Children)
			{
				if (names.Contains(child.Name))
				{
					return (T)Convert.ChangeType(child.Value, typeof(T), CultureInfo.InvariantCulture);
				}
			}
			return default(T);
		}

		public static StorableNode GetNode(this StorableNode node, params string[] names)
		{
			return node.Children.FirstOrDefault( (StorableNode child) => names.Contains(child.Name));
		}
	}
}
