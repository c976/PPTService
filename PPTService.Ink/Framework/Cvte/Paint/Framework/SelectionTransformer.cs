using System;
using System.Collections.Generic;
using System.Windows;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework.SelectionBoxs;

namespace Cvte.Paint.Framework
{
	public sealed class SelectionTransformer : ITransformAdapter
	{
		private SelectionPartitionTransformer _partition;

		private Rect _originBounds;

		private double _originAngle;

		[NotNull]
		internal ISelectionOperator Owner { get; }

		private ElementsTransformer Container { get; }

		public IList<Element> Elements => Owner.SelectedElements;

		public Rect Bounds => Owner.Bounds;

		public double Rotation => Owner.Rotation;

		public Rect DesiredBounds { get; private set; }

		public double DesiredRotation { get; private set; }

		public SelectionTransformer([NotNull] ISelectionOperator owner)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}
			Owner = owner;
			Container = new ElementsTransformer((DependencyObject)owner, owner.SelectedElements);
		}

		public void StartTransform(TransformRoutedEventArgs args)
		{
			_originBounds = Owner.Bounds;
			_originAngle = Owner.Rotation;
			_partition = CreatePartition(args.PrimaryBehavior);
			_partition.StartTransform(this, args);
			Container.StartTransform(_originBounds, _originAngle, args);
			Owner.NotifyTransformStarted(args);
		}

		public void CompleteTransform(TransformRoutedEventArgs args)
		{
			Owner.NotifyTransformCompleted(args);
			Container.CompleteTransform(args);
			_partition.CompleteTransform(args.IsCanceled);
			_partition = null;
		}

		public void PerformTransform(Rect targetBounds, object[] args)
		{
			_partition.PerformTransform(targetBounds, args);
			DesiredBounds = _partition.DesiredBounds;
			DesiredRotation = _originAngle;
			Container.PerformTransform(targetBounds, args);
		}

		public void PerformTransform(Vector movedOffset, object[] args)
		{
			_partition.PerformTransform(movedOffset, args);
			DesiredBounds = _partition.DesiredBounds;
			DesiredRotation = _originAngle;
			Container.PerformTransform(_partition.DesiredBounds, args);
		}

		public void PerformTransform(double angle, object[] args)
		{
			_partition.PerformTransform(angle, args);
			DesiredBounds = _originBounds;
			DesiredRotation = _partition.DesiredRotation;
			Container.PerformTransform(_partition.DesiredRotation - _originAngle, args);
		}

		public void Apply(Rect finalBounds, double finalRotation)
		{
			Owner.Bounds = finalBounds;
			Owner.Rotation = finalRotation;
			Container.Apply(finalBounds, finalRotation);
		}

		private SelectionPartitionTransformer CreatePartition(TransformBehavior behavior)
		{
			if (behavior.IsTranslation())
			{
				return new SelectionTranslateTransformer();
			}
			if (behavior.IsRotation())
			{
				return new SelectionRotateTransformer();
			}
			if (behavior.IsScaling())
			{
				return new SelectionScaleTransformer();
			}
			return new SelectionPartitionTransformer();
		}
	}
}
