// SA_COMPAT: compile-compatible reconstruction of SmartAssembly internals.
// Delegate-typed proxy fields keep expression trees / extension methods
// statically typed. Runtime tokens cannot be reproduced after a rebuild.
using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using Cvte.Pipe.Http.Response;
using Cvte.WebService.Utils;

internal static class SA_Proxy
{
	public static Type _0001___0001(params object[] args) { return default(Type); }
	public static Encoding _0003___0003(params object[] args) { return default(Encoding); }
	public static byte[] _007E_0005___0004(params object[] args) { return default(byte[]); }
	public static ICryptoTransform _007E_000F___0008(params object[] args) { return default(ICryptoTransform); }
	public static ICryptoTransform _007E_0010___0008(params object[] args) { return default(ICryptoTransform); }
	public static void _007E_0011___000E(params object[] args) { return; }
	public static void _007E_0012___000F(params object[] args) { return; }
	public static void _007E_0013___000F(params object[] args) { return; }
	public static byte[] _0015___0010(params object[] args) { return default(byte[]); }
	public static byte[] _007E_0014___0010(params object[] args) { return default(byte[]); }
	public static string _007E_0016___0011(params object[] args) { return default(string); }
	public static string _007E_0017___0012(params object[] args) { return default(string); }
	public static int _007E_0018___0013(params object[] args) { return default(int); }
	public static string _007E_0019___0014(params object[] args) { return default(string); }
	public static string _007E_001A___0014(params object[] args) { return default(string); }
	public static byte _001B___0015(params object[] args) { return default(byte); }
	public static bool _001D___0016(params object[] args) { return default(bool); }
	public static bool _001E___0016(params object[] args) { return default(bool); }
	public static string _007E_007F___0018(params object[] args) { return default(string); }
	public static string _007E_0080___0018(params object[] args) { return default(string); }
	public static string _007E_0081___0018(params object[] args) { return default(string); }
	public static string _007E_0082___0018(params object[] args) { return default(string); }
	public static string _007E_0083___0018(params object[] args) { return default(string); }
	public static string _007E_0084___0018(params object[] args) { return default(string); }
	public static string _0082___0018(params object[] args) { return default(string); }
	public static string _0083___0018(params object[] args) { return default(string); }
	public static string _0086___0018(params object[] args) { return default(string); }
	public static byte[] _007E_0087___0019(params object[] args) { return default(byte[]); }
	public static StringBuilder _007E_0088___001A(params object[] args) { return default(StringBuilder); }
	public static bool _008B___001D(params object[] args) { return default(bool); }
	public static Type _007E_008C___001E(params object[] args) { return default(Type); }
	public static Type _008C___001E(params object[] args) { return default(Type); }
	public static PropertyInfo[] _007E_008D___001F(params object[] args) { return default(PropertyInfo[]); }
	public static object _007E_008E___007F(params object[] args) { return default(object); }
	public static string _008F___0080(params object[] args) { return default(string); }
	public static string _0090___0081(params object[] args) { return default(string); }
	public static void _007E_0092___0082(params object[] args) { return; }
	public static void _0091___0082(params object[] args) { return; }
	public static bool _007E_0094___0083(params object[] args) { return default(bool); }
	public static bool _007E_0095___0083(params object[] args) { return default(bool); }
	public static bool _007E_0096___0083(params object[] args) { return default(bool); }
	public static bool _0094___0083(params object[] args) { return default(bool); }
	public static string _0097___0084(params object[] args) { return default(string); }
	public static int _0098___0086(params object[] args) { return default(int); }
	public static ErrorCode _007E_009A___0087(params object[] args) { return default(ErrorCode); }
	public static Delegate _009B___0088(params object[] args) { return default(Delegate); }
	public static Delegate _009C___0088(params object[] args) { return default(Delegate); }
	public static string _009E___008A(params object[] args) { return default(string); }
	public static DateTime _009F___008B(params object[] args) { return default(DateTime); }
	public static TimeSpan _0001_0002___008C(params object[] args) { return default(TimeSpan); }
	public static DirectoryInfo _0002_0002___008D(params object[] args) { return default(DirectoryInfo); }
	public static void _007E_0003_0002___008E(params object[] args) { return; }
	public static string _0004_0002___008F(params object[] args) { return default(string); }
	public static int _007E_0005_0002___0090(params object[] args) { return default(int); }
	public static string _007E_0006_0002___0091(params object[] args) { return default(string); }
	public static void _007E_0008_0002___0093(params object[] args) { return; }
	public static ParameterExpression _000E_0002___0094(params object[] args) { return default(ParameterExpression); }
	public static MethodBase _000F_0002___0095(params object[] args) { return default(MethodBase); }
	public static MemberExpression _0010_0002___0096(params object[] args) { return default(MemberExpression); }
	public static ConstantExpression _0011_0002___0097(params object[] args) { return default(ConstantExpression); }
	public static FieldInfo _0012_0002___0098(params object[] args) { return default(FieldInfo); }
	public static MemberExpression _0013_0002___0099(params object[] args) { return default(MemberExpression); }
	public static BinaryExpression _0014_0002___009A(params object[] args) { return default(BinaryExpression); }
	public static BinaryExpression _0015_0002___009A(params object[] args) { return default(BinaryExpression); }
	public static NewExpression _0016_0002___009B(params object[] args) { return default(NewExpression); }
	public static MemberAssignment _0017_0002___009C(params object[] args) { return default(MemberAssignment); }
	public static MethodCallExpression _0018_0002___009D(params object[] args) { return default(MethodCallExpression); }
	public static MemberInitExpression _0019_0002___009E(params object[] args) { return default(MemberInitExpression); }
	public static dynamic _0001;
}

