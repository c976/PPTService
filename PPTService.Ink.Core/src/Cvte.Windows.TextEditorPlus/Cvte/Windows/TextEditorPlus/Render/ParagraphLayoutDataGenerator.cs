using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Render
{
	internal static class ParagraphLayoutDataGenerator
	{
		public struct GeneratorContext
		{
			public ParagraphBase Paragraph
			{
				
				get;
				set;
			}

			public int StartRedrawLineIndex
			{
				
				get;
				set;
			}

			public ParagraphVisualMappingCache ParagraphVisualMappingCache
			{
				
				get;
				set;
			}

			public ArrangingType ArrangingType
			{
				
				get;
				set;
			}
		}

		public static ParagraphLayoutData GenerateParagraphLayoutDataFromParagraph(GeneratorContext context)
		{
			ParagraphBase paragraph = context.Paragraph;
			int startRedrawLineIndex = context.StartRedrawLineIndex;
			ParagraphVisualMappingCache paragraphVisualMappingCache = context.ParagraphVisualMappingCache;
			List<LineVisualBase> list = new List<LineVisualBase>(paragraph.LineRenderData.Count);
			ParagraphLayoutData value;
			if (paragraphVisualMappingCache.TryGetValue(paragraph, out value))
			{
				List<LineVisualBase> lines = value.Lines;
				for (int i = 0; i < lines.Count; i++)
				{
					if (i < startRedrawLineIndex)
					{
						LineVisualBase lineVisualBase = lines[i];
						list.Add(lineVisualBase);
						lineVisualBase.IsAlive = true;
					}
					else
					{
						lines[i].Dispose();
					}
				}
			}
			for (int j = startRedrawLineIndex; j < paragraph.LineRenderData.Count; j++)
			{
				LineVisualBase item = new LineVisualBase(paragraph.LineRenderData[j], context.ArrangingType);
				list.Add(item);
			}
			return paragraphVisualMappingCache[paragraph] = new ParagraphLayoutData
			{
				Lines = list,
				ParagraphBefore = paragraph.TargetParagraphProperty.ParagraphBefore,
				ParagraphAfter = paragraph.TargetParagraphProperty.ParagraphAfter,
				MarkerVisual = paragraph.ParagraphTextBeforeData.MarkerCharVisual
			};
		}
	}
}
