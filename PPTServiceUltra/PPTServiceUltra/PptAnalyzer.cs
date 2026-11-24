using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Office.Interop.PowerPoint;

namespace PPTServiceUltra
{
	public static class PptAnalyzer
	{
		private static bool _turn;

		private static int _count;

		private static string _md5;

		private static Thread _thread;

		public static bool IsBreakExport;

		private static int _waitTime = 999;

		private static bool _isAnalyzingAll;

		private static bool _isBreakExportAsync;

		public static string ThumbnailFilePath;

		public static double ThumbnailRate = 1.7;

		private static int _thumbnailWidth = 960;

		private static int _thumbnailHeight = 540;

		public static int CurrentShowPosition = 1;

		public static AnalyzeRate CurrentAnalyzeRate;

		public static readonly string JpgSuffix = ".jpg";

		public static readonly string FilterName = "*.jpg";

		private static readonly object AnalyzeLocker = new object();

		public static Dictionary<int, string> AnalyzedIndexAndThumbnailFilePathDictionary = new Dictionary<int, string>();

		private static readonly Dictionary<string, string> ThumbnailFilePathDictionary = new Dictionary<string, string>();

		private static readonly Dictionary<string, string> AnalyzeFinishedFilePathDictionary = new Dictionary<string, string>();

		private static readonly Dictionary<string, string> AnalyzeFinishedThumbnailPathDictionary = new Dictionary<string, string>();

		private static readonly Dictionary<string, Dictionary<int, string>> PptMd5AndAnalyzedIndexDictionary = new Dictionary<string, Dictionary<int, string>>();

		public static bool IsAnalyzeAllAsyncRunning
		{
			get
			{
				if (_thread != null)
				{
					return _thread.IsAlive;
				}
				return false;
			}
		}

		public static void StartAnalyzeAllAsync(AnalyzeRate rate)
		{
			if (!IsAnalyzeOver() && !_isAnalyzingAll)
			{
				if (!IsAnalyzeAllAsyncRunning && PptOperator.Instance.IsShowing)
				{
					_thread = new Thread(AnalyzeAsync)
					{
						IsBackground = true
					};
					_isBreakExportAsync = false;
					_thread.Start();
				}
				CurrentAnalyzeRate = rate;
				switch (CurrentAnalyzeRate)
				{
				case AnalyzeRate.Highest:
					_waitTime = 0;
					break;
				case AnalyzeRate.Middle:
					_waitTime = 333;
					break;
				case AnalyzeRate.Lowest:
					_waitTime = 999;
					break;
				}
			}
		}

		public static void StopAnalyzeAllAsyn()
		{
			if (IsAnalyzeAllAsyncRunning)
			{
				_isBreakExportAsync = true;
			}
		}

		public static bool SeewoLinkRequest(int startIndex, int range = 4)
		{
			if (startIndex + range < 1 || startIndex - range > PptOperator.Instance.SlidesCount)
			{
				return false;
			}
			StopAnalyzeAllAsyn();
			bool result = PptExportRangeImage(PptOperator.Instance.PptFilePath, null, startIndex, range);
			CurrentShowPosition = startIndex;
			return result;
		}

		private static void AnalyzeAsync()
		{
			if (IsAnalyzeOver())
			{
				return;
			}
			while (!IsAnalyzeOver() && !_isBreakExportAsync && PptOperator.Instance.IsShowing)
			{
				bool flag = true;
				if (_turn)
				{
					for (int num = CurrentShowPosition; num >= 1; num--)
					{
						if (!AnalyzedIndexAndThumbnailFilePathDictionary.ContainsKey(num))
						{
							flag = PptExportRangeImage(PptOperator.Instance.PptFilePath, null, num, 0);
							break;
						}
					}
				}
				else
				{
					for (int i = CurrentShowPosition; i <= PptOperator.Instance.SlidesCount; i++)
					{
						if (!AnalyzedIndexAndThumbnailFilePathDictionary.ContainsKey(i))
						{
							flag = PptExportRangeImage(PptOperator.Instance.PptFilePath, null, i, 0);
							break;
						}
					}
				}
				if (!flag)
				{
					break;
				}
				_count++;
				if (_count > 5)
				{
					_count = 0;
					UpdateListBoxItem();
				}
				Thread.Sleep(_waitTime);
				_turn = !_turn;
			}
			UpdateListBoxItem();
		}

		public static void InitPptAnalyzer(string pptFilePath)
		{
			List<string> imageFiles;
			FindAlreadyAnalyzedFiles(pptFilePath, out imageFiles);
			CreateOrGetAnalyzedIndexList();
			CreateOrGetThumbnailFilePath();
			SetThumbnailSize(PptOperator.Instance.Slides.FirstOrDefault());
		}

