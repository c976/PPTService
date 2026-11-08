namespace Cvte.Documents
{
	public abstract class ModifiedOperation<SA_T0, SA_T1> : IOperation
	{
		private readonly SA_T0 _target;

		private readonly SA_T1 _oldValue;

		private readonly SA_T1 _newValue;

		protected ModifiedOperation(SA_T0 target, SA_T1 oldValue, SA_T1 newValue)
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

		protected abstract void Apply(SA_T0 target, SA_T1 value);
	}
}
