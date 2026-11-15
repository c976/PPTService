using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	internal class GeometryPool
	{
		private readonly List<CharGeometryInfo> _charGeometriesInfos = new List<CharGeometryInfo>();

		internal void SetDirty(int index, int length)
		{
			for (int i = index; i < index + length; i++)
			{
				_charGeometriesInfos[i].IsDirty = true;
			}
		}

		internal List<CharGeometryInfo> CreateCharGeometriesInfos(int count)
		{
			List<CharGeometryInfo> list = new List<CharGeometryInfo>();
			for (int i = 0; i < count; i++)
			{
				list.Add(new CharGeometryInfo(Geometry.Empty, Size.Empty, null)
				{
					IsDirty = true
				});
			}
			return list;
		}

		internal CharGeometryInfo GetCharGeometriesInfo(int index)
		{
			return _charGeometriesInfos[index];
		}

		internal List<CharGeometryInfo> GetRange(int index, int count)
		{
			List<CharGeometryInfo> list = new List<CharGeometryInfo>();
			for (int i = index; i < index + count; i++)
			{
				list.Add(_charGeometriesInfos[i]);
			}
			return list;
		}

		internal void RemoveRangeCharGeometriesInfo(int index, int count)
		{
			_charGeometriesInfos.RemoveRange(index, count);
		}

		internal void InsertRangeCharGeometriesInfos(int index, List<CharGeometryInfo> charGeometriesInfos)
		{
			_charGeometriesInfos.InsertRange(index, charGeometriesInfos);
		}

		internal void UpdateRangeCharGeometriesInfo(int index, CharGeometryInfo charGeometriesInfo)
		{
			_charGeometriesInfos[index] = charGeometriesInfo;
			_charGeometriesInfos[index].SetOriginalMatrix(charGeometriesInfo.Geometry.Transform.Value);
		}

		internal void CorrectGeometry(CharsManager charsManager, ArrangingType oldValue, ArrangingType newValue)
		{
			if (_charGeometriesInfos.Count != 0)
			{
				_charGeometriesInfos.ForEach((CharGeometryInfo x) =>
				{
					x.RecoverOriginalMatrix();
				});
				if (oldValue == ArrangingType.Horizontal)
				{
					TextHelperInner.CorrectToVerticalChar(charsManager, _charGeometriesInfos);
				}
				else if (newValue == ArrangingType.Horizontal)
				{
					TextHelperInner.CorrectToHorizontalChar(charsManager, _charGeometriesInfos);
				}
				_charGeometriesInfos.ForEach((CharGeometryInfo x) =>
				{
					x.SetOriginalMatrix(x.Geometry.Transform.Value);
				});
			}
		}
	}
}
