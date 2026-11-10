// SA_COMPAT_ORIGINAL - original ILSpy output preserved for reference.
// Superseded by SA_Compat.cs; not compiled.
#if SA_ORIGINAL_DECOMPILED
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Cvte.Pipe.Http.Request;
using Cvte.WebService.Utils;

namespace _0005
{
	internal static class _0001<_0001> where _0001 : Attribute
	{
		public static _0001 _0001(PropertyInfo _0002, bool _0003 = false)
		{
			_0001 result = default(_0001);
			if (4 != 0)
			{
				_0001[] source = _0005._0001<_0001>._0001(_0002, _0003);
				if (source.Any())
				{
					if (0 == 0)
					{
						return source.First();
					}
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public static _0001[] _0001(PropertyInfo _0002, bool _0003 = false)
		{
			return (_0001[])global::_0002._007E_0002(_0002, _0001._0001(typeof(_0001).TypeHandle), _0003);
		}
	}
}
namespace _0002
{
	internal class _0001
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "WritePrivateProfileString")]
		private static extern long _0001(string P_0, string P_1, string P_2, string P_3);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetPrivateProfileString")]
		private static extern int _0001(string P_0, string P_1, string P_2, StringBuilder P_3, int P_4, string P_5);

		public static void _0001(string P_0, string P_1, string P_2, string P_3)
		{
			try
			{
				if (0 == 0 && (false || !_0016._001C(P_3)))
				{
					_0017._001F(P_3, "", global::_0003._0004());
				}
			}
			catch (Exception)
			{
			}
			do
			{
				if (2 != 0)
				{
					_0001(P_0, P_1, P_2, P_3);
				}
			}
			while (false);
		}

		public static string _0001(string P_0, string P_1, string P_2)
		{
			while (true)
			{
				int capacity = 2048;
				while (true)
				{
					StringBuilder stringBuilder = new StringBuilder(capacity);
					if (8 == 0)
					{
						break;
					}
					capacity = _0001(P_0, P_1, "", stringBuilder, 2048, P_2);
					if (7 == 0)
					{
						continue;
					}
					return _0018._007E_007F(stringBuilder);
				}
			}
		}
	}
}
namespace _0006
{
	internal class _0001
	{
		public unsafe static string _0001(string P_0)
		{
			do
			{
				if (P_0 == null)
				{
					throw new ArgumentNullException("valueToEncrypt");
				}
			}
			while (8 == 0);
			byte[] array = _0004._007E_0005(global::_0003._0003(), P_0);
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			try
			{
				byte[] array2;
				StringBuilder stringBuilder;
				int num;
				if (0 == 0)
				{
					array2 = _0019._007E_0087(mD5CryptoServiceProvider, array);
					stringBuilder = new StringBuilder();
					num = 0;
				}
				while (true)
				{
					int num2 = num;
					while (true)
					{
						IntPtr intPtr = (IntPtr)(void*)array2.LongLength;
						int num3;
						while (true)
						{
							num3 = (int)(long)intPtr;
							if (5 == 0)
							{
								break;
							}
							intPtr = (IntPtr)num3;
							if (false)
							{
								continue;
							}
							goto IL_008e;
						}
						goto IL_007d;
						IL_0079:
						if (7 == 0)
						{
							continue;
						}
						num3 = 1;
						goto IL_007d;
						IL_008e:
						if (num2 < num3)
						{
							_001A._007E_0088(stringBuilder, array2[num].ToString("x2"));
							num2 = num;
							goto IL_0079;
						}
						return _0018._007E_007F(stringBuilder);
						IL_007d:
						num2 += num3;
						if (0 == 0)
						{
							break;
						}
						goto IL_0079;
					}
					num = num2;
				}
			}
			finally
			{
				if (mD5CryptoServiceProvider != null)
				{
					global::_000F._007E_0013(mD5CryptoServiceProvider);
				}
			}
		}
	}
}
namespace _0008
{
	internal static class _0001
	{
		public static string _0001(string P_0, Encoding P_1)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (true)
			{
				string text;
				if (uint.MaxValue != 0)
				{
					text = P_0;
				}
				while (true)
				{
					IL_0016:
					int num = 0;
					while (true)
					{
						IL_0148:
						int num2 = num;
						while (8 != 0)
						{
							string text2;
							string text3;
							if (num2 < _0013._007E_0018(text))
							{
								text2 = _001B._007E_0089(text, num2).ToString();
								text3 = _001C._008A(text2, P_1);
								if (!_001D._008B(text2, "+"))
								{
									if (_001D._008B(text2, " "))
									{
										goto IL_00b2;
									}
									if (!_001D._008B(text2, "~"))
									{
										if (0 == 0)
										{
											num = (_001D._008B(text2, "'") ? 1 : 0);
											if (false)
											{
												goto IL_0148;
											}
											if (num != 0)
											{
												text3 = "'";
											}
										}
									}
									else
									{
										if (6 == 0)
										{
											goto IL_0016;
										}
										text3 = "~";
									}
								}
								else
								{
									text3 = "%2b";
								}
								goto IL_00d4;
							}
							return _0018._007E_007F(stringBuilder);
							IL_00d4:
							StringBuilder stringBuilder2 = stringBuilder;
							string value;
							if (_001D._008B(text2, text3))
							{
								value = text2;
							}
							else
							{
								value = ((text3 != null) ? _0018._0080(text3) : null);
							}
							stringBuilder2.Append(value);
							goto IL_0102;
							IL_00b2:
							text3 = "%20";
							if (0 == 0)
							{
								goto IL_00d4;
							}
							goto IL_0102;
							IL_0102:
							if (2 != 0)
							{
								num2++;
								continue;
							}
							goto IL_00b2;
						}
						break;
					}
					break;
				}
			}
		}
	}
}
namespace _000F
{
	[Reqeust("/file/pics", ReqeustType.Post)]
	internal class _0001 : UploadFileRequest
	{
		[SpecialName]
		public virtual string _0001()
		{
			return "picture";
		}
	}
}
namespace _0008
{
	[Reqeust("/file/attachments", ReqeustType.Post)]
	internal class _0002 : UploadFileRequest
	{
		[SpecialName]
		public virtual string _0001()
		{
			return "attachment";
		}
	}
}
namespace _000F
{
	internal class _0002 : IniSetting
	{
		public string this[string P_0]
		{
			get
			{
				return base._0001(P_0, string.Empty);
			}
			set
			{
				base._0001(text, text2);
			}
		}

