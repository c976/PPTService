using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.SaveInfoManagerInjections
{
	
	
	
	public struct SaveInfoCreatorInfo
	{
		public Type SaveInfoType { get; }

		public SaveInfoContractAttribute SaveInfoContractAttribute { get; }

		
		
		public Func<SaveInfo> Creator
		{
			
			get;
		}

		public SaveInfoCreatorInfo(Type saveInfoType, SaveInfoContractAttribute saveInfoContractAttribute,  Func<SaveInfo> creator)
		{
			SaveInfoType = saveInfoType;
			SaveInfoContractAttribute = saveInfoContractAttribute;
			Creator = creator;
		}
	}
}
