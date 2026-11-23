using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Cli.Core;

namespace PPTServiceUltra.Ink.Core.Cli.Parsers
{
	
	
	internal abstract class RuntimeCommandLineOptionParser< T> : ICommandLineOptionParser<T>, IRawCommandLineOptionParser<T>
	{
		
		
		public string Verb
		{
			
			get;
		}

		
		protected RuntimeCommandLineOptionParser(string verb)
		{
			Verb = verb;
		}

		public abstract void SetValue(IReadOnlyList<string> values);

		public abstract void SetValue(char shortName, bool value);

		public abstract void SetValue(char shortName, string value);

		public abstract void SetValue(char shortName, IReadOnlyList<string> values);

		public abstract void SetValue(string longName, bool value);

		public abstract void SetValue(string longName, string value);

		public abstract void SetValue(string longName, IReadOnlyList<string> values);

		
		public abstract void SetValue(char shortName, SingleOptimizedStrings values);

		public abstract void SetValue(string longName,  SingleOptimizedStrings values);

		public abstract T Commit();

		internal static RuntimeCommandLineOptionParser<T> Create()
		{
			string verb = (typeof(T).IsDefined(typeof(VerbAttribute)) ? typeof(T).GetCustomAttribute<VerbAttribute>().VerbName : null);
			List<PropertyInfo> list = (from x in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
				where x.IsDefined(typeof(OptionAttribute)) || x.IsDefined(typeof(ValueAttribute))
				select x).ToList();
			if (list.Count == 0)
			{
				throw new NotSupportedException("类型 " + typeof(T).FullName + " 中没有发现任何可以作为命令行参数的可见属性。");
			}
			bool flag = list.All( (PropertyInfo x) => !x.CanWrite);
			if (list.All( (PropertyInfo x) => x.CanWrite && x.SetMethod.IsPublic) | flag)
			{
				if (!flag)
				{
					return new RuntimeOptionParser<T>(verb, list);
				}
				return new ImmutableRuntimeOptionParser<T>(verb, list);
			}
			throw new NotSupportedException("运行时命令行解析器仅支持两种类型的命令行参数类型：\r\n1. 全部属性都只有 public get 的（public string Property { get; }）；\r\n2. 全部属性都可以 public get set 的（public string Property { get; set; }）");
		}
	}
}