		public _0002(string P_0, string P_1)
			: base(P_0, P_1)
		{
		}

		[SpecialName]
		public string _0001()
		{
			return this["LastUserResourceId"];
		}

		[SpecialName]
		public void _0001(string P_0)
		{
			this["LastUserResourceId"] = P_0;
		}

		[SpecialName]
		public string _0002()
		{
			return this["LastUserId"];
		}

		[SpecialName]
		public void _0002(string P_0)
		{
			this["LastUserId"] = P_0;
		}

		[SpecialName]
		public string _0003()
		{
			return this["LastUserName"];
		}

		[SpecialName]
		public void _0003(string P_0)
		{
			this["LastUserName"] = P_0;
		}

		[SpecialName]
		public string _0004()
		{
			return this["LastToken"];
		}

		[SpecialName]
		public void _0004(string P_0)
		{
			this["LastToken"] = P_0;
		}

		[SpecialName]
		public string _0005()
		{
			return this["Phone"];
		}

		[SpecialName]
		public void _0005(string P_0)
		{
			this["Phone"] = P_0;
		}

		[SpecialName]
		public string _0006()
		{
			return this["Stage"];
		}

		[SpecialName]
		public void _0006(string P_0)
		{
			this["Stage"] = P_0;
		}

		[SpecialName]
		public string _0007()
		{
			return this["Subject"];
		}

		[SpecialName]
		public void _0007(string P_0)
		{
			this["Subject"] = P_0;
		}

		[SpecialName]
		public string _0008()
		{
			return this["StageName"];
		}

		[SpecialName]
		public void _0008(string P_0)
		{
			this["StageName"] = P_0;
		}

		[SpecialName]
		public string _000E()
		{
			return this["SubjectName"];
		}

		[SpecialName]
		public void _000E(string P_0)
		{
			this["SubjectName"] = P_0;
		}
	}
}
namespace _0002
{
	internal class _0002
	{
		private string m__0001;

