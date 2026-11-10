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
				case '*':
					_operation = (decimal a, decimal b) => a * b;
					break;
				case '+':
					_operation = (decimal a, decimal b) => a + b;
					break;
				case '-':
					_operation = (decimal a, decimal b) => a - b;
					break;
				default:
					throw new ArgumentException("Invalid operation " + operation);
				case '/':
					_operation = (decimal a, decimal b) => a / b;
					break;
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
					RequireEndOfText();
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
				while (true)
				{
					if (_pos < _text.Length)
					{
						char c = _text[_pos];
						if (c != '+' && c != '-')
						{
							break;
						}
						_pos++;
						IExpression right = ParseTerm();
						expression = new BinaryOperation(c, expression, right);
						continue;
					}
					return expression;
				}
				return expression;
			}

			private IExpression ParseTerm()
			{
				IExpression expression = ParseFactor();
				while (true)
				{
					if (_pos < _text.Length)
					{
						char c = _text[_pos];
						if (c != '*' && c != '/')
						{
							break;
						}
						_pos++;
						IExpression right = ParseFactor();
						expression = new BinaryOperation(c, expression, right);
						continue;
					}
					return expression;
				}
				return expression;
			}

			private IExpression ParseFactor()
			{
				SkipWhiteSpace();
				if (_pos >= _text.Length)
				{
					throw new ArgumentException("Unexpected end of text");
				}
				char c = _text[_pos];
				switch (c)
				{
				case '+':
					_pos++;
					return ParseFactor();
				case '-':
					_pos++;
					return new Negate(ParseFactor());
				case '(':
				{
					_pos++;
					IExpression result = ParseExpression();
					SkipWhiteSpace();
					Require(')');
					SkipWhiteSpace();
					return result;
				}
				case '{':
				{
					_pos++;
					int num = _text.IndexOf('}', _pos);
					if (num < 0)
					{
						_pos--;
						throw new ArgumentException("Unmatched '{'");
					}
					if (num == _pos)
					{
						throw new ArgumentException("Missing parameter index after '{'");
					}
					Variable result2 = new Variable(_text.Substring(_pos, num - _pos).Trim());
					_pos = num + 1;
					SkipWhiteSpace();
					return result2;
				}
				default:
				{
					Match match = Regex.Match(_text.Substring(_pos), "(\\d+\\.?\\d*|\\d*\\.?\\d+)");
					if (match.Success)
					{
						_pos += match.Length;
						SkipWhiteSpace();
						return new Constant(match.Value);
					}
					throw new ArgumentException($"Unexpeted character '{c}'");
				}
				case 'd':
				case 't':
					return CreateVariable(3);
				case 'c':
				case 'z':
					return CreateVariable(2);
				case 'b':
				case 'y':
					return CreateVariable(1);
				case 'a':
				case 'x':
					return CreateVariable(0);
				}
			}

			private IExpression CreateVariable(int n)
			{
				_pos++;
				SkipWhiteSpace();
				return new Variable(n);
			}

			private void SkipWhiteSpace()
			{
				while (_pos < _text.Length && char.IsWhiteSpace(_text[_pos]))
				{
					_pos++;
				}
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
				if (_pos != _text.Length)
				{
					throw new ArgumentException("Unexpected character '" + _text[_pos] + "'");
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

			public decimal Eval(object[] args)
			{
				if (_index >= args.Length)
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
				if (targetType == typeof(decimal))
				{
					return num;
				}
				if (targetType == typeof(string))
				{
					return num.ToString(CultureInfo.InvariantCulture);
				}
				if (targetType == typeof(int))
				{
					return (int)num;
				}
				if (targetType == typeof(double))
				{
					return (double)num;
				}
				if (!(targetType == typeof(long)))
				{
					throw new ArgumentException($"Unsupported target type {targetType.FullName}");
				}
				return (long)num;
			}
			catch (Exception ex)
			{
				ProcessException(ex);
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
				value = new Parser().Parse(s);
				_storedExpressions[s] = value;
			}
			return value;
		}
	}
}
