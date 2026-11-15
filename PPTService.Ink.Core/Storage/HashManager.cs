using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	public class HashManager
	{
		private readonly ConcurrentDictionary<string, Tuple<long, string>> _pathTupleMapping = new ConcurrentDictionary<string, Tuple<long, string>>();

		public string GetHash(string filePath)
		{
			if (!File.Exists(filePath))
			{
				return string.Empty;
			}
			long length = new FileInfo(filePath).Length;
			Tuple<long, string> value;
			if (_pathTupleMapping.TryGetValue(filePath, out value))
			{
				if (value.Item1 == length)
				{
					return value.Item2;
				}
				_pathTupleMapping.TryRemove(filePath, out value);
			}
			string md5HashFromFile = Md5SecurityUtility.GetMd5HashFromFile(filePath);
			_pathTupleMapping.TryAdd(filePath, new Tuple<long, string>(length, md5HashFromFile));
			return md5HashFromFile;
		}
	}
}
