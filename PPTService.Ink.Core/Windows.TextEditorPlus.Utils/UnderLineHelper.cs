using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Render;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	internal class UnderLineHelper
	{
		private class CharPropertySegment : ISegment
		{
			public int StartOffset { get; }

			public int Length { get; set; } = 1;

			public int EndOffset => StartOffset + Length;

			public int FrontOffset
			{
				get
				{
					if (StartOffset <= EndOffset)
					{
						return StartOffset;
					}
					return EndOffset;
				}
			}

			public int BehindOffset
			{
				get
				{
					if (StartOffset <= EndOffset)
					{
						return EndOffset;
					}
					return StartOffset;
				}
			}

			public CharPropertySegment(int startOffset)
			{
				StartOffset = startOffset;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass1_0
		{
			public LineOffset lastUnderLineCharLineOffset;

			public DocumentOffset startOffset;

			public RunProperty rawLastRunProperty;

			public CharPropertySegment lastSamePropertyIndexes;

			public RunProperty lastRunProperty;

			public List<ISegment> samePropertyIndexesList;

			public PropertyManager propertyManager;

			public LineInfo newLineInfo;

			internal void _003CGetUnderLineAndArtSamePropertySegmentList_003Eg__AddSameProperty_007C0(int offset, ref _003C_003Ec__DisplayClass1_2 P_1)
			{
				lastUnderLineCharLineOffset = new LineOffset(offset, startOffset);
				rawLastRunProperty = P_1.rawCurrentRunProperty;
				lastSamePropertyIndexes.Length++;
			}

			internal void _003CGetUnderLineAndArtSamePropertySegmentList_003Eg__ChangeSamePropertyList_007C1(int offset, ref _003C_003Ec__DisplayClass1_1 P_1, ref _003C_003Ec__DisplayClass1_2 P_2)
			{
				lastUnderLineCharLineOffset = new LineOffset(offset, startOffset);
				rawLastRunProperty = P_2.rawCurrentRunProperty;
				lastRunProperty = P_2.currentRunProperty;
				lastSamePropertyIndexes = new CharPropertySegment(P_1.i);
				if (IsRunPropertyExistUnderLine(P_2.rawCurrentRunProperty))
				{
					samePropertyIndexesList.Add(lastSamePropertyIndexes);
				}
			}

			internal RunProperty _003CGetUnderLineAndArtSamePropertySegmentList_003Eg__GetLastRunProperty_007C2()
			{
				if (lastRunProperty == null)
				{
					DocumentOffset documentOffset = lastUnderLineCharLineOffset.DocumentOffset;
					lastRunProperty = propertyManager.GetRenderRunProperty(documentOffset);
				}
				return lastRunProperty;
			}
		}

		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass1_1
		{
			public int i;
		}

		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass1_2
		{
			public RunProperty rawCurrentRunProperty;

			public RunProperty currentRunProperty;
		}

		public static IReadOnlyList<ISegment> GetUnderLineAndArtSamePropertySegmentList(LineInfo newLineInfo, DocumentOffset startOffset, PropertyManager propertyManager)
		{
			_003C_003Ec__DisplayClass1_0 obj = new _003C_003Ec__DisplayClass1_0();
			obj.startOffset = startOffset;
			obj.propertyManager = propertyManager;
			obj.newLineInfo = newLineInfo;
			obj.samePropertyIndexesList = new List<ISegment>();
			obj.lastRunProperty = null;
			if (obj.newLineInfo.CharVisualsCount == 0)
			{
				return obj.samePropertyIndexesList;
			}
			ValueTuple<bool, CharPropertySegment, LineOffset, RunProperty> valueTuple = TryFindFirstUnderLineCharLineOffset(obj.newLineInfo, obj.startOffset, obj.propertyManager);
			bool item = valueTuple.Item1;
			obj.lastSamePropertyIndexes = valueTuple.Item2;
			LineOffset item2 = valueTuple.Item3;
			obj.rawLastRunProperty = valueTuple.Item4;
			if (!item)
			{
				return obj.samePropertyIndexesList;
			}
			obj.samePropertyIndexesList.Add(obj.lastSamePropertyIndexes);
			obj.lastUnderLineCharLineOffset = item2;
			_003C_003Ec__DisplayClass1_1 obj2 = default(_003C_003Ec__DisplayClass1_1);
			obj2.i = obj.lastUnderLineCharLineOffset.Offset + 1;
			_003C_003Ec__DisplayClass1_2 obj3 = default(_003C_003Ec__DisplayClass1_2);
			while (obj2.i < obj.newLineInfo.CharVisualsCount)
			{
				if (obj.newLineInfo.CharVisuals[obj2.i] is TextCharVisual)
				{
					DocumentOffset offset = new DocumentOffset((int)obj.startOffset + obj2.i);
					obj3.rawCurrentRunProperty = obj.propertyManager.GetRunProperty(offset);
					obj3.currentRunProperty = null;
					if (obj2.i - obj.lastUnderLineCharLineOffset.Offset == 1)
					{
						if (obj.rawLastRunProperty == obj3.rawCurrentRunProperty)
						{
							obj._003CGetUnderLineAndArtSamePropertySegmentList_003Eg__AddSameProperty_007C0(obj2.i, ref obj3);
						}
						else
						{
							obj3.currentRunProperty = obj.propertyManager.GetRenderRunProperty(offset);
							obj.lastRunProperty = obj._003CGetUnderLineAndArtSamePropertySegmentList_003Eg__GetLastRunProperty_007C2();
							if (CheckSameUnderLineAndArtRunProperty(obj.lastRunProperty, obj3.currentRunProperty))
							{
								obj._003CGetUnderLineAndArtSamePropertySegmentList_003Eg__AddSameProperty_007C0(obj2.i, ref obj3);
							}
							else
							{
								obj._003CGetUnderLineAndArtSamePropertySegmentList_003Eg__ChangeSamePropertyList_007C1(obj2.i, ref obj2, ref obj3);
							}
						}
					}
					else
					{
						obj._003CGetUnderLineAndArtSamePropertySegmentList_003Eg__ChangeSamePropertyList_007C1(obj2.i, ref obj2, ref obj3);
					}
				}
				else
				{
					_003C_003Ec__DisplayClass1_0 obj4 = obj;
					int n = obj2.i;
					TextTracer.Debug(() => $"发现不支持计算下划线的文字{obj4.newLineInfo.CharVisuals[n]}", "下划线计算");
				}
				obj2.i++;
			}
			return obj.samePropertyIndexesList;
		}

		
		private static ValueTuple<bool, CharPropertySegment, LineOffset, RunProperty> TryFindFirstUnderLineCharLineOffset(LineInfo newLineInfo, DocumentOffset startOffset, PropertyManager propertyManager)
		{
			for (int i = 0; i < newLineInfo.CharVisualsCount; i++)
			{
				LineOffset item = new LineOffset(startOffset, i);
				DocumentOffset documentOffset = item.DocumentOffset;
				RunProperty runProperty = propertyManager.GetRunProperty(documentOffset);
				if (IsRunPropertyExistUnderLine(runProperty))
				{
					if (newLineInfo.CharVisuals[item.Offset] is TextCharVisual)
					{
						CharPropertySegment item2 = new CharPropertySegment(item.Offset);
						return new ValueTuple<bool, CharPropertySegment, LineOffset, RunProperty>(true, item2, item, runProperty);
					}
					TextTracer.Debug(() => $"发现不支持计算下划线的文字{newLineInfo.CharVisuals[0]}", "下划线计算");
				}
			}
			return new ValueTuple<bool, CharPropertySegment, LineOffset, RunProperty>(false, null, default(LineOffset), null);
		}

		private static bool IsRunPropertyExistUnderLine(RunProperty runProperty)
		{
			if (runProperty.Hyperlink != null)
			{
				return true;
			}
			if (runProperty.TextDecoration != null && runProperty.TextDecoration.Count > 0)
			{
				return true;
			}
			return false;
		}

		private static bool CheckSameUnderLineAndArtRunProperty(RunProperty lastRunProperty, RunProperty currentRunProperty)
		{
			if (Converter.AreEquals(lastRunProperty.TextDecoration, currentRunProperty.TextDecoration) && Converter.AreEquals(lastRunProperty.EffectsArgs, currentRunProperty.EffectsArgs) && Converter.AreEquals(lastRunProperty.FontVariants, currentRunProperty.FontVariants) && Converter.AreEquals(lastRunProperty.Foreground, currentRunProperty.Foreground))
			{
				return Converter.AreEquals(lastRunProperty.UnderlineBrush, currentRunProperty.UnderlineBrush);
			}
			return false;
		}
	}
}
