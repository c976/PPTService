using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Cvte.ComponentModel;

namespace Cvte.Paint.Framework
{
	public class PaintInitializationManager
	{
		private static readonly Lazy<PaintInitializationManager> PaintInitializationManagerLazy = new Lazy<PaintInitializationManager>(() => new PaintInitializationManager());

		private bool _isInitialized;

		private const string InitializerCollectorName = "Cvte.Paint.__PaintInitializerCollector__";

		private readonly Lazy<List<IPaintInitializer>> _initialziersLazy;

		internal static PaintInitializationManager Instance => PaintInitializationManagerLazy.Value;

		private List<IPaintBoardInitializer> PaintBoardInitializerList { get; } = new List<IPaintBoardInitializer>();

		private List<IPaintInitializer> Initializers => _initialziersLazy.Value;

		private PaintInitializationManager()
		{
			_initialziersLazy = new Lazy<List<IPaintInitializer>>(() => CollectInitializers().ToList());
		}

		public static void RegisterPaintBoardInitializer(IPaintBoardInitializer boardInitializer)
		{
			if (DebuggingProperties.IsDebug && Instance._isInitialized)
			{
				throw new InvalidOperationException("在注入 Board 初始化之前，已有 Board 创建完毕，请检查是否有时机问题。这个机制是给全量的 Board 提供初始化，不提供只给某些 Board 作初始化，因此请确定注入时机足够早。此异常仅调试下抛出，请找德熙");
			}
			Instance.PaintBoardInitializerList.Add(boardInitializer);
		}

		public static void RegisterPaintBoardInitializer(params IPaintBoardInitializer[] boardInitializerList)
		{
			if (DebuggingProperties.IsDebug && Instance._isInitialized)
			{
				throw new InvalidOperationException("在注入 Board 初始化之前，已有 Board 创建完毕，请检查是否有时机问题。这个机制是给全量的 Board 提供初始化，不提供只给某些 Board 作初始化，因此请确定注入时机足够早。此异常仅调试下抛出，请找德熙");
			}
			Instance.PaintBoardInitializerList.AddRange(boardInitializerList);
		}

		public static void RegisterPaintBoardInitializer<T>() where T : IPaintBoardInitializer, new()
		{
			RegisterPaintBoardInitializer(new T());
		}

		internal void Initialzie()
		{
			if (_isInitialized)
			{
				return;
			}
			try
			{
				foreach (IPaintInitializer initializer in Initializers)
				{
					initializer.Initialize();
				}
			}
			finally
			{
				_isInitialized = true;
			}
		}

		internal void Initialzie(Board board)
		{
			try
			{
				foreach (IPaintInitializer initializer in Initializers)
				{
					initializer.Initialize(board);
				}
			}
			finally
			{
				_isInitialized = true;
			}
			PaintBoardInitializerList.Capacity = PaintBoardInitializerList.Count;
			foreach (IPaintBoardInitializer paintBoardInitializer in PaintBoardInitializerList)
			{
				paintBoardInitializer.Initialize(board);
			}
		}

		internal IEnumerable<IPaintInitializer> CollectInitializers()
		{
			IPaintInitializerCollector paintInitializerCollector = FindInitializerCollector();
			if (paintInitializerCollector == null)
			{
				return Enumerable.Empty<IPaintInitializer>();
			}
			return CollectInitializers(paintInitializerCollector);
		}

		private IEnumerable<IPaintInitializer> CollectInitializers(IPaintInitializerCollector collector)
		{
			return from x in collector.Collect(null)
				select x();
		}

		private static IPaintInitializerCollector FindInitializerCollector()
		{
			foreach (Assembly item in EnumerateStackAssemblies())
			{
				Type type = item.GetType("Cvte.Paint.__PaintInitializerCollector__", false);
				if (type != null)
				{
					return (IPaintInitializerCollector)Activator.CreateInstance(type);
				}
			}
			return null;
		}

		private static IEnumerable<Assembly> EnumerateStackAssemblies()
		{
			HashSet<Assembly> assemblySet = new HashSet<Assembly>();
			StackTrace stackTrace = new StackTrace(1);
			for (int i = 0; i < stackTrace.FrameCount; i++)
			{
				Assembly assembly = stackTrace.GetFrame(i).GetMethod().DeclaringType?.Assembly;
				if (assembly != null && !assemblySet.Contains(assembly))
				{
					assemblySet.Add(assembly);
					yield return assembly;
				}
			}
		}
	}
}
