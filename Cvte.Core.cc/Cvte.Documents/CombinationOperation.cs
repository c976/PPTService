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
			while (true)
			{
				if (-1 == 0)
				{
					goto IL_0031;
				}
				_operations.Reverse().ForEach((IOperation o) =>
				{
					o.Undo();
				});
				goto IL_004c;
				IL_0031:
				if (0 == 0)
				{
					if (false)
					{
						continue;
					}
					if (5 != 0)
					{
						break;
					}
				}
				goto IL_004c;
				IL_004c:
				base.Undo();
				goto IL_0031;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = default(StringBuilder);
			if (0 == 0 && -1 != 0)
			{
				stringBuilder = new StringBuilder();
			}
			stringBuilder.AppendLine("<---- CombinationOperation ---->");
			IEnumerator<IOperation> enumerator = _operations.GetEnumerator();
			try
			{
				if (0 == 0)
				{
					goto IL_0048;
				}
				goto IL_0052;
				IL_0048:
				if (enumerator.MoveNext())
				{
					goto IL_0052;
				}
				goto end_IL_0031;
				IL_0052:
				IOperation current = enumerator.Current;
				stringBuilder.AppendLine(current.ToString());
				goto IL_0048;
				end_IL_0031:;
			}
			finally
			{
				if (5 == 0 || enumerator != null)
				{
					enumerator.Dispose();
				}
			}
			stringBuilder.AppendLine("<---------------------------->");
			return stringBuilder.ToString();
		}
	}
}
