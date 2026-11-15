using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;
using PPTService.Ink.Core.Cli.Core;
using PPTService.Ink.Core.Cli.Utils;

namespace PPTService.Ink.Core.Cli.Standard
{
	
	
	[Filter]
	internal class GnuFilter : CommandLineOptionParser<GnuFilter>, ICommandLineFilter
	{
		
		private class LocalizableStrings
		{
			private Dictionary<string, string> _defaultDictionary = new Dictionary<string, string> { { "", "" } };

			internal Dictionary<string, string> _versionOptionDescription = new Dictionary<string, string>
			{
				{ "", "Display version." },
				{ "zh-CN", "显示版本。" }
			};

			internal Dictionary<string, string> _helpOptionDescription = new Dictionary<string, string>
			{
				{ "", "Display help." },
				{ "zh-CN", "显示使用说明。" }
			};

			internal Dictionary<string, string> _usageHeader = new Dictionary<string, string>
			{
				{ "", "Usage: " },
				{ "zh-CN", "用法：" }
			};

			internal Dictionary<string, string> _optionsHeader = new Dictionary<string, string>
			{
				{ "", "Options: " },
				{ "zh-CN", "选项：" }
			};

			internal Dictionary<string, string> _commandHeader = new Dictionary<string, string>
			{
				{ "", "Commands: " },
				{ "zh-CN", "命令：" }
			};

			internal Dictionary<string, string> _unknownCommandFormat = new Dictionary<string, string>
			{
				{ "", "{0}: '{2}' is not a {0} command. See '{1} --help'." },
				{ "zh-CN", "{0}: 无法执行“{2}”命令，请参阅“{1} --help”。" }
			};

			internal string VersionOptionDescription => GetString(Thread.CurrentThread.CurrentUICulture, "VersionOptionDescription");

			internal string HelpOptionDescription => GetString(Thread.CurrentThread.CurrentUICulture, "HelpOptionDescription");

			internal string UsageHeader => GetString(Thread.CurrentThread.CurrentUICulture, "UsageHeader");

			internal string OptionsHeader => GetString(Thread.CurrentThread.CurrentUICulture, "OptionsHeader");

			internal string CommandHeader => GetString(Thread.CurrentThread.CurrentUICulture, "CommandHeader");

			internal string UnknownCommandFormat => GetString(Thread.CurrentThread.CurrentUICulture, "UnknownCommandFormat");

			private string GetString(CultureInfo culture, [CallerMemberName] string propertyName = null)
			{
				return GetString(propertyName, culture) ?? "";
			}

			
			internal string GetString(string name, CultureInfo currentUICulture)
			{
				Dictionary<string, string> dictionary;
				switch (name)
				{
				case "VersionOptionDescription":
					dictionary = _versionOptionDescription;
					break;
				case "HelpOptionDescription":
					dictionary = _helpOptionDescription;
					break;
				case "UsageHeader":
					dictionary = _usageHeader;
					break;
				case "OptionsHeader":
					dictionary = _optionsHeader;
					break;
				case "CommandHeader":
					dictionary = _commandHeader;
					break;
				case "UnknownCommandFormat":
					dictionary = _unknownCommandFormat;
					break;
				default:
					dictionary = _defaultDictionary;
					break;
				}
				Dictionary<string, string> dictionary2 = dictionary;
				string value;
				if (!dictionary2.TryGetValue(currentUICulture.Name, out value))
				{
					return dictionary2[""];
				}
				return value;
			}
		}

		
		
		private struct NameDescription
		{
			public string Name { get; }

			public string Description { get; }

			public NameDescription(string name, string description)
			{
				this = default(NameDescription);
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				Name = name;
				if (description == null)
				{
					throw new ArgumentNullException("description");
				}
				Description = description;
			}

			internal static NameDescription? FromVerbType(Type verbType,  ResourceManager _resourceManager)
			{
				if (verbType.IsDefined(typeof(VerbAttribute)))
				{
					VerbAttribute customAttribute = verbType.GetCustomAttribute<VerbAttribute>();
					string verbName = customAttribute.VerbName;
					string localizedDescription = GetLocalizedDescription(customAttribute, _resourceManager);
					return new NameDescription(verbName, localizedDescription);
				}
				return null;
			}

