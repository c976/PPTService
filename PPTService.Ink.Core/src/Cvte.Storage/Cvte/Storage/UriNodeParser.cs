using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Storage.Standard;

namespace PPTService.Ink.Core.Storage
{
	
	
	public class UriNodeParser : NodeParserBase<PPTService.Ink.Core.Storage.Standard.Uri>
	{
		private readonly Func<IReferenceManager> _getReferenceManager;

		public UriNodeParser(Func<IReferenceManager> getReferenceManager)
		{
			_getReferenceManager = getReferenceManager;
		}

		protected override PPTService.Ink.Core.Storage.Standard.Uri DoParse(StorableNode node, Type type, PropertyInfo property)
		{
			PPTService.Ink.Core.Storage.Standard.Uri uri = PPTService.Ink.Core.Storage.Standard.Uri.Create(node.StringValue);
			if (uri == null)
			{
				string absolutePath;
				if (_getReferenceManager().TryGet(node.StringValue, out absolutePath))
				{
					return new FileUri(absolutePath);
				}
				return new FileUri(node.StringValue);
			}
			string absolutePath2;
			if (uri is IdUri && _getReferenceManager().TryGet(uri.Value, out absolutePath2))
			{
				return new FileUri(absolutePath2);
			}
			return uri;
		}

		protected override void DoDeparse(string name, PPTService.Ink.Core.Storage.Standard.Uri uri, StorableNode node, PropertyInfo property)
		{
			if (uri is FileUri)
			{
				if (!File.Exists(uri.Value))
				{
					return;
				}
				uri = new IdUri(_getReferenceManager().Add(uri.Value));
			}
			string value = uri.Encode();
			node.AddChild(new StorableNode(name)
			{
				Value = value,
				IsDeltaSupported = (property == null || property.GetMemberIsDeltaSupported())
			});
		}
	}
}