internal static class SA_Helpers
{
	public static string Str(params object[] args) { return null; }
}

internal static class SA_Attr<T> where T : Attribute
{
	private static Type Target(object o)
	{
		if (o is Type) return (Type)o;
		if (o is MemberInfo) return ((MemberInfo)o).DeclaringType;
		return o.GetType();
	}
	public static bool _0001(object o, bool inherit = false)
	{
		if (o is PropertyInfo) return ((PropertyInfo)o).IsDefined(typeof(T), inherit);
		return Target(o).IsDefined(typeof(T), inherit);
	}
	public static T _0001(PropertyInfo p, bool inherit = false)
	{
		return p.GetCustomAttributes(typeof(T), inherit).Cast<T>().FirstOrDefault();
	}
	public static T _0001(Type t, bool inherit = false)
	{
		return t.GetCustomAttributes(typeof(T), inherit).Cast<T>().FirstOrDefault();
	}
}

namespace _0002 { }
namespace _0005 { }
namespace _0006 { }
namespace _0008 { }
namespace _000F { }

namespace _0002
{
	// linq2db entity. The no-arg getters are used inside expression trees, so
	// they must be statically typed; the one-arg setters take object.
	internal class _0002
	{
		public long? _0001() { return null; }
		public string _0002_Get() { return null; }
		public string _0003() { return null; }
		public string _0004() { return null; }
		public string _0005() { return null; }
		public string _0006() { return null; }
		public void _0001(object value) { }
		public void _0002_Set(object value) { }
		public void _0003(object value) { }
		public void _0004(object value) { }
		public void _0005(object value) { }
		public void _0006(object value) { }
	}
	internal class _0001 { }
}

namespace _0008
{
	internal class _0002 : Cvte.Pipe.Http.Request.UploadFileRequest { }
}

namespace _000F
{
	internal class _0001 : Cvte.Pipe.Http.Request.UploadFileRequest { }
	internal class _0002 : IniSetting
	{
		public _0002(string path, string section) : base(path, section) { }
	}
}
