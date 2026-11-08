using System;
using System.Collections.Generic;
using System.Linq;

namespace Cvte.Documents
{
	internal class UndoRedoTransaction : IDisposable, IUndoRedoTransaction
	{
		private readonly IUndoRedoProvider _provider;

		private readonly IList<IOperation> _operations;

		private readonly Action _disposeAction;

		public UndoRedoTransaction(IUndoRedoProvider provider, Action disposeAction)
		{
			_provider = provider;
			_operations = new List<IOperation>();
			_disposeAction = disposeAction;
		}

		public void Insert(IOperation operation)
		{
			_operations.Add(operation);
		}

		public void Commit()
		{
			if (_operations.Any())
			{
				_provider.Insert(new TransactionOperation(_operations.ToList()));
				Dispose();
			}
		}

		public void Rollback()
		{
			Dispose();
		}

		public void Dispose()
		{
			_disposeAction();
			_operations.Clear();
		}
	}
}