		private static void UpdateListBoxItem()
		{
			SyntacticSugar.TryCatchAction(() =>
			{
				SyntacticSugar.CurrentDispatcherBeginInvoke(() =>
				{
					string[] array = new string[PptOperator.Instance.SlidesCount];
					foreach (KeyValuePair<int, string> item in AnalyzedIndexAndThumbnailFilePathDictionary)
					{
						int num = item.Key - 1;
						if (num >= 0 && num <= array.Length - 1)
						{
							array[num] = item.Value;
						}
					}
					ViewHelper.MainWindow.UpdateListBoxItem(array.ToList());
				});
			});
		}

		public static void PptExportAllImages(string pptFilePath, Action<double> exportProgress, out List<string> imageFiles)
		{
			lock (AnalyzeLocker)
			{
				_isAnalyzingAll = true;
				IsBreakExport = false;
				imageFiles = new List<string>();
				StopAnalyzeAllAsyn();
				try
				{
					// ROT 通道拿不到强类型 Slide 列表，按页数逐页走通道自身的后期绑定导出。
					IList<Slide> availableSlides = PptOperator.Instance.Slides;
					int total = PptOperator.Instance.SlidesCount;
					for (int index = 1; index <= total; index++)
					{
						if (IsBreakExport)
						{
							break;
						}
						if (AnalyzedIndexAndThumbnailFilePathDictionary.ContainsKey(index))
						{
							imageFiles.Add(AnalyzedIndexAndThumbnailFilePathDictionary[index]);
							continue;
						}
						string text = Path.Combine(ThumbnailFilePath, index + JpgSuffix);
						string usedPath;
						if (!ExportOneThumbnail(index, text, out usedPath))
						{
							continue;
						}
						imageFiles.Add(usedPath);
						AnalyzedIndexAndThumbnailFilePathDictionary.Add(index, usedPath);
						exportProgress?.Invoke((double)index / (double)total);
					}
					UpdateListBoxItem();
					if (IsBreakExport)
					{
						imageFiles = new List<string>();
						_isAnalyzingAll = false;
						IsBreakExport = false;
						return;
					}
					AddInfoToDictionary(pptFilePath);
					imageFiles = imageFiles.OrderBy((string x) => Convert.ToInt32(Path.GetFileNameWithoutExtension(x))).ToList();
					exportProgress?.Invoke(1.0);
				}
				catch (Exception)
				{
					imageFiles = new List<string>();
				}
				finally
				{
					IsBreakExport = false;
					_isAnalyzingAll = false;
					GarbageCollection.Collect();
				}
			}
		}

		public static bool PptExportRangeImage(string pptFilePath, Action<double> exportProgress, int startIndex, int range = 4)
		{
			lock (AnalyzeLocker)
			{
				int slidesCount = PptOperator.Instance.SlidesCount;
				int num = startIndex - range;
				int num2 = startIndex + range;
				if (num <= 0)
				{
					num2 -= num - 1;
				}
				if (num2 > slidesCount)
				{
					num -= num2 - slidesCount;
				}
				if (num2 < num)
				{
					return false;
				}
				if (num < 1)
				{
					num = 1;
				}
				if (num2 > slidesCount)
				{
					num2 = slidesCount;
				}
				try
				{
					for (int slideIndex = num; slideIndex <= num2; slideIndex++)
					{
						if (AnalyzedIndexAndThumbnailFilePathDictionary.ContainsKey(slideIndex))
						{
							continue;
						}
						string text = Path.Combine(ThumbnailFilePath, slideIndex + JpgSuffix);
						string usedPath;
						if (ExportOneThumbnail(slideIndex, text, out usedPath))
						{
							AnalyzedIndexAndThumbnailFilePathDictionary.Add(slideIndex, usedPath);
						}
						exportProgress?.Invoke((double)(slideIndex - num + 1) / ((double)(num2 - num + 1)));
					}
					AddInfoToDictionary(pptFilePath);
					exportProgress?.Invoke(1.0);
				}
				catch (Exception)
				{
					return false;
				}
				finally
				{
					GarbageCollection.Collect();
				}
				return true;
			}
		}

		/// <summary>
		/// 导出单页缩略图：优先走当前通道的后期绑定导出（ROT 通道的 COM 对象
		/// 无法强转成 Slide 接口），失败再回退到强类型 Slide.Export。
		/// </summary>
		private static bool ExportOneThumbnail(int slideIndex, string filePath, out string usedPath)
		{
			usedPath = filePath;
			if (PptOperator.Instance.ExportThumbnail(slideIndex, filePath, _thumbnailWidth, _thumbnailHeight))
			{
				return true;
			}
			IList<Slide> slides = PptOperator.Instance.Slides;
			int position = slideIndex - 1;
			if (slides == null || position < 0 || position >= slides.Count || slides[position] == null)
			{
				return false;
			}
			try
			{
				slides[position].Export(filePath, JpgSuffix, _thumbnailWidth, _thumbnailHeight);
				return File.Exists(filePath);
			}
			catch
			{
				return false;
			}
		}

