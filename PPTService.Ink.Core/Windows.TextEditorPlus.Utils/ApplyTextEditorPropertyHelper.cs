using System;
using PPTService.Ink.Core.Diagnostics.Annotations;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	internal static class ApplyTextEditorPropertyHelper
	{
		public static void SetTextEditorProperty([NotNull] TextEditorProperty property, TextEditor textEditor)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			SetDocumentProperty(property, textEditor);
			SetParagraphProperty(property, textEditor);
			SetRunProperty(property, textEditor);
			SetAllGeometryDirty(textEditor);
			textEditor.RenderManager.Redraw();
		}

		private static void SetAllGeometryDirty(TextEditor textEditor)
		{
			textEditor.DocumentManager.CharsManager.GeometryPool.SetDirty(0, textEditor.CharCount);
		}

		private static void SetRunProperty(TextEditorProperty property, TextEditor textEditor)
		{
			if (property.RunProperty != null)
			{
				PropertyManager propertyManager = textEditor.DocumentManager.PropertyManager;
				propertyManager.ClearRunProperties();
				RunProperty runProperty = property.RunProperty.Clone();
				RunPropertyToken runPropertyToken = propertyManager.GetOrRegisterRunProperty(runProperty);
				propertyManager.SetAllRunProperties(ref runPropertyToken);
				propertyManager.SetCurrentProperty(runProperty);
			}
		}

		private static void SetParagraphProperty(TextEditorProperty property, TextEditor textEditor)
		{
			if (property.ParagraphProperty != null)
			{
				PropertyManager propertyManager = textEditor.DocumentManager.PropertyManager;
				propertyManager.ClearParagraphProperties();
				ParagraphProperty paragraphProperty = property.ParagraphProperty.Clone();
				int paragraphPropertyIndex = propertyManager.GetParagraphPropertyIndex(paragraphProperty);
				for (int i = 0; i < propertyManager.ParagraphProperties.Count; i++)
				{
					propertyManager.ParagraphProperties[i] = paragraphPropertyIndex;
				}
				propertyManager.SetCurrentProperty(paragraphProperty);
			}
		}

		private static void SetDocumentProperty(TextEditorProperty property, TextEditor textEditor)
		{
			if (property.ArrangingType.HasValue)
			{
				textEditor.DocumentManager.ArrangingType = property.ArrangingType.Value;
			}
			if (property.TextVerticalAlignment.HasValue)
			{
				textEditor.DocumentManager.TextVerticalAlignment = property.TextVerticalAlignment.Value;
			}
		}
	}
}
