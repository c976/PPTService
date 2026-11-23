using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	[AttributeUsage(AttributeTargets.Class)]
	public class SaveInfoContractAttribute : Attribute
	{
		public string Name { get; }

		public string Description { get; set; }

		public string UniqueIdentifyMemberName { get; set; }

		public bool IsSubPropertyDeltaSaveEnabled { get; set; } = true;

		public bool Overwrite { get; set; }

		public SaveInfoType SaveInfoType { get; set; }

		public string FriendlyName { get; set; } = string.Empty;

		
		
		public Type FallbackSaveInfoType
		{
			
			get;
			
			set;
		}

		public bool IsFallbackElement { get; set; }

		public SaveInfoContractAttribute(string name)
		{
			Name = name;
		}
	}
}
