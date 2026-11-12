using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Operations;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Settings;
using Cvte.Windows.Mathematics.Geometries;

namespace Cvte.Paint.Inks
{
	internal class EraserMask
	{
		private class ClipInfo
		{
			public Geometry Clip { get; }

			public bool Cliping { get; set; }

			public ClipInfo(Geometry clip, bool cliping)
			{
				Clip = clip;
				Cliping = cliping;
			}
		}

		private Point? _prePosition;

		private DispatcherTimer _setClipTimer;

		private static readonly int MinLength = 5;

		private readonly Dictionary<Element, List<Point>> _erasePoints;

		private readonly Dictionary<Element, ClipInfo> _elementClipDictionary;

		private readonly Dictionary<Element, EraseOperationInfo> _eraseOperationInfos;

		private readonly Dictionary<Element, Geometry> _tempGeometrys;

		private readonly Dictionary<Element, bool> _canSetGeometrys;

		private readonly InkBoardSettings _inkBoardSettings;

		private Slide CurrentSlide { get; }

		public EraserMask(Slide slide)
		{
			CurrentSlide = slide;
			_erasePoints = new Dictionary<Element, List<Point>>();
			_elementClipDictionary = new Dictionary<Element, ClipInfo>();
			_eraseOperationInfos = new Dictionary<Element, EraseOperationInfo>();
			_tempGeometrys = new Dictionary<Element, Geometry>();
			_canSetGeometrys = new Dictionary<Element, bool>();
			_inkBoardSettings = slide.GetSettings<InkBoardSettings>();
			InitEraser();
		}

		private void InitEraser()
		{
			_prePosition = null;
			foreach (Element item in CurrentSlide.Elements.Where((Element element) => element.IsErasable || element.HasMask))
			{
				_elementClipDictionary.Add(item, new ClipInfo(GetElementGeometryByBounds(item), false));
				_erasePoints.Add(item, new List<Point>());
				_tempGeometrys.Add(item, null);
				_canSetGeometrys.Add(item, true);
			}
		}

		public void MoveTexture(Point position, Size size)
		{
			if (!_prePosition.HasValue)
			{
				_prePosition = position;
			}
			if (!_elementClipDictionary.Any())
			{
				return;
			}
			double num = ((_inkBoardSettings.InkScale > 0.0) ? _inkBoardSettings.InkScale : 1.0);
			if (_prePosition != position && (position - _prePosition.Value).Length < (double)MinLength * num)
			{
				return;
			}
			Geometry geometry = new RectangleGeometry(GetRect(position, size));
			foreach (KeyValuePair<Element, ClipInfo> item in _elementClipDictionary)
			{
				IntersectionDetail intersectionDetail = item.Value.Clip.FillContainsWithDetail(geometry);
				CheckClipState(item.Key, item.Value, intersectionDetail, position, size);
				if (intersectionDetail != IntersectionDetail.Empty)
				{
					Element key = item.Key;
					if (!_eraseOperationInfos.ContainsKey(key))
					{
						(key as IEraseSensitive)?.OnErasingStarted();
						_eraseOperationInfos.Add(key, new EraseOperationInfo(key)
						{
							OldGeometry = key.ClipGeometry
						});
					}
					Point elementPointBySlidePoint = GetElementPointBySlidePoint(item.Key, position);
					DrawTexture(key, elementPointBySlidePoint, size);
				}
				else
				{
					_erasePoints[item.Key].Clear();
				}
			}
			_prePosition = position;
		}

		private Rect GetRect(Point position, Size size)
		{
			return new Rect(position.X - size.Width / 2.0, position.Y - size.Height / 2.0, size.Width, size.Height);
		}

		private void CheckClipState(Element element, ClipInfo info, IntersectionDetail detail, Point position, Size size)
		{
			if (detail != IntersectionDetail.Empty && !info.Cliping)
			{
				info.Cliping = true;
				List<Point> list = _erasePoints[element];
				if (_prePosition.HasValue)
				{
					list.Add(GetElementPointBySlidePoint(element, _prePosition.Value));
				}
			}
			else if (detail == IntersectionDetail.Empty && info.Cliping)
			{
				info.Cliping = false;
				if (!_eraseOperationInfos.ContainsKey(element))
				{
					_eraseOperationInfos.Add(element, new EraseOperationInfo(element)
					{
						OldGeometry = element.ClipGeometry
					});
				}
				Point elementPointBySlidePoint = GetElementPointBySlidePoint(element, position);
				DrawTexture(element, elementPointBySlidePoint, size);
			}
		}

