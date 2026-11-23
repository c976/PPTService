using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public class LocalReferenceManager : IReferenceManager
	{
		public IList<ReferenceInfo> References { get; private set; }

		public string FilePath => "Reference";

		public string TargetPath => "Resources";

		public LocalReferenceManager()
		{
			References = new List<ReferenceInfo>();
		}

		public string Add(string absolutePath)
		{
			return absolutePath;
		}

		public string AddReference(string referenceId)
		{
			return referenceId;
		}

		public bool TryGet(string referenceId, out string absolutePath)
		{
			absolutePath = referenceId;
			return true;
		}

		public void Clear()
		{
			References.Clear();
		}

		public void Reset()
		{
		}

		public void Load(IList<ReferenceInfo> references)
		{
			References = references;
		}

		public string GetRelativePath(string referenceId)
		{
			return "";
		}
	}
}
