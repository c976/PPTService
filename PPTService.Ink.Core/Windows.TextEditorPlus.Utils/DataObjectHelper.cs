using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using PPTService.Ink.Core.IO;
using PPTService.Ink.Core.Windows.Media.Imaging;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Editing;
using PPTService.Ink.Core.Windows.TextEditorPlus.Extensions;
using PPTService.Ink.Core.Windows.TextEditorPlus.Extensions.SupportedExtChars;
using PPTService.Ink.Core.Windows.TextEditorPlus.Storage;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils.Paste;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	public static class DataObjectHelper
	{
		public const string InnerTextFormat = "CoreLibWindowsTextEditor";

		public static System.Windows.DataObject CreateDataObjectFromSelection(TextArea textArea)
		{
			System.Windows.DataObject dataObject = new System.Windows.DataObject();
			ISegment segment = textArea.DocumentManager.Selection.ToSegment();
			List<int> paragraphIndexBySegment = TextHelperInner.GetParagraphIndexBySegment(textArea.DocumentManager.CharsManager, segment);
			List<ParagraphSaveInfo> list = new List<ParagraphSaveInfo>();
			int num = segment.FrontOffset;
			foreach (int item in paragraphIndexBySegment)
			{
				ParagraphNode paragraphNodeByNumber = textArea.DocumentManager.CharsManager.ParagraphTree.GetParagraphNodeByNumber(item + 1);
				num = ((item == paragraphIndexBySegment.First()) ? (num - paragraphNodeByNumber.StartOffset) : 0);
				int totalLength = ((item == paragraphIndexBySegment.Last()) ? (segment.BehindOffset - paragraphNodeByNumber.StartOffset) : paragraphNodeByNumber.TotalLength);
				ParagraphSaveInfo paragraphSaveInfo = TextHelperInner.GetParagraphSaveInfo(textArea.DocumentManager, item, num, totalLength);
				list.Add(paragraphSaveInfo);
			}
			RichTextSaveInfo saveInfo = new RichTextSaveInfo
			{
				ArrangingType = textArea.DocumentManager.ArrangingType,
				SizeToContent = textArea.DocumentManager.SizeToContent,
				VerticalTextAlignment = textArea.DocumentManager.TextVerticalAlignment,
				Paragraphs = list
			};
			textArea.OnBeforCopy(saveInfo);
			string data = TextContext.StorageParser.Deparse(saveInfo);
			dataObject.SetData("CoreLibWindowsTextEditor", data);
			dataObject.SetData("source", textArea.VisualAncestor<TextEditor>());
			SetPlainText(dataObject, textArea);
			return dataObject;
		}

		private static void SetPlainText(System.Windows.DataObject dataObject, TextArea textArea)
		{
			ISelection selection = textArea.DocumentManager.Selection;
			string text = textArea.DocumentManager.CharsManager.GetText(selection.FrontOffset.Offset + 1, selection.Length);
			dataObject.SetData(System.Windows.DataFormats.Text, text);
		}

		public static RichTextProperty DigRichData(System.Windows.Forms.IDataObject dataObject, bool isExtCharSupported, RunProperty extCharRunProperty)
		{
			return DigRichDataInner(new DataObjectAdapter(dataObject), isExtCharSupported, extCharRunProperty);
		}

		public static RichTextProperty DigRichData(System.Windows.IDataObject dataObject, bool isExtCharSupported, RunProperty extCharRunProperty)
		{
			return DigRichDataInner(new DataObjectAdapter(dataObject), isExtCharSupported, extCharRunProperty);
		}

		internal static RichTextProperty DigRichDataInner(DataObjectAdapter dataObject, bool isExtCharSupported, RunProperty extCharRunProperty)
		{
			if (dataObject == null)
			{
				throw new ArgumentNullException("dataObject");
			}
			RichTextProperty result = new RichTextProperty();
			try
			{
				DataObjectContext dataObjectContext = new DataObjectContext(dataObject);
				DigRichDataFilter[] array = new DigRichDataFilter[4]
				{
					new InnerTextFormatDigRichDataFilter(),
					new HtmlDigRichDataFilter(),
					new RtfDigRichDataFilter(),
					new PlainTextDigRichDataFilter()
				};
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Handle(dataObjectContext))
					{
						return dataObjectContext.RichTextProperty;
					}
				}
				if (isExtCharSupported)
				{
					return result;
				}
			}
			catch (OutOfMemoryException e)
			{
				Log.Error(e);
			}
			return result;
		}

		private static RichTextProperty HandleImageChar(RunProperty runProperty, List<string> imagePathList)
		{
			RichTextProperty richTextProperty = new RichTextProperty();
			List<ICharObject> list = new List<ICharObject>();
			foreach (string imagePath in imagePathList)
			{
				string extension = Path.GetExtension(imagePath);
				if (extension != null && InnerTextContext.ImageSupportedType.Contains(extension.ToLowerInvariant()) && File.Exists(imagePath))
				{
					System.Windows.Size textCharSize = TextEditorExtManager.GetTextCharSize(runProperty);
					double imageWidthWithExtractHeight = TextHelperInner.GetImageWidthWithExtractHeight(imagePath, textCharSize.Height);
					ImageCharSaveInfo saveInfo = new ImageCharSaveInfo
					{
						Source = new Uri(imagePath),
						Size = new System.Windows.Size(imageWidthWithExtractHeight, textCharSize.Height),
						LogicalCenter = new System.Windows.Point(0.0, textCharSize.Height / 2.0)
					};
					ImageChar imageChar = new ImageChar();
					imageChar.SetSaveInfo(saveInfo);
					list.Add(imageChar);
				}
			}
			richTextProperty.CharObjects = list;
			return richTextProperty;
		}

		private static string SaveBitmap(DataObjectAdapter dataObject, string format)
		{
			try
			{
				string text = Path.Combine(TextContext.TempFolder, Guid.NewGuid().ToString() + ".png");
				object data = dataObject.GetData(format);
				BitmapSource bitmapSource = data as BitmapSource;
				if (bitmapSource != null)
				{
					bitmapSource.ToBitmap()?.Save(text, System.Drawing.Imaging.ImageFormat.Png);
				}
				else
				{
					(data as Bitmap)?.Save(text, System.Drawing.Imaging.ImageFormat.Png);
				}
				return text;
			}
			catch (Exception exception)
			{
				TextTracer.Error(exception, "文本粘贴获取剪贴板图片");
			}
			return null;
		}

		internal static string DigPlainData(DataObjectAdapter dataObject)
		{
			if (dataObject == null)
			{
				throw new ArgumentNullException("dataObject");
			}
			string result = string.Empty;
			try
			{
				if (dataObject.GetDataPresent("CoreLibWindowsTextEditor"))
				{
					string text = dataObject.GetData("CoreLibWindowsTextEditor").ToString();
					RichTextSaveInfo richTextSaveInfo = TextContext.StorageParser.Parse(text) as RichTextSaveInfo;
					if (richTextSaveInfo != null)
					{
						result = GetText(richTextSaveInfo);
					}
				}
				else if (dataObject.GetDataPresent(System.Windows.DataFormats.Rtf))
				{
					string text2 = dataObject.GetData(System.Windows.DataFormats.Rtf) as string;
					if (text2 != null)
					{
						text2 = EnSureRtfStringValid(text2);
						result = GetText(text2.ToFlowDocument().ToRichTextInfo());
					}
				}
				else if (dataObject.GetDataPresent(System.Windows.DataFormats.UnicodeText))
				{
					result = (string)dataObject.GetData(System.Windows.DataFormats.UnicodeText);
				}
				else if (dataObject.GetDataPresent(System.Windows.DataFormats.Text))
				{
					result = (string)dataObject.GetData(System.Windows.DataFormats.Text);
				}
				return result;
			}
			catch (OutOfMemoryException)
			{
				return result;
			}
		}

		private static string EnSureRtfStringValid(string rtfString)
		{
			int num = rtfString.LastIndexOf("}", StringComparison.Ordinal);
			if (num <= 0)
			{
				return rtfString;
			}
			return rtfString.Remove(num);
		}

		private static string GetText(RichTextSaveInfo saveInfo)
		{
			StringBuilder str = new StringBuilder();
			saveInfo.Paragraphs.ForEach((ParagraphSaveInfo paragraph) =>
			{
				paragraph.TextRuns.ForEach((RunSaveInfo run) =>
				{
					str.Append(run.Text);
				});
			});
			return str.ToString();
		}
	}
}