		private void DrawTexture(Element element, Point point, Size size)
		{
			List<Point> list = _erasePoints[element];
			list.Add(point);
			if (list.Count < 2)
			{
				list.Add(point);
			}
			Geometry erasedGeometry = GetErasedGeometry(list.First(), point, size);
			list.RemoveAt(0);
			if (_setClipTimer == null)
			{
				InitialSetClipTimer();
			}
			_tempGeometrys[element] = ((_tempGeometrys[element] == null) ? erasedGeometry : Geometry.Combine(_tempGeometrys[element], erasedGeometry, GeometryCombineMode.Union, null));
			if (_canSetGeometrys[element])
			{
				_canSetGeometrys[element] = false;
				_setClipTimer.Start();
				if (!element.IsErased)
				{
					element.IsErased = true;
				}
				if (element.HasMask)
				{
					element.SetMaskClip(_tempGeometrys[element], false);
				}
				element.SetErasedClip(_tempGeometrys[element], false);
				_eraseOperationInfos[element].NewGeometry = element.ClipGeometry;
				_tempGeometrys[element] = null;
			}
		}

		private void InitialSetClipTimer()
		{
			_setClipTimer = new DispatcherTimer();
			_setClipTimer.Interval = TimeSpan.FromMilliseconds(16.0);
			_setClipTimer.Tick += (object sender, EventArgs args) =>
			{
				Element[] array = _canSetGeometrys.Keys.ToArray();
				foreach (Element key in array)
				{
					_canSetGeometrys[key] = true;
				}
				_setClipTimer.Stop();
			};
		}

		private Geometry GetErasedGeometry(Point p1, Point p2, Size size)
		{
			Point[] geometryPoints = GetGeometryPoints(p1, size);
			Point[] geometryPoints2 = GetGeometryPoints(p2, size);
			RectangleGeometry geometry = new RectangleGeometry(new Rect(geometryPoints[0], size));
			RectangleGeometry geometry2 = new RectangleGeometry(new Rect(geometryPoints2[0], size));
			PathGeometry geometry3 = Geometry.Combine(geometry, geometry2, GeometryCombineMode.Union, null);
			Point[] array = new Point[4];
			if ((p1.X <= p2.X && p1.Y <= p2.Y) || (p1.X > p2.X && p1.Y > p2.Y))
			{
				array[0] = geometryPoints[1];
				array[1] = geometryPoints[3];
				array[2] = geometryPoints2[3];
				array[3] = geometryPoints2[1];
			}
			else
			{
				array[0] = geometryPoints[0];
				array[1] = geometryPoints[2];
				array[2] = geometryPoints2[2];
				array[3] = geometryPoints2[0];
			}
			StreamGeometry streamGeometry = new StreamGeometry();
			using (StreamGeometryContext streamGeometryContext = streamGeometry.Open())
			{
				streamGeometryContext.BeginFigure(array[0], true, false);
				for (int i = 1; i < array.Length; i++)
				{
					streamGeometryContext.LineTo(array[i], false, false);
				}
			}
			return Geometry.Combine(geometry3, streamGeometry, GeometryCombineMode.Union, null);
		}

		private Point[] GetGeometryPoints(Point position, Size size)
		{
			return new Point[4]
			{
				new Point(position.X - size.Width / 2.0, position.Y - size.Height / 2.0),
				new Point(position.X + size.Width / 2.0, position.Y - size.Height / 2.0),
				new Point(position.X + size.Width / 2.0, position.Y + size.Height / 2.0),
				new Point(position.X - size.Width / 2.0, position.Y + size.Height / 2.0)
			};
		}

		public ElementOperation CreateUndoRedo()
		{
			if (!_eraseOperationInfos.Any())
			{
				return null;
			}
			return new ErasedElementOperation(CurrentSlide, _eraseOperationInfos.Select((KeyValuePair<Element, EraseOperationInfo> temp) => temp.Value).ToArray());
		}

		public IList<Element> GetErasedElementList()
		{
			return _eraseOperationInfos.Select((KeyValuePair<Element, EraseOperationInfo> temp) => temp.Key).ToList();
		}

		[Pure]
		public Geometry GetElementGeometryByBounds(Element element)
		{
			if (element.Children.Count == 0)
			{
				return new RectangleGeometry(element.Bounds);
			}
			ITransformedBounds transformedBounds = element as ITransformedBounds;
			if (transformedBounds != null)
			{
				return new RectangleGeometry(transformedBounds.TransformedBounds);
			}
			return new RectangleGeometry(element.Outline.GetExternalBounds());
		}

		private Point GetElementPointBySlidePoint(Element element, Point point)
		{
			point = CurrentSlide.TranslatePoint(point, element);
			Rect bounds = element.Bounds;
			Point point2 = new Point(bounds.Width / 2.0, bounds.Height / 2.0);
			Vector vector = Geometry2D.Rotate(point - point2, element.Rotation);
			return point2 + vector;
		}
	}
}
