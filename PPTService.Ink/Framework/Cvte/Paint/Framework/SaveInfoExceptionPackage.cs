using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework
{
	public class SaveInfoExceptionPackage
	{
		private static readonly object LockHelper = new object();

		private static readonly Dictionary<Board, SaveInfoExceptionPackage> SaveInfoExceptions = new Dictionary<Board, SaveInfoExceptionPackage>();

		private static SaveInfoExceptionPackage _currentExceptionPackage;

		private readonly List<Exception> _exceptions = new List<Exception>();

		public static SaveInfoExceptionPackage CurrentExceptionPackage
		{
			get
			{
				return _currentExceptionPackage ?? (_currentExceptionPackage = new SaveInfoExceptionPackage());
			}
			private set
			{
				_currentExceptionPackage = value;
			}
		}

		public IReadOnlyList<Exception> Exceptions => _exceptions;

		public static SaveInfoExceptionPackage GetExceptionCollection([NotNull] Slide slide)
		{
			lock (LockHelper)
			{
				Board board = SaveInfoExceptions.Keys.FirstOrDefault((Board b) => b.Slides.Contains(slide));
				if (board != null)
				{
					CurrentExceptionPackage = SaveInfoExceptions[board];
					return CurrentExceptionPackage;
				}
				Board board2 = slide.Parent as Board;
				if (board2 != null)
				{
					CurrentExceptionPackage = new SaveInfoExceptionPackage(board2);
					return CurrentExceptionPackage;
				}
				CurrentExceptionPackage = new SaveInfoExceptionPackage();
				return CurrentExceptionPackage;
			}
		}

		public static SaveInfoExceptionPackage GetExceptionCollection([NotNull] Board board)
		{
			lock (LockHelper)
			{
				CurrentExceptionPackage = (SaveInfoExceptions.ContainsKey(board) ? SaveInfoExceptions[board] : new SaveInfoExceptionPackage(board));
				return CurrentExceptionPackage;
			}
		}

		public SaveInfoExceptionPackage([NotNull] Board board)
		{
			lock (LockHelper)
			{
				SaveInfoExceptions[board] = this;
			}
		}

		private SaveInfoExceptionPackage()
		{
		}

		public void Add(Exception ex)
		{
			if (ex == null)
			{
				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}
			}
			else
			{
				_exceptions.Add(ex);
			}
		}

		public void AddRange(IList<Exception> ex)
		{
			if (ex == null)
			{
				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}
			}
			else
			{
				_exceptions.AddRange(ex);
			}
		}

		public void Clear()
		{
			_exceptions.Clear();
		}

		public bool HasException()
		{
			return Exceptions.Count > 0;
		}

		public string BuildReport()
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<SaveInfoException> list = Exceptions.OfType<SaveInfoException>().ToList();
			foreach (SaveInfoException item in list)
			{
				stringBuilder.AppendLine("====");
				stringBuilder.AppendLine(item.ToString());
			}
			foreach (Exception item2 in Exceptions.Except(list).ToList())
			{
				stringBuilder.AppendLine("====");
				stringBuilder.AppendLine(item2.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
