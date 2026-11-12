using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework.SelectionBoxs;
using Cvte.Paint.Performance;
using Cvte.Windows.Mathematics.Geometries;

namespace Cvte.Paint.Framework
{
	public class Transformer
	{
		public static readonly RoutedEvent TransformStartedEvent = EventManager.RegisterRoutedEvent("TransformStartedEvent", RoutingStrategy.Bubble, typeof(TransformRoutedEventHandler), typeof(Transformer));

		public static readonly RoutedEvent TransformAppendedEvent = EventManager.RegisterRoutedEvent("TransformAppendedEvent", RoutingStrategy.Bubble, typeof(TransformRoutedEventHandler), typeof(Transformer));

		public static readonly RoutedEvent TransformDeltaEvent = EventManager.RegisterRoutedEvent("TransformDeltaEvent", RoutingStrategy.Bubble, typeof(TransformRoutedEventHandler), typeof(Transformer));

		public static readonly RoutedEvent TransformCompletedEvent = EventManager.RegisterRoutedEvent("TransformCompletedEvent", RoutingStrategy.Bubble, typeof(TransformRoutedEventHandler), typeof(Transformer));

		private static readonly DependencyPropertyKey TransformerPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Transformer", typeof(Transformer), typeof(Transformer), new PropertyMetadata((object)null));

		public static readonly DependencyProperty TransformerProperty = TransformerPropertyKey.DependencyProperty;

		private readonly Slide _slide;

		private readonly CoTransformer _coTransformer;

		private readonly Lazy<IPerformanceCollector> _performanceCollectorLazy;

		private IContinuousProgressPerformanceCollector _collector;

		private bool _isTransforming;

		private static readonly double Sqrt2 = Math.Sqrt(2.0);

		private IPerformanceCollector PerformanceCollector => _performanceCollectorLazy.Value;

		public TransformProxyBehavior ProxyBehavior
		{
			get
			{
				return _coTransformer.ProxyBehavior;
			}
			set
			{
				_coTransformer.ProxyBehavior = value;
			}
		}

		public bool IsTransforming
		{
			get
			{
				if (!_isTransforming)
				{
					return _coTransformer.IsTransforming;
				}
				return true;
			}
			set
			{
				_isTransforming = value;
			}
		}

		[NotNull]
		public static Transformer GetTransformer([NotNull] Slide slide)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			Transformer transformer = (Transformer)slide.GetValue(TransformerProperty);
			if (transformer == null)
			{
				transformer = new Transformer(slide);
				slide.SetValue(TransformerPropertyKey, transformer);
			}
			return transformer;
		}

