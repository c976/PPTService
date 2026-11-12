namespace Cvte.Paint.Framework
{
	internal static class PathFigureSyntax
	{
		public const string FillRuleEvenOdd = "F0";

		public const string FillRuleNonzero = "F1";

		public const char AbsoluteStartPointChar = 'M';

		public const char RelativeStartPointChar = 'm';

		public static readonly char[] LineChars = new char[2] { 'L', 'l' };

		public static readonly char[] CubicBezierChars = new char[2] { 'C', 'c' };

		public static readonly char[] QuadraticBezierChars = new char[2] { 'Q', 'q' };

		public static readonly char[] ArcChars = new char[2] { 'A', 'a' };

		public static readonly char[] CloseChars = new char[2] { 'Z', 'z' };
	}
}
