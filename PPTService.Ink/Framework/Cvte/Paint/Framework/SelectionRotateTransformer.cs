using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Cvte.Paint.Framework
{
	public class SelectionRotateTransformer : SelectionPartitionTransformer
	{
		private static readonly double[] AlignAngles = new double[4] { 0.0, 90.0, 180.0, 270.0 };

		private List<double> _aligningAngles;

		private double _originAngle;

		private static List<double> _sensitiveAngles = new List<double>();

		private static bool _sensitive;

		private const double CtrlAligningStep = 15.0;

		private static readonly Lazy<double[]> CtrlAligningAngles = new Lazy<double[]>(() => (from x in Enumerable.Range(0, 24)
			select (double)x * 15.0).ToArray());

		protected override void OnStarted()
		{
			_originAngle = Owner.Rotation;
			if (TransformingElements.Length == 1)
			{
				IRotateSensitive rotateSensitive = TransformingElements[0] as IRotateSensitive;
				if (rotateSensitive != null)
				{
					_sensitive = true;
					_sensitiveAngles = rotateSensitive.GetSensitiveAngles();
					Aligner.Prepare(Slide, Slide.Elements.Except(TransformingElements), TransformingElements);
					return;
				}
			}
			_aligningAngles = AlignAngles.ToList();
			if (!_aligningAngles.Contains(_originAngle))
			{
				_aligningAngles.Add(_originAngle);
			}
		}

		protected override void OnDelta(double userRotated, object[] args)
		{
			double num = (args.Any() ? ((double)args[0]) : 0.0);
			double num2 = _originAngle + userRotated;
			num2 = Math.Round((num2 % 360.0 + 360.0) % 360.0, 1);
			if (_sensitive)
			{
				Aligner.Clear();
			}
			if (!SelectionPartitionTransformer.WithAlt)
			{
				double num3;
				if (SelectionPartitionTransformer.WithShift)
				{
					num3 = 15.0;
				}
				else
				{
					num3 = ((num <= 0.0) ? 3.0 : (Math.Asin(10.0 / num) * 180.0 / Math.PI));
					num3 = Math.Min(15.0, num3);
					num3 = Math.Max(3.0, num3);
				}
				ValueTuple<double, double> valueTuple = FindNearestMinAngle(num2, _aligningAngles);
				double item = valueTuple.Item1;
				if (valueTuple.Item2 < num3)
				{
					num2 = item;
					if (_sensitive && !(Aligner is EmptyAlignmentEngine))
					{
						IRotateSensitive rotateSensitive = TransformingElements[0] as IRotateSensitive;
						if (rotateSensitive != null)
						{
							AlignResult alignResult = rotateSensitive.GetAlignResult(num2);
							Aligner.DrawAlignResult(alignResult);
						}
					}
				}
			}
			DesiredRotation = num2;
		}

		protected override void OnCompleted()
		{
			_originAngle = 0.0;
			if (_sensitive)
			{
				Aligner?.Clear();
				_sensitive = false;
			}
			else
			{
				_aligningAngles = null;
			}
		}

		public static ValueTuple<double, double> FindNearestMinAngle(double angle, IList<double> aligningAngles = null)
		{
			if (aligningAngles == null)
			{
				aligningAngles = AlignAngles.ToList();
			}
			IList<double> list;
			if (!SelectionPartitionTransformer.WithShift)
			{
				list = aligningAngles;
			}
			else
			{
				IList<double> list2 = aligningAngles.Union(CtrlAligningAngles.Value).Distinct().ToList();
				list = list2;
			}
			aligningAngles = list;
			IList<double> list3;
			if (!_sensitive)
			{
				list3 = aligningAngles;
			}
			else
			{
				IList<double> list2 = _sensitiveAngles;
				list3 = list2;
			}
			IList<double> list4 = list3;
			double item = list4[0];
			double num = Math.Abs(list4[0] - angle);
			num = ((num > 180.0) ? (360.0 - num) : num);
			for (int i = 1; i < list4.Count; i++)
			{
				double num2 = list4[i];
				double num3 = Math.Abs(num2 - angle);
				num3 = ((num3 > 180.0) ? (360.0 - num3) : num3);
				if (num3 < num)
				{
					num = num3;
					item = num2;
				}
			}
			return new ValueTuple<double, double>(item, num);
		}

		[Obsolete("建议使用 FindNearestMinAngle 方法代替")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Tuple<double, double> FindNearestAngle(double angle, IList<double> aligningAngles = null)
		{
			ValueTuple<double, double> valueTuple = FindNearestMinAngle(angle, aligningAngles);
			double item = valueTuple.Item1;
			double item2 = valueTuple.Item2;
			return new Tuple<double, double>(item, item2);
		}

		public static bool IsRotationBigEnough(double rotation, double threshold)
		{
			if (Math.Abs(rotation) % 360.0 > threshold)
			{
				return Math.Abs(rotation) % 360.0 < 360.0 - threshold;
			}
			return false;
		}
	}
}
