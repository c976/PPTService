using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PPTServiceUltra.Ink.Core.Cli.Utils
{
	[StructLayout(LayoutKind.Auto)]
	
	
	
	internal struct ValueTupleSlim< T1,  T2>
	{
		public T1 Item1 { get; }

		public T2 Item2 { get; }

		public ValueTupleSlim(T1 item1, T2 item2)
		{
			Item1 = item1;
			Item2 = item2;
		}

		public void Deconstruct(out T1 item1, out T2 item2)
		{
			item1 = Item1;
			item2 = Item2;
		}
	}
}