		private static void SetThumbnailSize(Slide slide)
		{
			if (slide != null)
			{
				ThumbnailRate = slide.Master.Width / slide.Master.Height;
				_thumbnailWidth = Convert.ToInt32((double)_thumbnailHeight * ThumbnailRate);
			}
		}

		private static bool IsAnalyzeOver()
		{
			return AnalyzedIndexAndThumbnailFilePathDictionary.Count == PptOperator.Instance.SlidesCount;
		}

		private static void AddInfoToDictionary(string pptFilePath)
		{
			if (!string.IsNullOrEmpty(_md5) && IsAnalyzeOver())
			{
				if (!AnalyzeFinishedFilePathDictionary.ContainsKey(_md5))
				{
					AnalyzeFinishedFilePathDictionary.Add(_md5, pptFilePath);
				}
				if (!AnalyzeFinishedThumbnailPathDictionary.ContainsKey(_md5))
				{
					AnalyzeFinishedThumbnailPathDictionary.Add(_md5, ThumbnailFilePath);
				}
			}
		}

		private static void CreateOrGetThumbnailFilePath()
		{
			if (string.IsNullOrEmpty(_md5))
			{
				ThumbnailFilePath = Path.Combine(AppPath.TemPath(), Guid.NewGuid().ToString());
				if (!Directory.Exists(ThumbnailFilePath))
				{
					Directory.CreateDirectory(ThumbnailFilePath);
				}
			}
			else if (!ThumbnailFilePathDictionary.ContainsKey(_md5))
			{
				ThumbnailFilePath = Path.Combine(AppPath.TemPath(), Guid.NewGuid().ToString());
				if (!Directory.Exists(ThumbnailFilePath))
				{
					Directory.CreateDirectory(ThumbnailFilePath);
				}
				// 用索引器赋值代替 Add：快速连续放映（ShowBegin 重入）时两个线程可能
				// 同时通过 ContainsKey 检查，Add 会抛「已添加了具有相同键的项」。
				ThumbnailFilePathDictionary[_md5] = ThumbnailFilePath;
			}
			else
			{
				string value;
				ThumbnailFilePathDictionary.TryGetValue(_md5, out value);
				ThumbnailFilePath = value;
			}
		}

		private static void CreateOrGetAnalyzedIndexList()
		{
			if (string.IsNullOrEmpty(_md5))
			{
				AnalyzedIndexAndThumbnailFilePathDictionary = new Dictionary<int, string>();
			}
			else if (!PptMd5AndAnalyzedIndexDictionary.ContainsKey(_md5))
			{
				AnalyzedIndexAndThumbnailFilePathDictionary = new Dictionary<int, string>();
				PptMd5AndAnalyzedIndexDictionary.Add(_md5, AnalyzedIndexAndThumbnailFilePathDictionary);
			}
			else
			{
				Dictionary<int, string> value;
				PptMd5AndAnalyzedIndexDictionary.TryGetValue(_md5, out value);
				AnalyzedIndexAndThumbnailFilePathDictionary = value;
			}
		}

		public static bool FindAlreadyAnalyzedFiles(string filePath, out List<string> imageFiles)
		{
			try
			{
				imageFiles = new List<string>();
				if (!RegexHelper.IsLocalPath(filePath))
				{
					_md5 = null;
					return false;
				}
				if (!File.Exists(filePath) && AnalyzeFinishedFilePathDictionary.ContainsValue(filePath))
				{
					_md5 = AnalyzeFinishedFilePathDictionary.FirstOrDefault((KeyValuePair<string, string> d) => d.Value == filePath).Key;
				}
				else if (File.Exists(filePath))
				{
					_md5 = Md5SecurityUtility.GetShaFromFile(filePath);
				}
				if (string.IsNullOrEmpty(_md5))
				{
					return false;
				}
				string text = (AnalyzeFinishedThumbnailPathDictionary.ContainsKey(_md5) ? AnalyzeFinishedThumbnailPathDictionary.FirstOrDefault((KeyValuePair<string, string> d) => d.Key == _md5).Value : null);
				if (text == null)
				{
					return false;
				}
				DirectoryInfo directoryInfo = new DirectoryInfo(text);
				if (directoryInfo.GetFiles(FilterName).Any())
				{
					imageFiles.AddRange((from imageFile in directoryInfo.GetFiles(FilterName)
						orderby Convert.ToInt32(Path.GetFileNameWithoutExtension(imageFile.Name))
						select imageFile.FullName).ToList());
					ThumbnailFilePath = text;
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				imageFiles = new List<string>();
				Log.WriteError(ex.ToString());
				return false;
			}
		}
	}
}