		private string m__0002;

		private string m__0003;

		private string m__0004;

		private string m__0005;

		private string m__0006;

		private long? m__0001;

		[SpecialName]
		public string _0001()
		{
			return this.m__0001;
		}

		[SpecialName]
		public void _0001(string P_0)
		{
			this.m__0001 = P_0;
		}

		[SpecialName]
		public string _0002()
		{
			return this.m__0002;
		}

		[SpecialName]
		public void _0002(string P_0)
		{
			this.m__0002 = P_0;
		}

		[SpecialName]
		public string _0003()
		{
			return this.m__0003;
		}

		[SpecialName]
		public void _0003(string P_0)
		{
			this.m__0003 = P_0;
		}

		[SpecialName]
		public string _0004()
		{
			return this.m__0004;
		}

		[SpecialName]
		public void _0004(string P_0)
		{
			this.m__0004 = P_0;
		}

		[SpecialName]
		public string _0005()
		{
			return this.m__0005;
		}

		[SpecialName]
		public void _0005(string P_0)
		{
			this.m__0005 = P_0;
		}

		[SpecialName]
		public string _0006()
		{
			return this.m__0006;
		}

		[SpecialName]
		public void _0006(string P_0)
		{
			this.m__0006 = P_0;
		}

		[SpecialName]
		public long? _0001()
		{
			return this.m__0001;
		}

		[SpecialName]
		public void _0001(long? P_0)
		{
			this.m__0001 = P_0;
		}
	}
}
namespace _0001
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method)]
	internal sealed class _0001 : Attribute
	{
	}
}
namespace _0007
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate)]
	internal class _0001 : Attribute
	{
	}
}
namespace _000F
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate)]
	internal class _0003 : Attribute
	{
	}
}
namespace _0006
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
	internal class _0002 : Attribute
	{
	}
}
namespace _0007
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
	internal class _0002 : Attribute
	{
	}
}
namespace _0002
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	internal class _0003 : Attribute
	{
	}
}
namespace _0003
{
	internal sealed class _0001
	{
		[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 116)]
		private struct _0001
		{
		}

