using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Cli.Utils
{
	
	
	internal static class CommandLineAssigningExtensions
	{
		public static object ToAssignableValue( this IReadOnlyList<string> values, Type assignableType)
		{
			bool result;
			if (assignableType == typeof(bool))
			{
				return !((!((values != null && bool.TryParse(values[0], out result)) ? new bool?(result) : ((bool?)null))) ?? false);
			}
			if (assignableType.IsAssignableFrom(typeof(string)))
			{
				return MergeList(values);
			}
			if (assignableType.IsAssignableFrom(typeof(byte)))
			{
				byte result2;
				byte.TryParse(MergeList(values), NumberStyles.Integer, CultureInfo.InvariantCulture, out result2);
				return result2;
			}
			if (assignableType.IsAssignableFrom(typeof(short)))
			{
				short result3;
				short.TryParse(MergeList(values), NumberStyles.Integer, CultureInfo.InvariantCulture, out result3);
				return result3;
			}
			if (assignableType.IsAssignableFrom(typeof(ushort)))
			{
				ushort result4;
				ushort.TryParse(MergeList(values), NumberStyles.Integer, CultureInfo.InvariantCulture, out result4);
				return result4;
			}
			if (assignableType.IsAssignableFrom(typeof(int)))
			{
				int result5;
				int.TryParse(MergeList(values), NumberStyles.Integer, CultureInfo.InvariantCulture, out result5);
				return result5;
			}
			if (assignableType.IsAssignableFrom(typeof(uint)))
			{
				uint result6;
				uint.TryParse(MergeList(values), NumberStyles.Integer, CultureInfo.InvariantCulture, out result6);
				return result6;
			}
			if (assignableType.IsAssignableFrom(typeof(long)))
			{
				long result7;
				long.TryParse(MergeList(values), NumberStyles.Integer, CultureInfo.InvariantCulture, out result7);
				return result7;
			}
			if (assignableType.IsAssignableFrom(typeof(ulong)))
			{
				ulong result8;
				ulong.TryParse(MergeList(values), NumberStyles.Integer, CultureInfo.InvariantCulture, out result8);
				return result8;
			}
			if (assignableType.IsAssignableFrom(typeof(float)))
			{
				float result9;
				float.TryParse(MergeList(values), NumberStyles.Float, CultureInfo.InvariantCulture, out result9);
				return result9;
			}
			if (assignableType.IsAssignableFrom(typeof(double)))
			{
				double result10;
				double.TryParse(MergeList(values), NumberStyles.Float, CultureInfo.InvariantCulture, out result10);
				return result10;
			}
			if (assignableType.IsAssignableFrom(typeof(decimal)))
			{
				decimal result11;
				decimal.TryParse(MergeList(values), NumberStyles.Float, CultureInfo.InvariantCulture, out result11);
				return result11;
			}
			if (assignableType.IsAssignableFrom(typeof(FileInfo)))
			{
				return new FileInfo(MergeList(values).Trim());
			}
			if (assignableType.IsAssignableFrom(typeof(DirectoryInfo)))
			{
				return new DirectoryInfo(MergeList(values).Trim());
			}
			if (assignableType.IsAssignableFrom(typeof(string[])))
			{
				return values.ToArray();
			}
			if (assignableType.IsAssignableFrom(typeof(List<string>)))
			{
				return values.ToList();
			}
			if (assignableType.IsAssignableFrom(typeof(Collection<string>)))
			{
				return new Collection<string>(values.ToList());
			}
			if (assignableType.IsAssignableFrom(typeof(Dictionary<string, string>)))
			{
				return ParseArgsToDictionary(values);
			}
			if (assignableType.IsAssignableFrom(typeof(KeyValuePair<string, string>)))
			{
				return ParseArgsToDictionary(values).FirstOrDefault();
			}
			throw new NotSupportedException("The property type " + assignableType.FullName + " is not supported.\r\nSupported types are: bool, string?, string[]?, List<string>? and all it's interfaces, Collection<string>? and all it's interfaces, Dictionary<string, string>? and all it's interfaces.");
		}

		
		public static string GetAssignableTypeName(Type assignableType)
		{
			if (assignableType.IsEnum)
			{
				return assignableType.Name;
			}
			if (assignableType.IsAssignableFrom(typeof(FileInfo)))
			{
				return "FILE_PATH";
			}
			if (assignableType.IsAssignableFrom(typeof(DirectoryInfo)))
			{
				return "DIRECTORY_PATH";
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Dictionary<string, string> ParseArgsToDictionary( IReadOnlyList<string> list)
		{
			return (from x in MergeList(list).Split(';')
				select x.Split('=') into x
				where x.Length == 2
				select x).ToDictionary( (string[] x) => x[0].Trim(),  (string[] x) => x[1].Trim(), StringComparer.OrdinalIgnoreCase);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string MergeList( IReadOnlyList<string> list)
		{
			if (list != null)
			{
				if (list.Count != 1)
				{
					return string.Join(" ", list);
				}
				return list[0];
			}
			return "";
		}
	}
}
