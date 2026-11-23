using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.Encryption
{
	
	
	public class AesFileContentParser : IFileContentParser
	{
		private string _encryptionKey;

		private string _storageKey;

		public virtual string Name => "AES";

		public string FileExtensionName => ".cryt";

		public string DeparseToFile(string buildPath, string content)
		{
			string text = Path.Combine(buildPath, Guid.NewGuid().ToString() + FileExtensionName);
			AesEncryptHelper.EncryptStringToFile(text, content, _encryptionKey);
			return text;
		}

		public string ParseFromFile(string fileAbsolutePath)
		{
			return AesEncryptHelper.DecryptStringFromFile(fileAbsolutePath, _encryptionKey);
		}

		public FileContentParserSaveInfo BuildParserSaveInfo()
		{
			return new AesFileContentParserSaveInfo(Name)
			{
				Key = _storageKey
			};
		}

		public void LoadParserInfo(FileContentParserSaveInfo parserInfo)
		{
			AesFileContentParserSaveInfo aesFileContentParserSaveInfo = parserInfo as AesFileContentParserSaveInfo;
			if (aesFileContentParserSaveInfo != null)
			{
				_storageKey = aesFileContentParserSaveInfo.Key;
				_encryptionKey = BuildDecryptKey(_storageKey);
				if (string.IsNullOrWhiteSpace(_encryptionKey))
				{
					throw new FileContentParseException("Loaded EncrytionKey Is Empty.");
				}
				return;
			}
			throw new InvalidCastException("Can not convet " + parserInfo.GetType().FullName + " to AesFileContentParserSaveInfo");
		}

		public void Initialize()
		{
			_encryptionKey = CreateNewEncryptKey(out _storageKey);
			if (string.IsNullOrWhiteSpace(_encryptionKey))
			{
				throw new FileContentDeparseException("Created EncrytionKey Is Empty.");
			}
		}

		protected virtual string CreateNewEncryptKey(out string storageKey)
		{
			return storageKey = Guid.NewGuid().ToString("N");
		}

		protected virtual string BuildDecryptKey(string storageKey)
		{
			return storageKey;
		}
	}
}
