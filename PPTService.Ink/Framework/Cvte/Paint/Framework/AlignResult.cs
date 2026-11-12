using System;
using System.Diagnostics;
using System.Windows;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework
{
	[DebuggerDisplay("AlignResult: IsAligned={IsAligned,nq}, Offset={Offset,nq}, H={Horizontal.AlignmentData.Count,nq}, V={Vertical.AlignmentData.Count,nq}")]
	public sealed class AlignResult
	{
		private Func<object, object> _applyMethod;

		public bool IsAligned
		{
			get
			{
				if (!Horizontal.IsAligned)
				{
					return Vertical.IsAligned;
				}
				return true;
			}
		}

		public SingleAlignResult Horizontal { get; }

		public SingleAlignResult Vertical { get; }

		public Vector Offset => new Vector(Horizontal.Offset, Vertical.Offset);

		public AlignResult([NotNull] SingleAlignResult horizontal, [NotNull] SingleAlignResult vertical)
		{
			if (horizontal == null)
			{
				throw new ArgumentNullException("horizontal");
			}
			if (vertical == null)
			{
				throw new ArgumentNullException("vertical");
			}
			Horizontal = horizontal;
			Vertical = vertical;
		}

		public void SetApplyMethod<T>([NotNull] Func<AlignResult, T, T> method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			_applyMethod = (object source) => method(this, (T)source);
		}

		public T ApplyOffset<T>(T source)
		{
			if (_applyMethod == null)
			{
				return source;
			}
			return (T)_applyMethod(source);
		}
	}
}
