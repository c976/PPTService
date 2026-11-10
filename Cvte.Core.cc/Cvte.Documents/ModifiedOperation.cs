namespace Cvte.Documents
{
	public abstract class ModifiedOperation<TTarget, TValue> : IOperation
	{
		private readonly TTarget _target;

		private readonly TValue _oldValue;

		private readonly TValue _newValue;

		protected ModifiedOperation(TTarget target, TValue oldValue, TValue newValue)
		{
			_target = target;
			_oldValue = oldValue;
			_newValue = newValue;
		}

		public void Undo()
		{
			Apply(_target, _oldValue);
		}

		public void Redo()
		{
			Apply(_target, _newValue);
		}

		protected abstract void Apply(TTarget target, TValue value);
	}
}
