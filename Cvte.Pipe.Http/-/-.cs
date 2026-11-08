// SA_COMPAT
#if SA_ORIGINAL_DECOMPILED
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;

namespace _0003
{
	internal static class _0001<_0001> where _0001 : Attribute
	{
		public static bool _0001(object _0002, bool _0003 = false)
		{
			return global::_0003._0001<_0001>._0001(_0002.GetType(), false);
		}

		public static bool _0001(Type _0002, bool _0003 = false)
		{
			return _0002.IsDefined(typeof(_0001), _0003);
		}

		public static bool _0001(PropertyInfo _0002, bool _0003 = false)
		{
			return _0002.IsDefined(typeof(_0001), _0003);
		}

		public static _0001 _0001(object _0002, bool _0003 = false)
		{
			return global::_0003._0001<_0001>._0001(_0002.GetType(), false);
		}

		public static _0001 _0001(Type _0002, bool _0003 = false)
		{
			_0001[] source = global::_0003._0001<_0001>._0001(_0002, _0003);
			if (source.Any())
			{
				return source.First();
			}
			return null;
		}

		public static _0001 _0001(PropertyInfo _0002, bool _0003 = false)
		{
			_0001[] source = global::_0003._0001<_0001>._0001(_0002, _0003);
			if (source.Any())
			{
				return source.First();
			}
			return null;
		}

		public static _0001[] _0001(Type _0002, bool _0003 = false)
		{
			return (_0001[])_0002.GetCustomAttributes(typeof(_0001), _0003);
		}

		public static _0001[] _0001(PropertyInfo _0002, bool _0003 = false)
		{
			return (_0001[])_0002.GetCustomAttributes(typeof(_0001), _0003);
		}
	}
}
namespace _0002
{
	internal class _0001
	{
		public static byte[] _0001<_0001>(_0001 _0002)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				new DataContractJsonSerializer(_0002.GetType()).WriteObject((Stream)memoryStream, (object)_0002);
				return memoryStream.ToArray();
			}
		}

		public static byte[] _0002<_0001>(_0001 _0002)
		{
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					new DataContractJsonSerializer(_0002.GetType()).WriteObject((Stream)memoryStream, (object)_0002);
					return memoryStream.ToArray();
				}
			}
			catch (IOException)
			{
				return new byte[0];
			}
			catch (Exception)
			{
				return new byte[0];
			}
		}

		public static byte[] _0001(Type _0002, object _0003)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				new DataContractJsonSerializer(_0002).WriteObject((Stream)memoryStream, _0003);
				return memoryStream.ToArray();
			}
		}

		public static _0001 _0001<_0001>(byte[] _0002) where _0001 : class
		{
			try
			{
				using (MemoryStream stream = new MemoryStream(_0002))
				{
					return (_0001)new DataContractJsonSerializer(typeof(_0001)).ReadObject((Stream)stream);
				}
			}
			catch (IOException)
			{
				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static _0001 _0002<_0001>(byte[] _0002)
		{
			using (MemoryStream stream = new MemoryStream(_0002))
			{
				return (_0001)new DataContractJsonSerializer(typeof(_0001)).ReadObject((Stream)stream);
			}
		}

		public static _0001 _0001<_0001>(Stream _0002)
		{
			return (_0001)new DataContractJsonSerializer(typeof(_0001)).ReadObject(_0002);
		}

		public static object _0001(Type _0002, byte[] _0003)
		{
			using (MemoryStream stream = new MemoryStream(_0003))
			{
				return new DataContractJsonSerializer(_0002).ReadObject((Stream)stream);
			}
		}
	}
	internal class _0002
	{
		public static string _0001(string _0002)
		{
			if (!File.Exists(_0002))
			{
				throw new FileNotFoundException(_0002);
			}
			using (FileStream inputStream = new FileStream(_0002, FileMode.Open, FileAccess.Read))
			{
				using (MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider())
				{
					byte[] array = mD5CryptoServiceProvider.ComputeHash(inputStream);
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < array.Length; i++)
					{
						stringBuilder.Append(array[i].ToString("x2"));
					}
					return stringBuilder.ToString();
				}
			}
		}

		public static string _0002(string _0002)
		{
			if (_0002 == null)
			{
				throw new ArgumentNullException("valueToEncrypt");
			}
			byte[] bytes = Encoding.UTF8.GetBytes(_0002);
			using (MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider())
			{
				byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append(array[i].ToString("x2"));
				}
				return stringBuilder.ToString();
			}
		}
	}
}

