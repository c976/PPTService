using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using PPTServiceUltra.Ink.Core.Cli.Compatibility;
using PPTServiceUltra.Ink.Core.Cli.Core;
using PPTServiceUltra.Ink.Core.Cli.StateMachine;
using PPTServiceUltra.Ink.Core.Cli.Utils;

namespace PPTServiceUltra.Ink.Core.Cli
{
	
	
	[DebuggerDisplay("CommandLine: {DebuggerDisplay,nq}")]
	[DebuggerTypeProxy(typeof(CommandLineDebugView))]
	public sealed class CommandLine : ICommandLineHandlerBuilder, IEnumerable<KeyValuePair<string, SingleOptimizedStrings>>, IEnumerable
	{
		
		private class CommandLineDebugView
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			private readonly CommandLine _owner;

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			private string[] Options => _owner._optionArgs.ForOptions().Select( (KeyValuePair<string, SingleOptimizedStrings> pair) =>
			{
				string text = (string.IsNullOrEmpty(pair.Key) ? "" : ("-" + pair.Key + " "));
				IEnumerable<string> value = pair.Value;
				return text + string.Join(" ", value ?? new string[0]);
			}).ToArray();

			public CommandLineDebugView(CommandLine owner)
			{
				_owner = owner;
			}
		}

		private readonly ListGroup<SingleOptimizedStrings> _optionArgs;

		private readonly List<CommandLineFilterMatch> _filterCreatorList = new List<CommandLineFilterMatch>();

		
		private readonly List<CommandLineTypeMatcher<Task<int>>> _toMatchList = new List<CommandLineTypeMatcher<Task<int>>>();

		
		
		internal ResourceManager ResourceManager
		{
			
			get;
		}

		internal IReadOnlyList<CommandLineFilterMatch> FilterMatchList => _filterCreatorList;

		
		internal IReadOnlyList<CommandLineTypeMatcher<Task<int>>> VerbMatchList
		{
			
			get
			{
				return _toMatchList;
			}
		}

		CommandLine ICommandLineHandlerBuilder.CommandLine => this;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string DebuggerDisplay => string.Join(" ", _optionArgs.ForOptions().Select( (KeyValuePair<string, SingleOptimizedStrings> pair) =>
		{
			string text = (string.IsNullOrEmpty(pair.Key) ? "" : ("-" + pair.Key + " "));
			IEnumerable<string> value = pair.Value;
			return text + string.Join(" ", value ?? new string[0]);
		}));

		private CommandLine(ListGroup<SingleOptimizedStrings> optionArgs,  ResourceManager resourceManager)
		{
			if (optionArgs == null)
			{
				throw new ArgumentNullException("optionArgs");
			}
			_optionArgs = optionArgs;
			ResourceManager = resourceManager;
		}

		public T As< T>()
		{
			ICommandLineOptionParser<T> parser = CommandLineHelpers.FindParser<T>();
			return As(parser);
		}

		public T As< T>(ICommandLineOptionParser<T> parser)
		{
			if (parser == null)
			{
				throw new ArgumentNullException("parser");
			}
			foreach (KeyValuePair<string, SingleOptimizedStrings> optionArg in _optionArgs)
			{
				string key = optionArg.Key;
				SingleOptimizedStrings value = optionArg.Value;
				int num = value?.Count ?? 0;
				if (key.Length == 0)
				{
					if (num > 0)
					{
						if (parser.Verb == null)
						{
							parser.SetValue(value);
						}
						else
						{
							parser.SetValue(value.Skip(1).ToList());
						}
					}
					continue;
				}
				if (key.Length == 1)
				{
					char shortName = key[0];
					IRawCommandLineOptionParser<T> rawCommandLineOptionParser = parser as IRawCommandLineOptionParser<T>;
					if (rawCommandLineOptionParser != null)
					{
						rawCommandLineOptionParser.SetValue(shortName, value);
						continue;
					}
					switch (num)
					{
					case 0:
						parser.SetValue(shortName, true);
						break;
					case 1:
					{
						bool result;
						if (bool.TryParse(value[0], out result))
						{
							parser.SetValue(shortName, result);
						}
						else
						{
							parser.SetValue(shortName, value[0]);
						}
						break;
					}
					default:
						parser.SetValue(shortName, value);
						break;
					}
					continue;
				}
				string longName = key;
				IRawCommandLineOptionParser<T> rawCommandLineOptionParser2 = parser as IRawCommandLineOptionParser<T>;
				if (rawCommandLineOptionParser2 != null)
				{
					rawCommandLineOptionParser2.SetValue(longName, value);
					continue;
				}
				switch (num)
				{
				case 0:
					parser.SetValue(longName, true);
					break;
				case 1:
				{
					bool result2;
					if (bool.TryParse(value[0], out result2))
					{
						parser.SetValue(longName, result2);
					}
					else
					{
						parser.SetValue(longName, value[0]);
					}
					break;
				}
				default:
					parser.SetValue(longName, value);
					break;
				}
			}
			return parser.Commit();
		}

