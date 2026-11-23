using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using PPTServiceUltra.Ink.Core.ComponentModel;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Ink.Core.Linq;
using PPTServiceUltra.Ink.Core.Threading.Reentrancy;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Extensions;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class RenderManager : ITextGeometryRender
	{
		private readonly ParagraphVisualMappingCache _paragraphVisualMappingCache = new ParagraphVisualMappingCache();

		private readonly KeepLastReentrancyTask<object, object> _reentrancyTask;

		private ISegment _redrawSegment = SimpleSegment.Invalid;

		private List<ParagraphBase> _paragraphs = new List<ParagraphBase>();

		private readonly Dispatcher _dispatcher;

		public bool IsCharGeometryCacheable { get; set; }

		public double DeltaOutlineTop
		{
			get
			{
				if (Paragraphs.Count > 0)
				{
					return Paragraphs.First().DeltaOutlineTop;
				}
				return 0.0;
			}
		}

		public double DeltaOutlineBottom
		{
			get
			{
				if (Paragraphs.Count > 0)
				{
					return Paragraphs.Last().DeltaOutlineBottom;
				}
				return 0.0;
			}
		}

		internal double DocumentWidth { get; set; }

		internal double DocumentHeight { get; set; }

		internal Size DocumentOutlineSize => new Size(DocumentWidth, DocumentHeight + DeltaOutlineTop + DeltaOutlineBottom);

		internal DocumentManager DocumentManager { get; }

		internal IReadOnlyList<ParagraphBase> Paragraphs => _paragraphs;

		public bool IsRedrawEnabled { get; set; } = true;

		internal event EventHandler<List<ParagraphLayoutData>> RenderCompleted;

		internal event EventHandler<Dictionary<int, double>> UpdateMarkerOpacity;

		internal void CleanParagraphs()
		{
			_paragraphs.Clear();
		}

		private void SetParagraphs(List<ParagraphBase> paragraphs)
		{
			_paragraphs = paragraphs;
		}

		private void OnUpdateMarkerOpacity(Dictionary<int, double> opacityList)
		{
			UpdateMarkerOpacity?.Invoke(this, opacityList);
		}

		internal RenderManager(DocumentManager documentManager)
		{
			DocumentManager = documentManager;
			DocumentManager.DocumentChanged += DocumentManager_OnDocumentChanged;
			DocumentManager.StyleChanged += DocumentManager_OnStyleChanged;
			DocumentManager.ArrangingTypeChanged += DocumentManager_OnArrangingTypeChanged;
			RenderCompleted += (object sender, List<ParagraphLayoutData> list) =>
			{
				DocumentManager.IsDocumentBuilding = false;
			};
			_reentrancyTask = new KeepLastReentrancyTask<object, object>(RedrawTask);
			_dispatcher = Dispatcher.CurrentDispatcher;
			DocumentManager.CaretInfo.OffsetChanged += DocumentManager_OnCaretInfoOnOffsetChanged;
			DocumentManager.SelectionChanged += DocumentManagerOnSelectionChanged;
		}

		private void DocumentManagerOnSelectionChanged(object sender, ValueChangeEventArgs<ISelection> e)
		{
			UpdateMarkerState();
		}

		private void DocumentManager_OnCaretInfoOnOffsetChanged(object sender, ValueChangeEventArgs<CaretOffset> e)
		{
			UpdateMarkerState();
		}

		private void DocumentManager_OnStyleChanged(object sender, StyleChangeEventArgs obj)
		{
			if (InnerTextContext.AffectArrangePropertyTypes.Contains(obj.ChangedProperty))
			{
				DocumentManager.CharsManager.GeometryPool.DirtyGeometriesFromStyleChanged(obj);
			}
			TextTracer.Debug("因为样式改变，文本进行重新渲染", "文本渲染");
			Redraw(obj.ChangedSegment);
		}

		private void DocumentManager_OnDocumentChanged(object sender, CharChangeEventArgs obj)
		{
			DocumentManager.CharsManager.GeometryPool.DirtyGeometriesFromCharChanged(obj);
			Redraw(new SimpleSegment(obj.Offset, obj.InsertLength));
		}

		private void DocumentManager_OnArrangingTypeChanged(object sender, ValueChangeEventArgs<ArrangingType> e)
		{
			DocumentManager.CharsManager.GeometryPool.CorrectGeometry(DocumentManager.CharsManager, e.OldValue, e.NewValue);
			DocumentManager.UpdateExtensionalCharsSize();
		}

		internal void Redraw()
		{
			Redraw(new SimpleSegment(0, DocumentManager.CharsManager.CharCount));
		}

		internal void Redraw(ISegment segment)
		{
			InvalidateParagraphs(segment);
			_redrawSegment = segment;
			_reentrancyTask.InvokeAsync(null);
		}

		private async Task<object> RedrawTask(object parameter)
		{
			await _dispatcher.InvokeAsync(RedrawInner, DispatcherPriority.Render);
			return null;
		}

		private void RedrawInner()
		{
			if (!IsRedrawEnabled)
			{
				TextTracer.Debug("因为RenderManager.IsEnableRedraw为false因此跳过本次渲染", "RenderManager");
				return;
			}
			UpdateGeometries();
			double renderWidth = GetRenderWidth();
			List<ParagraphLayoutData> e = BuildParagraphs(_redrawSegment, renderWidth);
			RenderCompleted?.Invoke(this, e);
			UpdateMarkerState();
		}

		internal double GetRenderWidth()
		{
			double num = 32000.0;
			if (DocumentManager.SizeToContent != SizeToContent.Width && DocumentManager.SizeToContent != SizeToContent.WidthAndHeight)
			{
				num = ((DocumentManager.ArrangingType == ArrangingType.Horizontal) ? DocumentManager.EditorWidth : DocumentManager.EditorHeight);
			}
			else if (DocumentManager.PropertyManager.ParagraphProperties.Any((int x) => DocumentManager.PropertyManager.ParagraphPropDic[x].TextAlignment != TextAlignment.Left))
			{
				DocumentManager.PropertyManager.ParagraphPropDic.ForEach((KeyValuePair<int, ParagraphProperty> x) =>
				{
					if (x.Value.TextAlignment == TextAlignment.Justify)
					{
						x.Value.TextAlignment = TextAlignment.Left;
					}
				});
				num = CalcDocumentWidthRenderInOneLine() + 0.001;
			}
			num = (double.IsNaN(num) ? 0.0 : num);
			double maximum = num;
			double minimum = 1.0;
			if (DocumentManager.ArrangingType == ArrangingType.Horizontal)
			{
				if (!double.IsNaN(DocumentManager.MaxWidth))
				{
					maximum = DocumentManager.MaxWidth;
				}
				if (!double.IsNaN(DocumentManager.MinWidth))
				{
					minimum = DocumentManager.MinWidth;
				}
			}
			else
			{
				if (!double.IsNaN(DocumentManager.MaxHeight))
				{
					maximum = DocumentManager.MaxHeight;
				}
				if (!double.IsNaN(DocumentManager.MinHeight))
				{
					minimum = DocumentManager.MinHeight;
				}
			}
			return num.CoerceValue(minimum, maximum);
		}

		internal void ForceRedraw()
		{
			ForceRedraw(new SimpleSegment(0, DocumentManager.CharsManager.CharCount));
		}

		internal void ForceRedraw(ISegment segment)
		{
			InvalidateParagraphs(segment);
			_redrawSegment = segment;
			RedrawInner();
		}

		private List<ParagraphLayoutData> BuildParagraphs(ISegment segment, double renderWidth)
		{
			Dictionary<int, ParagraphTextBeforeData> dictionary = PreParagraphDatas();
			List<ParagraphLayoutData> list = new List<ParagraphLayoutData>();
			List<ParagraphBase> list2 = new List<ParagraphBase>();
			ParagraphNode paragraphNode = DocumentManager.CharsManager.ParagraphTree.First();
			while (paragraphNode != null)
			{
				ParagraphBase paragraphByParagraphNumber = GetParagraphByParagraphNumber(paragraphNode.ParagraphNumber);
				ParagraphBase paragraphBase = null;
				if (paragraphByParagraphNumber != null)
				{
					paragraphByParagraphNumber.ParagraphTextBeforeData = dictionary[paragraphNode.ParagraphNumber];
					if (paragraphByParagraphNumber.IsAlive)
					{
						paragraphBase = paragraphByParagraphNumber;
						_paragraphVisualMappingCache[paragraphBase].MarkerVisual = dictionary[paragraphNode.ParagraphNumber].MarkerCharVisual;
						list.Add(_paragraphVisualMappingCache[paragraphBase]);
					}
					else if (!paragraphByParagraphNumber.ParagraphNode.IsDeleted)
					{
						int startRedrawLineIndex = paragraphByParagraphNumber.BuildRenderData(segment, renderWidth);
						ParagraphLayoutData item = GenerateParagraphLayoutDataFromParagraph(paragraphByParagraphNumber, startRedrawLineIndex);
						list.Add(item);
						paragraphBase = paragraphByParagraphNumber;
					}
				}
				else
				{
					paragraphBase = CreateParagraph(paragraphNode);
					paragraphBase.ParagraphTextBeforeData = dictionary[paragraphNode.ParagraphNumber];
					paragraphBase.BuildRenderData(renderWidth);
					ParagraphLayoutData item2 = GenerateParagraphLayoutDataFromParagraph(paragraphBase);
					list.Add(item2);
				}
				paragraphNode = paragraphNode.NextNode;
				list2.Add(paragraphBase);
			}
			SetParagraphs(list2);
			UpdateExtensionalCharVisual();
			Paragraphs.ForEach((ParagraphBase x) =>
			{
				x.IsAlive = true;
			});
			BuildDocumentHeightAndWidth();
			HandleMarker(list);
			ArrangeLineVisual(list);
			UpdateVisualTop(list);
			UpdateCharVisualMappingCache();
			return list;
		}

		private void BuildDocumentHeightAndWidth()
		{
			DocumentHeight = Paragraphs.Sum((ParagraphBase x) => x.VisualHeight);
			DocumentWidth = Paragraphs.Max((ParagraphBase x) => x.VisualWidth);
		}

		private void UpdateExtensionalCharVisual()
		{
			DocumentManager.CharsManager.Chars.OfType<ExtensionalChar>().ForEach((ExtensionalChar c) =>
			{
				c.UpdateRender();
			});
		}

		private void ArrangeLineVisual(List<ParagraphLayoutData> paragraphLayoutData)
		{
			for (int i = 0; i < paragraphLayoutData.Count; i++)
			{
				ParagraphTextBeforeData paragraphTextBeforeData = Paragraphs[i].ParagraphTextBeforeData;
				List<LineVisualBase> lines = paragraphLayoutData[i].Lines;
				for (int j = 0; j < lines.Count; j++)
				{
					if (DocumentManager.ArrangingType == ArrangingType.Horizontal)
					{
						double num;
						if (j == 0)
						{
							num = lines[j].LineRenderData.Width - paragraphTextBeforeData.FirstLineTextIndent;
							lines[j].StartArrangeOffset = paragraphTextBeforeData.FirstLineTextIndent;
						}
						else
						{
							num = lines[j].LineRenderData.Width - paragraphTextBeforeData.NotFirstLineTextIndent;
							lines[j].StartArrangeOffset = paragraphTextBeforeData.NotFirstLineTextIndent;
						}
						lines[j].Width = num;
						lines[j].Height = lines[j].LineRenderData.Height;
						if (double.IsInfinity(num))
						{
							num = 0.0;
						}
						num = ((num < 0.0) ? 0.0 : num);
						lines[j].Arrange(new Rect(default(Point), new Size(num, lines[j].LineRenderData.Height)));
					}
					else
					{
						double num2;
						if (j == 0)
						{
							num2 = lines[j].LineRenderData.Width - paragraphTextBeforeData.FirstLineTextIndent;
							lines[j].StartArrangeOffset = paragraphTextBeforeData.FirstLineTextIndent;
						}
						else
						{
							num2 = lines[j].LineRenderData.Width - paragraphTextBeforeData.NotFirstLineTextIndent;
							lines[j].StartArrangeOffset = paragraphTextBeforeData.NotFirstLineTextIndent;
						}
						lines[j].Width = lines[j].LineRenderData.Height;
						lines[j].Height = num2;
						lines[j].Arrange(new Rect(default(Point), new Size(lines[j].LineRenderData.Height, (num2 < 0.0) ? 0.0 : num2)));
					}
				}
			}
		}

		private void HandleMarker(List<ParagraphLayoutData> paragraphLayoutDatas)
		{
			HandleMarkerEffects(paragraphLayoutDatas);
			HandleMarkerPosition(paragraphLayoutDatas);
		}

		private void HandleMarkerEffects(List<ParagraphLayoutData> paragraphLayoutDatas)
		{
			bool flag = DocumentManager.ArrangingType == ArrangingType.Horizontal;
			for (int i = 0; i < Paragraphs.Count; i++)
			{
				ParagraphProperty targetParagraphProperty = Paragraphs[i].TargetParagraphProperty;
				List<LineInfo> lines = Paragraphs[i].Lines;
				ParagraphTextBeforeData paragraphTextBeforeData = Paragraphs[i].ParagraphTextBeforeData;
				if (targetParagraphProperty.MarkerProperty == null || targetParagraphProperty.MarkerProperty.IsEmpty() || !targetParagraphProperty.DefaultRunProperty.EffectsArgs.Any())
				{
					continue;
				}
				double lineSpacing = targetParagraphProperty.LineSpacing;
				RunProperty runProperty = targetParagraphProperty.DefaultRunProperty.Clone();
				runProperty = DocumentManager.PropertyManager.GetRenderDefaultRunProperty(runProperty);
				ValueTuple<Geometry, Size> valueTuple = TextHelperInner.BuildDefaultCharGeometry(runProperty);
				Geometry geometry = valueTuple.Item1;
				Size size = valueTuple.Item2;
				if (!flag)
				{
					geometry = TextHelperInner.CorrectToVerticalGeometry(geometry, size);
					size = new Size(size.Height, size.Width);
				}
				runProperty.FontFamily = targetParagraphProperty.MarkerProperty.FontFamily;
				if (targetParagraphProperty.MarkerProperty.AutoNumber != null)
				{
					runProperty.FontFamily = targetParagraphProperty.DefaultRunProperty.FontFamily;
				}
				if (targetParagraphProperty.MarkerProperty.Foreground != null)
				{
					runProperty.Foreground = targetParagraphProperty.MarkerProperty.Foreground;
				}
				double height = (flag ? lines[0].DefaultRunBounds.Height : size.Width);
				TextCharVisual textCharVisual = MarkerFactory.BuildMarkerTextCharVisual(runProperty, (TextChar)paragraphTextBeforeData.MarkerChar, height, DocumentManager.ArrangingType != ArrangingType.Horizontal);
				double num2;
				if (flag)
				{
					double num = geometry.Bounds.Bottom + lineSpacing * 0.8;
					if (i == 0 && DocumentManager.PropertyManager.LineSpacingStrategy == LineSpacingStrategy.FirstLineShrink)
					{
						num = geometry.Bounds.Bottom;
					}
					num2 = (lines[0].CharVisuals.Any() ? lines[0].Baseline : num);
					num2 = num2 - lines[0].DefaultRunBounds.Top + lines[0].DefaultRunBounds.Height / 2.0 - textCharVisual.Bounds.Height / 2.0;
				}
				else
				{
					num2 = MarkerFactory.GetMarkerBaseLine(Paragraphs[i].TargetParagraphProperty, (TextChar)Paragraphs[i].ParagraphTextBeforeData.MarkerChar);
				}
				List<DrawingVisualContainer> textEffectContainers = TextEffectFactory.GetTextEffectContainers(new List<TextCharVisual> { textCharVisual }, runProperty.EffectsArgs, DrawType.TextChar, null, num2, DocumentManager.ArrangingType);
				UIElementContainer uIElementContainer = (UIElementContainer)Paragraphs[i].ParagraphTextBeforeData.MarkerCharVisual.CharView;
				uIElementContainer.Elements.Clear();
				uIElementContainer.AddChildren(textEffectContainers);
				uIElementContainer.AddChild(MarkerFactory.CreateMarkerCharVisualContainer(textCharVisual, runProperty));
				paragraphLayoutDatas[i].MarkerVisual.CharView = Paragraphs[i].ParagraphTextBeforeData.MarkerCharVisual.CharView;
			}
		}

		private void HandleMarkerPosition(List<ParagraphLayoutData> paragraphLayoutDatas)
		{
			bool flag = DocumentManager.ArrangingType == ArrangingType.Horizontal;
			for (int i = 0; i < Paragraphs.Count; i++)
			{
				if (Paragraphs[i].TargetParagraphProperty.MarkerProperty != null && !Paragraphs[i].TargetParagraphProperty.MarkerProperty.IsEmpty())
				{
					double y = (flag ? Paragraphs[i].Lines[0].DefaultRunBounds.Y : (Paragraphs[i].ParagraphTextBeforeData.MarkerCharVisual.Bounds.Y + Paragraphs[i].Lines[0].DefaultRunBounds.Y));
					double x;
					if (flag)
					{
						x = Paragraphs[i].ParagraphTextBeforeData.MarkerCharVisual.Bounds.X + Paragraphs[i].Lines[0].DefaultRunBounds.X;
					}
					else
					{
						double markerBaseLine = MarkerFactory.GetMarkerBaseLine(Paragraphs[i].TargetParagraphProperty, (TextChar)Paragraphs[i].ParagraphTextBeforeData.MarkerChar);
						ValueTuple<Geometry, Size> valueTuple = TextHelperInner.BuildDefaultCharGeometry(Paragraphs[i].TargetParagraphProperty.DefaultRunProperty);
						Geometry item = valueTuple.Item1;
						Size item2 = valueTuple.Item2;
						item = TextHelperInner.CorrectToVerticalGeometry(item, item2);
						double left = item.Bounds.Left;
						x = (Paragraphs[i].Lines[0].CharVisuals.Any() ? (Paragraphs[i].Lines[0].Baseline - markerBaseLine) : (left - markerBaseLine));
					}
					Rect bounds = new Rect(new Point(x, y), Paragraphs[i].ParagraphTextBeforeData.MarkerCharVisual.Bounds.Size);
					Paragraphs[i].ParagraphTextBeforeData.MarkerCharVisual.Bounds = bounds;
					paragraphLayoutDatas[i].MarkerVisual.Bounds = bounds;
				}
			}
		}

		internal double CalcDocumentWidthRenderInOneLine()
		{
			double width = 32000.0;
			ParagraphNode paragraphNode = DocumentManager.CharsManager.ParagraphTree.First();
			UpdateGeometries();
			Dictionary<int, ParagraphTextBeforeData> dictionary = PreParagraphDatas();
			double num = 0.0;
			while (paragraphNode != null)
			{
				ParagraphBase paragraphBase = CreateParagraph(paragraphNode);
				paragraphBase.ParagraphTextBeforeData = dictionary[paragraphNode.ParagraphNumber];
				paragraphBase.BuildRenderData(width);
				paragraphNode = paragraphNode.NextNode;
				num = Math.Max(num, paragraphBase.VisualWidth);
			}
			return num;
		}

		internal void UpdateGeometries()
		{
			GeometryPool geometryPool = DocumentManager.CharsManager.GeometryPool;
			for (int i = 0; i < DocumentManager.CharsManager.CharCount; i++)
			{
				if (!geometryPool.GetCharGeometriesInfo(i).IsDirty)
				{
					continue;
				}
				ICharObject charAt = DocumentManager.CharsManager.GetCharAt(i);
				TextChar textChar = charAt as TextChar;
				if (textChar != null)
				{
					if (TextUtilities.IsLigature(textChar.ToText()))
					{
						CharGeometryInfo charGeometriesInfo = new CharGeometryInfo(Geometry.Empty, Size.Empty, null);
						geometryPool.UpdateRangeCharGeometriesInfo(i, charGeometriesInfo);
					}
					else
					{
						CharGeometryInfo charGeometriesInfo = BuildGeometryInfo(textChar, i);
						geometryPool.UpdateRangeCharGeometriesInfo(i, charGeometriesInfo);
					}
				}
				else
				{
					ExtensionalChar extensionalChar = (ExtensionalChar)charAt;
					CharGeometryInfo charGeometriesInfo = new CharGeometryInfo(Geometry.Empty, extensionalChar.Size, null);
					geometryPool.UpdateRangeCharGeometriesInfo(i, charGeometriesInfo);
				}
			}
		}

		private CharGeometryInfo BuildGeometryInfo(TextChar textChar, int index)
		{
			RunProperty renderRunProperty = DocumentManager.PropertyManager.GetRenderRunProperty(index);
			if (renderRunProperty.FontVariants != FontVariants.Normal)
			{
				renderRunProperty.FontSize /= 2.0;
			}
			Geometry textGeometry;
			Size textGeometrySize;
			TextCharGlyphData textCharGlyphData;
			BuildCharGeometry(renderRunProperty, textChar).Deconstruct(out textGeometry, out textGeometrySize, out textCharGlyphData);
			Geometry geometry = textGeometry;
			Size size = textGeometrySize;
			TextCharGlyphData data = textCharGlyphData;
			CharGeometryInfo charGeometryInfo = new CharGeometryInfo(geometry, size, data);
			if (DocumentManager.ArrangingType != ArrangingType.Horizontal)
			{
				if (TextUtilities.NeedRotation(textChar.ToText()))
				{
					charGeometryInfo.Geometry = TextHelperInner.CorrectToVerticalGeometry(charGeometryInfo.Geometry, charGeometryInfo.Size);
					charGeometryInfo.Size = new Size(charGeometryInfo.Size.Height, charGeometryInfo.Size.Width);
				}
				else
				{
					charGeometryInfo.Size = new Size(charGeometryInfo.Size.Height, charGeometryInfo.Size.Width);
					Rect bounds = charGeometryInfo.Geometry.Bounds;
					double x = (charGeometryInfo.Size.Width - bounds.Width) / 2.0 - charGeometryInfo.Geometry.Bounds.Left;
					double y = (charGeometryInfo.Size.Height - bounds.Height) / 2.0 - bounds.Top;
					charGeometryInfo.Geometry.TransformTranslate(x, y);
				}
			}
			return charGeometryInfo;
		}

		internal Dictionary<int, ParagraphTextBeforeData> PreParagraphDatas()
		{
			Dictionary<int, ParagraphTextBeforeData> dictionary = new Dictionary<int, ParagraphTextBeforeData>();
			int num = DocumentManager.CharsManager.ParagraphTree.Count();
			for (int i = 0; i < num; i++)
			{
				ParagraphProperty paragraphProperty = DocumentManager.PropertyManager.GetParagraphProperty(i);
				ParagraphTextBeforeData paragraphTextBeforeData = new ParagraphTextBeforeData();
				if (paragraphProperty.MarkerProperty != null && !paragraphProperty.MarkerProperty.IsEmpty())
				{
					RunProperty runProperty = paragraphProperty.DefaultRunProperty.Clone();
					runProperty = DocumentManager.PropertyManager.GetRenderDefaultRunProperty(runProperty);
					runProperty.FontFamily = paragraphProperty.MarkerProperty.FontFamily;
					ICharObject charObject = paragraphProperty.MarkerProperty.MarkerChar;
					if (paragraphProperty.MarkerProperty.AutoNumber != null)
					{
						charObject = MarkerFactory.GetCurrentAutoNumber(DocumentManager.CharsManager, DocumentManager.PropertyManager, i);
						runProperty.FontFamily = paragraphProperty.DefaultRunProperty.FontFamily;
					}
					if (string.IsNullOrEmpty(charObject.ToText()))
					{
						if (DebuggingProperties.IsDebug)
						{
							throw new ArgumentException("项目符号获取到的文本是空文本，请确定存储数据是否正确。请将德熙拉过来你旁边。仅调试下抛出");
						}
						continue;
					}
					if (paragraphProperty.MarkerProperty.Foreground != null)
					{
						runProperty.Foreground = paragraphProperty.MarkerProperty.Foreground;
					}
					bool flag = DocumentManager.ArrangingType != ArrangingType.Horizontal;
					double height = TextHelperInner.BuildDefaultCharSize(paragraphProperty.DefaultRunProperty).Height;
					UiElementCharVisual uiElementCharVisual = MarkerFactory.BuildMarkerCharVisual(runProperty, charObject, height, flag);
					double markerWidth = ((!flag) ? uiElementCharVisual.Bounds.Width : uiElementCharVisual.Bounds.Height);
					ValueTuple<double, double> valueTuple = TextHelperInner.CalculateIndent(paragraphProperty, markerWidth);
					double item = valueTuple.Item1;
					double item2 = valueTuple.Item2;
					paragraphTextBeforeData.FirstLineTextIndent = item;
					uiElementCharVisual.Bounds = (flag ? new Rect(new Point(0.0, item2), uiElementCharVisual.Bounds.Size) : new Rect(new Point(item2, 0.0), uiElementCharVisual.Bounds.Size));
					paragraphTextBeforeData.MarkerChar = charObject;
					paragraphTextBeforeData.MarkerCharVisual = uiElementCharVisual;
				}
				else
				{
					double item3 = TextHelperInner.CalculateIndent(paragraphProperty).Item1;
					paragraphTextBeforeData.FirstLineTextIndent = item3;
				}
				paragraphTextBeforeData.NotFirstLineTextIndent = paragraphProperty.MarginLeft;
				dictionary.Add(i + 1, paragraphTextBeforeData);
			}
			return dictionary;
		}

		private void UpdateVisualTop(List<ParagraphLayoutData> paragraphLayoutDatas)
		{
			double currentParagraphsHeight = Paragraphs.Sum((ParagraphBase x) => x.VisualHeight);
			double yOffset = 0.0;
			if (DocumentManager.SizeToContent == SizeToContent.Manual || DocumentManager.SizeToContent == SizeToContent.Width)
			{
				double editorHeight = ((DocumentManager.ArrangingType == ArrangingType.Horizontal) ? DocumentManager.EditorHeight : DocumentManager.EditorWidth);
				yOffset = GetVerticalAlignOffset(currentParagraphsHeight, editorHeight);
			}
			switch (DocumentManager.ArrangingType)
			{
			case ArrangingType.Horizontal:
				HandlerHorizontal(paragraphLayoutDatas, currentParagraphsHeight, yOffset);
				break;
			case ArrangingType.Vertical:
				HandlerVertical(paragraphLayoutDatas, currentParagraphsHeight, yOffset);
				break;
			case ArrangingType.Mongolian:
				HandlerMongolian(paragraphLayoutDatas, currentParagraphsHeight, yOffset);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void HandlerMongolian(List<ParagraphLayoutData> paragraphLayoutDatas, double currentParagraphsHeight, double yOffset)
		{
			for (int i = 0; i < paragraphLayoutDatas.Count; i++)
			{
				double num = ((i == 0) ? 0.0 : paragraphLayoutDatas[i].ParagraphBefore);
				Paragraphs[i].VisualTop = yOffset + num;
				paragraphLayoutDatas[i].VisualTop = yOffset + num;
				if (paragraphLayoutDatas[i].MarkerVisual != null)
				{
					double x = yOffset + num + paragraphLayoutDatas[i].MarkerVisual.Bounds.X;
					double y = paragraphLayoutDatas[i].MarkerVisual.Bounds.Y;
					paragraphLayoutDatas[i].MarkerVisual.Bounds = new Rect(new Point(x, y), paragraphLayoutDatas[i].MarkerVisual.Bounds.Size);
				}
				yOffset += Paragraphs[i].VisualHeight;
			}
		}

		private void HandlerHorizontal(List<ParagraphLayoutData> paragraphLayoutDatas, double currentParagraphsHeight, double yOffset)
		{
			for (int i = 0; i < Paragraphs.Count; i++)
			{
				double num = ((i == 0) ? 0.0 : paragraphLayoutDatas[i].ParagraphBefore);
				Paragraphs[i].VisualTop = yOffset;
				paragraphLayoutDatas[i].VisualTop = yOffset;
				if (paragraphLayoutDatas[i].MarkerVisual != null)
				{
					double num2 = paragraphLayoutDatas[i].MarkerVisual.Bounds.Y + yOffset + num;
					double x = paragraphLayoutDatas[i].MarkerVisual.Bounds.X;
					double y = num2;
					paragraphLayoutDatas[i].MarkerVisual.Bounds = new Rect(new Point(x, y), paragraphLayoutDatas[i].MarkerVisual.Bounds.Size);
				}
				yOffset += Paragraphs[i].VisualHeight;
			}
		}

		private void HandlerVertical(List<ParagraphLayoutData> paragraphLayoutDatas, double currentParagraphsHeight, double yOffset)
		{
			for (int i = 0; i < paragraphLayoutDatas.Count; i++)
			{
				double num = ((i == 0) ? 0.0 : paragraphLayoutDatas[i].ParagraphBefore);
				Paragraphs[i].VisualTop = yOffset + num;
				paragraphLayoutDatas[i].VisualTop = yOffset + num;
				if (paragraphLayoutDatas[i].MarkerVisual != null)
				{
					double x = 0.0 - paragraphLayoutDatas[i].Lines[0].Width - yOffset - num + paragraphLayoutDatas[i].MarkerVisual.Bounds.X;
					double y = paragraphLayoutDatas[i].MarkerVisual.Bounds.Y;
					paragraphLayoutDatas[i].MarkerVisual.Bounds = new Rect(new Point(x, y), paragraphLayoutDatas[i].MarkerVisual.Bounds.Size);
				}
				yOffset += Paragraphs[i].VisualHeight;
			}
		}

		private double GetVerticalAlignOffset(double currentParagraphsHeight, double editorHeight)
		{
			double val = 0.0;
			switch (DocumentManager.TextVerticalAlignment)
			{
			case VerticalAlignment.Center:
				val = (editorHeight - currentParagraphsHeight) / 2.0;
				break;
			case VerticalAlignment.Bottom:
				val = editorHeight - currentParagraphsHeight;
				break;
			default:
				throw new InvalidOperationException();
			case VerticalAlignment.Top:
				break;
			}
			return Math.Max(val, 0.0);
		}

		private void UpdateCharVisualMappingCache()
		{
			Paragraphs.ForEach((ParagraphBase x) =>
			{
				if (!_paragraphVisualMappingCache.ContainsKey(x))
				{
					_paragraphVisualMappingCache.Remove(x);
				}
			});
		}

		private ParagraphLayoutData GenerateParagraphLayoutDataFromParagraph(ParagraphBase paragraph, int startRedrawLineIndex = 0)
		{
			ParagraphVisualMappingCache paragraphVisualMappingCache = _paragraphVisualMappingCache;
			ArrangingType arrangingType = DocumentManager.ArrangingType;
			return ParagraphLayoutDataGenerator.GenerateParagraphLayoutDataFromParagraph(new ParagraphLayoutDataGenerator.GeneratorContext
			{
				Paragraph = paragraph,
				StartRedrawLineIndex = startRedrawLineIndex,
				ArrangingType = arrangingType,
				ParagraphVisualMappingCache = paragraphVisualMappingCache
			});
		}

		[NotNull]
		internal ParagraphBase CreateParagraph(ParagraphNode paragraphNode)
		{
			switch (DocumentManager.ArrangingType)
			{
			case ArrangingType.Horizontal:
				return new HorizontalParagraph(DocumentManager.CharsManager, DocumentManager.PropertyManager, paragraphNode, this);
			case ArrangingType.Vertical:
				return new VerticalParagraph(DocumentManager.CharsManager, DocumentManager.PropertyManager, paragraphNode, this);
			case ArrangingType.Mongolian:
				return new MongolianParagraph(DocumentManager.CharsManager, DocumentManager.PropertyManager, paragraphNode, this);
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void InvalidateParagraphs(ISegment segment)
		{
			if (DocumentManager.TextVerticalAlignment != VerticalAlignment.Top || DocumentManager.PropertyManager.ParagraphProperties.Any((int x) => DocumentManager.PropertyManager.ParagraphPropDic[x].TextAlignment != TextAlignment.Left))
			{
				segment = new SimpleSegment(0, DocumentManager.CharsManager.CharCount);
			}
			for (int num = 0; num < Paragraphs.Count; num++)
			{
				ParagraphNode paragraphNode = Paragraphs[num].ParagraphNode;
				if (paragraphNode.IsDeleted)
				{
					Paragraphs[num].IsAlive = false;
				}
				else if (new SimpleSegment(paragraphNode.StartOffset, paragraphNode.Length).IsSegmentInvalid(segment) && Paragraphs.Count > num)
				{
					Paragraphs[num].IsAlive = false;
				}
			}
		}

		internal CaretOffset GetCaretOffsetByPosition(Point controlPosition)
		{
			ParagraphBase paragraphByPosition = GetParagraphByPosition(controlPosition);
			Point paragraphPosition = new Point(controlPosition.X, controlPosition.Y);
			switch (DocumentManager.ArrangingType)
			{
			case ArrangingType.Horizontal:
				paragraphPosition.Y = controlPosition.Y - paragraphByPosition.VisualTop;
				break;
			case ArrangingType.Vertical:
				paragraphPosition.X = DocumentManager.EditorWidth - controlPosition.X - paragraphByPosition.VisualTop;
				break;
			case ArrangingType.Mongolian:
				paragraphPosition.X = controlPosition.X - paragraphByPosition.VisualTop;
				break;
			default:
				throw new ArgumentOutOfRangeException("ArrangingType", "the arraingtype is not validate");
			}
			return paragraphByPosition.GetCaretOffsetByParagraphPosition(paragraphPosition);
		}

		internal Rect GetRunBoundsByDocumentOffset(DocumentOffset documentOffset)
		{
			CaretOffset caretOffset = new CaretOffset
			{
				Offset = documentOffset
			};
			return GetRunBoundsByCaretOffset(caretOffset);
		}

		internal Rect GetRunBoundsByCaretOffset(CaretOffset caretOffset)
		{
			ParagraphBase paragraphByCaretOffset = GetParagraphByCaretOffset(caretOffset);
			Rect runBoundsByCaretOffset = paragraphByCaretOffset.GetRunBoundsByCaretOffset(caretOffset);
			double num = ((DocumentManager.CharsManager.ParagraphTree.Count() == paragraphByCaretOffset.ParagraphNode.ParagraphNumber) ? 0.0 : paragraphByCaretOffset.TargetParagraphProperty.ParagraphAfter) + ((paragraphByCaretOffset.ParagraphNode.ParagraphNumber == 1) ? 0.0 : paragraphByCaretOffset.TargetParagraphProperty.ParagraphBefore);
			switch (DocumentManager.ArrangingType)
			{
			case ArrangingType.Horizontal:
				return runBoundsByCaretOffset.OffsetRect(0.0, paragraphByCaretOffset.VisualTop);
			case ArrangingType.Vertical:
				return runBoundsByCaretOffset.OffsetRect(DocumentManager.EditorWidth - paragraphByCaretOffset.VisualTop - paragraphByCaretOffset.VisualHeight + num, 0.0);
			case ArrangingType.Mongolian:
				return runBoundsByCaretOffset.OffsetRect(paragraphByCaretOffset.VisualTop, 0.0);
			default:
				throw new ArgumentOutOfRangeException("ArrangingType", "the arraingtype is not validate");
			}
		}

		internal FlowDirection GetRunDirectionByCaretOffset(CaretOffset caretOffset)
		{
			return GetParagraphByCaretOffset(caretOffset).GetRunDirectionByCaretOffset(caretOffset);
		}

		internal DocumentOffset GetDocumentOffsetByPositionFromRunBounds(Point position)
		{
			ParagraphBase paragraphByPosition = GetParagraphByPosition(position);
			if (DocumentManager.ArrangingType == ArrangingType.Vertical)
			{
				position.X = DocumentManager.EditorWidth - position.X;
			}
			return paragraphByPosition.GetDocumentOffsetByPositionFromRunBounds(position);
		}

		[CanBeNull]
		internal ParagraphBase GetParagraphByParagraphNumber(int paragraphNumber)
		{
			foreach (ParagraphBase paragraph in Paragraphs)
			{
				if (!paragraph.ParagraphNode.IsDeleted && paragraphNumber == paragraph.ParagraphNode.ParagraphNumber)
				{
					return paragraph;
				}
			}
			return null;
		}

		internal ParagraphBase GetParagraphByCaretOffset(CaretOffset caretOffset)
		{
			ParagraphNode paragraphNodeByDocumentOffset = DocumentManager.CharsManager.ParagraphTree.GetParagraphNodeByDocumentOffset(caretOffset.Offset);
			int index = ((caretOffset.IsAtParagraphStart && caretOffset.Offset != -1) ? paragraphNodeByDocumentOffset.ParagraphNumber : (paragraphNodeByDocumentOffset.ParagraphNumber - 1));
			return Paragraphs[index];
		}

		internal ParagraphBase GetParagraphByPosition(Point controlPosition)
		{
			ParagraphBase paragraphBase;
			switch (DocumentManager.ArrangingType)
			{
			case ArrangingType.Horizontal:
				paragraphBase = ((Math.Abs(controlPosition.Y - Paragraphs.First().VisualTop) <= 1E-05 || controlPosition.Y < Paragraphs.First().VisualTop) ? Paragraphs.First() : Paragraphs.Where((ParagraphBase vl) => controlPosition.Y > vl.VisualTop).FirstOrDefault((ParagraphBase vl) => controlPosition.Y < vl.VisualTop + vl.VisualHeight));
				break;
			case ArrangingType.Vertical:
				paragraphBase = ((DocumentManager.EditorWidth - controlPosition.X < Paragraphs.First().VisualTop) ? Paragraphs.First() : Paragraphs.Where((ParagraphBase vl) => DocumentManager.EditorWidth - controlPosition.X > vl.VisualTop).FirstOrDefault((ParagraphBase vl) => DocumentManager.EditorWidth - controlPosition.X < vl.VisualTop + vl.VisualHeight));
				break;
			case ArrangingType.Mongolian:
				paragraphBase = ((controlPosition.X <= Paragraphs.First().VisualTop) ? Paragraphs.First() : Paragraphs.Where((ParagraphBase vl) => controlPosition.X > vl.VisualTop).FirstOrDefault((ParagraphBase vl) => controlPosition.X < vl.VisualTop + vl.VisualHeight));
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			return paragraphBase ?? Paragraphs.Last();
		}

		internal double CalculateDocumentMinWidth()
		{
			if (!Paragraphs.Any())
			{
				ForceRedraw();
			}
			return Paragraphs.Max((ParagraphBase p) => CalculateParagraphMinWidth(p));
		}

		internal double CalculateDocumentMinHeight()
		{
			if (!Paragraphs.Any())
			{
				ForceRedraw();
			}
			return Paragraphs.Sum((ParagraphBase p) => p.LineRenderData.Sum((LineRenderData x) => x.Height));
		}

		private double CalculateParagraphMinWidth(ParagraphBase paragraph)
		{
			double num = 0.0;
			List<ICharVisual> source = paragraph.LineRenderData.SelectMany((LineRenderData line) => line.CharVisuals).ToList();
			if (source.Any())
			{
				num = ((DocumentManager.ArrangingType == ArrangingType.Horizontal) ? source.Max((ICharVisual x) => x.Bounds.Width) : source.Max((ICharVisual x) => x.Bounds.Height));
			}
			return num + ((paragraph.ParagraphTextBeforeData.FirstLineTextIndent > paragraph.ParagraphTextBeforeData.NotFirstLineTextIndent) ? paragraph.ParagraphTextBeforeData.FirstLineTextIndent : paragraph.ParagraphTextBeforeData.NotFirstLineTextIndent);
		}

		internal void UpdateMarkerState()
		{
			if (DocumentManager.IsDocumentBuilding)
			{
				return;
			}
			int paragraphCount = DocumentManager.CharsManager.ParagraphTree.ParagraphCount;
			Dictionary<int, double> dictionary = new Dictionary<int, double>();
			for (int i = 0; i < paragraphCount; i++)
			{
				if (Paragraphs[i].TargetParagraphProperty.MarkerProperty == null || Paragraphs[i].TargetParagraphProperty.MarkerProperty.IsEmpty())
				{
					continue;
				}
				if (DocumentManager.Selection.IsEmpty)
				{
					if (DocumentManager.CharsManager.ParagraphTree.ElementAt(i).Length != 0)
					{
						dictionary[i] = MarkerFactory.OriginalMarkerOpacity;
					}
					else if (DocumentManager.CaretInfo.CurrentCaretOffset.Offset + 1 == DocumentManager.CharsManager.ParagraphTree.ElementAt(i).FrontOffset && DocumentManager.IsEditable)
					{
						dictionary[i] = MarkerFactory.HalfMarkerOpacity;
					}
					else
					{
						dictionary[i] = MarkerFactory.HiddenMarkerOpacity;
					}
				}
				else if (DocumentManager.CharsManager.ParagraphTree.ElementAt(i).Length != 0)
				{
					dictionary[i] = MarkerFactory.OriginalMarkerOpacity;
				}
				else
				{
					dictionary[i] = MarkerFactory.HiddenMarkerOpacity;
				}
			}
			OnUpdateMarkerOpacity(dictionary);
		}

		internal List<Rect> GetBoundsBySegment(ISegment segment)
		{
			List<Rect> list = new List<Rect>();
			foreach (ParagraphBase paragraph in Paragraphs)
			{
				if (paragraph.ParagraphNode.Length != 0)
				{
					list.AddRange(TextHelperInner.GetRunBoundsBySegmentFromParagraph(paragraph, segment, DocumentManager.EditorWidth));
					if (paragraph.ParagraphNode.BehindOffset >= segment.FrontOffset && paragraph.ParagraphNode.BehindOffset < segment.BehindOffset)
					{
						Rect rect = ((list.Count > 0) ? list.Last() : TextHelperInner.CreateTailRect(DocumentManager.ArrangingType, paragraph, DocumentManager.EditorWidth));
						Rect item = ((DocumentManager.ArrangingType == ArrangingType.Horizontal) ? new Rect(rect.Right, rect.Y, 10.0, rect.Height) : new Rect(rect.X, rect.Bottom, rect.Width, 10.0));
						list.Add(item);
					}
				}
				else if (paragraph.ParagraphNode.StartOffset >= segment.FrontOffset && paragraph.ParagraphNode.StartOffset < segment.BehindOffset && DocumentManager.SelectionContainsEmptyLine)
				{
					Rect item2 = TextHelperInner.CreateTailRect(DocumentManager.ArrangingType, paragraph, DocumentManager.EditorWidth);
					if (DocumentManager.ArrangingType == ArrangingType.Horizontal)
					{
						item2.Width = 10.0;
					}
					else
					{
						item2.Height = 10.0;
					}
					list.Add(item2);
				}
			}
			return list;
		}

		public TextGeometryRenderResult BuildCharGeometry(RunProperty runProperty, ICharObject textChar)
		{
			if (!(textChar is TextChar))
			{
				throw new ArgumentException("Only TextChar Could be Build Geometry");
			}
			return BuildTextGeometry(runProperty, textChar.ToText());
		}

		public TextGeometryRenderResult BuildTextGeometry(RunProperty runProperty, string text)
		{
			if (IsCharGeometryCacheable)
			{
				return TextHelperInner.TextGeometry.GetOrBuildTextGeometry(runProperty, text);
			}
			return TextHelperInner.TextGeometry.BuildTextGeometry(runProperty, text);
		}
	}
}