		internal static readonly _0001 _0001/* Not supported: data(01 00 02 00 03 00 04 00 05 00 06 00 07 00 08 00 0E 00 0F 00 10 00 11 00 12 00 13 00 14 00 15 00 16 00 17 00 18 00 19 00 1A 00 1B 00 1C 00 1D 00 1E 00 1F 00 7F 00 80 00 81 00 82 00 83 00 84 00 86 00 87 00 88 00 89 00 8A 00 8B 00 8C 00 8D 00 8E 00 8F 00 90 00 91 00 92 00 93 00 94 00 95 00 96 00 97 00 98 00 99 00 9A 00 9B 00 9C 00 9D 00 9E 00 9F 00) */;
	}
	internal class _0002
	{
		private static readonly string m__0001;

		private static readonly string m__0002;

		private static readonly byte[] m__0001;

		private static readonly Dictionary<int, string> m__0001;

		private static readonly object m__0001;

		private static readonly bool m__0001;

		private static readonly int m__0001;

		public static string _0001(int P_0)
		{
			int num = P_0;
			if (8 != 0)
			{
				int num2 = 107396847;
				do
				{
					if (num2 != 0)
					{
						P_0 = num ^ num2;
						num = P_0;
						num2 = global::_0003._0002.m__0001;
					}
				}
				while (false);
				P_0 = num - num2;
				num = (global::_0003._0002.m__0001 ? 1 : 0);
			}
			if (num == 0 && 0 == 0)
			{
				return _0003(P_0);
			}
			return _0002(P_0);
		}

		public static string _0002(int P_0)
		{
			object obj = global::_0003._0002.m__0001;
			_009F._001A_0002(obj);
			string result = default(string);
			try
			{
				string value;
				global::_0003._0002.m__0001.TryGetValue(P_0, out value);
				while (value != null)
				{
					if (false)
					{
						continue;
					}
					if (7 == 0)
					{
						goto IL_0061;
					}
					result = value;
					return result;
				}
			}
			finally
			{
				do
				{
					_009F._001B_0002(obj);
				}
				while (false);
			}
			return _0003(P_0);
			IL_0061:
			return result;
		}

		public static string _0003(int P_0)
		{
			int num = P_0;
			byte[] array = global::_0003._0002.m__0001;
			int num2 = num;
			int num3 = num2 + 1;
			if (0 == 0)
			{
				num = num3;
			}
			int num4 = array[num2];
			int num6;
			while (true)
			{
				int num5 = num4;
				if ((num5 & 0x80) == 0)
				{
					num6 = num5;
					if (num6 != 0)
					{
						break;
					}
					return string.Empty;
				}
				num4 = num5;
				if (-1 == 0)
				{
					continue;
				}
				int num8;
				if (7 != 0)
				{
					int num7 = num4 & 0x40;
					while (num7 != 0)
					{
						num8 = ((num5 & 0x1F) << 24) + (global::_0003._0002.m__0001[num++] << 16);
						while (true)
						{
							num7 = num8 + (global::_0003._0002.m__0001[num++] << 8);
							if (false)
							{
								break;
							}
							num8 = num7 + global::_0003._0002.m__0001[num++];
							if (false)
							{
								continue;
							}
							goto IL_00a2;
						}
					}
					int num9 = num5 & 0x3F;
					int num10 = 8;
					do
					{
						num9 <<= num10;
						num10 = global::_0003._0002.m__0001[num++];
					}
					while (false);
					num4 = num9 + num10;
				}
				num6 = num4;
				break;
				IL_00a2:
				num6 = num8;
				break;
			}
			while (false)
			{
			}
			string result;
			try
			{
				byte[] array2 = _0002_0002._001D_0002(_0001_0002._007E_001C_0002(global::_0003._0003(), global::_0003._0002.m__0001, num, num6));
				string text = _0003_0002._001E_0002(_0001_0002._007E_001C_0002(global::_0003._0003(), array2, 0, array2.Length));
				if (global::_0003._0002.m__0001)
				{
					_0001(P_0, text);
				}
				result = text;
			}
			catch
			{
				do
				{
					result = null;
				}
				while (false);
			}
			return result;
		}

		public static void _0001(int P_0, string P_1)
		{
			try
			{
				object obj;
				do
				{
					obj = global::_0003._0002.m__0001;
				}
				while (false);
				_009F._001A_0002(obj);
				try
				{
					do
					{
						if (4 != 0)
						{
							global::_0003._0002.m__0001.Add(P_0, P_1);
						}
					}
					while (false);
				}
				finally
				{
					if (0 == 0)
					{
						_009F._001B_0002(obj);
					}
				}
			}
			catch
			{
			}
		}

		static _0002()
		{
			global::_0003._0002.m__0001 = "0";
			if (-1 != 0)
			{
				global::_0003._0002.m__0002 = "243";
				global::_0003._0002.m__0001 = null;
				global::_0003._0002.m__0001 = new object();
				global::_0003._0002.m__0001 = false;
				global::_0003._0002.m__0001 = 0;
				if (!_001D._008B(global::_0003._0002.m__0001, "1"))
				{
					goto IL_0059;
				}
			}
			global::_0003._0002.m__0001 = true;
			global::_0003._0002.m__0001 = new Dictionary<int, string>();
			goto IL_0059;
			IL_0059:
			global::_0003._0002.m__0001 = _0086._0099(global::_0003._0002.m__0002);
			if (false)
			{
				return;
			}
			Stream stream = _0005_0002._007E_007F_0002(_0004_0002._001F_0002(), "{5105e566-34cb-46bb-95c8-e98ebc6d33c1}");
			try
			{
				int num = _0007_0002._0081_0002(_0006_0002._007E_0080_0002(stream));
				global::_0003._0002.m__0001 = new byte[num];
				_0008_0002._007E_0082_0002(stream, global::_0003._0002.m__0001, 0, num);
			}
			finally
			{
				if (stream != null)
				{
					global::_000F._007E_0013(stream);
				}
			}
		}
	}
}
namespace _0002
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate)]
	internal sealed class _0004 : Attribute
	{
	}
}
namespace _0007
{
	[AttributeUsage(AttributeTargets.Method)]
	internal class _0003 : Attribute
	{
	}
}

#endif
