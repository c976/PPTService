using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public abstract class CompoundDocumentAppendantItem< T> : ICompoundDocumentAppendantItem where T : SaveInfo
	{
		public string RelativePath { get; }

		protected CompoundDocumentAppendantItem(string relativePath)
		{
			RelativePath = relativePath;
		}

		void ICompoundDocumentAppendantItem.SetSaveInfo(SaveInfo saveInfo)
		{
			T val = saveInfo as T;
			if (val != null)
			{
				SetAppendantSaveInfo(val);
				return;
			}
			throw new InvalidCastException("Cannot Parse SaveInfo Of " + RelativePath + "; Real Type Is:" + saveInfo.GetType().FullName);
		}

		
		SaveInfo ICompoundDocumentAppendantItem.GetSaveInfo(CompoundDocument compoundDocument)
		{
			return GetAppendantSaveInfo(compoundDocument);
		}

		protected abstract void SetAppendantSaveInfo(T saveInfo);

		
		protected abstract T GetAppendantSaveInfo(CompoundDocument compoundDocument);
	}
}
