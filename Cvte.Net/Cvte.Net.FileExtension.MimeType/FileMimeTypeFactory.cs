using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Cvte.Net.FileExtension.MimeType
{
	
	
	internal class FileMimeTypeFactory : IFileMimeTypeFactory
	{
		private const string DefaultMimeType = "application/octet-stream";

		private List<IFileMimeTypeBuilder> _fileMimeTypeBuilders = new List<IFileMimeTypeBuilder>();

		public void SetFileMimeTypeBuilder(IFileMimeTypeBuilder mimeTypeBuilder)
		{
			if (mimeTypeBuilder == null)
			{
				throw new ArgumentNullException("mimeTypeBuilder");
			}
			_fileMimeTypeBuilders.Add(mimeTypeBuilder);
			_fileMimeTypeBuilders.Sort(new FileMimeTypeBuilderComparer());
		}

		public string BuildFileMimeType(string filePath)
		{
			foreach (IFileMimeTypeBuilder fileMimeTypeBuilder in _fileMimeTypeBuilders)
			{
				string mimeType;
				if (fileMimeTypeBuilder.TryBuildFileMimeType(filePath, out mimeType))
				{
					return mimeType;
				}
			}
			return "application/octet-stream";
		}
	}
}