#else
// SA_COMPAT: reconstructed SmartAssembly helpers for Cvte.Pipe.Http.
// SmartAssembly emitted several methods that differ ONLY by return type;
// IL allows that, C# does not.  They are split into named methods below.
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;

internal static class SA_Attr<T> where T : Attribute
{
	private static Type TypeOf(object o)
	{
		if (o is Type) return (Type)o;
		if (o is PropertyInfo) return ((PropertyInfo)o).DeclaringType;
		if (o is MemberInfo) return ((MemberInfo)o).DeclaringType;
		return o.GetType();
	}

	public static bool Has(object o, bool inherit = false)
	{
		if (o is PropertyInfo) return ((PropertyInfo)o).IsDefined(typeof(T), inherit);
		return TypeOf(o).IsDefined(typeof(T), inherit);
	}

	public static bool Has(Type t, bool inherit = false)
	{
		return t.IsDefined(typeof(T), inherit);
	}

	public static bool Has(PropertyInfo p, bool inherit = false)
	{
		return p.IsDefined(typeof(T), inherit);
	}

	public static T Get(object o, bool inherit = false)
	{
		return GetAll(TypeOf(o), inherit).FirstOrDefault();
	}

	public static T Get(Type t, bool inherit = false)
	{
		return GetAll(t, inherit).FirstOrDefault();
	}

	public static T Get(PropertyInfo p, bool inherit = false)
	{
		return GetAll(p, inherit).FirstOrDefault();
	}

	public static T[] GetAll(Type t, bool inherit = false)
	{
		return (T[])t.GetCustomAttributes(typeof(T), inherit);
	}

	public static T[] GetAll(PropertyInfo p, bool inherit = false)
	{
		return (T[])p.GetCustomAttributes(typeof(T), inherit);
	}
}

internal static class SA_Json
{
	public static byte[] ToJson<T>(T value)
	{
		using (MemoryStream ms = new MemoryStream())
		{
			new DataContractJsonSerializer(value == null ? typeof(T) : value.GetType())
				.WriteObject(ms, value);
			return ms.ToArray();
		}
	}

	public static byte[] ToJson(Type type, object value)
	{
		using (MemoryStream ms = new MemoryStream())
		{
			new DataContractJsonSerializer(type).WriteObject(ms, value);
			return ms.ToArray();
		}
	}

	public static object FromJson(Type type, byte[] bytes)
	{
		using (MemoryStream ms = new MemoryStream(bytes))
		{
			return new DataContractJsonSerializer(type).ReadObject(ms);
		}
	}

	public static object FromJson(Type type, Stream stream)
	{
		return new DataContractJsonSerializer(type).ReadObject(stream);
	}

	public static T FromJson<T>(byte[] bytes) where T : class
	{
		using (MemoryStream ms = new MemoryStream(bytes))
		{
			return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
		}
	}

	public static T FromJson<T>(Stream stream) where T : class
	{
		return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(stream);
	}
}

internal static class SA_Md5
{
	public static string FileMD5(string path)
	{
		if (!File.Exists(path)) throw new FileNotFoundException(path);
		using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
		using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
		{
			return string.Concat(md5.ComputeHash(fs).Select(b => b.ToString("x2")));
		}
	}

	public static string StringMD5(string value)
	{
		if (value == null) throw new ArgumentNullException("valueToEncrypt");
		using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
		{
			return string.Concat(md5.ComputeHash(Encoding.UTF8.GetBytes(value))
				.Select(b => b.ToString("x2")));
		}
	}
}
#endif
