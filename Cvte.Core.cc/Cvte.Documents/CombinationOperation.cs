using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Cvte.Linq;

namespace Cvte.Documents
{
	public class CombinationOperation : NotifyOperation
	{
		private readonly IList<IOperation> _operations;

		public IReadOnlyList<IOperation> Operations { get; }

		public CombinationOperation(IList<IOperation> operations)
		{
			_operations = operations;
			Operations = new ReadOnlyCollection<IOperation>(operations);
		}

		public CombinationOperation(params IOperation[] operations)
		{
			_operations = operations;
			Operations = new ReadOnlyCollection<IOperation>(operations);
		}

		public override void Redo()
		{
			_operations.ForEach((IOperation o) =>
			{
				o.Redo();
			});
			base.Redo();
		}

		public override void Undo()
		{
			_operations.Reverse().ForEach((IOperation o) =>
			{
				o.Undo();
			});
			base.Undo();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<---- CombinationOperation ---->");
			foreach (IOperation operation in _operations)
			{
				stringBuilder.AppendLine(operation.ToString());
			}
			stringBuilder.AppendLine("<---------------------------->");
			return stringBuilder.ToString();
		}
	}
}
