using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	[SaveInfoContract("StorableTypeItem")]
	public sealed class ItemSaveInfo : SaveInfo
	{
		public const string PackageSaveInfoName = "StorableTypeItem";

		public const string PropertyItemName = "Item";

		[SaveInfoMember("Item")]
		public object Item { get; set; }
	}
}
