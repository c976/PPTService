using System;
using System.Collections.Generic;
using System.Linq;
using PPTServiceUltra.Ink.Core.Linq;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Storage;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils.CaretPosition;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus
{
	[Obsolete("此类是为了加快接入速度做的妥协，若要使用此API，务必知会文本小组")]
	public class LegacyApi
	{
		public class AspectRatioInfo
		{
			public RatioFontSizeInfo PreRatioFontSizeInfo { get; set; }

			public RatioFontSizeInfo CurrentRatioFontSizeInfo { get; set; }

			public AspectRatioInfo()
			{
				PreRatioFontSizeInfo = new RatioFontSizeInfo();
				CurrentRatioFontSizeInfo = new RatioFontSizeInfo();
			}
		}

		public class RatioFontSizeInfo
		{
			public List<Tuple<RunProperty, double>> RunPropertyFontSize { get; set; }

			public List<Tuple<ParagraphProperty, double>> ParagraphPropertyFontSize { get; set; }

			public RatioFontSizeInfo()
			{
				RunPropertyFontSize = new List<Tuple<RunProperty, double>>();
				ParagraphPropertyFontSize = new List<Tuple<ParagraphProperty, double>>();
			}
		}

		private readonly TextEditor _textEditor;

		public bool SelectionContainsEmptyLine
		{
			get
			{
				return _textEditor.DocumentManager.SelectionContainsEmptyLine;
			}
			set
			{
				_textEditor.DocumentManager.SelectionContainsEmptyLine = value;
			}
		}

		public event EventHandler<RichTextSaveInfo> BeforCopy;

		public LegacyApi(TextEditor editor)
		{
			_textEditor = editor;
			_textEditor.TextArea.BeforCopy += TextArea_OnBeforeCopy;
		}

		public SpecialCaretPosition GetSpecialCaretPositionByCaretOffset()
		{
			return CaretPositionExtension.GetSpecialCaretPositionByCaretOffset(_textEditor);
		}

		public void ChangeFontSizeFromPound(double poundStep)
		{
			if (!_textEditor.CurrentFeature.HasFeature(Features.IncreaseFontSizeInner) || !_textEditor.CurrentFeature.HasFeature(Features.DecreaseFontSizeInner))
			{
				return;
			}
			Action<RunProperty> action = (RunProperty p) =>
			{
				double num = Math.Round(0.75 * p.FontSize) + poundStep;
				if (!(num > 500.0) && !(num < 1.0))
				{
					p.FontSize = num * 1.333333333;
				}
			};
			_textEditor.DocumentManager.SetRunProperty(action, PropertyType.FontSize);
		}

		public static bool TextEditorsPropertyAreSame(IEnumerable<TextEditor> editors, Func<RunProperty, object> action)
		{
			object obj = null;
			foreach (TextEditor editor in editors)
			{
				foreach (RunProperty allRunProperty in editor.DocumentManager.PropertyManager.GetAllRunProperties())
				{
					if (obj == null)
					{
						obj = action(allRunProperty);
					}
					else if (!Converter.AreEquals(obj, action(allRunProperty)))
					{
						return false;
					}
				}
			}
			return true;
		}

		public void SetSelection(ISelection selection)
		{
			_textEditor.DocumentManager.Selection = selection;
		}

		public IEnumerable<int> GetSamePropertyAroundIndexes(int documentOffset, Func<RunProperty, bool> runPropertyEquals)
		{
			return _textEditor.DocumentManager.GetSamePropertyAroundIndexes(documentOffset, runPropertyEquals);
		}

		public void ChangeParagraphPropertyByIndexes(IEnumerable<int> indexes, Action<ParagraphProperty> action)
		{
			_textEditor.DocumentManager.ChangePropertyByIndexes(indexes, action);
			_textEditor.RenderManager.Redraw();
		}

		public void ChangeRunPropertyByIndexes(IEnumerable<int> indexes, Action<RunProperty> action)
		{
			_textEditor.DocumentManager.ChangePropertyByIndexes(indexes, action);
			_textEditor.RenderManager.Redraw();
		}

		public void ChangeRunPropertyBySegment(ISegment segment, Action<RunProperty> action)
		{
			List<int> list = new List<int>();
			for (int i = segment.FrontOffset; i < segment.BehindOffset; i++)
			{
				list.Add(i);
			}
			_textEditor.DocumentManager.ChangePropertyByIndexes(list, action);
			_textEditor.RenderManager.Redraw();
		}

		[Obsolete("还没实现")]
		public int GetLineEndOffsetByDocumentOffset(int documentOffset)
		{
			return 0;
		}

		[Obsolete("还没实现")]
		public bool IsHit(int position, Func<RunProperty, bool> runPropertyEquals)
		{
			return false;
		}

		[Obsolete("还没实现")]
		public void SetUnderlineGap(double gapValue)
		{
		}

		public void ApplyControlProperty(Action<RunProperty> propAction)
		{
			_textEditor.DocumentManager.PropertyManager.RunPropDic.Values.ForEach(propAction);
			_textEditor.DocumentManager.PropertyManager.ParagraphPropDic.Values.ForEach((ParagraphProperty x) =>
			{
				propAction(x.DefaultRunProperty);
			});
			propAction(_textEditor.DocumentManager.PropertyManager.CurrentRunProperty);
			propAction(_textEditor.DocumentManager.PropertyManager.CurrentParagraphProperty.DefaultRunProperty);
		}

		public bool IsAtEndOfParagraph(int offset)
		{
			return _textEditor.DocumentManager.CharsManager.ParagraphTree.Any((ParagraphNode x) => x.EndOffset == offset);
		}

		private void TextArea_OnBeforeCopy(object sender, RichTextSaveInfo e)
		{
			BeforCopy?.Invoke(_textEditor, e);
		}

		public List<int> GetRunIndexesByCondition(Func<RunProperty, bool> runPropertyEquals)
		{
			PropertyManager propertyManager = _textEditor.DocumentManager.PropertyManager;
			return propertyManager.RunProperties.Where((int x) => runPropertyEquals(propertyManager.RunPropDic[x])).ToList();
		}

		public List<int> GetRunIndexesByCondition(int startOffset, int length, Func<RunProperty, bool> runPropertyEquals)
		{
			PropertyManager propertyManager = _textEditor.DocumentManager.PropertyManager;
			List<int> list = new List<int>();
			for (int i = startOffset; i < startOffset + length; i++)
			{
				RunProperty runProperty = propertyManager.GetRunProperty(i);
				if (runPropertyEquals(runProperty))
				{
					list.Add(i);
				}
			}
			return list;
		}

		public void ChangeProperty(Action<RunProperty> propAction, PropertyType type)
		{
			_textEditor.DocumentManager.SetRunProperty(propAction, type);
		}

		public AspectRatioInfo AdaptAspectRatio(double ratio)
		{
			AspectRatioInfo aspectRatioInfo = new AspectRatioInfo();
			List<Tuple<RunProperty, double>> runPropertyFontSize = aspectRatioInfo.PreRatioFontSizeInfo.RunPropertyFontSize;
			List<Tuple<RunProperty, double>> runPropertyFontSize2 = aspectRatioInfo.CurrentRatioFontSizeInfo.RunPropertyFontSize;
			List<Tuple<ParagraphProperty, double>> paragraphPropertyFontSize = aspectRatioInfo.PreRatioFontSizeInfo.ParagraphPropertyFontSize;
			List<Tuple<ParagraphProperty, double>> paragraphPropertyFontSize2 = aspectRatioInfo.CurrentRatioFontSizeInfo.ParagraphPropertyFontSize;
			foreach (RunProperty value in _textEditor.DocumentManager.PropertyManager.RunPropDic.Values)
			{
				runPropertyFontSize.Add(new Tuple<RunProperty, double>(value, value.FontSize));
				value.FontSize *= ratio;
				runPropertyFontSize2.Add(new Tuple<RunProperty, double>(value, value.FontSize));
			}
			foreach (ParagraphProperty value2 in _textEditor.DocumentManager.PropertyManager.ParagraphPropDic.Values)
			{
				paragraphPropertyFontSize.Add(new Tuple<ParagraphProperty, double>(value2, value2.DefaultRunProperty.FontSize));
				value2.DefaultRunProperty.FontSize = value2.DefaultRunProperty.FontSize * ratio;
				paragraphPropertyFontSize2.Add(new Tuple<ParagraphProperty, double>(value2, value2.DefaultRunProperty.FontSize));
			}
			_textEditor.DocumentManager.CharsManager.GeometryPool.SetDirty(0, _textEditor.CharCount);
			_textEditor.RenderManager.Redraw();
			return aspectRatioInfo;
		}

		public void SetPropertyByRatioFontSizeInfo(RatioFontSizeInfo ratioFontSizeInfo)
		{
			List<Tuple<RunProperty, double>> runPropertyFontSize = ratioFontSizeInfo.RunPropertyFontSize;
			for (int i = 0; i < runPropertyFontSize.Count; i++)
			{
				_textEditor.DocumentManager.PropertyManager.RunPropDic[i].FontSize = runPropertyFontSize[i].Item2;
			}
			List<Tuple<ParagraphProperty, double>> paragraphPropertyFontSize = ratioFontSizeInfo.ParagraphPropertyFontSize;
			for (int j = 0; j < paragraphPropertyFontSize.Count; j++)
			{
				_textEditor.DocumentManager.PropertyManager.ParagraphPropDic[j].DefaultRunProperty.FontSize = paragraphPropertyFontSize[j].Item2;
			}
			_textEditor.DocumentManager.CharsManager.GeometryPool.SetDirty(0, _textEditor.CharCount);
			_textEditor.RenderManager.Redraw();
		}
	}
}
