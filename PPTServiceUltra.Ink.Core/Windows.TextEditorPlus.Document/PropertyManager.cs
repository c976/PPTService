using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.ComponentModel;
using PPTServiceUltra.Ink.Core.Linq;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public class PropertyManager
	{
		private readonly CharsManager _charsManager;

		private readonly Dictionary<int, RunProperty> _innerRunPropDic;

		private readonly Dictionary<int, ParagraphProperty> _innerParagraphPropDic;

		private RunProperty _currentRunProperty;

		private ParagraphProperty _currentParagraphProperty;

		internal IReadOnlyDictionary<int, RunProperty> RunPropDic { get; }

		internal IReadOnlyDictionary<int, ParagraphProperty> ParagraphPropDic { get; }

		internal List<int> ParagraphProperties { get; set; }

		internal List<int> RunProperties { get; set; }

		internal int RunPropertiesCount => RunProperties.Count;

		internal RunProperty DefaultRunProperty { get; set; }

		internal Dictionary<int, List<LineProperty>> ParagraphLineInfoRenderingData { get; set; }

		internal ParagraphProperty CurrentParagraphProperty
		{
			get
			{
				return _currentParagraphProperty;
			}
			private set
			{
				ParagraphProperty currentParagraphProperty = _currentParagraphProperty;
				_currentParagraphProperty = value;
				CurrentParagraphPropertyChanged?.Invoke(null, new ValueChangeEventArgs<ParagraphProperty>(currentParagraphProperty, value));
			}
		}

		internal RunProperty CurrentRunProperty
		{
			get
			{
				return _currentRunProperty;
			}
			private set
			{
				RunProperty currentRunProperty = _currentRunProperty;
				_currentRunProperty = value;
				CurrentRunPropertyChanged?.Invoke(null, new ValueChangeEventArgs<RunProperty>(currentRunProperty, value));
			}
		}

		internal TextWrapping TextWrapping { get; set; } = TextWrapping.Wrap;

		internal LineSpacingStrategy LineSpacingStrategy { get; set; }

		internal event ValueChangedEventHandler<RunProperty> CurrentRunPropertyChanged;

		internal event ValueChangedEventHandler<ParagraphProperty> CurrentParagraphPropertyChanged;

		internal PropertyManager(ParagraphProperty defaultParagraphProperty, CharsManager charsManager)
		{
			_charsManager = charsManager;
			_innerParagraphPropDic = new Dictionary<int, ParagraphProperty>();
			_innerRunPropDic = new Dictionary<int, RunProperty>();
			RunPropDic = new ReadOnlyDictionary<int, RunProperty>(_innerRunPropDic);
			ParagraphPropDic = new ReadOnlyDictionary<int, ParagraphProperty>(_innerParagraphPropDic);
			ParagraphLineInfoRenderingData = new Dictionary<int, List<LineProperty>>();
			SetCurrentProperty(defaultParagraphProperty);
			SetCurrentProperty(defaultParagraphProperty.DefaultRunProperty.Clone());
			ParagraphProperties = new List<int>();
			RunProperties = new List<int>();
			ParagraphProperties.Add(GetParagraphPropertyIndex(defaultParagraphProperty));
			DefaultRunProperty = new RunProperty
			{
				FontSize = 40.0,
				FontFamily = TextContext.DefaultFontFamily
			};
			_charsManager.ParagraphInserted += OnParagraphInserted;
			_charsManager.ParagraphRemoving += OnParagraphRemoving;
			_charsManager.DocumentTreeBuilt += OnDocumentTreeBuilt;
		}

		private void OnDocumentTreeBuilt(List<ParagraphNode> paragraphs)
		{
			SetCurrentProperty(ParagraphPropDic[ParagraphProperties.First()]);
			ParagraphProperties.Clear();
			for (int i = 0; i < paragraphs.Count; i++)
			{
				ParagraphProperties.Add(GetParagraphPropertyIndex(CurrentParagraphProperty));
			}
		}

		private void OnParagraphRemoving(ParagraphNode paragraph)
		{
			ParagraphProperties.RemoveAt(paragraph.ParagraphNumber - 1);
		}

		private void OnParagraphInserted(ParagraphNode paragraph)
		{
			ParagraphProperty paragraphProperty = GetParagraphProperty(paragraph.ParagraphNumber - 2).Clone();
			paragraphProperty.DefaultRunProperty = CurrentRunProperty.Clone();
			ParagraphProperties.Insert(paragraph.ParagraphNumber - 1, GetParagraphPropertyIndex(paragraphProperty));
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("请使用 GetOrRegisterRunProperty 方法代替，本方法本质调用了 GetOrRegisterRunProperty 方法")]
		internal int GetRunPropertyIndex(RunProperty runProperty)
		{
			return GetOrRegisterRunProperty(runProperty).Token;
		}

		internal RunPropertyToken GetOrRegisterRunProperty(RunProperty runProperty)
		{
			if (_innerRunPropDic.ContainsValue(runProperty))
			{
				return new RunPropertyToken(_innerRunPropDic.Keys.First((int x) => Converter.AreEquals(runProperty, _innerRunPropDic[x])), this);
			}
			return RegisterRunPropertyToken(runProperty);
		}

		internal RunPropertyToken RegisterRunPropertyToken(RunProperty runProperty)
		{
			_innerRunPropDic.Add(_innerRunPropDic.Count, runProperty);
			return new RunPropertyToken(_innerRunPropDic.Count - 1, this);
		}

		internal int GetParagraphPropertyIndex(ParagraphProperty paragraphProperty)
		{
			if (_innerParagraphPropDic.ContainsValue(paragraphProperty))
			{
				return _innerParagraphPropDic.Keys.First((int x) => Converter.AreEquals(paragraphProperty, _innerParagraphPropDic[x]));
			}
			_innerParagraphPropDic.Add(_innerParagraphPropDic.Count, paragraphProperty);
			return _innerParagraphPropDic.Count - 1;
		}

		internal RunProperty GetRenderRunProperty(DocumentOffset offset)
		{
			return GetRunProperty(offset).RenderRunProperty;
		}

		internal RunProperty GetRenderDefaultRunProperty(RunProperty runProperty)
		{
			if (runProperty == null)
			{
				throw new ArgumentNullException("runProperty");
			}
			if (runProperty.EffectsArgs.Any())
			{
				runProperty.EffectsArgs.ForEach((ITextEffectArgs x) =>
				{
					x.Opacity = runProperty.Opacity;
				});
				Brush brush = runProperty.EffectsArgs.OfType<FillEffectArgs>().FirstOrDefault()?.FillBrush;
				if (brush != null)
				{
					if (!brush.IsFrozen)
					{
						brush.Opacity = runProperty.Opacity;
					}
					runProperty.Foreground = brush.Clone();
				}
			}
			else
			{
				if (runProperty.Foreground == null)
				{
					runProperty.Foreground = TextContext.DefaultForeground;
				}
				Brush brush2 = runProperty.Foreground.CloneCurrentValue();
				if (!brush2.IsFrozen)
				{
					brush2.Opacity = runProperty.Opacity;
				}
				runProperty.Foreground = brush2;
			}
			return runProperty;
		}

		internal RunProperty GetRunProperty(DocumentOffset offset)
		{
			if (!RunProperties.Any())
			{
				return CurrentRunProperty;
			}
			if (offset == DocumentOffset.DefaultDocumentOffset)
			{
				return GetParagraphProperty(0).DefaultRunProperty;
			}
			return RunPropDic[RunProperties[offset]];
		}

		internal IEnumerable<RunProperty> GetAllRunProperties()
		{
			foreach (int runProperty in RunProperties)
			{
				yield return RunPropDic[runProperty];
			}
		}

		internal ParagraphProperty GetParagraphProperty(int offset)
		{
			if (ParagraphProperties.Any())
			{
				return ParagraphPropDic[ParagraphProperties[offset]];
			}
			return CurrentParagraphProperty;
		}

		internal void SetCurrentProperty(RunProperty prop)
		{
			if (!Converter.AreEquals(prop, CurrentRunProperty))
			{
				CurrentRunProperty = prop.Clone();
			}
		}

		internal void SetCurrentProperty(ParagraphProperty prop)
		{
			if (!Converter.AreEquals(prop, CurrentParagraphProperty))
			{
				CurrentParagraphProperty = prop.Clone();
			}
		}

		internal void ClearParagraphProperties()
		{
			_innerParagraphPropDic.Clear();
		}

		internal void ClearRunProperties()
		{
			_innerRunPropDic.Clear();
		}

		public void SetRunProperty(DocumentOffset documentOffset, RunProperty runProperty)
		{
			RunPropertyToken runPropertyToken = GetOrRegisterRunProperty(runProperty);
			SetRunProperty(documentOffset, ref runPropertyToken);
		}

		public void SetRunProperty(DocumentOffset documentOffset, [In] ref RunPropertyToken runPropertyToken)
		{
			if (runPropertyToken.PropertyManager != this)
			{
				throw new ArgumentException("此 RunPropertyToken 没有在当前 PropertyManager 注册，请先调用 GetOrRegisterRunProperty 方法注册");
			}
			if (documentOffset.Offset > RunProperties.Count)
			{
				throw new ArgumentOutOfRangeException(string.Format("当前 {0} 长度为 RunProperties.Count={1} 传入 currentDocumentOffset 为 currentDocumentOffset={2}", "RunProperties", RunProperties.Count, documentOffset.Offset));
			}
			RunProperties[documentOffset.Offset] = runPropertyToken.Token;
		}

		public void SetRunProperties(ISegment documentSegment, RunProperty runProperty)
		{
			RunPropertyToken runPropertyToken = GetOrRegisterRunProperty(runProperty);
			SetRunProperties(documentSegment, ref runPropertyToken);
		}

		public void SetRunProperties(ISegment documentSegment, [In] ref RunPropertyToken runPropertyToken)
		{
			if (runPropertyToken.PropertyManager != this)
			{
				throw new ArgumentException("此 RunPropertyToken 没有在当前 PropertyManager 注册，请先调用 GetOrRegisterRunProperty 方法注册");
			}
			if (documentSegment.FrontOffset == documentSegment.BehindOffset)
			{
				if (DebuggingProperties.IsDebug)
				{
					throw new ArgumentException("documentSegment", $"传入的修改范围是 0 documentSegment 的 FrontOffset 和 BehindOffset 值相等，都是 {documentSegment.FrontOffset} 此时调用此方法无效。仅调试下抛出，请找德熙");
				}
				return;
			}
			List<int> runProperties = RunProperties;
			if (documentSegment.FrontOffset >= runProperties.Count || documentSegment.BehindOffset > runProperties.Count)
			{
				throw new ArgumentOutOfRangeException("documentSegment", string.Format("当前 {0} 长度为 RunProperties.Count={1} 传入 documentSegment 为 documentSegment.FrontOffset={2} documentSegment.BehindOffset={3}", "RunProperties", RunProperties.Count, documentSegment.FrontOffset, documentSegment.BehindOffset));
			}
			for (int i = documentSegment.FrontOffset; i < documentSegment.BehindOffset; i++)
			{
				runProperties[i] = runPropertyToken.Token;
			}
		}

		public void SetAllRunProperties([In] ref RunPropertyToken runPropertyToken)
		{
			if (RunProperties.Count != 0)
			{
				SimpleSegment documentSegment = new SimpleSegment(0, RunProperties.Count);
				SetRunProperties(documentSegment, ref runPropertyToken);
			}
		}

		internal void ReplaceWithCurrentProperty(ISegment segment, int insertLength)
		{
			RunProperties.RemoveRange(segment.StartOffset, segment.Length);
			RunProperty runProperty = CurrentRunProperty.Clone();
			RunPropertyToken orRegisterRunProperty = GetOrRegisterRunProperty(runProperty);
			for (int i = 0; i < insertLength; i++)
			{
				RunProperties.Insert(segment.StartOffset + i, orRegisterRunProperty.Token);
			}
			ParagraphNode paragraphNodeByDocumentOffset = _charsManager.ParagraphTree.GetParagraphNodeByDocumentOffset(segment.StartOffset);
			ParagraphProperty paragraphProperty = GetParagraphProperty(paragraphNodeByDocumentOffset.ParagraphNumber - 1);
			if (paragraphProperty.MarkerProperty != null && insertLength == 0 && !paragraphProperty.MarkerProperty.IsEmpty() && segment.FrontOffset == paragraphNodeByDocumentOffset.StartOffset && segment.Length == 0)
			{
				ParagraphProperty paragraphProperty2 = paragraphProperty.Clone();
				paragraphProperty2.MarkerProperty.Clear();
				paragraphProperty2.Indent = 0.0;
				paragraphProperty2.MarginLeft = (double)paragraphProperty2.IndentLevel * 24.0 * 1.4;
				ParagraphProperties[paragraphNodeByDocumentOffset.ParagraphNumber - 1] = GetParagraphPropertyIndex(paragraphProperty2);
				SetCurrentProperty(paragraphProperty2);
			}
		}

		internal void ReplaceWithRichTextProperty(ISegment segment, RichTextProperty richTextProperty)
		{
			RunProperties.RemoveRange(segment.StartOffset, segment.Length);
			List<int> list = new List<int>(richTextProperty.RunProperties.Count);
			foreach (RunProperty runProperty in richTextProperty.RunProperties)
			{
				list.Add(GetOrRegisterRunProperty(runProperty).Token);
			}
			RunProperties.InsertRange(segment.StartOffset, list);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ICharObject charObject in richTextProperty.CharObjects)
			{
				stringBuilder.Append(charObject.ToText());
			}
			int count = stringBuilder.ToString().Split(new string[1] { "\r\n" }, StringSplitOptions.None).Length - 1;
			ParagraphNode paragraphNodeByDocumentOffset = _charsManager.ParagraphTree.GetParagraphNodeByDocumentOffset(segment.FrontOffset);
			ParagraphProperties.RemoveRange(paragraphNodeByDocumentOffset.ParagraphNumber, count);
			if (paragraphNodeByDocumentOffset.StartOffset != segment.FrontOffset)
			{
				richTextProperty.ParagraphProperties.RemoveAt(0);
				richTextProperty.ParagraphProperties.Insert(0, GetParagraphProperty(paragraphNodeByDocumentOffset.ParagraphNumber - 1));
			}
			ParagraphProperties.RemoveAt(paragraphNodeByDocumentOffset.ParagraphNumber - 1);
			List<int> insertingLineIndex = new List<int>();
			richTextProperty.ParagraphProperties.ForEach((ParagraphProperty x) =>
			{
				insertingLineIndex.Add(GetParagraphPropertyIndex(x));
			});
			if (_charsManager.CharCount == 0)
			{
				ParagraphProperties.Clear();
			}
			ParagraphProperties.InsertRange(paragraphNodeByDocumentOffset.ParagraphNumber - 1, insertingLineIndex);
			SetCurrentProperty(richTextProperty.CharObjects.Any() ? richTextProperty.RunProperties.Last() : richTextProperty.ParagraphProperties.Last().DefaultRunProperty);
		}

		internal void ApplyProperty(ParagraphProperty prop)
		{
			if (prop == null)
			{
				throw new ArgumentNullException("prop", "prop can not be null");
			}
			int paragraphPropertyIndex = GetParagraphPropertyIndex(prop.Clone());
			int count = ParagraphProperties.Count;
			ParagraphProperties.Clear();
			for (int i = 0; i < count; i++)
			{
				ParagraphProperties.Add(paragraphPropertyIndex);
			}
		}

		public void ApplyProperty(RunProperty runProperty)
		{
			if (runProperty == null)
			{
				throw new ArgumentNullException("runProperty", "runProp can not be null");
			}
			RunPropertyToken runPropertyToken = GetOrRegisterRunProperty(runProperty);
			SetAllRunProperties(ref runPropertyToken);
			_charsManager.GeometryPool.SetDirty(0, _charsManager.CharCount);
		}
	}
}
