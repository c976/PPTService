using System.IO;
using System.Runtime.CompilerServices;

namespace Cvte.Net.WebServices.CStoreUpload
{
	
	
	public class Crc32Provider
	{
		public const uint Ieee = 3988292384u;

		private readonly uint[] _table;

		private uint _value;

		public Crc32Provider()
		{
			_value = 0u;
			_table = MakeTable(3988292384u);
		}

		public void Write(byte[] p, int offset, int count)
		{
			_value = Update(_value, _table, p, offset, count);
		}

		public uint Sum()
		{
			return _value;
		}

		private static uint[] MakeTable(uint poly)
		{
			uint[] array = new uint[256];
			for (int i = 0; i < 256; i++)
			{
				uint num = (uint)i;
				for (int j = 0; j < 8; j++)
				{
					num = (((num & 1) != 1) ? (num >> 1) : ((num >> 1) ^ poly));
				}
				array[i] = num;
			}
			return array;
		}

		public static uint Update(uint crc, uint[] table, byte[] p, int offset, int count)
		{
			crc = ~crc;
			for (int i = 0; i < count; i++)
			{
				crc = table[(byte)crc ^ p[offset + i]] ^ (crc >> 8);
			}
			return ~crc;
		}

		public static uint CheckSumBytes(byte[] data)
		{
			Crc32Provider crc32Provider = new Crc32Provider();
			crc32Provider.Write(data, 0, data.Length);
			return crc32Provider.Sum();
		}

		public static uint CheckSumSlice(byte[] data, int offset, int count)
		{
			Crc32Provider crc32Provider = new Crc32Provider();
			crc32Provider.Write(data, offset, count);
			return crc32Provider.Sum();
		}

		public static uint CheckSumFile(string filePath)
		{
			Crc32Provider crc32Provider = new Crc32Provider();
			int num = 32768;
			using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
			{
				byte[] array = new byte[num];
				while (true)
				{
					int num2 = fileStream.Read(array, 0, num);
					if (num2 == 0)
					{
						break;
					}
					crc32Provider.Write(array, 0, num2);
				}
			}
			return crc32Provider.Sum();
		}

		public static uint CheckSumFileWithStream(Stream fileStream)
		{
			Crc32Provider crc32Provider = new Crc32Provider();
			int num = 32768;
			fileStream.Seek(0L, SeekOrigin.Begin);
			byte[] array = new byte[num];
			while (true)
			{
				int num2 = fileStream.Read(array, 0, num);
				if (num2 == 0)
				{
					break;
				}
				crc32Provider.Write(array, 0, num2);
			}
			return crc32Provider.Sum();
		}
	}
}
