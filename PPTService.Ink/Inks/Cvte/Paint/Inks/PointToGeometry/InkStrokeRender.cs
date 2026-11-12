using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Internal.Ink;

namespace Cvte.Paint.Inks.PointToGeometry
{
	public class InkStrokeRender
	{
		private InkStrokeRenderSettings InkStrokeRenderSettings { get; }

		public InkStrokeRender()
		{
		}

		public InkStrokeRender(InkStrokeRenderSettings settings)
		{
			InkStrokeRenderSettings = settings;
		}

		public void RenderInk(DrawingContext drawingContext, StylusPointCollection stylusPoints, DrawingAttributes drawingAttributes, Brush brush = null, bool? shouldDrawDebugRender = null)
		{
			bool flag = ((!shouldDrawDebugRender.HasValue) ? (InkStrokeRenderSettings?.IsInDebugMode ?? false) : shouldDrawDebugRender.Value);
			InkStrokeRenderDebugInfo? debugInfo = null;
			DrawingVisual drawingVisual = null;
			DrawingContext drawingContext2 = null;
			if (brush == null)
			{
				brush = new SolidColorBrush(drawingAttributes.Color);
			}
			if (flag)
			{
				drawingVisual = new DrawingVisual();
				drawingContext2 = drawingVisual.RenderOpen();
				debugInfo = new InkStrokeRenderDebugInfo(drawingContext2, 1.0);
			}
			Geometry geometry = BuildGeometry(stylusPoints, drawingAttributes, ref debugInfo);
			drawingContext.DrawGeometry(brush, null, geometry);
			if (flag)
			{
				drawingContext2.Close();
				drawingContext.DrawDrawing(drawingVisual.Drawing);
			}
		}

		public Geometry BuildGeometry(StylusPointCollection stylusPoints, DrawingAttributes drawingAttributes, [In]ref InkStrokeRenderDebugInfo? debugInfo)
		{
			InkStrokeNodeIterator iterator = new InkStrokeNodeIterator(stylusPoints, drawingAttributes, InkStrokeRenderSettings);
			return GetGeometry(iterator, drawingAttributes, debugInfo?.DebugDrawingContext, debugInfo?.FeedbackSize ?? 0.0, debugInfo.HasValue);
		}

