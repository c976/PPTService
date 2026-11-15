using System;
using System.Windows.Input;
using PPTService.Ink.Core.ComponentModel;
using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Ink.Core.Windows.Input
{
	public class DelegateCommand : ICommand
	{
		private readonly Action _execute;

		private readonly Func<bool> _canExecute;

		public event EventHandler CanExecuteChanged
		{
			add
			{
				CommandManager.RequerySuggested += value;
			}
			remove
			{
				CommandManager.RequerySuggested -= value;
			}
		}

		public DelegateCommand([NotNull] Action execute, Func<bool> canExecute)
		{
			if (execute == null)
			{
				throw new ArgumentNullException("execute");
			}
			_execute = execute;
			_canExecute = canExecute;
		}

		public DelegateCommand([NotNull] Action execute)
			: this(execute, null)
		{
		}

		public void Execute(object parameter)
		{
			_execute();
		}

		public bool CanExecute(object parameter)
		{
			if (_canExecute == null)
			{
				return true;
			}
			return _canExecute();
		}
	}
	public class DelegateCommand<T> : ICommand
	{
		private readonly Action<T> _execute;

		private readonly Predicate<T> _canExecute;

		private static bool IsNullableValueType => Nullable.GetUnderlyingType(typeof(T)) != null;

		public event EventHandler CanExecuteChanged
		{
			add
			{
				CommandManager.RequerySuggested += value;
			}
			remove
			{
				CommandManager.RequerySuggested -= value;
			}
		}

		public DelegateCommand([NotNull] Action<T> execute, Predicate<T> canExecute)
		{
			if (execute == null)
			{
				throw new ArgumentNullException("execute");
			}
			_execute = execute;
			_canExecute = canExecute;
		}

		public DelegateCommand([NotNull] Action<T> execute)
			: this(execute, (Predicate<T>)null)
		{
		}

		public void Execute(T parameter)
		{
			_execute(parameter);
		}

		public bool CanExecute(T parameter)
		{
			if (_canExecute == null)
			{
				return true;
			}
			return _canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			Execute(Cast(parameter));
		}

		public bool CanExecute(object parameter)
		{
			if (parameter is T)
			{
				T parameter2 = (T)parameter;
				try
				{
					return CanExecute(parameter2);
				}
				catch
				{
					return true;
				}
			}
			return true;
		}

		private static T Cast(object obj)
		{
			if (obj == null)
			{
				if (IsNullableValueType)
				{
					return default(T);
				}
				if (typeof(T).IsValueType)
				{
					if (DebuggingProperties.IsDebug)
					{
						throw new ArgumentException("传入的参数 parameter 为空，不能转换为泛型的值类型 " + typeof(T).FullName + " 请检查命令参数是否已经传入正确值");
					}
					return default(T);
				}
				return default(T);
			}
			if (obj is T)
			{
				return (T)obj;
			}
			throw new ArgumentException("传入的参数 parameter = " + (obj?.GetType().FullName ?? "null") + " 不能转换为泛型类型 " + typeof(T).FullName + " 请检查命令参数是否已经传入正确值");
		}
	}
}
