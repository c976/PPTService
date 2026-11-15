using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using PPTService.Ink.Core.ComponentModel;
using PPTService.Ink.Core.Diagnostics.Annotations;
using PPTService.Ink.Core.Windows.Controls;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class TextEditorRuntimeDecoration
	{
		private const string TextEditorRuntimeDecorationTag = "文本运行时装饰";

		private static readonly DependencyProperty TextEditorRuntimeDecorationProperty = DependencyProperty.RegisterAttached("TextEditorRuntimeDecoration", typeof(TextEditorRuntimeDecoration), typeof(TextEditorRuntimeDecoration), new PropertyMetadata((object)null));

		private TextEditor TextEditor { get; }

		private TextEditorRuntimeDecoration(TextEditor text)
		{
			TextEditor = text;
			text.ArrangingTypeChanged += Text_ArrangingTypeChanged;
		}

		private void Text_ArrangingTypeChanged(object sender, ValueChangedRoutedEventArgs<ArrangingType> e)
		{
			if (e.NewValue == ArrangingType.Horizontal)
			{
				return;
			}
			PropertyManager propertyManager = TextEditor.DocumentManager.PropertyManager;
			ISegment segment = Selection.GetFullSelection(TextEditor).ToSegment();
			foreach (RunProperty item in GetDifferenceRunPropertyInSelection(segment, propertyManager))
			{
				GetTextEditorDecorationCollection(item)?.Clear();
			}
			TextTracer.Debug("切换文本横竖排，清空所有文本运行时装饰", "文本运行时装饰");
		}

		[CanBeNull]
		public TextEditorDecorationCollection GetDecorationProperty(DocumentOffset offset)
		{
			PropertyManager propertyManager = TextEditor.DocumentManager.PropertyManager;
			int runPropertiesCount = propertyManager.RunPropertiesCount;
			if (DebuggingProperties.IsDebug && TextEditor.CharCount != runPropertiesCount)
			{
				throw new InvalidOperationException($"文本还没布局完成就调用此方法，当前的文本字符数量和文本属性数量不对应 CharCount={TextEditor.CharCount} RunPropertiesCount={runPropertiesCount}");
			}
			if (offset == DocumentOffset.DefaultDocumentOffset)
			{
				return null;
			}
			if (offset < 0 || offset > runPropertiesCount)
			{
				throw new ArgumentOutOfRangeException($"传入的偏移量需要是非负同时小于当前文本数量 offset={offset} CharCount={TextEditor.CharCount} RunPropertiesCount={runPropertiesCount}");
			}
			return GetTextEditorDecorationCollection(propertyManager.GetRunProperty(offset));
		}

		public void HideDecorationInCurrentSelection<T>(bool enableUndoRedo = false) where T : TextEditorDecoration
		{
			ISelection selection = TextEditor.Selection;
			HideDecoration<T>(selection, enableUndoRedo);
		}

		public void HideDecoration<T>(ISelection selection, bool enableUndoRedo = false) where T : TextEditorDecoration
		{
			if (selection.IsEmpty)
			{
				return;
			}
			ISegment segment = selection.ToSegment();
			PropertyManager propertyManager = TextEditor.DocumentManager.PropertyManager;
			IEnumerable<RunProperty> differenceRunPropertyInSelection = GetDifferenceRunPropertyInSelection(segment, propertyManager);
			TextEditor.DocumentManager.OnStyleChanging(new StyleChangeEventArgs(segment, PropertyType.RunProperty)
			{
				IsFromUndoRedo = !enableUndoRedo
			});
			foreach (RunProperty item in differenceRunPropertyInSelection)
			{
				RemoveRunPropertyDecoration<T>(item);
			}
			RunProperty runProperty = propertyManager.CurrentRunProperty.Clone();
			RemoveRunPropertyDecoration<T>(runProperty);
			propertyManager.SetCurrentProperty(runProperty);
			TextEditor.DocumentManager.OnStyleChanged(new StyleChangeEventArgs(segment, PropertyType.RunProperty)
			{
				IsFromUndoRedo = !enableUndoRedo
			});
		}

		public void ToggleTextEditorDecorationInCurrentSelection<T>(bool enableUndoRedo = false) where T : TextEditorDecoration, new()
		{
			ISelection selection = TextEditor.Selection;
			ISegment segment = selection.ToSegment();
			if (CheckSegmentPropertyContainDecoration<T>(segment))
			{
				HideDecoration<T>(selection, enableUndoRedo);
			}
			else
			{
				ShowDecoration<T>(selection, enableUndoRedo);
			}
		}

		public void ShowDecorationInCurrentSelection<T>(bool enableUndoRedo = false) where T : TextEditorDecoration, new()
		{
			ShowDecoration<T>(TextEditor.Selection, enableUndoRedo);
		}

		public void ShowDecoration<T>(ISelection selection, bool enableUndoRedo = false) where T : TextEditorDecoration, new()
		{
			SetTextEditorDecoration(selection, new T(), enableUndoRedo);
		}

		public void SetTextEditorDecoration(ISelection selection, TextEditorDecoration textEditorDecoration, bool enableUndoRedo = false)
		{
			if (TextEditor.DocumentManager.ArrangingType != ArrangingType.Horizontal)
			{
				throw new NotSupportedException("当前只支持横排文本装饰，暂不支持竖排文本");
			}
			if (selection.IsEmpty)
			{
				selection = new Selection(CaretOffset.Default, new CaretOffset
				{
					Offset = TextEditor.DocumentManager.CharsManager.CharCount - 1
				});
			}
			ISegment segment = selection.ToSegment();
			DocumentManager documentManager = TextEditor.DocumentManager;
			documentManager.OnStyleChanging(new StyleChangeEventArgs(segment, PropertyType.RunProperty)
			{
				IsFromUndoRedo = !enableUndoRedo
			});
			PropertyManager propertyManager = documentManager.PropertyManager;
			foreach (RunProperty item in GetDifferenceRunPropertyInSelection(segment, propertyManager))
			{
				TextEditorDecorationCollection orCreateProperty = item.RunTimeAttachedRunPropertyCollection.GetOrCreateProperty<TextEditorDecorationCollection>();
				if (!orCreateProperty.Contains(textEditorDecoration))
				{
					orCreateProperty.Add(textEditorDecoration);
				}
			}
			documentManager.OnStyleChanged(new StyleChangeEventArgs(segment, PropertyType.RunProperty)
			{
				IsFromUndoRedo = !enableUndoRedo
			});
		}

		[NotNull]
		public static TextEditorRuntimeDecoration GetOrCreateTextEditorRuntimeDecoration([NotNull] TextEditor text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			TextEditorRuntimeDecoration textEditorRuntimeDecoration = GetTextEditorRuntimeDecoration(text);
			if (textEditorRuntimeDecoration == null)
			{
				textEditorRuntimeDecoration = new TextEditorRuntimeDecoration(text);
				SetTextEditorRuntimeDecoration(text, textEditorRuntimeDecoration);
			}
			return textEditorRuntimeDecoration;
		}

		private static void RemoveRunPropertyDecoration<T>(RunProperty runProperty) where T : TextEditorDecoration
		{
			GetTextEditorDecorationCollection(runProperty)?.RemoveAll((TextEditorDecoration temp) => temp is T);
		}

		internal static TextEditorDecorationCollection GetTextEditorDecorationCollection(RunProperty runProperty)
		{
			return runProperty.RunTimeAttachedRunPropertyCollection.GetProperty<TextEditorDecorationCollection>();
		}

		private bool CheckSegmentPropertyContainDecoration<T>(ISegment segment) where T : TextEditorDecoration, new()
		{
			return TextEditor.DocumentManager.GetRunPropertiesBySegment(segment).Any(CheckRunPropertyContainDecoration<T>);
		}

		private static bool CheckRunPropertyContainDecoration<T>(RunProperty runProperty) where T : TextEditorDecoration, new()
		{
			return GetTextEditorDecorationCollection(runProperty)?.Any((TextEditorDecoration textEditorDecoration) => textEditorDecoration is T) ?? false;
		}

		private IEnumerable<RunProperty> GetDifferenceRunPropertyInSelection(ISegment segment, PropertyManager propertyManager)
		{
			List<RunProperty> list = new List<RunProperty>();
			DocumentOffset documentOffset = segment.StartOffset;
			RunProperty runProperty = propertyManager.GetRunProperty(documentOffset);
			propertyManager.GetOrRegisterRunProperty(runProperty);
			RunProperty runProperty2 = runProperty.Clone();
			RunPropertyToken runPropertyToken = propertyManager.RegisterRunPropertyToken(runProperty2);
			propertyManager.SetRunProperty(documentOffset, ref runPropertyToken);
			list.Add(runProperty2);
			int length = segment.Length;
			RunProperty runProperty3 = runProperty;
			RunPropertyToken runPropertyToken2 = runPropertyToken;
			for (int i = 1; i < length; i++)
			{
				DocumentOffset documentOffset2 = (int)documentOffset + i;
				RunProperty runProperty4 = propertyManager.GetRunProperty(documentOffset2);
				if (runProperty3 == runProperty4)
				{
					propertyManager.SetRunProperty(documentOffset2, ref runPropertyToken2);
					continue;
				}
				runProperty3 = runProperty4;
				RunProperty runProperty5 = runProperty4.Clone();
				runPropertyToken2 = propertyManager.RegisterRunPropertyToken(runProperty5);
				propertyManager.SetRunProperty(documentOffset2, ref runPropertyToken2);
				list.Add(runProperty5);
			}
			return list;
		}

		internal static void SetTextEditorRuntimeDecoration(TextEditor element, TextEditorRuntimeDecoration value)
		{
			element.SetValue(TextEditorRuntimeDecorationProperty, value);
		}

		internal static TextEditorRuntimeDecoration GetTextEditorRuntimeDecoration(TextEditor element)
		{
			return (TextEditorRuntimeDecoration)element.GetValue(TextEditorRuntimeDecorationProperty);
		}
	}
}
