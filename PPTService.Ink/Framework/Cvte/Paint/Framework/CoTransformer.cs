using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;
using Cvte.Documents;
using Cvte.Linq;
using Cvte.Paint.Framework.Elements;
using Cvte.Paint.Framework.Operations;
using Cvte.Paint.Framework.SelectionBoxs;
using Cvte.Windows;
using Cvte.Windows.Mathematics;

namespace Cvte.Paint.Framework
{
	internal class CoTransformer
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass25_0
		{
			public SelectionTransformer primarySelection;

			public bool isGroupInnerTransform;

			public GroupElementBase groupElement;

			public ManipulateStartRoutedEventArgs args;

			public CoTransformer _003C_003E4__this;

			public Action<ITransformSensitive> _003C_003E9__18;

			public Action<ITransformSensitive> _003C_003E9__19;

			public Action<ITransformAdapter> _003C_003E9__20;

			internal bool _003CStartTransform_003Eb__0(ISelectionOperator x)
			{
				return !object.Equals(x, primarySelection.Owner);
			}

			internal void _003CStartTransform_003Eg__NotifyTransformStartToGroupElement_007C9()
			{
				if (isGroupInnerTransform)
				{
					(groupElement as ITransformSensitive)?.TransformStarted(args);
				}
			}

			internal void _003CStartTransform_003Eb__10()
			{
				_003C_003E4__this._primaryElements.OfType<ITransformSensitive>().ForEachWithPaintException((ITransformSensitive t) =>
				{
					t.TransformStarted(args);
				});
				_003CStartTransform_003Eg__NotifyTransformStartToGroupElement_007C9();
			}

			internal void _003CStartTransform_003Eb__18(ITransformSensitive t)
			{
				t.TransformStarted(args);
			}

			internal void _003CStartTransform_003Eb__11()
			{
				_003C_003E4__this._primaryTarget.StartTransform(args);
			}

			internal void _003CStartTransform_003Eb__12()
			{
				args.CurrentBehavior = TransformBehavior.Secondary;
				_003C_003E4__this._secondarylElements.OfType<ITransformSensitive>().ForEachWithPaintException((ITransformSensitive t) =>
				{
					t.TransformStarted(args);
				});
				_003CStartTransform_003Eg__NotifyTransformStartToGroupElement_007C9();
			}

			internal void _003CStartTransform_003Eb__19(ITransformSensitive t)
			{
				t.TransformStarted(args);
			}

			internal void _003CStartTransform_003Eb__13()
			{
				_003C_003E4__this._secondaryTargets.ForEachWithPaintException((ITransformAdapter x) =>
				{
					x.StartTransform(args);
				});
			}

			internal void _003CStartTransform_003Eb__20(ITransformAdapter x)
			{
				x.StartTransform(args);
			}

			internal void _003CStartTransform_003Eb__14()
			{
				args.CurrentBehavior = args.PrimaryBehavior;
				_003C_003E4__this._slide.RaiseEvent(args);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass27_0
		{
			public bool isGroupInnerTransform;

			public GroupElementBase groupElement;

			public ManipulateEndRoutedEventArgs args;

			public CoTransformer _003C_003E4__this;

			public bool isCanceled;

			public Action<ITransformSensitive> _003C_003E9__8;

			public Action<ITransformSensitive> _003C_003E9__9;

			public Action<ITransformAdapter> _003C_003E9__10;

			internal void _003CCompleteTransform_003Eg__NotifyTransformCompletedToGroupElement_007C0()
			{
				if (isGroupInnerTransform)
				{
					(groupElement as ITransformSensitive)?.TransformCompleted(args);
				}
			}

			internal void _003CCompleteTransform_003Eb__1()
			{
				_003C_003E4__this._primaryElements.OfType<ITransformSensitive>().ForEachWithPaintException((ITransformSensitive t) =>
				{
					t.TransformCompleted(args);
				});
				_003CCompleteTransform_003Eg__NotifyTransformCompletedToGroupElement_007C0();
			}

			internal void _003CCompleteTransform_003Eb__8(ITransformSensitive t)
			{
				t.TransformCompleted(args);
			}

			internal void _003CCompleteTransform_003Eb__2()
			{
				args.CurrentBehavior = TransformBehavior.Secondary;
			}

			internal void _003CCompleteTransform_003Eb__3()
			{
				_003C_003E4__this._secondarylElements.OfType<ITransformSensitive>().ForEachWithPaintException((ITransformSensitive t) =>
				{
					t.TransformCompleted(args);
				});
				_003CCompleteTransform_003Eg__NotifyTransformCompletedToGroupElement_007C0();
			}

			internal void _003CCompleteTransform_003Eb__9(ITransformSensitive t)
			{
				t.TransformCompleted(args);
			}

			internal void _003CCompleteTransform_003Eb__4()
			{
				if (!isCanceled && args.IsCanceled)
				{
					_003C_003E4__this.RestoreTransform();
				}
			}

			internal void _003CCompleteTransform_003Eb__5()
			{
				args.CurrentBehavior = args.PrimaryBehavior;
				_003C_003E4__this._primaryTarget.CompleteTransform(args);
			}

			internal void _003CCompleteTransform_003Eb__6()
			{
				args.CurrentBehavior = TransformBehavior.Secondary;
				_003C_003E4__this._secondaryTargets.ForEachWithPaintException((ITransformAdapter x) =>
				{
					x.CompleteTransform(args);
				});
			}

			internal void _003CCompleteTransform_003Eb__10(ITransformAdapter x)
			{
				x.CompleteTransform(args);
			}

			internal void _003CCompleteTransform_003Eb__7()
			{
				args.CurrentBehavior = args.PrimaryBehavior;
				_003C_003E4__this._slide.RaiseEvent(args);
			}
		}

		private readonly Slide _slide;

		private static readonly object[] EmptyArgs = new object[0];

		private TransformBehavior _behavior;

		private ITransformAdapter _primaryTarget;

		private Rect _primaryOriginBounds;

		private double _primaryOriginAngle;

		private ITransformAdapter[] _secondaryTargets;

		private Rect[] _secondaryOriginBounds;

		private double[] _secondaryOriginAngles;

		private Element[] _allElements;

		private Element[] _primaryElements;

		private Element[] _secondarylElements;

		private Dictionary<Element, Rect> _originBoundsDictionary;

		private TransformElementsOperation.Builder _operationBuilder;

		private IUndoRedoTransaction _transaction;

		private TransformProxy _transformProxy;

		private TransformProxyBehavior _proxyBehavior;

		private bool HasAnyChanged
		{
			get
			{
				bool flag = !Nearly.Equals(_primaryTarget.Bounds, _primaryOriginBounds) || !Nearly.Equals(_primaryTarget.Rotation, _primaryOriginAngle);
				if (!flag)
				{
					int num = 0;
					ITransformAdapter[] secondaryTargets = _secondaryTargets;
					foreach (ITransformAdapter transformAdapter in secondaryTargets)
					{
						flag = !Nearly.Equals(transformAdapter.Bounds, _secondaryOriginBounds[num]) || !Nearly.Equals(transformAdapter.Rotation, _secondaryOriginAngles[num]);
						if (flag)
						{
							break;
						}
					}
				}
				return flag;
			}
		}

		internal TransformProxyBehavior ProxyBehavior
		{
			get
			{
				return _proxyBehavior;
			}
			set
			{
				if (_proxyBehavior != value)
				{
					_proxyBehavior = value;
					switch (value)
					{
					case TransformProxyBehavior.None:
						_transformProxy?.Hide();
						_transformProxy = null;
						break;
					case TransformProxyBehavior.ProxyOnly:
						throw new NotImplementedException();
					case TransformProxyBehavior.ProxyWithBody:
						_transformProxy = new TransformProxy(_slide, _originBoundsDictionary);
						RestoreTransform();
						_transformProxy.Show();
						break;
					}
				}
			}
		}

		internal bool IsTransforming => _primaryTarget != null;

		internal CoTransformer(Slide slide)
		{
			_slide = slide;
		}

		internal void StartTransform([NotNull] ITransformAdapter primaryTarget, [CanBeNull] IEnumerable<ITransformAdapter> secondaryTargets, TransformBehavior behavior, TransformStepKind stepKind, bool isInOuterTransaction)
		{
			_003C_003Ec__DisplayClass25_0 CS_0024_003C_003E8__locals30 = new _003C_003Ec__DisplayClass25_0();
			CS_0024_003C_003E8__locals30._003C_003E4__this = this;
			_slide.SuspendKeyEvents();
			_behavior = behavior;
			_primaryTarget = primaryTarget;
			_primaryOriginAngle = _primaryTarget.Rotation;
			CS_0024_003C_003E8__locals30.isGroupInnerTransform = AreAllGroupInnerElements(primaryTarget.Elements);
			CS_0024_003C_003E8__locals30.groupElement = null;
			if (CS_0024_003C_003E8__locals30.isGroupInnerTransform)
			{
				CS_0024_003C_003E8__locals30.groupElement = _primaryTarget.Elements.First().VisualAncestor<GroupElementBase>();
			}
			CS_0024_003C_003E8__locals30.primarySelection = primaryTarget as SelectionTransformer;
			if (CS_0024_003C_003E8__locals30.primarySelection != null)
			{
				if (CS_0024_003C_003E8__locals30.isGroupInnerTransform)
				{
					Element element = primaryTarget.Elements.First();
					if (element.IsGroupInnerSelected())
					{
						IGroupInnerSelection groupInnerSelection = CS_0024_003C_003E8__locals30.groupElement as IGroupInnerSelection;
						if (groupInnerSelection != null)
						{
							secondaryTargets = from x in CS_0024_003C_003E8__locals30.groupElement.Elements.Where((Element e) => e.IsGroupInnerSelected()).Except(new Element[1] { element })
								select new SelectionTransformer(groupInnerSelection.GetSelectionOperator(x));
						}
					}
				}
				else
				{
					secondaryTargets = (secondaryTargets ?? Enumerable.Empty<ITransformAdapter>()).Union(from x in _slide.GetSelectionOperators()
						where !object.Equals(x, CS_0024_003C_003E8__locals30.primarySelection.Owner)
						where x.SelectedElements.All((Element e) => !e.GetIsLocked() && !e.IsGroupInnerSelected())
						select new SelectionTransformer(x));
				}
			}
			_secondaryTargets = (secondaryTargets as ITransformAdapter[]) ?? secondaryTargets?.ToArray() ?? new ITransformAdapter[0];
			_secondaryOriginBounds = _secondaryTargets.Select((ITransformAdapter x) => x.Bounds).ToArray();
			_secondaryOriginAngles = _secondaryTargets.Select((ITransformAdapter x) => x.Rotation).ToArray();
			_originBoundsDictionary = new ITransformAdapter[1] { _primaryTarget }.Union(_secondaryTargets).SelectMany((ITransformAdapter x) => x.Elements).ToDictionary((Element x) => x, (Element x) => x.Bounds);
			_primaryElements = _primaryTarget.Elements.ToArray();
			_secondarylElements = _secondaryTargets.SelectMany((ITransformAdapter x) => x.Elements).ToArray();
			_allElements = _primaryElements.Union(_secondarylElements).ToArray();
			if (!isInOuterTransaction)
			{
				_transaction = Board.GetUndoRedoProvider(_slide).BeginTransaction();
			}
			_operationBuilder = new TransformElementsOperation.Builder(_slide);
			if (CS_0024_003C_003E8__locals30.isGroupInnerTransform)
			{
				_operationBuilder.RecordSource(new Element[1] { CS_0024_003C_003E8__locals30.groupElement });
			}
			else
			{
				_operationBuilder.RecordSource(_allElements);
			}
			CS_0024_003C_003E8__locals30.args = new ManipulateStartRoutedEventArgs(_slide, _primaryElements)
			{
				PrimaryBehavior = behavior,
				CurrentBehavior = behavior,
				TransformingElements = _allElements,
				IsAbsoluteMatrix = true
			};
			SelectionTransformer selectionTransformer = _primaryTarget as SelectionTransformer;
			if (selectionTransformer != null)
			{
				selectionTransformer.Owner.NotifyTransformStarting(CS_0024_003C_003E8__locals30.args);
				_primaryOriginBounds = selectionTransformer.Owner.Bounds;
			}
			else
			{
				_primaryOriginBounds = _primaryTarget.Bounds;
			}
			PaintAggregateExceptionBuilder.DoActionsWithException(() =>
			{
				CS_0024_003C_003E8__locals30._003C_003E4__this._primaryElements.OfType<ITransformSensitive>().ForEachWithPaintException((ITransformSensitive t) =>
				{
					t.TransformStarted(CS_0024_003C_003E8__locals30.args);
				});
				CS_0024_003C_003E8__locals30._003CStartTransform_003Eg__NotifyTransformStartToGroupElement_007C9();
			}, () =>
			{
				CS_0024_003C_003E8__locals30._003C_003E4__this._primaryTarget.StartTransform(CS_0024_003C_003E8__locals30.args);
			}, () =>
			{
				CS_0024_003C_003E8__locals30.args.CurrentBehavior = TransformBehavior.Secondary;
				CS_0024_003C_003E8__locals30._003C_003E4__this._secondarylElements.OfType<ITransformSensitive>().ForEachWithPaintException((ITransformSensitive t) =>
				{
					t.TransformStarted(CS_0024_003C_003E8__locals30.args);
				});
				CS_0024_003C_003E8__locals30._003CStartTransform_003Eg__NotifyTransformStartToGroupElement_007C9();
			}, () =>
			{
				CS_0024_003C_003E8__locals30._003C_003E4__this._secondaryTargets.ForEachWithPaintException((ITransformAdapter x) =>
				{
					x.StartTransform(CS_0024_003C_003E8__locals30.args);
				});
			}, () =>
			{
				CS_0024_003C_003E8__locals30.args.CurrentBehavior = CS_0024_003C_003E8__locals30.args.PrimaryBehavior;
				CS_0024_003C_003E8__locals30._003C_003E4__this._slide.RaiseEvent(CS_0024_003C_003E8__locals30.args);
			});
		}

		private bool AreAllGroupInnerElements([NotNull] IList<Element> elements)
		{
			if (!elements.Any())
			{
				return false;
			}
			bool flag = false;
			bool flag2 = false;
			foreach (Element element in elements)
			{
				if (element.IsGroupInnerElement())
				{
					flag = true;
				}
				else
				{
					flag2 = true;
				}
				if (flag2 & flag)
				{
					throw new NotSupportedException("不支持同时对组合内部元素和组合外的元素同时变换");
				}
			}
			return flag;
		}

		internal void CompleteTransform(bool isCanceled, CancelReason cancelResReason)
		{
			_003C_003Ec__DisplayClass27_0 CS_0024_003C_003E8__locals32 = new _003C_003Ec__DisplayClass27_0();
			CS_0024_003C_003E8__locals32._003C_003E4__this = this;
			CS_0024_003C_003E8__locals32.isCanceled = isCanceled;
			_slide.ResumeKeyEvents();
			if (_primaryTarget == null)
			{
				throw new InvalidOperationException("在 CompleteTransform 之前，必须先 StartTransform。");
			}
			if (CS_0024_003C_003E8__locals32.isCanceled)
			{
				RestoreTransform();
			}
			CS_0024_003C_003E8__locals32.args = new ManipulateEndRoutedEventArgs(_slide, _primaryElements)
			{
				PrimaryBehavior = _behavior,
				CurrentBehavior = _behavior,
				IsCanceled = CS_0024_003C_003E8__locals32.isCanceled,
				CancelReason = cancelResReason,
				TransformingElements = _allElements,
				IsAbsoluteMatrix = true
			};
			CS_0024_003C_003E8__locals32.isGroupInnerTransform = AreAllGroupInnerElements(_primaryElements);
			CS_0024_003C_003E8__locals32.groupElement = null;
			if (CS_0024_003C_003E8__locals32.isGroupInnerTransform)
			{
				CS_0024_003C_003E8__locals32.groupElement = _primaryElements.First().VisualAncestor<GroupElementBase>();
			}
			try
			{
				PaintAggregateExceptionBuilder.DoActionsWithException(() =>
				{
					CS_0024_003C_003E8__locals32._003C_003E4__this._primaryElements.OfType<ITransformSensitive>().ForEachWithPaintException((ITransformSensitive t) =>
					{
						t.TransformCompleted(CS_0024_003C_003E8__locals32.args);
					});
					CS_0024_003C_003E8__locals32._003CCompleteTransform_003Eg__NotifyTransformCompletedToGroupElement_007C0();
				}, () =>
				{
					CS_0024_003C_003E8__locals32.args.CurrentBehavior = TransformBehavior.Secondary;
				}, () =>
				{
					CS_0024_003C_003E8__locals32._003C_003E4__this._secondarylElements.OfType<ITransformSensitive>().ForEachWithPaintException((ITransformSensitive t) =>
					{
						t.TransformCompleted(CS_0024_003C_003E8__locals32.args);
					});
					CS_0024_003C_003E8__locals32._003CCompleteTransform_003Eg__NotifyTransformCompletedToGroupElement_007C0();
				}, () =>
				{
					if (!CS_0024_003C_003E8__locals32.isCanceled && CS_0024_003C_003E8__locals32.args.IsCanceled)
					{
						CS_0024_003C_003E8__locals32._003C_003E4__this.RestoreTransform();
					}
				}, () =>
				{
					CS_0024_003C_003E8__locals32.args.CurrentBehavior = CS_0024_003C_003E8__locals32.args.PrimaryBehavior;
					CS_0024_003C_003E8__locals32._003C_003E4__this._primaryTarget.CompleteTransform(CS_0024_003C_003E8__locals32.args);
				}, () =>
				{
					CS_0024_003C_003E8__locals32.args.CurrentBehavior = TransformBehavior.Secondary;
					CS_0024_003C_003E8__locals32._003C_003E4__this._secondaryTargets.ForEachWithPaintException((ITransformAdapter x) =>
					{
						x.CompleteTransform(CS_0024_003C_003E8__locals32.args);
					});
				}, () =>
				{
					CS_0024_003C_003E8__locals32.args.CurrentBehavior = CS_0024_003C_003E8__locals32.args.PrimaryBehavior;
					CS_0024_003C_003E8__locals32._003C_003E4__this._slide.RaiseEvent(CS_0024_003C_003E8__locals32.args);
				});
			}
			finally
			{
				if (CS_0024_003C_003E8__locals32.isCanceled)
				{
					_operationBuilder.CancelRecording();
					_transaction?.Rollback();
				}
				else if (HasAnyChanged)
				{
					_operationBuilder.IgnoreRecordingSource(CS_0024_003C_003E8__locals32.args.ElementsThatOperationHasSuppressed);
					if (_operationBuilder.AnyElementsChanged)
					{
						IOperation operation = _operationBuilder.RecordTarget();
						Board.GetUndoRedoProvider(_slide).Insert(operation);
					}
				}
				_transaction?.Commit();
				_operationBuilder = null;
				_transaction?.Dispose();
				_transaction = null;
				_primaryTarget = null;
				_primaryElements = null;
				_secondaryTargets = null;
				_secondarylElements = null;
				_allElements = null;
				_originBoundsDictionary = null;
			}
		}

		public void PerformTransform(Rect targetBounds, object[] args)
		{
			if (_primaryElements != null)
			{
				_primaryTarget.PerformTransform(targetBounds, args);
				_primaryTarget.Apply(_primaryTarget.DesiredBounds, _primaryTarget.DesiredRotation);
				PerformSecondaryTransform();
			}
		}

		internal void PerformTransform(Vector movedOffset, object[] args)
		{
			if (_primaryElements != null && _primaryTarget.Elements.Count > 0)
			{
				GroupElementBase groupElementBase = _primaryTarget.Elements.First().VisualAncestor<GroupElementBase>();
				if (groupElementBase != null)
				{
					movedOffset *= new RotateTransform(0.0 - groupElementBase.Rotation).Value;
				}
				_primaryTarget.PerformTransform(movedOffset, args);
				Vector offset = _primaryTarget.DesiredBounds.Location - _primaryOriginBounds.Location;
				if (ProxyBehavior == TransformProxyBehavior.None)
				{
					_primaryTarget.Apply(_primaryTarget.DesiredBounds.ToRemote(), _primaryTarget.DesiredRotation.ToRemote());
					PerformSecondaryTransform();
					ManipulatingRoutedEventArgs e = new ManipulatingRoutedEventArgs(_slide, _primaryElements)
					{
						PrimaryBehavior = _behavior,
						CurrentBehavior = _behavior,
						DesiredBounds = _primaryTarget.DesiredBounds.ToRemote(),
						DesiredRotation = _primaryTarget.DesiredRotation.ToRemote(),
						IsAbsoluteMatrix = true,
						TransformMatrix = GetTransformMatrix(movedOffset)
					};
					e.CurrentBehavior = e.PrimaryBehavior;
					_slide.RaiseEvent(e);
				}
				else
				{
					_transformProxy.Offset = offset;
				}
			}
		}

		private Matrix GetTransformMatrix(Vector movedOffset)
		{
			return new Matrix
			{
				OffsetX = movedOffset.X,
				OffsetY = movedOffset.Y
			};
		}

		internal void PerformTransform(double rotatedAngle, object[] args)
		{
			_primaryTarget.PerformTransform(rotatedAngle, args);
			_primaryTarget.Apply(_primaryTarget.DesiredBounds.ToRemote(), _primaryTarget.DesiredRotation.ToRemote());
			PerformSecondaryTransform();
		}

		private void RestoreTransform()
		{
			_primaryTarget.Apply(_primaryOriginBounds, _primaryOriginAngle);
			PerformSecondaryTransform(_primaryOriginBounds, _primaryOriginBounds, _primaryOriginAngle, _primaryOriginAngle);
		}

		private void PerformSecondaryTransform()
		{
			PerformSecondaryTransform(_primaryOriginBounds, _primaryTarget.DesiredBounds.ToRemote(), _primaryOriginAngle, _primaryTarget.DesiredRotation.ToRemote());
		}

		private void PerformSecondaryTransform(Rect oldBounds, Rect newBounds, double oldAngle, double newAngle)
		{
			if (_behavior == TransformBehavior.Rotate)
			{
				PerformSecondaryRotation(oldAngle, newAngle, PerformSecondaryTransformForRotation);
			}
			else if (_behavior == TransformBehavior.Translate)
			{
				PerformSecondaryTranslation(oldBounds, newBounds, PerformSecondaryTransformForBounds);
			}
			else
			{
				PerformSecondaryScaling(oldBounds, newBounds, PerformSecondaryTransformForBounds);
			}
		}

		private void PerformSecondaryTransformForBounds(ITransformAdapter target, Rect oldBounds, Rect newBounds)
		{
			target.PerformTransform(newBounds, EmptyArgs);
			target.Apply(newBounds, target.DesiredRotation);
		}

		private void PerformSecondaryTransformForRotation(ITransformAdapter target, double oldAngle, double newAngle)
		{
			target.PerformTransform(newAngle, EmptyArgs);
			target.Apply(target.DesiredBounds, newAngle);
		}

		private void PerformSecondaryTranslation(Rect oldBounds, Rect newBounds, Action<ITransformAdapter, Rect, Rect> perform)
		{
			Vector offset = newBounds.TopLeft - oldBounds.TopLeft;
			_secondaryTargets.ForEach((ITransformAdapter target, int i) =>
			{
				if (FilterTransform(target))
				{
					Rect rect = _secondaryOriginBounds[i];
					Rect arg = rect;
					arg.Offset(offset);
					perform(target, rect, arg);
				}
			});
		}

		private void PerformSecondaryScaling(Rect oldBounds, Rect newBounds, Action<ITransformAdapter, Rect, Rect> perform)
		{
			Rect boundsRatio = new Rect((newBounds.X - oldBounds.X) / oldBounds.Width, (newBounds.Y - oldBounds.Y) / oldBounds.Height, newBounds.Width / oldBounds.Width, newBounds.Height / oldBounds.Height);
			_secondaryTargets.ForEach((ITransformAdapter target, int i) =>
			{
				if (FilterTransform(target))
				{
					Rect arg = _secondaryOriginBounds[i];
					Rect arg2 = new Rect(arg.X + arg.Width * boundsRatio.X, arg.Y + arg.Height * boundsRatio.Y, arg.Width * boundsRatio.Width, arg.Height * boundsRatio.Height);
					perform(target, arg, arg2);
				}
			});
		}

		private void PerformSecondaryRotation(double oldAngle, double newAngle, Action<ITransformAdapter, double, double> perform)
		{
			double angleDelta = newAngle - oldAngle;
			_secondaryTargets.ForEach((ITransformAdapter target, int i) =>
			{
				if (FilterTransform(target))
				{
					double num = _secondaryOriginAngles[i];
					double arg = num + angleDelta;
					perform(target, num, arg);
				}
			});
		}

		private bool FilterTransform(ITransformAdapter target)
		{
			return target.Elements.All((Element e) =>
			{
				PermittedTransforms permittedTransform = e.GetPermittedTransform();
				return permittedTransform.HasFlag(_behavior.ToPermittedTransforms());
			});
		}
	}
}