			internal static NameDescription? FromPropertyInfo(PropertyInfo propertyInfo, LocalizableStrings _localizableStrings,  ResourceManager _resourceManager)
			{
				Assembly assembly = typeof(GnuFilter).Assembly;
				if (propertyInfo.IsDefined(typeof(OptionAttribute)))
				{
					Type declaringType = propertyInfo.DeclaringType;
					Type propertyType = propertyInfo.PropertyType;
					OptionAttribute customAttribute = propertyInfo.GetCustomAttribute<OptionAttribute>();
					char? shortName = customAttribute.ShortName;
					string oldName = customAttribute.LongName ?? propertyInfo.Name;
					string typeName = GetTypeName(propertyType, customAttribute.TypeName);
					string name = ((!shortName.HasValue) ? "" : $"{shortName}|") + "--" + NamingHelper.MakeKebabCase(oldName) + ((typeName == null) ? "" : (" <" + typeName + ">"));
					string description = ((declaringType.Assembly == assembly) ? GetLocalizedDescription(customAttribute, _localizableStrings) : GetLocalizedDescription(customAttribute, _resourceManager));
					return new NameDescription(name, description);
				}
				return null;
			}

			internal static IEnumerable<NameDescription> EnumerateFromVerbType(Type type, LocalizableStrings _localizableStrings,  ResourceManager _resourceManager)
			{
				return (from x in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
					select FromPropertyInfo(x, _localizableStrings, _resourceManager)).OfType<NameDescription>();
			}

			
			private static string GetTypeName( Type type, string typeName)
			{
				if (typeName != null && !string.IsNullOrWhiteSpace(typeName))
				{
					return typeName;
				}
				return CommandLineAssigningExtensions.GetAssignableTypeName(type);
			}
		}

		
		private readonly ResourceManager _resourceManager;

		
		[NotNull]
		private LocalizableStrings _localizableStrings;

		[Option("Version", LocalizableDescription = "VersionOptionDescription")]
		public bool Version { get; private set; }

		[Option('h', "Help", LocalizableDescription = "HelpOptionDescription")]
		public bool Help { get; private set; }

		
		internal GnuFilter(CommandLine commandLine)
		{
			_resourceManager = commandLine?.ResourceManager;
			AddMatch("Version", (bool v) =>
			{
				Version = v;
			});
			AddMatch('h', "Help", (bool v) =>
			{
				Help = v;
			});
			SetResult(() => this);
		}

		public void Filter(ICommandLineFilterContext context)
		{
			Run(context, false);
		}

		public void PostFilter(ICommandLineFilterContext context)
		{
			string verb = context.Verb;
			if (string.IsNullOrWhiteSpace(verb))
			{
				Run(context, true);
				return;
			}
			context.SuppressFurtherHandlers();
			PrintUnknownVerbHelpText(verb);
		}

		private void Run(ICommandLineFilterContext contextInterface, bool helpEmptyVerb)
		{
			CommandLineFilterContext commandLineFilterContext = (CommandLineFilterContext)contextInterface;
			List<Type> relatedTypes = commandLineFilterContext.EnumerateRelatedTypes().ToList();
			if (_localizableStrings == null)
			{
				_localizableStrings = new LocalizableStrings();
			}
			if (Help)
			{
				commandLineFilterContext.SuppressFurtherHandlers();
				string verb = commandLineFilterContext.Verb;
				if (verb != null && !string.IsNullOrWhiteSpace(verb))
				{
					PrintVerbHelpText(commandLineFilterContext.GetVerbType(), verb);
				}
				else
				{
					PrintDetailHelpText(relatedTypes);
				}
			}
			else if (Version)
			{
				commandLineFilterContext.SuppressFurtherHandlers();
				PrintVersionText();
			}
			else if (helpEmptyVerb)
			{
				commandLineFilterContext.SuppressFurtherHandlers();
				PrintHelpText(relatedTypes);
			}
		}

		private void PrintHelpText(IReadOnlyList<Type> relatedTypes)
		{
			Assembly assembly = typeof(GnuFilter).Assembly;
			List<NameDescription> list = relatedTypes.Select( (Type x) => NameDescription.FromVerbType(x, _resourceManager)).OfType<NameDescription>().ToList();
			List<NameDescription> list2 = relatedTypes.Where( (Type x) => !x.IsDefined(typeof(VerbAttribute))).SelectMany( (Type x) => NameDescription.EnumerateFromVerbType(x, _localizableStrings, _resourceManager)).ToList();
			int val = ((list.Count != 0) ? list.Max((NameDescription x) => x.Name.Length) : 0);
			int val2 = Math.Max((list2.Count != 0) ? list2.Max((NameDescription x) => x.Name.Length) : 0, val);
			val2 = Math.Max(12, val2);
			Console.Write(_localizableStrings.UsageHeader);
			if (list.Count > 0)
			{
				Console.WriteLine("[options] [command] [command-options] [arguments]");
			}
			else
			{
				Console.WriteLine("[options] [arguments]");
			}
			Console.WriteLine();
			if (list2.Count > 0)
			{
				Console.WriteLine(_localizableStrings.OptionsHeader);
			}
			foreach (NameDescription item in list2)
			{
				Console.Write(GetColumnString(item.Name, val2));
				Console.WriteLine(item.Description);
			}
			Console.WriteLine();
			if (list.Count > 0)
			{
				Console.WriteLine(_localizableStrings.CommandHeader);
			}
			foreach (NameDescription item2 in list)
			{
				Console.Write(GetColumnString(item2.Name, val2));
				Console.WriteLine(item2.Description);
			}
		}

