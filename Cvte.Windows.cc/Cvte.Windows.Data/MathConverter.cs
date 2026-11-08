using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;

namespace Cvte.Windows.Data
{
	public class MathConverter : IValueConverter, IMultiValueConverter
	{
		private interface IExpression
		{
			decimal Eval(object[] args);
		}

		private class BinaryOperation : IExpression
		{
			private readonly IExpression _left;

			private readonly Func<decimal, decimal, decimal> _operation;

			private readonly IExpression _right;

			public BinaryOperation(char operation, IExpression left, IExpression right)
			{
				_left = left;
				_right = right;
				switch (operation)
				{
				case '+':
					_operation = (decimal a, decimal b) => a + b;
					break;
				case '-':
					_operation = (decimal a, decimal b) => a - b;
					break;
				case '*':
					_operation = (decimal a, decimal b) => a * b;
					break;
				case '/':
					_operation = (decimal a, decimal b) => a / b;
					break;
				default:
					throw new ArgumentException("Invalid operation " + operation);
				}
			}

			public decimal Eval(object[] args)
			{
				return _operation(_left.Eval(args), _right.Eval(args));
			}
		}

		private class Constant : IExpression
		{
			private readonly decimal _value;

			public Constant(string text)
			{
				if (!decimal.TryParse(text, out _value))
				{
					throw new ArgumentException($"'{text}' is not a valid number");
				}
			}

			public decimal Eval(object[] args)
			{
				return _value;
			}
		}

		private class Negate : IExpression
		{
			private readonly IExpression _param;

			public Negate(IExpression param)
			{
				_param = param;
			}

			public decimal Eval(object[] args)
			{
				return -_param.Eval(args);
			}
		}

		private class Parser
		{
			private int _pos;

			private string _text;

			public IExpression Parse(string text)
			{
				try
				{
					_pos = 0;
					_text = text;
					IExpression result = ParseExpression();
					do
					{
						RequireEndOfText();
					}
					while (8 == 0);
					return result;
				}
				catch (Exception ex)
				{
					string message = $"MathConverter: error parsing expression '{text}'. {ex.Message} at position {_pos}";
					throw new ArgumentException(message, ex);
				}
			}

			private IExpression ParseExpression()
			{
				IExpression expression = ParseTerm();
				IExpression expression2 = default(IExpression);
				if (0 == 0)
				{
					expression2 = expression;
				}
				char c = default(char);
				while (true)
				{
					int num;
					if (7 != 0)
					{
						num = _pos;
						if (-1 == 0)
						{
							goto IL_0053;
						}
						if (num >= _text.Length)
						{
							break;
						}
					}
					c = _text[_pos];
					num = c;
					goto IL_0053;
					IL_0053:
					int num2 = 43;
					while (num != num2)
					{
						if (0 == 0)
						{
							num = c;
							num2 = 45;
							if (num2 == 0)
							{
								continue;
							}
							if (num == num2)
							{
								break;
							}
						}
						return expression2;
					}
					_pos++;
					IExpression right = ParseTerm();
					if (6 != 0)
					{
						expression2 = new BinaryOperation(c, expression2, right);
					}
				}
				return expression2;
			}

			private IExpression ParseTerm()
			{
				IExpression expression = ParseFactor();
				IExpression expression2 = default(IExpression);
				if (0 == 0)
				{
					expression2 = expression;
				}
				char c = default(char);
				while (true)
				{
					int num;
					if (7 != 0)
					{
						num = _pos;
						if (-1 == 0)
						{
							goto IL_0053;
						}
						if (num >= _text.Length)
						{
							break;
						}
					}
					c = _text[_pos];
					num = c;
					goto IL_0053;
					IL_0053:
					int num2 = 42;
					while (num != num2)
					{
						if (0 == 0)
						{
							num = c;
							num2 = 47;
							if (num2 == 0)
							{
								continue;
							}
							if (num == num2)
							{
								break;
							}
						}
						return expression2;
					}
					_pos++;
					IExpression right = ParseFactor();
					if (6 != 0)
					{
						expression2 = new BinaryOperation(c, expression2, right);
					}
				}
				return expression2;
			}

