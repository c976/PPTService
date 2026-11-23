using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Cli.Compatibility;
using PPTServiceUltra.Ink.Core.Cli.Core;

namespace PPTServiceUltra.Ink.Core.Cli.StateMachine
{
	
	
	internal class CommandLineStateMachine
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass7_0
		{
			
			public ListGroup<SingleOptimizedStrings> parsedArgs;

			internal void _003CRun_003Eg__OnOptionCollected_007C0(string option,  SingleOptimizedStrings values)
			{
				if (values != null && values.Count > 0)
				{
					parsedArgs.Add(option ?? "", values);
				}
				else if (!string.IsNullOrEmpty(option))
				{
					parsedArgs.Add(option, values);
				}
			}
		}

		private readonly IReadOnlyList<string> _commandLineArgs;

		private readonly char _optionPrefix;

		
		private Action<string, SingleOptimizedStrings> _optionCollectedAction;

		private bool _isOptionSectionEnded;

		
		private string _currentOption;

		
		private SingleOptimizedStrings _currentValues;

		public CommandLineStateMachine(IReadOnlyList<string> args, char optionPrefix)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			_commandLineArgs = args;
			if (optionPrefix == '-' || optionPrefix == '/')
			{
				_optionPrefix = optionPrefix;
				return;
			}
			throw new NotSupportedException("仅支持 - 或者 / 作为选项的前缀。");
		}

		public ListGroup<SingleOptimizedStrings> Run()
		{
			_003C_003Ec__DisplayClass7_0 obj = new _003C_003Ec__DisplayClass7_0();
			obj.parsedArgs = new ListGroup<SingleOptimizedStrings>();
			_currentOption = null;
			_currentValues = null;
			_optionCollectedAction = obj._003CRun_003Eg__OnOptionCollected_007C0;
			foreach (string commandLineArg in _commandLineArgs)
			{
				if (_isOptionSectionEnded)
				{
					AppendValue(commandLineArg);
				}
				else if (string.IsNullOrEmpty(commandLineArg))
				{
					AppendValue(commandLineArg);
				}
				else if (commandLineArg[0] == _optionPrefix)
				{
					if (_optionPrefix == '-' && commandLineArg.Length == 2 && commandLineArg[1] == _optionPrefix)
					{
						_isOptionSectionEnded = true;
						Commit();
						SetOption(null);
					}
					else
					{
						Commit();
						SetOption(commandLineArg);
					}
				}
				else
				{
					AppendValue(commandLineArg);
				}
			}
			Commit();
			_currentOption = null;
			_currentValues = null;
			_optionCollectedAction = null;
			_isOptionSectionEnded = false;
			return obj.parsedArgs;
		}

		
		private void SetOption(string option)
		{
			if (option == null)
			{
				_currentOption = null;
				_currentValues = null;
				return;
			}
			int num = option.IndexOf(':', StringComparison.OrdinalIgnoreCase);
			if (num < 0 || num >= option.Length - 1)
			{
				_currentOption = option;
				_currentValues = null;
			}
			else
			{
				_currentOption = option.Substring(0, num);
				_currentValues = new SingleOptimizedStrings(option.Substring(num + 1, option.Length - num - 1));
			}
		}

		private void AppendValue(string value)
		{
			if (_currentValues == null)
			{
				_currentValues = new SingleOptimizedStrings(value);
			}
			else
			{
				_currentValues.Add(value);
			}
		}

		private void Commit()
		{
			_optionCollectedAction(_currentOption, _currentValues);
		}
	}
}