		private void PrintVerbHelpText(Type verbType,  string commandLineVerb)
		{
			VerbAttribute customAttribute = verbType.GetCustomAttribute<VerbAttribute>();
			if (customAttribute != null)
			{
				string text = NamingHelper.MakeKebabCase(customAttribute.VerbName);
				Console.Write(_localizableStrings.UsageHeader);
				Console.WriteLine(text + " [options]");
				Console.WriteLine();
				List<NameDescription> list = (from x in verbType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
					select NameDescription.FromPropertyInfo(x, _localizableStrings, _resourceManager)).OfType<NameDescription>().ToList();
				int columnLength = ((list.Count != 0) ? list.Max((NameDescription x) => x.Name.Length) : 0);
				if (list.Count > 0)
				{
					Console.WriteLine(_localizableStrings.OptionsHeader);
				}
				{
					foreach (NameDescription item in list)
					{
						Console.Write(GetColumnString(item.Name, columnLength));
						Console.WriteLine(item.Description);
					}
					return;
				}
			}
			if (!string.IsNullOrWhiteSpace(commandLineVerb))
			{
				PrintUnknownVerbHelpText(commandLineVerb);
			}
		}

		
		private void PrintUnknownVerbHelpText(string verb)
		{
			string item;
			string item2;
			GetCommandName().Deconstruct(out item, out item2);
			string text = item;
			string text2 = item2;
			Console.WriteLine(string.Format(CultureInfo.CurrentCulture, _localizableStrings.UnknownCommandFormat, new object[3] { text, text2, verb }));
		}

		private void PrintDetailHelpText(IReadOnlyList<Type> relatedTypes)
		{
			PrintHelpText(relatedTypes);
		}

		private static void PrintVersionText()
		{
			string value = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
			if (string.IsNullOrWhiteSpace(value))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("此程序没有指定版本号，建议开发者在开发时通过项目属性指定版本号，或者设置程序集的 AssemblyInformationalVersionAttribute 特性。");
				Console.ResetColor();
			}
			else
			{
				Console.WriteLine(value);
			}
		}

		
		private static ValueTupleSlim<string, string> GetCommandName()
		{
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			if ((object)entryAssembly == null)
			{
				string processName = Process.GetCurrentProcess().ProcessName;
				return new ValueTupleSlim<string, string>(processName, processName);
			}
			string processName2 = Process.GetCurrentProcess().ProcessName;
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(entryAssembly.Location);
			string fileName = Path.GetFileName(entryAssembly.Location);
			if (string.Equals(processName2, fileNameWithoutExtension, StringComparison.OrdinalIgnoreCase))
			{
				return new ValueTupleSlim<string, string>(processName2, processName2);
			}
			return new ValueTupleSlim<string, string>(fileNameWithoutExtension, processName2 + " " + fileName);
		}

		private static string GetColumnString(string originalString, int columnLength)
		{
			return "  " + originalString.PadRight(columnLength, ' ') + "  ";
		}

		private static string GetLocalizedDescription(CommandLineAttribute attribute,  ResourceManager resourceManager)
		{
			string text;
			if (resourceManager == null || string.IsNullOrWhiteSpace(attribute.LocalizableDescription))
			{
				text = attribute.Description;
				if (text == null)
				{
					return "";
				}
			}
			else
			{
				text = resourceManager.GetString(attribute.LocalizableDescription, CultureInfo.CurrentUICulture) ?? "";
			}
			return text;
		}

		private static string GetLocalizedDescription(CommandLineAttribute attribute, LocalizableStrings resourceManager)
		{
			string text;
			if (attribute.LocalizableDescription == null || string.IsNullOrWhiteSpace(attribute.LocalizableDescription))
			{
				text = attribute.Description;
				if (text == null)
				{
					return "";
				}
			}
			else
			{
				text = resourceManager.GetString(attribute.LocalizableDescription, CultureInfo.CurrentUICulture) ?? "";
			}
			return text;
		}
	}
}
