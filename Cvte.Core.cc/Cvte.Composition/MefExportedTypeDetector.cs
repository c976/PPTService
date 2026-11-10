using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;

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

		private readonly HashSet<Assembly> _processedAssemblies;

		private readonly AggregateCatalog _aggregateCatalog;

		public CompositionContainer CompositionContainer { get; set; }

		public event EventHandler<NewTypesDetectedEventArgs> NewTypesDetected;

		public MefExportedTypeDetector()
		{
			_processedAssemblies = new HashSet<Assembly>();
			_aggregateCatalog = new AggregateCatalog();
			CompositionContainer = new CompositionContainer(_aggregateCatalog);
		}

		public void Detect(Assembly assembly)
		{
			if (!_processedAssemblies.Contains(assembly))
			{
				_processedAssemblies.Add(assembly);
				AssemblyCatalog assemblyCatalog = new AssemblyCatalog(assembly);
				CompositionContainer container = new CompositionContainer(assemblyCatalog);
				_aggregateCatalog.Catalogs.Add(assemblyCatalog);
				OnNewTypesDetected(new NewTypesDetectedEventArgs(new ContainerTypeCollection(container), assembly));
			}
		}

		public ReadOnlyCollection<T> GetDetectedObjects<T>()
		{
			return new ReadOnlyCollection<T>((from x in CompositionContainer.GetExports<T>()
				select x.Value).ToArray());
		}

		protected virtual void OnNewTypesDetected(NewTypesDetectedEventArgs e)
		{
			NewTypesDetected?.Invoke(this, e);
		}
	}
}