		internal void AddMatch(CommandLineFilterMatch filterCreator)
		{
			_filterCreatorList.Add(filterCreator);
		}

		
		internal void AddMatch<TVerb>( CommandLineTypeMatcher<Task<int>> matcher)
		{
			_toMatchList.Add(matcher);
		}

		public Dictionary<string, string> ToDictionary()
		{
			return this.ToDictionary( (KeyValuePair<string, SingleOptimizedStrings> x) => x.Key,  (KeyValuePair<string, SingleOptimizedStrings> x) => (x.Value != null) ? string.Join(" ", x.Value) : "", StringComparer.OrdinalIgnoreCase);
		}

		
		IEnumerator<KeyValuePair<string, SingleOptimizedStrings>> IEnumerable<KeyValuePair<string, SingleOptimizedStrings>>.GetEnumerator()
		{
			return _optionArgs.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _optionArgs.GetEnumerator();
		}

		public static CommandLine Parse(string[] args,  string protocolName = null)
		{
			return Parse(args, protocolName, null);
		}

		public static CommandLine Parse(string[] args, ResourceManager resourceManager)
		{
			return Parse(args, null, resourceManager);
		}

		public static CommandLine Parse(string[] args,  string protocolName,  ResourceManager resourceManager)
		{
			if (args == null || args.Length == 0)
			{
				return new CommandLine(new ListGroup<SingleOptimizedStrings>(), resourceManager);
			}
			if (!string.IsNullOrWhiteSpace(protocolName) && args.Length != 0 && args[0].StartsWith(protocolName + "://", StringComparison.OrdinalIgnoreCase))
			{
				args = ConvertUrlToArgs(args[0]);
			}
			char optionPrefix = AutoFindPrefix(args);
			ListGroup<SingleOptimizedStrings> listGroup = new CommandLineStateMachine(args, optionPrefix).Run();
			NormalizeParsedArgs(listGroup, optionPrefix);
			return new CommandLine(listGroup, resourceManager);
		}

		private static string[] ConvertUrlToArgs(string url)
		{
			if (url != null)
			{
				url = HttpUtility.UrlDecode(url);
				int num = url.IndexOf('?', StringComparison.OrdinalIgnoreCase);
				if (num >= 0 && url != null)
				{
					return (from keyValueString in url.Substring(num + 1).Split(new char[1] { '&' }, StringSplitOptions.RemoveEmptyEntries)
						let keyValue = keyValueString.Split('=')
						select new string[2]
						{
							FormatShellLongName(keyValue[0]),
							keyValue[1]
						}).SelectMany( (string[] x) => x).ToArray();
				}
			}
			return new string[0];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void NormalizeParsedArgs(ListGroup<SingleOptimizedStrings> parsedArgs, char optionPrefix)
		{
			foreach (KeyValuePair<int, string> item in parsedArgs.ForIndexes())
			{
				int key = item.Key;
				string value = item.Value;
				if (string.IsNullOrWhiteSpace(value))
				{
					continue;
				}
				if (value.Length == 2 && value[0] == optionPrefix)
				{
					parsedArgs.ReplaceKey(key, value[1].ToString(CultureInfo.InvariantCulture));
					continue;
				}
				if (value.Length > 2 && value[0] == optionPrefix)
				{
					string newKey = ((value[1] == optionPrefix) ? FormatCoreLongName(value, optionPrefix) : value.Substring(1));
					parsedArgs.ReplaceKey(key, newKey);
					continue;
				}
				throw new NotSupportedException("不支持命令行选项：" + value + "。");
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string FormatShellLongName(string option)
		{
			char[] array = option.ToCharArray();
			if (char.IsUpper(array[0]))
			{
				return "-" + option;
			}
			array[0] = char.ToUpper(array[0], CultureInfo.InvariantCulture);
			return "-" + new string(array);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string FormatCoreLongName(IEnumerable<char> option, char prefix)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (char item in option.Skip(2))
			{
				if (item == prefix)
				{
					flag = true;
				}
				else if (flag)
				{
					flag = false;
					stringBuilder.Append(char.ToUpper(item, CultureInfo.InvariantCulture));
				}
				else
				{
					stringBuilder.Append(item);
				}
			}
			return stringBuilder.ToString();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static char AutoFindPrefix(string[] args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			for (int i = 0; i < args.Length; i++)
			{
				char c = args[i][0];
				if (c == '-' || c == '/')
				{
					return c;
				}
			}
			return '-';
		}
	}
}
