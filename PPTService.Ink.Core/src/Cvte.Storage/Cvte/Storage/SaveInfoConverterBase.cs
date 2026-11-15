using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Ink.Core.Storage
{
	
	
	public abstract class SaveInfoConverterBase< T> : ISaveInfoConverter where T : SaveInfo
	{
		private IReferenceManager _referenceManager;

		private SaveInfoConverterManager _saveInfoConverterManager;

		protected IStorageLogger Logger;

		public SaveInfo Convert(StorableNode node)
		{
			return DoConvert(node);
		}

		[CanBeNull]
		public virtual List<SaveInfo> ConvertToSaveInfos(StorableNode node)
		{
			return null;
		}

		public void SetSaveInfoManager(SaveInfoConverterManager saveInfoConverterManager)
		{
			_saveInfoConverterManager = saveInfoConverterManager;
		}

		public void SetLogger(IStorageLogger logger)
		{
			Logger = logger;
		}

		public void SetReferenceManager(IReferenceManager referenceManager)
		{
			_referenceManager = referenceManager;
		}

		protected abstract T DoConvert(StorableNode node);

		protected string GetReference(string id)
		{
			if (_referenceManager == null)
			{
				return string.Empty;
			}
			string absolutePath;
			if (_referenceManager.TryGet(id, out absolutePath))
			{
				return absolutePath;
			}
			return string.Empty;
		}

		protected bool TryGetConverter(string name, out ISaveInfoConverter converter)
		{
			if (_saveInfoConverterManager.TryGetConverter(name, out converter))
			{
				converter.SetSaveInfoManager(_saveInfoConverterManager);
				converter.SetReferenceManager(_referenceManager);
				converter.SetLogger(Logger);
				return true;
			}
			return false;
		}
	}
}
