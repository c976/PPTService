using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public class CompoundDocumentReferenceManager : IReferenceManager
	{
		private readonly IList<Tuple<Predicate<string>, Func<string, string>>> _rules = new List<Tuple<Predicate<string>, Func<string, string>>>();

		private readonly HashManager _hashManager = new HashManager();

		private readonly IList<ReferenceInfo> _references;

		public IList<ReferenceInfo> References => _references.Where( (ReferenceInfo r) => r.Counter > 0).ToList();

		public string FilePath => "Reference";

		public string TargetPath => "Resources";

		public CompoundDocumentReferenceManager()
		{
			_references = new List<ReferenceInfo>();
		}

		public string Add(string absolutePath)
		{
			if (!File.Exists(absolutePath))
			{
				return "";
			}
			string hash = _hashManager.GetHash(absolutePath);
			string referenceId = GetReferenceId(absolutePath, hash);
			ReferenceInfo referenceInfo = _references.FirstOrDefault( (ReferenceInfo r) => string.Equals(r.ReferenceId, referenceId, StringComparison.Ordinal));
			if (referenceInfo != null)
			{
				referenceInfo.Counter++;
				return referenceInfo.ReferenceId;
			}
			_references.Add(new ReferenceInfo
			{
				Counter = 1,
				AbsolutePath = absolutePath,
				ReferenceId = referenceId,
				Hash = hash
			});
			return referenceId;
		}

		public string AddReference(string referenceId)
		{
			ReferenceInfo referenceInfo = _references.FirstOrDefault( (ReferenceInfo r) => r.ReferenceId == referenceId);
			if (referenceInfo != null)
			{
				referenceInfo.Counter++;
				return referenceInfo.ReferenceId;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ReferenceInfo reference in _references)
			{
				stringBuilder.Append($"#{reference.ReferenceId},{reference.AbsolutePath},{reference.Counter}#");
			}
			throw new ReferenceMissingException(referenceId)
			{
				ExtendMessage = $"Cache Reference Count:{_references.Count};{stringBuilder}"
			};
		}

		public bool TryGet(string referenceId, out string absolutePath)
		{
			ReferenceInfo referenceInfo = _references.FirstOrDefault( (ReferenceInfo s) => s.ReferenceId == referenceId);
			if (referenceInfo != null)
			{
				absolutePath = referenceInfo.AbsolutePath;
				return true;
			}
			absolutePath = string.Empty;
			return false;
		}

		public void Clear()
		{
			_references.Clear();
		}

		public void Reset()
		{
			foreach (ReferenceInfo reference in _references)
			{
				reference.Counter = 0;
			}
		}

		public void Load(IList<ReferenceInfo> references)
		{
			if (references == null)
			{
				throw new ArgumentNullException("references");
			}
			foreach (ReferenceInfo reference in references)
			{
				if (!_references.Any( (ReferenceInfo r) => r.Hash == reference.Hash && r.ReferenceId == reference.ReferenceId))
				{
					_references.Add(reference);
				}
			}
		}

		public string GetRelativePath(string referenceId)
		{
			string absolutePath;
			if (!TryGet(referenceId, out absolutePath))
			{
				return string.Empty;
			}
			using (IEnumerator<Tuple<Predicate<string>, Func<string, string>>> enumerator = _rules.Where( (Tuple<Predicate<string>, Func<string, string>> rule) => rule.Item1(absolutePath)).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current.Item2(absolutePath);
				}
			}
			return Path.Combine(TargetPath, referenceId + Path.GetExtension(absolutePath));
		}

		public void AddRelativePathRule(Predicate<string> predicate, Func<string, string> func)
		{
			if (predicate != null && func != null)
			{
				_rules.Add(new Tuple<Predicate<string>, Func<string, string>>(predicate, func));
			}
		}

		private string GetReferenceId(string absolutePath, string hash)
		{
			string extension = Path.GetExtension(absolutePath);
			return Md5SecurityUtility.Encrypt32(hash + extension);
		}
	}
}
