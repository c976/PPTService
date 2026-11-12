using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Cvte.Paint.Framework
{
	public class SelectionPartitionTransformer
	{
		private Lazy<IAlignmentEngine> _alignerLazy;

		protected Slide Slide { get; private set; }

		protected SelectionTransformer Owner { get; private set; }

		protected TransformBehavior Behavior { get; private set; }

		protected IAlignmentEngine Aligner => _alignerLazy?.Value;

		protected Element[] TransformingElements { get; private set; }

		protected internal Rect DesiredBounds { get; protected set; }

		protected internal double DesiredRotation { get; protected set; }

		protected static bool WithShift => Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);

		protected static bool WithCtrl => Keyboard.Modifiers.HasFlag(ModifierKeys.Control);

		protected static bool WithAlt => Keyboard.Modifiers.HasFlag(ModifierKeys.Alt);

		internal void StartTransform(SelectionTransformer owner, TransformRoutedEventArgs args)
		{
			Owner = owner;
			Slide = args.Slide;
			TransformingElements = args.TransformingElements;
			Behavior = args.CurrentBehavior;
			if (owner.Elements.All((Element e) => e.IsGroupInnerElement()))
			{
				_alignerLazy = new Lazy<IAlignmentEngine>(() => new EmptyAlignmentEngine(), LazyThreadSafetyMode.None);
			}
			else
			{
				_alignerLazy = new Lazy<IAlignmentEngine>(() => AlignmentDependencyProperty.GetAlignmentEngine((DependencyObject)owner.Owner) ?? new EmptyAlignmentEngine(), LazyThreadSafetyMode.None);
			}
			OnStarted();
		}

		internal void CompleteTransform(bool isCanceled)
		{
			OnCompleted();
			Slide = null;
			TransformingElements = null;
			Owner = null;
			_alignerLazy = null;
		}

		internal void PerformTransform(Rect targetBounds, object[] args)
		{
		}

		internal void PerformTransform(Vector userOffset, object[] args)
		{
			OnDelta(userOffset, args);
		}

		internal void PerformTransform(double userRotated, object[] args)
		{
			OnDelta(userRotated, args);
		}

		protected virtual void OnStarted()
		{
		}

		protected virtual void OnDelta(Vector userOffset, object[] args)
		{
		}

		protected virtual void OnDelta(double userRotated, object[] args)
		{
		}

		protected virtual void OnCompleted()
		{
		}
	}
}
