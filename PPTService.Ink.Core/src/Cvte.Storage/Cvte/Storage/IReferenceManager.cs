using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	public interface IReferenceManager
	{
		IList<ReferenceInfo> References { get; }

		string FilePath { get; }

		string TargetPath { get; }

		string Add(string absolutePath);

		string AddReference(string referenceId);

		bool TryGet(string referenceId, out string absolutePath);

		void Clear();

		void Reset();

		void Load(IList<ReferenceInfo> references);

		string GetRelativePath(string referenceId);
	}
}