		public Geometry GetGeometry(InkStrokeNodeIterator iterator, DrawingAttributes drawingAttributes, DrawingContext debugDC = null, double feedbackSize = 0.0, bool showFeedback = false)
		{
			StreamGeometry streamGeometry = new StreamGeometry
			{
				FillRule = FillRule.Nonzero
			};
			StreamGeometryContext streamGeometryContext = streamGeometry.Open();
			using (streamGeometryContext)
			{
				Rect empty = Rect.Empty;
				StrokeNode strokeNode = default(StrokeNode);
				StrokeNode strokeNodePrevious = default(StrokeNode);
				StrokeNode strokeNode2 = default(StrokeNode);
				StrokeNode strokeNode3 = default(StrokeNode);
				Rect rect = empty;
				Rect rect2 = empty;
				Rect rect3 = empty;
				double num = 95.0;
				double num2 = Math.Max(drawingAttributes.Height, drawingAttributes.Width);
				num += Math.Min(4.99999, num2 / 20.0 * 5.0);
				double lastAngle = double.MinValue;
				bool flag = true;
				List<Point> list = new List<Point>();
				List<Point> list2 = new List<Point>();
				List<Point> list3 = new List<Point>(4);
				int count = iterator.Count;
				int num3 = 0;
				int previousIndex = -1;
				while (num3 < count)
				{
					if (!strokeNodePrevious.IsValid)
					{
						if (!strokeNode2.IsValid)
						{
							strokeNodePrevious = iterator[num3++, previousIndex++];
							rect = strokeNodePrevious.GetBounds();
							continue;
						}
						strokeNodePrevious = strokeNode2;
						rect = rect2;
						strokeNode2 = strokeNode;
					}
					if (!strokeNode2.IsValid)
					{
						if (!strokeNode3.IsValid)
						{
							strokeNode2 = iterator[num3++, previousIndex];
							rect2 = strokeNode2.GetBounds();
							switch (StrokeRenderer.FuzzyContains(rect2, rect, flag ? 99.99999 : num))
							{
							case StrokeRenderer.RectCompareResult.Rect1ContainsRect2:
								strokeNodePrevious = iterator[num3 - 1, strokeNodePrevious.Index - 1];
								rect = Rect.Union(rect2, rect);
								strokeNode2 = strokeNode;
								previousIndex = num3 - 1;
								break;
							case StrokeRenderer.RectCompareResult.Rect2ContainsRect1:
								strokeNode2 = strokeNode;
								break;
							default:
								previousIndex = num3 - 1;
								break;
							}
							continue;
						}
						strokeNode2 = strokeNode3;
						rect2 = rect3;
						strokeNode3 = strokeNode;
					}
					if (!strokeNode3.IsValid)
					{
						strokeNode3 = iterator[num3++, previousIndex];
						rect3 = strokeNode3.GetBounds();
						StrokeRenderer.RectCompareResult rectCompareResult = StrokeRenderer.FuzzyContains(rect3, rect2, flag ? 99.99999 : num);
						StrokeRenderer.RectCompareResult rectCompareResult2 = StrokeRenderer.FuzzyContains(rect3, rect, flag ? 99.99999 : num);
						if (flag && rectCompareResult == StrokeRenderer.RectCompareResult.Rect1ContainsRect2 && rectCompareResult2 == StrokeRenderer.RectCompareResult.Rect1ContainsRect2)
						{
							if (list.Count > 0)
							{
								strokeNode2.GetPointsAtEndOfSegment(list, list2);
								StrokeRenderer.ReverseDCPointsRenderAndClear(streamGeometryContext, list, list2, list3, true, true);
							}
							strokeNodePrevious = iterator[num3 - 1, strokeNodePrevious.Index - 1];
							rect = strokeNodePrevious.GetBounds();
							strokeNode2 = strokeNode;
							strokeNode3 = strokeNode;
							previousIndex = num3 - 1;
							continue;
						}
						switch (rectCompareResult)
						{
						case StrokeRenderer.RectCompareResult.Rect1ContainsRect2:
							strokeNode3 = iterator[num3 - 1, strokeNode2.Index - 1];
							if (!strokeNode3.GetConnectingQuad().IsEmpty)
							{
								strokeNode2 = strokeNode3;
								rect2 = Rect.Union(rect3, rect2);
								previousIndex = num3 - 1;
							}
							strokeNode3 = strokeNode;
							lastAngle = double.MinValue;
							continue;
						case StrokeRenderer.RectCompareResult.Rect2ContainsRect1:
							strokeNode3 = strokeNode;
							continue;
						}
						previousIndex = num3 - 1;
					}
					bool flag2 = rect.IntersectsWith(rect3);
					if (list.Count == 0)
					{
						if (flag & flag2)
						{
							strokeNodePrevious.GetContourPoints(list3);
							StrokeRenderer.AddFigureToStreamGeometryContext(streamGeometryContext, list3, strokeNodePrevious.IsEllipse);
							list3.Clear();
						}
						strokeNode2.GetPointsAtStartOfSegment(list, list2);
						flag = false;
					}
					if (lastAngle == double.MinValue)
					{
						lastAngle = GetAngleBetween(strokeNodePrevious.Position, strokeNode2.Position);
					}
					double angleDeltaFromLast = GetAngleDeltaFromLast(strokeNode2.Position, strokeNode3.Position, ref lastAngle);
					bool flag3 = Math.Abs(angleDeltaFromLast) > 90.0 && Math.Abs(angleDeltaFromLast) < 270.0;
					bool flag4 = flag2 && strokeNode3.PressureFactor != 1f && Math.Abs(angleDeltaFromLast) > 30.0 && Math.Abs(angleDeltaFromLast) < 330.0;
					double num4 = rect2.Height * rect2.Width;
					double num5 = rect3.Height * rect3.Width;
					bool flag5 = num4 != num5 || num4 != rect.Height * rect.Width;
					bool flag6 = false;
					if ((flag2 & flag5) && Math.Min(num4, num5) / Math.Max(num4, num5) <= 0.9)
					{
						flag6 = true;
					}
					if (flag5 || angleDeltaFromLast != 0.0 || num3 >= count)
					{
						if ((flag2 && (flag4 | flag6)) | flag3)
						{
							strokeNode2.GetPointsAtEndOfSegment(list, list2);
							StrokeRenderer.ReverseDCPointsRenderAndClear(streamGeometryContext, list, list2, list3, true, true);
							if (flag6)
							{
								strokeNode2.GetContourPoints(list3);
								StrokeRenderer.AddFigureToStreamGeometryContext(streamGeometryContext, list3, strokeNode2.IsEllipse);
								list3.Clear();
							}
						}
						else
						{
							bool missingIntersection;
							strokeNode3.GetPointsAtMiddleSegment(strokeNode2, angleDeltaFromLast, list, list2, out missingIntersection);
							if (missingIntersection)
							{
								strokeNode2.GetPointsAtEndOfSegment(list, list2);
								StrokeRenderer.ReverseDCPointsRenderAndClear(streamGeometryContext, list, list2, list3, true, true);
							}
						}
					}
					strokeNodePrevious = strokeNode;
					rect = empty;
				}
				if (strokeNodePrevious.IsValid)
				{
					if (strokeNode2.IsValid)
					{
						if (list.Count > 0)
						{
							strokeNode2.GetPointsAtEndOfSegment(list, list2);
							StrokeRenderer.ReverseDCPointsRenderAndClear(streamGeometryContext, list, list2, list3, true, false);
						}
						else
						{
							StrokeRenderer.RenderTwoStrokeNodes(streamGeometryContext, strokeNodePrevious, rect, strokeNode2, rect2, list, list2, list3);
						}
					}
					else
					{
						strokeNodePrevious.GetContourPoints(list);
						StrokeRenderer.AddFigureToStreamGeometryContext(streamGeometryContext, list, strokeNodePrevious.IsEllipse);
					}
				}
				else if (strokeNode2.IsValid && strokeNode3.IsValid)
				{
					if (list.Count > 0)
					{
						strokeNode3.GetPointsAtEndOfSegment(list, list2);
						StrokeRenderer.ReverseDCPointsRenderAndClear(streamGeometryContext, list, list2, list3, true, false);
						if (StrokeRenderer.FuzzyContains(rect3, rect2, 70.0) != StrokeRenderer.RectCompareResult.NoItersection)
						{
							strokeNode3.GetContourPoints(list3);
							StrokeRenderer.AddFigureToStreamGeometryContext(streamGeometryContext, list3, strokeNode3.IsEllipse);
						}
					}
					else
					{
						StrokeRenderer.RenderTwoStrokeNodes(streamGeometryContext, strokeNode2, rect2, strokeNode3, rect3, list, list2, list3);
					}
				}
			}
			return streamGeometry;
		}

