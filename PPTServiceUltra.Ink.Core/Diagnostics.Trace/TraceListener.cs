using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Diagnostics.Trace
{
	
	
	public static class TraceListener
	{
		private static readonly Dictionary<Type, ITracerReporter> _tracerReporters = new Dictionary<Type, ITracerReporter>();

		private static ITracerListener _instance = ConsoleTracer.Instance;

		public static ITracerListener Instance
		{
			get
			{
				return _instance;
			}
			set
			{
				(_instance as INotifyTracer)?.Changing(value);
				_instance = value;
			}
		}

		public static ITracerReporter GetTracerReporters< T>() where T : ITracerReporter, new()
		{
			if (_tracerReporters.ContainsKey(typeof(T)))
			{
				return _tracerReporters[typeof(T)];
			}
			_tracerReporters[typeof(T)] = new T();
			return _tracerReporters[typeof(T)];
		}
	}
}
