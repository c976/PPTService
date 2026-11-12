using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Cvte.Paint.Framework
{
	public static class GeometryHelper
	{
		public static Geometry GetBezierGeometry(List<Point> freeLinePoints, BezierInfos bezierInfos, bool isClosed, bool isSmoothJoin = true)
		{
			StreamGeometry streamGeometry = new StreamGeometry();
			using (StreamGeometryContext streamGeometryContext = streamGeometry.Open())
			{
				streamGeometryContext.BeginFigure(freeLinePoints[0], isClosed, isClosed);
				for (int i = 0; i < freeLinePoints.Count - 1; i++)
				{
					if (bezierInfos.StartIndex.Contains(i + 1))
					{
						streamGeometryContext.BeginFigure(freeLinePoints[i + 1], isClosed, isClosed);
					}
					else if (!TryDrawBezierLine(freeLinePoints, bezierInfos, isClosed, isSmoothJoin, streamGeometryContext, ref i))
					{
						streamGeometryContext.LineTo(freeLinePoints[i + 1], true, true);
					}
				}
				return streamGeometry;
			}
		}

		private static bool TryDrawBezierLine(List<Point> points, BezierInfos bezierInfos, bool isClosed, bool isSmoothJoin, StreamGeometryContext ctx, ref int index)
		{
			if (bezierInfos.BezierIndexList.Contains(index))
			{
				int index2 = bezierInfos.BezierIndexList.IndexOf(index);
				if (bezierInfos.BezierTypes[index2] == BezierType.Quadratic)
				{
					int index3 = ((index + 2 < points.Count) ? (index + 2) : 0);
					ctx.QuadraticBezierTo(points[index + 1], points[index3], true, true);
					index++;
				}
				else
				{
					int index4 = ((index + 3 < points.Count) ? (index + 3) : 0);
					ctx.BezierTo(points[index + 1], points[index + 2], points[index4], true, true);
					index += 2;
				}
				return true;
			}
			return false;
		}

		public static List<Geometry> GetBezierGeometries(List<Point> freeLinePoints, List<BezierInfos> bezierInfoses, List<int> pointCounts, bool isClosed)
		{
			List<Geometry> list = new List<Geometry>(bezierInfoses.Count);
			int num = 0;
			for (int i = 0; i < bezierInfoses.Count; i++)
			{
				list.Add(GetBezierGeometry(freeLinePoints.GetRange(num, pointCounts[i]), bezierInfoses[i], isClosed));
				num += pointCounts[i];
			}
			return list;
		}

		public static Point GetBezierP1(Point p02, Point p0, Point p2)
		{
			double num = 0.5;
			Point point = new Point((1.0 - num) * (1.0 - num) * p0.X, (1.0 - num) * (1.0 - num) * p0.Y);
			Point point2 = new Point(num * num * p2.X, num * num * p2.Y);
			return new Point((p02.X - point.X - point2.X) / (2.0 * num * (1.0 - num)), (p02.Y - point.Y - point2.Y) / (2.0 * num * 1.0 - num));
		}

		public static List<Point> GetKeyPointsFromData(string data, out BezierInfos bezierInfos)
		{
			bezierInfos = new BezierInfos();
			List<Point> list = new List<Point>();
			data = data.ToUpper(CultureInfo.InvariantCulture);
			string[] array = data.Split(new char[1] { 'M' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string mData in array)
			{
				GetPointsAndBezierInfos(bezierInfos, mData, list);
			}
			return list;
		}

		private static void GetPointsAndBezierInfos(BezierInfos bezierInfos, string mData, List<Point> points)
		{
			string[] array = mData.Split(new char[2] { 'L', 'Z' }, StringSplitOptions.RemoveEmptyEntries);
			bezierInfos.StartIndex.Add(points.Count);
			string[] array2 = array;
			foreach (string text in array2)
			{
				string[] array3 = text.Split(new char[2] { 'Q', 'C' }, StringSplitOptions.RemoveEmptyEntries);
				char[] array4 = text.ToUpper(CultureInfo.InvariantCulture).ToCharArray();
				List<BezierType> list = new List<BezierType>();
				char[] array5 = array4;
				for (int j = 0; j < array5.Length; j++)
				{
					switch (array5[j])
					{
					case 'Q':
						list.Add(BezierType.Quadratic);
						break;
					case 'C':
						list.Add(BezierType.Cubic);
						break;
					}
				}
				for (int k = 0; k < array3.Length; k++)
				{
					if (k == 0)
					{
						string[] source = array3[k].Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
						points.AddRange(source.Select(Point.Parse));
						continue;
					}
					List<Point> list2 = new List<Point>();
					string[] array6 = array3[k].Split(' ');
					foreach (string text2 in array6)
					{
						if (!string.IsNullOrWhiteSpace(text2))
						{
							string[] array7 = text2.Split(',');
							for (int l = 0; l < array7.Length; l += 2)
							{
								list2.Add(new Point(double.Parse(array7[l], CultureInfo.InvariantCulture), double.Parse(array7[l + 1], CultureInfo.InvariantCulture)));
							}
						}
					}
					int num = ((list[k - 1] == BezierType.Quadratic) ? 2 : 3);
					for (int m = 0; m < list2.Count; m += num)
					{
						bezierInfos.BezierIndexList.Add(points.Count - 1);
						bezierInfos.BezierTypes.Add(list[k - 1]);
						for (int n = m; n < m + num; n++)
						{
							points.Add(list2[n]);
						}
					}
				}
			}
		}

		public static Dictionary<int, List<Point>> GetKeyPointsFromDataRespectively(string data, out List<BezierInfos> bezierInfos)
		{
			bezierInfos = new List<BezierInfos>();
			Dictionary<int, List<Point>> dictionary = new Dictionary<int, List<Point>>();
			data = data.ToUpper(CultureInfo.InvariantCulture);
			data = data.Replace("F1", "").Replace("F0", "");
			string[] array = data.Split(new char[1] { 'M' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				dictionary.Add(i, new List<Point>());
				bezierInfos.Add(new BezierInfos());
				GetPointsAndBezierInfos(bezierInfos[i], array[i], dictionary[i]);
			}
			return dictionary;
		}

		public static Dictionary<int, List<Point>> GetKeyPointsFromDataRespectively(string data, out List<PathResolveInfos> infos)
		{
			infos = new List<PathResolveInfos>();
			Dictionary<int, List<Point>> dictionary = new Dictionary<int, List<Point>>();
			data = data.Replace("F0", "").Replace("F1", "");
			string[] array = data.Split(new char[1] { 'M' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				dictionary.Add(i, new List<Point>());
				PathResolveInfos pathResolveInfos = new PathResolveInfos();
				GetPointsAndResolveInfos(pathResolveInfos, array[i], dictionary[i]);
				infos.Add(pathResolveInfos);
			}
			return dictionary;
		}

		public static List<Geometry> GetGeometries(List<Point> points, List<PathResolveInfos> infos, List<int> pointCounts, bool isClosed, bool isSmoothJoin)
		{
			List<Geometry> list = new List<Geometry>(infos.Count);
			int num = 0;
			for (int i = 0; i < infos.Count; i++)
			{
				list.Add(GetGeometry(points.GetRange(num, pointCounts[i]), infos[i], isClosed, isSmoothJoin));
				num += pointCounts[i];
			}
			return list;
		}

		private static Geometry GetGeometry(List<Point> points, PathResolveInfos info, bool isClosed, bool isSmoothJoin)
		{
			StreamGeometry streamGeometry = new StreamGeometry();
			using (StreamGeometryContext streamGeometryContext = streamGeometry.Open())
			{
				streamGeometryContext.BeginFigure(points[0], isClosed, isClosed);
				for (int i = 0; i < points.Count - 1; i++)
				{
					if (!TryDrawBezierLine(points, info.BezierInfos, isClosed, isSmoothJoin, streamGeometryContext, ref i))
					{
						if (info.ArcInfos.StartIndexes.Contains(i))
						{
							int index = info.ArcInfos.StartIndexes.IndexOf(i);
							ArcInfo arcInfo = info.ArcInfos.Infos[index];
							streamGeometryContext.ArcTo(arcInfo.EndPoint, arcInfo.RadiusSize, arcInfo.Rotation, arcInfo.IsLargeArc, arcInfo.SweepDirection, true, isSmoothJoin);
						}
						else
						{
							streamGeometryContext.LineTo(points[i + 1], true, true);
						}
					}
				}
				return streamGeometry;
			}
		}

		private static void GetPointsAndResolveInfos(PathResolveInfos infos, string mData, List<Point> points)
		{
			string[] array = mData.Split(PathFigureSyntax.LineChars.Concat(PathFigureSyntax.CloseChars).ToArray(), StringSplitOptions.RemoveEmptyEntries);
			foreach (string obj in array)
			{
				string[] array2 = obj.Split(PathFigureSyntax.QuadraticBezierChars.Concat(PathFigureSyntax.CubicBezierChars).Concat(PathFigureSyntax.ArcChars).ToArray(), StringSplitOptions.RemoveEmptyEntries);
				List<CurveType> list = new List<CurveType>();
				string text = obj;
				foreach (char value in text)
				{
					if (PathFigureSyntax.QuadraticBezierChars.Contains(value))
					{
						list.Add(CurveType.Quadratic);
					}
					else if (PathFigureSyntax.CubicBezierChars.Contains(value))
					{
						list.Add(CurveType.Cubic);
					}
					else if (PathFigureSyntax.ArcChars.Contains(value))
					{
						list.Add(CurveType.Arc);
					}
				}
				for (int k = 0; k < array2.Length; k++)
				{
					BezierType result;
					if (k == 0)
					{
						string[] source = array2[k].Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
						points.AddRange(source.Select(Point.Parse));
					}
					else if (Enum.TryParse<BezierType>(list[k - 1].ToString(), out result))
					{
						List<Point> list2 = new List<Point>();
						string[] array3 = array2[k].Split(' ');
						foreach (string text2 in array3)
						{
							if (!string.IsNullOrWhiteSpace(text2))
							{
								string[] array4 = text2.Split(',');
								for (int l = 0; l < array4.Length; l += 2)
								{
									list2.Add(new Point(double.Parse(array4[l], CultureInfo.InvariantCulture), double.Parse(array4[l + 1], CultureInfo.InvariantCulture)));
								}
							}
						}
						int num = ((result == BezierType.Quadratic) ? 2 : 3);
						for (int m = 0; m < list2.Count; m += num)
						{
							infos.BezierInfos.BezierIndexList.Add(points.Count - 1);
							infos.BezierInfos.BezierTypes.Add(result);
							for (int n = m; n < m + num; n++)
							{
								points.Add(list2[n]);
							}
						}
					}
					else if (list[k - 1] == CurveType.Arc && !TryAnalyzeArcInfo(array2[k], infos, points))
					{
						throw new ArgumentException("解析到的Arc参数不正确!");
					}
				}
			}
		}

		private static bool TryAnalyzeArcInfo(string data, PathResolveInfos infos, List<Point> points)
		{
			if (!TryAnalyzeArcInfoByWhiteSpace(data, infos, points))
			{
				return TryAnalyzeArcInfoByComma(data, infos, points);
			}
			return true;
		}

		private static bool TryAnalyzeArcInfoByWhiteSpace(string data, PathResolveInfos infos, List<Point> points)
		{
			string[] source = data.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			source = source.Where((string d) => !string.IsNullOrWhiteSpace(d)).ToArray();
			if (source.Length % 5 != 0)
			{
				return false;
			}
			for (int num = 0; num < source.Length; num += 5)
			{
				ArcInfo arcInfo = new ArcInfo
				{
					StartPoint = points.Last(),
					RadiusSize = Size.Parse(source[num]),
					Rotation = Convert.ToDouble(source[num + 1]),
					IsLargeArc = (Convert.ToInt16(source[num + 2]) == 1),
					SweepDirection = ((Convert.ToInt16(source[num + 3]) == 1) ? SweepDirection.Clockwise : SweepDirection.Counterclockwise),
					EndPoint = Point.Parse(source[num + 4])
				};
				infos.ArcInfos.StartIndexes.Add(points.Count - 1);
				points.Add(arcInfo.EndPoint);
				infos.ArcInfos.Infos.Add(arcInfo);
			}
			return true;
		}

		private static bool TryAnalyzeArcInfoByComma(string data, PathResolveInfos infos, List<Point> points)
		{
			string[] source = data.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			source = source.Where((string d) => !string.IsNullOrWhiteSpace(d)).ToArray();
			if (source.Length % 7 != 0)
			{
				return false;
			}
			for (int num = 0; num < source.Length; num += 7)
			{
				ArcInfo arcInfo = new ArcInfo
				{
					StartPoint = points.Last(),
					RadiusSize = new Size(Convert.ToDouble(source[num]), Convert.ToDouble(source[num + 1])),
					Rotation = Convert.ToDouble(source[num + 2]),
					IsLargeArc = (Convert.ToInt16(source[num + 3]) == 1),
					SweepDirection = ((Convert.ToInt16(source[num + 4]) == 1) ? SweepDirection.Clockwise : SweepDirection.Counterclockwise),
					EndPoint = new Point(Convert.ToDouble(source[num + 5]), Convert.ToDouble(source[num + 6]))
				};
				infos.ArcInfos.StartIndexes.Add(points.Count - 1);
				points.Add(arcInfo.EndPoint);
				infos.ArcInfos.Infos.Add(arcInfo);
			}
			return true;
		}

		public static Point GetQuarticPoint(Point p0, Point p1, Point p2)
		{
			double num = 0.5;
			double x = p0.X * Math.Pow(1.0 - num, 2.0) + 2.0 * num * (1.0 - num) * p1.X + Math.Pow(num, 2.0) * p2.X;
			double y = p0.Y * Math.Pow(1.0 - num, 2.0) + 2.0 * num * (1.0 - num) * p1.Y + Math.Pow(num, 2.0) * p2.Y;
			return new Point(x, y);
		}

		public static Point GetQuadraticBezierPoint(Point p0, Point p1, Point p2, double t)
		{
			double num = Math.Pow(1.0 - t, 2.0);
			double num2 = 2.0 * t * (1.0 - t);
			double num3 = t * t;
			return new Point(num * p0.X + num2 * p1.X + num3 * p2.X, num * p0.Y + num2 * p1.Y + num3 * p2.Y);
		}

		public static Point GetCubicBezierPoint(Point p0, Point p1, Point p2, Point p3, double t)
		{
			double num = Math.Pow(1.0 - t, 3.0);
			double num2 = 3.0 * Math.Pow(1.0 - t, 2.0) * t;
			double num3 = 3.0 * (1.0 - t) * t * t;
			double num4 = t * t * t;
			return new Point(num * p0.X + num2 * p1.X + num3 * p2.X + num4 * p3.X, num * p0.Y + num2 * p1.Y + num3 * p2.Y + num4 * p3.Y);
		}

		public static Point GetBezierP1(Point p02, Point p0, Point p2, ref Point p01, ref Point p11)
		{
			double num = 0.5;
			Point point = new Point((1.0 - num) * (1.0 - num) * p0.X, (1.0 - num) * (1.0 - num) * p0.Y);
			Point point2 = new Point(num * num * p2.X, num * num * p2.Y);
			Point result = new Point((p02.X - point.X - point2.X) / (2.0 * num * (1.0 - num)), (p02.Y - point.Y - point2.Y) / (2.0 * num * 1.0 - num));
			p01 = new Point((1.0 - num) * p0.X + num * result.X, (1.0 - num) * p0.Y + num * result.Y);
			p11 = new Point((1.0 - num) * result.X + num * p2.X, (1.0 - num) * result.Y + num * p2.Y);
			return result;
		}

		public static IEnumerable<Point> GetPoints(Geometry geometry)
		{
			PathSegment[] source = geometry.GetFlattenedPathGeometry().Figures.SelectMany((PathFigure x) => x.Segments).ToArray();
			List<Point> list = source.OfType<PolyLineSegment>().SelectMany((PolyLineSegment x) => x.Points).Concat(from x in source.OfType<LineSegment>()
				select x.Point)
				.Concat(source.OfType<QuadraticBezierSegment>().SelectMany((QuadraticBezierSegment x) => new Point[2] { x.Point1, x.Point2 }))
				.ToList();
			if (!list.Any())
			{
				BezierInfos bezierInfos;
				list = GetKeyPointsFromData(geometry.ToString(CultureInfo.InvariantCulture), out bezierInfos);
			}
			return list;
		}
	}
}
