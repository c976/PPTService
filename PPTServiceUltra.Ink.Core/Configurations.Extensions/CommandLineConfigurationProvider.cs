using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Configurations.Extensions
{
	
	
	internal class CommandLineConfigurationProvider
	{
		
		private readonly Dictionary<string, string> _switchMappings;

		protected IEnumerable<string> Args { get; private set; }

		public CommandLineConfigurationProvider(IEnumerable<string> args,  IDictionary<string, string> switchMappings = null)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			Args = args;
			if (switchMappings != null)
			{
				_switchMappings = GetValidatedSwitchMappingsCopy(switchMappings);
			}
		}

		public Dictionary<string, string> Load()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			using (IEnumerator<string> enumerator = Args.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string text = enumerator.Current;
					int num = 0;
					if (text.StartsWith("--", StringComparison.OrdinalIgnoreCase))
					{
						num = 2;
					}
					else if (text.StartsWith("-", StringComparison.OrdinalIgnoreCase))
					{
						num = 1;
					}
					else if (text.StartsWith("/", StringComparison.OrdinalIgnoreCase))
					{
						text = "--" + text.Substring(1);
						num = 2;
					}
					int num2 = text.IndexOf('=');
					string key;
					string value2;
					if (num2 < 0)
					{
						if (num == 0)
						{
							continue;
						}
						string value;
						if (_switchMappings != null && _switchMappings.TryGetValue(text, out value))
						{
							key = value;
						}
						else
						{
							if (num == 1)
							{
								continue;
							}
							key = text.Substring(num);
						}
						string current = enumerator.Current;
						if (!enumerator.MoveNext())
						{
							continue;
						}
						value2 = enumerator.Current;
					}
					else
					{
						string key2 = text.Substring(0, num2);
						string value3;
						if (_switchMappings != null && _switchMappings.TryGetValue(key2, out value3))
						{
							key = value3;
						}
						else
						{
							if (num == 1)
							{
								throw new FormatException("Short Switch " + text + " Not Defined");
							}
							key = text.Substring(num, num2 - num);
						}
						value2 = text.Substring(num2 + 1);
					}
					dictionary[key] = value2;
				}
				return dictionary;
			}
		}

		private Dictionary<string, string> GetValidatedSwitchMappingsCopy(IDictionary<string, string> switchMappings)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(switchMappings.Count, StringComparer.OrdinalIgnoreCase);
			foreach (KeyValuePair<string, string> switchMapping in switchMappings)
			{
				if (!switchMapping.Key.StartsWith("-", StringComparison.Ordinal) && !switchMapping.Key.StartsWith("--", StringComparison.Ordinal))
				{
					throw new ArgumentException("The map key should start with `-` or `--`, but the current key is " + switchMapping.Key, "switchMappings");
				}
				if (dictionary.ContainsKey(switchMapping.Key))
				{
					throw new ArgumentException("Duplicated Key In SwitchMappings, the duplicated key is " + switchMapping.Key, "switchMappings");
				}
				dictionary.Add(switchMapping.Key, switchMapping.Value);
			}
			return dictionary;
		}
	}
}
