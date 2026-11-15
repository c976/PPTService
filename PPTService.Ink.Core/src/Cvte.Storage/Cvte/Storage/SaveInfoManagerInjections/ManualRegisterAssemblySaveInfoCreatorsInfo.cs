using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.SaveInfoManagerInjections
{
	
	
	
	public struct ManualRegisterAssemblySaveInfoCreatorsInfo
	{
		public Assembly RegisterAssembly { get; }

		public IEnumerable<SaveInfoCreatorInfo> SaveInfoCreatorInfoList { get; }

		public ManualRegisterAssemblySaveInfoCreatorsInfo(Assembly registerAssembly, IEnumerable<SaveInfoCreatorInfo> saveInfoCreatorInfoList)
		{
			RegisterAssembly = registerAssembly;
			SaveInfoCreatorInfoList = saveInfoCreatorInfoList;
		}
	}
}
