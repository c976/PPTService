using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Cvte.Diagnostics.Trace;

namespace Cvte.Paint.Inks
{
	internal static class InkTracer
	{
		public const string InkTag = "Ink";

		private static readonly Lazy<ILogTracer> _logTracer = new Lazy<ILogTracer>(() => Tracer.BuildLogTracer("Ink"));

		public static ILogTracer LogTracer => _logTracer.Value;

		[Conditional("DEBUG_取消注释这句话调试笔迹")]
		public static void Trace(string record = "", [CallerMemberName] string name = "")
		{
			string message = $"{DateTime.Now:hh:mm:ss} [{name}] {record}";
			LogTracer.Debug(message);
		}
	}
}