		private Transformer([NotNull] Slide slide)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			_slide = slide;
			_coTransformer = new CoTransformer(slide);
			_performanceCollectorLazy = new Lazy<IPerformanceCollector>(() => UIPerformance.GetPerformanceCollector(slide), LazyThreadSafetyMode.None);
		}

		public void StartTransform([NotNull] Element element, TransformBehavior behavior = TransformBehavior.Manual, TransformStepKind stepKind = TransformStepKind.RelativeToOrigin, bool isInOuterTransaction = false)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			_collector = PerformanceCollector.StartElementOperations("Transform", FormatBehaviorForLog(behavior), new Element[1] { element }, _slide);
			_collector.CollectUIDelay();
			_coTransformer.StartTransform(CreateAdapter(element), null, behavior, stepKind, isInOuterTransaction);
		}

		public void StartTransform([NotNull] ISelectionOperator selection, TransformBehavior behavior = TransformBehavior.Manual, TransformStepKind stepKind = TransformStepKind.RelativeToOrigin, bool isInOuterTransaction = false)
		{
			if (selection == null)
			{
				throw new ArgumentNullException("selection");
			}
			_collector = PerformanceCollector.StartElementOperations("Transform", FormatBehaviorForLog(behavior), selection.SelectedElements, _slide);
			_collector.CollectUIDelay();
			_coTransformer.StartTransform(new SelectionTransformer(selection), null, behavior, stepKind, isInOuterTransaction);
		}

		public void StartTransform([NotNull] IEnumerable<Element> elements, TransformBehavior behavior = TransformBehavior.Manual, TransformStepKind stepKind = TransformStepKind.RelativeToOrigin, bool isInOuterTransaction = false)
		{
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			elements = (elements as IList<Element>) ?? elements.ToList();
			_collector = PerformanceCollector.StartElementOperations("Transform", FormatBehaviorForLog(behavior), elements, _slide);
			_collector.CollectUIDelay();
			ITransformAdapter primaryTarget = ((!elements.SequenceEqual(_slide.GetSelectedElements())) ? ((ITransformAdapter)new ElementsTransformer(_slide, (IList<Element>)elements)) : ((ITransformAdapter)new SelectionTransformer(elements.First().GetContainingOperator())));
			_coTransformer.StartTransform(primaryTarget, null, behavior, stepKind, isInOuterTransaction);
		}

		public void CompleteTransform(bool isCanceled = false, CancelReason cancelResReason = CancelReason.NormalOperate)
		{
			_collector?.End();
			_collector = null;
			if (_coTransformer.IsTransforming)
			{
				_coTransformer.CompleteTransform(isCanceled, cancelResReason);
			}
		}

		public void PerformTransform(Rect targetBounds, object[] args)
		{
			_collector?.CollectUIDelay();
			_coTransformer.PerformTransform(targetBounds, args);
		}

		public void PerformTransform(Vector movedOffset, params object[] args)
		{
			_collector?.CollectUIDelay();
			_coTransformer.PerformTransform(movedOffset, args);
		}

		public void PerformTransform(double rotatedAngle, params object[] args)
		{
			_collector?.CollectUIDelay();
			_coTransformer.PerformTransform(rotatedAngle, args);
		}

		internal ITransformAdapter CreateAdapter([NotNull] Element element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			if (element.GetIsSelected())
			{
				return new SelectionTransformer(element.GetContainingOperator());
			}
			return new ElementsTransformer(_slide, new Element[1] { element });
		}

		private string FormatBehaviorForLog(TransformBehavior behavior)
		{
			switch (behavior)
			{
			case TransformBehavior.None:
				return null;
			case TransformBehavior.ScaleLeftCenter:
			case TransformBehavior.ScaleTopLeft:
			case TransformBehavior.ScaleTopCenter:
			case TransformBehavior.ScaleTopRight:
			case TransformBehavior.ScaleRightCenter:
			case TransformBehavior.ScaleBottomRight:
			case TransformBehavior.ScaleBottomCenter:
			case TransformBehavior.ScaleBottomLeft:
				return "Scale";
			default:
				return behavior.ToString();
			}
		}

		public static void Transform(ref Rect bounds, ref double rotation, ManipulationDelta delta, Point manipulationCenter)
		{
			Point point = bounds.Center();
			double num = delta.Scale.Length;
			if (double.IsNaN(delta.Scale.X))
			{
				num = delta.Scale.Y;
			}
			if (double.IsNaN(delta.Scale.Y))
			{
				num = delta.Scale.X;
			}
			double num2 = num / Sqrt2;
			if (double.IsNaN(num2))
			{
				num2 = 1.0;
			}
			Matrix matrix = default(Matrix);
			matrix.Translate(delta.Translation.X, delta.Translation.Y);
			matrix.RotateAt(delta.Rotation, manipulationCenter.X, manipulationCenter.Y);
			matrix.ScaleAt(num2, num2, manipulationCenter.X, manipulationCenter.Y);
			Point point2 = matrix.Transform(point);
			Vector vector = new Vector(bounds.Width * num2, bounds.Height * num2) / 2.0;
			rotation += delta.Rotation;
			bounds = new Rect(point2 - vector, point2 + vector);
		}

		public static Matrix GetTransformMatrix(ManipulationDelta delta, Point manipulationCenter)
		{
			double num = delta.Scale.Length;
			if (double.IsNaN(delta.Scale.X))
			{
				num = delta.Scale.Y;
			}
			if (double.IsNaN(delta.Scale.Y))
			{
				num = delta.Scale.X;
			}
			double num2 = num / Sqrt2;
			if (double.IsNaN(num2))
			{
				num2 = 1.0;
			}
			Matrix result = default(Matrix);
			result.Translate(delta.Translation.X, delta.Translation.Y);
			result.RotateAt(delta.Rotation, manipulationCenter.X, manipulationCenter.Y);
			result.ScaleAt(num2, num2, manipulationCenter.X, manipulationCenter.Y);
			return result;
		}
	}
}
