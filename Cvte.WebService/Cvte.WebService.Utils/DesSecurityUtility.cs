using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Cvte.WebService.Utils
{
	internal class DesSecurityUtility
	{
		private static readonly string m__0001;

		public static byte[] _0001(byte[] _0002, string _0003, bool _0004 = true, bool _0005 = true)
		{
			DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider
			{
				Key = SA_Proxy._007E_0005___0004(SA_Proxy._0003___0003(), _0003),
				IV = SA_Proxy._007E_0005___0004(SA_Proxy._0003___0003(), DesSecurityUtility.m__0001),
				Mode = ((!_0005) ? CipherMode.CBC : CipherMode.ECB),
				Padding = ((!_0004) ? PaddingMode.None : PaddingMode.Zeros)
			};
			try
			{
				MemoryStream memoryStream = new MemoryStream();
				try
				{
					CryptoStream cryptoStream = new CryptoStream(memoryStream, SA_Proxy._007E_000F___0008(dESCryptoServiceProvider), CryptoStreamMode.Write);
					try
					{
						SA_Proxy._007E_0011___000E(cryptoStream, _0002, 0, _0002.Length);
						SA_Proxy._007E_0012___000F(cryptoStream);
					}
					finally
					{
						if (cryptoStream != null)
						{
							SA_Proxy._007E_0013___000F(cryptoStream);
						}
					}
					return SA_Proxy._007E_0014___0010(memoryStream);
				}
				finally
				{
					if (memoryStream != null)
					{
						SA_Proxy._007E_0013___000F(memoryStream);
					}
				}
			}
			finally
			{
				if (dESCryptoServiceProvider != null)
				{
					SA_Proxy._007E_0013___000F(dESCryptoServiceProvider);
				}
			}
		}

		public static string _0001(string P_0, string P_1)
		{
			return _0001(_0001(SA_Proxy._007E_0005___0004(SA_Proxy._0003___0003(), P_0), P_1));
		}

		public static string _0001(byte[] P_0)
		{
			if (P_0 == null)
			{
				throw new ArgumentNullException();
			}
			return P_0.Aggregate(string.Empty, (string text, byte b) => SA_Proxy._0004_0002___008F(text, b.ToString("X2")));
		}

		public static byte[] _0002(byte[] _0002, string _0003, bool _0004 = true, bool _0005 = true)
		{
			DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider
			{
				Key = SA_Proxy._007E_0005___0004(SA_Proxy._0003___0003(), _0003),
				IV = SA_Proxy._007E_0005___0004(SA_Proxy._0003___0003(), DesSecurityUtility.m__0001),
				Mode = ((!_0005) ? CipherMode.CBC : CipherMode.ECB),
				Padding = ((!_0004) ? PaddingMode.None : PaddingMode.Zeros)
			};
			try
			{
				MemoryStream memoryStream = new MemoryStream();
				try
				{
					CryptoStream cryptoStream = new CryptoStream(memoryStream, SA_Proxy._007E_0010___0008(dESCryptoServiceProvider), CryptoStreamMode.Write);
					try
					{
						SA_Proxy._007E_0011___000E(cryptoStream, _0002, 0, _0002.Length);
						SA_Proxy._007E_0012___000F(cryptoStream);
					}
					finally
					{
						if (cryptoStream != null)
						{
							SA_Proxy._007E_0013___000F(cryptoStream);
						}
					}
					return SA_Proxy._007E_0014___0010(memoryStream);
				}
				finally
				{
					if (memoryStream != null)
					{
						SA_Proxy._007E_0013___000F(memoryStream);
					}
				}
			}
			finally
			{
				if (dESCryptoServiceProvider != null)
				{
					SA_Proxy._007E_0013___000F(dESCryptoServiceProvider);
				}
			}
		}

		public static string _0002(string P_0, string P_1)
		{
			byte[] array = _0002(_0001(P_0), P_1);
			return SA_Proxy._007E_0017___0012(SA_Proxy._007E_0016___0011(SA_Proxy._0003___0003(), array), "\0", null);
		}

		public unsafe static byte[] _0001(string P_0)
		{
			int num;
			int num2;
			IntPtr intPtr;
			if (P_0 != null)
			{
				if (1 == 0)
				{
					goto IL_0071;
				}
				if (8 != 0)
				{
					P_0 = SA_Proxy._007E_0017___0012(P_0, " ", string.Empty);
					num = SA_Proxy._007E_0018___0013(P_0);
					while (true)
					{
						if (num % 2 != 0)
						{
							throw new FormatException("valueToConvert");
						}
						num2 = SA_Proxy._007E_0018___0013(P_0);
						intPtr = (IntPtr)2;
						if (2 == 0)
						{
							break;
						}
						num = num2 / 2;
						if (false)
						{
							continue;
						}
						goto IL_006b;
					}
					goto IL_009d;
				}
			}
			throw new ArgumentNullException("valueToConvert");
			IL_009d:
			byte[] array = default(byte[]);
			int num3 = default(int);
			if (-1 != 0)
			{
				if (num2 >= (int)(long)intPtr)
				{
					return array;
				}
				if (5 != 0)
				{
					array[num3] = SA_Proxy._001B___0015(SA_Proxy._007E_0019___0014(P_0, num3 * 2, 2), 16);
				}
				num2 = num3;
				intPtr = (IntPtr)1;
			}
			num3 = (int)((long)num2 + (long)intPtr);
			goto IL_009a;
			IL_006b:
			array = new byte[num];
			goto IL_0071;
			IL_0071:
			num3 = 0;
			goto IL_009a;
			IL_009a:
			num2 = num3;
			intPtr = (IntPtr)(void*)array.LongLength;
			goto IL_009d;
		}

		static DesSecurityUtility()
		{
			DesSecurityUtility.m__0001 = "dfgedfgb";
		}
	}
}
