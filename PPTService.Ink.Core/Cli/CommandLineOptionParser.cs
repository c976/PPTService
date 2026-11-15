using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Cli.Core;

namespace PPTService.Ink.Core.Cli
{
	
	
	public abstract class CommandLineOptionParser< T> : ICommandLineOptionParser<T>, IRawCommandLineOptionParser<T>
	{
		
		private string _verb;

		
		private Func<T> _commit;

		private readonly Dictionary<int, Action<string>> _indexString = new Dictionary<int, Action<string>>();

		private readonly Dictionary<char, Action<bool>> _shortNameBool = new Dictionary<char, Action<bool>>();

		private readonly Dictionary<char, Action<string>> _shortNameString = new Dictionary<char, Action<string>>();

		private readonly Dictionary<char, Action<IReadOnlyList<string>>> _shortNameStrings = new Dictionary<char, Action<IReadOnlyList<string>>>();

		private readonly Dictionary<string, Action<bool>> _longNameBool = new Dictionary<string, Action<bool>>();

		private readonly Dictionary<string, Action<string>> _longNameString = new Dictionary<string, Action<string>>();

		private readonly Dictionary<string, Action<IReadOnlyList<string>>> _longNameStrings = new Dictionary<string, Action<IReadOnlyList<string>>>();

		
		protected string Verb
		{
			
			get
			{
				return _verb;
			}
			
			set
			{
				_verb = (string.IsNullOrWhiteSpace(value) ? null : value);
			}
		}

		
		string ICommandLineOptionParser<T>.Verb
		{
			
			get
			{
				return Verb;
			}
		}

		protected void AddMatch(int index, Action<string> action)
		{
			Dictionary<int, Action<string>> indexString = _indexString;
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			indexString[index] = action;
		}

		protected void AddMatch(char shortName, Action<bool> action)
		{
			Dictionary<char, Action<bool>> shortNameBool = _shortNameBool;
			char key = shortName;
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			shortNameBool[key] = action;
		}

		protected void AddMatch(char shortName, Action<string> action)
		{
			Dictionary<char, Action<string>> shortNameString = _shortNameString;
			char key = shortName;
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			shortNameString[key] = action;
		}

		protected void AddMatch(char shortName, Action<IReadOnlyList<string>> action)
		{
			Dictionary<char, Action<IReadOnlyList<string>>> shortNameStrings = _shortNameStrings;
			char key = shortName;
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			shortNameStrings[key] = action;
		}

		protected void AddMatch(string longName, Action<bool> action)
		{
			Dictionary<string, Action<bool>> longNameBool = _longNameBool;
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			longNameBool[longName] = action;
		}

		protected void AddMatch(string longName, Action<string> action)
		{
			Dictionary<string, Action<string>> longNameString = _longNameString;
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			longNameString[longName] = action;
		}

		protected void AddMatch(string longName, Action<IReadOnlyList<string>> action)
		{
			Dictionary<string, Action<IReadOnlyList<string>>> longNameStrings = _longNameStrings;
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			longNameStrings[longName] = action;
		}

		protected void AddMatch(char shortName, string longName, Action<bool> action)
		{
			Dictionary<char, Action<bool>> shortNameBool = _shortNameBool;
			char key = shortName;
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			shortNameBool[key] = action;
			_longNameBool[longName] = action;
		}

		protected void AddMatch(char shortName, string longName, Action<string> action)
		{
			Dictionary<char, Action<string>> shortNameString = _shortNameString;
			char key = shortName;
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			shortNameString[key] = action;
			_longNameString[longName] = action;
		}

		protected void AddMatch(char shortName, string longName, Action<IReadOnlyList<string>> action)
		{
			Dictionary<char, Action<IReadOnlyList<string>>> shortNameStrings = _shortNameStrings;
			char key = shortName;
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			shortNameStrings[key] = action;
			_longNameStrings[longName] = action;
		}

		protected void SetResult(Func<T> commit)
		{
			if (commit == null)
			{
				throw new ArgumentNullException("commit");
			}
			_commit = commit;
		}

		void ICommandLineOptionParser<T>.SetValue(IReadOnlyList<string> values)
		{
			for (int i = 0; i < values.Count; i++)
			{
				Action<string> value;
				if (_indexString.TryGetValue(i, out value))
				{
					value(values[i]);
				}
			}
		}

		void ICommandLineOptionParser<T>.SetValue(char shortName, bool value)
		{
			Action<bool> value2;
			if (_shortNameBool.TryGetValue(shortName, out value2))
			{
				value2(value);
			}
		}

		void ICommandLineOptionParser<T>.SetValue(char shortName, string value)
		{
			Action<string> value2;
			if (_shortNameString.TryGetValue(shortName, out value2))
			{
				value2(value);
			}
		}

		void ICommandLineOptionParser<T>.SetValue(char shortName, IReadOnlyList<string> values)
		{
			Action<IReadOnlyList<string>> value;
			if (_shortNameStrings.TryGetValue(shortName, out value))
			{
				value(values);
			}
		}

		void ICommandLineOptionParser<T>.SetValue(string longName, bool value)
		{
			Action<bool> value2;
			if (_longNameBool.TryGetValue(longName, out value2))
			{
				value2(value);
			}
		}

		void ICommandLineOptionParser<T>.SetValue(string longName, string value)
		{
			Action<string> value2;
			if (_longNameString.TryGetValue(longName, out value2))
			{
				value2(value);
			}
		}

		void ICommandLineOptionParser<T>.SetValue(string longName, IReadOnlyList<string> values)
		{
			Action<IReadOnlyList<string>> value;
			if (_longNameStrings.TryGetValue(longName, out value))
			{
				value(values);
			}
		}

		
		void IRawCommandLineOptionParser<T>.SetValue(char shortName, SingleOptimizedStrings values)
		{
			Action<bool> value;
			Action<string> value2;
			Action<IReadOnlyList<string>> value3;
			if (_shortNameBool.TryGetValue(shortName, out value))
			{
				bool result;
				value(values == null || (bool.TryParse(values[0], out result) & result));
			}
			else if (_shortNameString.TryGetValue(shortName, out value2) && values != null)
			{
				value2(values[0]);
			}
			else if (_shortNameStrings.TryGetValue(shortName, out value3) && values != null)
			{
				value3(values);
			}
		}

		void IRawCommandLineOptionParser<T>.SetValue(string longName,  SingleOptimizedStrings values)
		{
			Action<bool> value;
			Action<string> value2;
			Action<IReadOnlyList<string>> value3;
			if (_longNameBool.TryGetValue(longName, out value))
			{
				bool result;
				value(values == null || (bool.TryParse(values[0], out result) & result));
			}
			else if (_longNameString.TryGetValue(longName, out value2) && values != null)
			{
				value2((values.Count == 1) ? values[0] : string.Join(" ", values));
			}
			else if (_longNameStrings.TryGetValue(longName, out value3) && values != null)
			{
				value3(values);
			}
		}

		T ICommandLineOptionParser<T>.Commit()
		{
			if (_commit == null)
			{
				throw new InvalidOperationException("在命令行参数解析完毕后依然没有无法提交解析的参数实例，是否忘记在 " + GetType().FullName + " 中调用 SetResult 方法指定如何提交参数解析的实例？");
			}
			return _commit();
		}
	}
}