			private IExpression ParseFactor()
			{
				int num = 0;
				Match match = default(Match);
				SkipWhiteSpace();
				if (_pos >= _text.Length)
				{
					throw new ArgumentException("Unexpected end of text");
				}
				char c;
				if (0 == 0)
				{
					c = _text[_pos];
					if (c == '+')
					{
						_pos++;
						return ParseFactor();
					}
					if (c == '-')
					{
						goto IL_006c;
					}
					if (c == 'x')
					{
						goto IL_0090;
					}
				}
				if (c == 'a')
				{
					goto IL_0090;
				}
				if (c == 'y')
				{
					goto IL_00a8;
				}
				if (0 == 0)
				{
					if (c == 'b')
					{
						goto IL_00a8;
					}
					if (c != 'z')
					{
						num = c;
						while (num != 99)
						{
							switch (c)
							{
							case 'd':
							case 't':
								return CreateVariable(3);
							case '(':
							{
								_pos++;
								IExpression result2 = ParseExpression();
								SkipWhiteSpace();
								Require(')');
								SkipWhiteSpace();
								return result2;
							}
							case '{':
							{
								int num2;
								if (0 == 0)
								{
									_pos++;
									num2 = _text.IndexOf('}', _pos);
									num = num2;
									if (1 == 0)
									{
										break;
									}
									if (num < 0)
									{
										_pos--;
										throw new ArgumentException("Unmatched '{'");
									}
									if (num2 == _pos)
									{
										throw new ArgumentException("Missing parameter index after '{'");
									}
								}
								Variable result = new Variable(_text.Substring(_pos, num2 - _pos).Trim());
								_pos = num2 + 1;
								SkipWhiteSpace();
								return result;
							}
							default:
								match = Regex.Match(_text.Substring(_pos), "(\\d+\\.?\\d*|\\d*\\.?\\d+)");
								num = (match.Success ? 1 : 0);
								break;
							}
							if (5 == 0)
							{
								continue;
							}
							goto IL_01d0;
						}
					}
					if (-1 != 0)
					{
						return CreateVariable(2);
					}
				}
				goto IL_01ec;
				IL_00a8:
				if (4 != 0)
				{
					return CreateVariable(1);
				}
				goto IL_006c;
				IL_01ec:
				return new Constant(match.Value);
				IL_0090:
				return CreateVariable(0);
				IL_01d0:
				if (num != 0)
				{
					_pos += match.Length;
					SkipWhiteSpace();
					goto IL_01ec;
				}
				throw new ArgumentException($"Unexpeted character '{c}'");
				IL_006c:
				_pos++;
				return new Negate(ParseFactor());
			}

			private IExpression CreateVariable(int n)
			{
				if (true)
				{
					_pos++;
				}
				SkipWhiteSpace();
				return new Variable(n);
			}

			private void SkipWhiteSpace()
			{
				if (-1 == 0)
				{
					goto IL_003c;
				}
				goto IL_0040;
				IL_0040:
				int num = _pos;
				while (num < _text.Length)
				{
					num = (char.IsWhiteSpace(_text[_pos]) ? 1 : 0);
					if (5 == 0)
					{
						continue;
					}
					goto IL_003a;
				}
				goto IL_003c;
				IL_003c:
				if (0 == 0)
				{
					return;
				}
				goto IL_0005;
				IL_0005:
				_pos++;
				goto IL_0040;
				IL_003a:
				if (num != 0)
				{
					goto IL_0005;
				}
				goto IL_003c;
			}

			private void Require(char c)
			{
				if (_pos >= _text.Length || _text[_pos] != c)
				{
					throw new ArgumentException("Expected '" + c + "'");
				}
				_pos++;
			}

			private void RequireEndOfText()
			{
				while (true)
				{
					if (2 != 0 && _pos != _text.Length)
					{
						if (0 == 0)
						{
							throw new ArgumentException("Unexpected character '" + _text[_pos] + "'");
						}
					}
					else if (0 == 0)
					{
						break;
					}
				}
			}
		}

		private class Variable : IExpression
		{
			private readonly int _index;

			public Variable(string text)
			{
				if (!int.TryParse(text, out _index) || _index < 0)
				{
					throw new ArgumentException($"'{text}' is not a valid parameter index");
				}
			}

			public Variable(int n)
			{
				_index = n;
			}

			public unsafe decimal Eval(object[] args)
			{
				int index = _index;
				IntPtr intPtr = (IntPtr)(void*)args.LongLength;
				do
				{
					if (6 != 0)
					{
						intPtr = (IntPtr)(int)(long)intPtr;
					}
				}
				while (false ? true : false);
				if ((long)index >= (long)intPtr || 4 == 0)
				{
					throw new ArgumentException($"MathConverter: parameter index {_index} is out of range. {args.Length} parameter(s) supplied");
				}
				return System.Convert.ToDecimal(args[_index]);
			}
		}

		private readonly Dictionary<string, IExpression> _storedExpressions = new Dictionary<string, IExpression>();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Convert(new object[1] { value }, targetType, parameter, culture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException("MathConverter 不支持根据正向数学表达式解方程。");
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				decimal num = Parse(parameter.ToString()).Eval(values);
				int num2 = ((targetType == typeof(decimal)) ? 1 : 0);
				if (7 != 0)
				{
					if (num2 != 0)
					{
						return num;
					}
					if (targetType == typeof(string))
					{
						return num.ToString(CultureInfo.InvariantCulture);
					}
					if (!(targetType == typeof(int)))
					{
						if (targetType == typeof(double))
						{
							return (double)num;
						}
						if (targetType == typeof(long))
						{
							return (long)num;
						}
						throw new ArgumentException($"Unsupported target type {targetType.FullName}");
					}
					num2 = (int)num;
				}
				return num2;
			}
			catch (Exception ex)
			{
				do
				{
					ProcessException(ex);
				}
				while (6 == 0);
			}
			return DependencyProperty.UnsetValue;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException("MathConverter 不支持根据正向数学表达式解方程。");
		}

		protected virtual void ProcessException(Exception ex)
		{
			Console.WriteLine(ex.Message);
		}

		private IExpression Parse(string s)
		{
			IExpression value;
			if (!_storedExpressions.TryGetValue(s, out value))
			{
				IExpression expression = new Parser().Parse(s);
				if (0 == 0)
				{
					value = expression;
				}
				if (true)
				{
					_storedExpressions[s] = value;
				}
			}
			return value;
		}
	}
}
