using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.ComponentModel;

namespace PPTService.Ink.Core.Storage
{
	internal static class CompoundDocumentExtension
	{
		
		[Obsolete("该方法仅用于调试")]
		public static bool AreEqual(this CompoundDocument document, CompoundDocument otherDocument)
		{
			if (!DebuggingProperties.IsDebug)
			{
				throw new InvalidOperationException("调试方法不可在非debug下运行");
			}
			if (document.Size != otherDocument.Size)
			{
				return false;
			}
			if (document.Name != otherDocument.Name)
			{
				return false;
			}
			if (document.Version != otherDocument.Version)
			{
				return false;
			}
			List<FileItem> list = document.Files.OrderBy((FileItem item) => item.RelativePath).ToList();
			List<FileItem> list2 = otherDocument.Files.OrderBy((FileItem item) => item.RelativePath).ToList();
			if (list.Count != list2.Count)
			{
				return false;
			}
			for (int num = 0; num < list.Count; num++)
			{
				if (list[num].RelativePath != list2[num].RelativePath)
				{
					return false;
				}
				string absolutePath = list[num].AbsolutePath;
				string absolutePath2 = list2[num].AbsolutePath;
				if (File.Exists(absolutePath) && File.Exists(absolutePath2))
				{
					try
					{
						string text = File.ReadAllText(absolutePath);
						string text2 = File.ReadAllText(absolutePath2);
						if (text != text2)
						{
							return false;
						}
					}
					catch (Exception)
					{
						return false;
					}
				}
				else if (File.Exists(absolutePath) || File.Exists(absolutePath2))
				{
					return false;
				}
			}
			return true;
		}
	}
}
