using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cvte.Composition
{
	public class MefExportedTypeDetector : ITypeDetector
	{
		private class ContainerTypeCollection : ITypeCollection
		{
			private readonly CompositionContainer _container;

			public ContainerTypeCollection(CompositionContainer container)
			{
				_container = container;
			}

			public IEnumerable<T> OfType<T>(Predicate<Type> typeFilter)
			{
				return from x in _container.GetExports<T>()
					select x.Value into x
					where typeFilter(x.GetType())
					select x;
			}
		}

		[CompilerGenerated]
		private EventHandler<NewTypesDetectedEventArgs> m_NewTypesDetected;

		private readonly HashSet<Assembly> _processedAssemblies;

		private readonly AggregateCatalog _aggregateCatalog;

		public CompositionContainer CompositionContainer { get; set; }

		public event EventHandler<NewTypesDetectedEventArgs> NewTypesDetected
		{
			[CompilerGenerated]
			add
			{
				EventHandler<NewTypesDetectedEventArgs> eventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						eventHandler = this.m_NewTypesDetected;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				EventHandler<NewTypesDetectedEventArgs> eventHandler2 = default(EventHandler<NewTypesDetectedEventArgs>);
				EventHandler<NewTypesDetectedEventArgs> value2 = (EventHandler<NewTypesDetectedEventArgs>)Delegate.Combine(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_NewTypesDetected, value2, eventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				EventHandler<NewTypesDetectedEventArgs> eventHandler3 = eventHandler;
				if (6u != 0)
				{
					eventHandler2 = eventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)eventHandler != eventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<NewTypesDetectedEventArgs> eventHandler;
				if (0 == 0)
				{
					if (0 == 0)
					{
						eventHandler = this.m_NewTypesDetected;
					}
					goto IL_000f;
				}
				goto IL_0038;
				IL_0038:
				if (false)
				{
					goto IL_0048;
				}
				return;
				IL_0048:
				EventHandler<NewTypesDetectedEventArgs> eventHandler2 = default(EventHandler<NewTypesDetectedEventArgs>);
				EventHandler<NewTypesDetectedEventArgs> value2 = (EventHandler<NewTypesDetectedEventArgs>)Delegate.Remove(eventHandler2, value);
				eventHandler = Interlocked.CompareExchange(ref this.m_NewTypesDetected, value2, eventHandler2);
				goto IL_0034;
				IL_000f:
				if (7 == 0)
				{
					goto IL_0034;
				}
				EventHandler<NewTypesDetectedEventArgs> eventHandler3 = eventHandler;
				if (6u != 0)
				{
					eventHandler2 = eventHandler3;
				}
				goto IL_0048;
				IL_0034:
				if ((object)eventHandler != eventHandler2)
				{
					goto IL_000f;
				}
				goto IL_0038;
			}
		}

		public MefExportedTypeDetector()
		{
			_processedAssemblies = new HashSet<Assembly>();
			_aggregateCatalog = new AggregateCatalog();
			CompositionContainer = new CompositionContainer(_aggregateCatalog);
		}

		public void Detect(Assembly assembly)
		{
			if (!_processedAssemblies.Contains(assembly) || 7 == 0)
			{
				_processedAssemblies.Add(assembly);
				AssemblyCatalog assemblyCatalog = new AssemblyCatalog(assembly);
				CompositionContainer container = new CompositionContainer(assemblyCatalog);
				do
				{
					_aggregateCatalog.Catalogs.Add(assemblyCatalog);
					OnNewTypesDetected(new NewTypesDetectedEventArgs(new ContainerTypeCollection(container), assembly));
				}
				while (1 == 0);
			}
		}

		public ReadOnlyCollection<T> GetDetectedObjects<T>()
		{
			return new ReadOnlyCollection<T>((from x in CompositionContainer.GetExports<T>()
				select x.Value).ToArray());
		}

		protected virtual void OnNewTypesDetected(NewTypesDetectedEventArgs e)
		{
			this.m_NewTypesDetected?.Invoke(this, e);
		}
	}
}
