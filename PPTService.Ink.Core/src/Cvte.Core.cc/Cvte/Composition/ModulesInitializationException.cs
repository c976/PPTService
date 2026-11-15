using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Composition
{
	
	
	public class ModulesInitializationException : AggregateException
	{
		
		public class Builder
		{
			private readonly Dictionary<IModule, Exception> _exceptions = new Dictionary<IModule, Exception>();

			public void Append(IModule module, Exception exception)
			{
				if (module == null)
				{
					throw new ArgumentNullException("module");
				}
				Dictionary<IModule, Exception> exceptions = _exceptions;
				if (exception == null)
				{
					throw new ArgumentNullException("exception");
				}
				exceptions.Add(module, exception);
			}

			[DebuggerStepThrough]
			public void ThrowsIfExceptionsAny(string message)
			{
				if (_exceptions.Any())
				{
					throw new ModulesInitializationException(message, _exceptions);
				}
			}
		}

		public Dictionary<IModule, Exception> ModuleExceptions { get; }

		public ModulesInitializationException(string message, Dictionary<IModule, Exception> moduleExceptions)
			: base(message, moduleExceptions?.Values)
		{
			if (moduleExceptions == null)
			{
				throw new ArgumentNullException("moduleExceptions");
			}
			ModuleExceptions = moduleExceptions;
		}
	}
}
