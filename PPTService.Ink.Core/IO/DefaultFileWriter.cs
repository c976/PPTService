using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PPTService.Ink.Core.IO
{
	
	
	internal class DefaultFileWriter : IFileWriter
	{
		private const int MaxRetryCount = 10;

		private long CurrentAllLogFileSize { get; set; }

		public string LastLogFilePath { get; }

		public string AllLogFilePath { get; }

		public long MaximumFileSize { get; }

		public int MaxSizeRollBackups { get; }

		
		
		private static byte[] LineBreakByteList
		{
			
			get;
		}

		public DefaultFileWriter(string lastLogFilePath, string allLogFilePath, long maximumFileSize, int maxSizeRollBackups)
		{
			LastLogFilePath = lastLogFilePath;
			AllLogFilePath = allLogFilePath;
			MaximumFileSize = maximumFileSize;
			MaxSizeRollBackups = maxSizeRollBackups;
			if (!string.IsNullOrEmpty(allLogFilePath) && File.Exists(allLogFilePath))
			{
				CurrentAllLogFileSize = new FileInfo(allLogFilePath).Length;
			}
		}

		
		public async Task AppendAllLinesAsync(IEnumerable<string> contentList)
		{
			List<FileStream> streamList = new List<FileStream>(2);
			if (!string.IsNullOrEmpty(LastLogFilePath))
			{
				FileStream fileStream = await BuildStream(LastLogFilePath).ConfigureAwait(false);
				if (fileStream != null)
				{
					streamList.Add(fileStream);
				}
			}
			if (!string.IsNullOrEmpty(AllLogFilePath))
			{
				FileStream fileStream2 = await BuildStream(AllLogFilePath).ConfigureAwait(false);
				if (fileStream2 != null)
				{
					streamList.Add(fileStream2);
				}
			}
			if (streamList.Count == 0)
			{
				return;
			}
			foreach (string content in contentList)
			{
				byte[] lineByteList = Encoding.UTF8.GetBytes(content);
				CurrentAllLogFileSize += lineByteList.Length;
				foreach (FileStream stream in streamList)
				{
					for (int i = 0; i < 10; i++)
					{
						try
						{
							await stream.WriteAsync(lineByteList, 0, lineByteList.Length).ConfigureAwait(false);
							await stream.WriteAsync(LineBreakByteList, 0, LineBreakByteList.Length).ConfigureAwait(false);
						}
						catch (Exception)
						{
							await Task.Delay(TimeSpan.FromMilliseconds(Math.Max(50, i + 10))).ConfigureAwait(false);
							continue;
						}
						break;
					}
				}
			}
			foreach (FileStream item in streamList)
			{
				try
				{
					item.Dispose();
				}
				catch (Exception)
				{
				}
			}
			LimitFileSize();
		}

		static DefaultFileWriter()
		{
			LineBreakByteList = Encoding.UTF8.GetBytes("\r\n");
		}

		private void LimitFileSize()
		{
			string allLogFilePath = AllLogFilePath;
			if (string.IsNullOrEmpty(allLogFilePath) || MaximumFileSize <= 0 || CurrentAllLogFileSize < MaximumFileSize)
			{
				return;
			}
			try
			{
				VersionFileHelper.ControlFileVersion(allLogFilePath, MaxSizeRollBackups);
				CurrentAllLogFileSize = 0L;
			}
			catch (Exception)
			{
			}
		}

		
		
		private static async Task<FileStream> BuildStream(string filePath)
		{
			for (int i = 0; i < 10; i++)
			{
				try
				{
					return new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None);
				}
				catch (Exception)
				{
					await Task.Delay(TimeSpan.FromMilliseconds(Math.Max(50, i + 10))).ConfigureAwait(false);
				}
			}
			return null;
		}
	}
}