		private static void AddPolylineFigureToStreamGeometryContext(StreamGeometryContext context, List<Point> abPoints, List<Point> dcPoints)
		{
			context.BeginFigure(abPoints[0], true, true);
			context.PolyLineTo(abPoints, true, true);
			context.PolyLineTo(dcPoints, true, true);
		}

		private static void AddArcToFigureToStreamGeometryContext(StreamGeometryContext context, List<Point> abPoints, List<Point> dcPoints, List<Point> polyLinePoints)
		{
			if (abPoints.Count == 0 || dcPoints.Count == 0)
			{
				return;
			}
			context.BeginFigure(abPoints[0], true, true);
			for (int i = 0; i < 2; i++)
			{
				List<Point> list = ((i == 0) ? abPoints : dcPoints);
				int num = ((i == 0) ? 1 : 0);
				while (num < list.Count)
				{
					Point point = list[num];
					if (point == StrokeRenderer.ArcToMarker)
					{
						if (polyLinePoints.Count > 0)
						{
							context.PolyLineTo(polyLinePoints, true, true);
							polyLinePoints.Clear();
						}
						if (num + 2 < list.Count)
						{
							Point point2 = list[num + 1];
							Size size = new Size(point2.X / 2.0, point2.Y / 2.0);
							Point point3 = list[num + 2];
							bool isLargeArc = false;
							context.ArcTo(point3, size, 0.0, isLargeArc, SweepDirection.Clockwise, true, true);
						}
						num += 3;
					}
					else
					{
						polyLinePoints.Add(point);
						num++;
					}
				}
				if (polyLinePoints.Count > 0)
				{
					context.PolyLineTo(polyLinePoints, true, true);
					polyLinePoints.Clear();
				}
			}
		}

		private static double GetAngleDeltaFromLast(Point previousPosition, Point currentPosition, ref double lastAngle)
		{
			double result = 0.0;
			double num = currentPosition.X * 1000.0 - previousPosition.X * 1000.0;
			double num2 = currentPosition.Y * 1000.0 - previousPosition.Y * 1000.0;
			if ((long)num == 0L && (long)num2 == 0L)
			{
				return result;
			}
			double angleBetween = GetAngleBetween(previousPosition, currentPosition);
			if (lastAngle >= 270.0 && angleBetween <= 90.0)
			{
				result = lastAngle - (360.0 + angleBetween);
			}
			else
			{
				result = ((!(lastAngle <= 90.0) || !(angleBetween >= 270.0)) ? (lastAngle - angleBetween) : (360.0 + lastAngle - angleBetween));
			}
			lastAngle = angleBetween;
			return result;
		}

		private static double GetAngleBetween(Point previousPosition, Point currentPosition)
		{
			double result = 0.0;
			double num = currentPosition.X * 1000.0 - previousPosition.X * 1000.0;
			double num2 = currentPosition.Y * 1000.0 - previousPosition.Y * 1000.0;
			if ((long)num == 0L && (long)num2 == 0L)
			{
				return result;
			}
			if (num == 0.0)
			{
				if (num2 == 0.0)
				{
					result = 0.0;
				}
				else
				{
					result = ((!(num2 > 0.0)) ? 4.71238898038469 : (Math.PI / 2.0));
				}
			}
			else if (num2 == 0.0)
			{
				result = ((!(num > 0.0)) ? Math.PI : 0.0);
			}
			else if (num < 0.0)
			{
				result = Math.Atan(num2 / num) + Math.PI;
			}
			else
			{
				result = ((!(num2 < 0.0)) ? Math.Atan(num2 / num) : (Math.Atan(num2 / num) + Math.PI * 2.0));
			}
			return result * 180.0 / Math.PI;
		}
	}
}
