using System.Runtime.CompilerServices;
using System.Windows;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public class ParagraphProperty
	{
		private int _indentLevel;

		public int IndentLevel
		{
			get
			{
				return _indentLevel;
			}
			set
			{
				int num = value.CoerceValue(0, 8);
				if (!Converter.AreEquals(num, _indentLevel))
				{
					_indentLevel = num;
				}
			}
		}

		public double MarginLeft { get; set; }

		public FlowDirection Direction { get; set; }

		public double Indent { get; set; }

		internal MarkerProperty MarkerProperty { get; set; } = new MarkerProperty();

		public double LineSpacing { get; set; } = 1.0;

		public double FixedLineSpacing { get; set; } = double.NaN;

		public TextAlignment TextAlignment { get; set; }

		public RunProperty DefaultRunProperty { get; set; } = new RunProperty();

		public double ParagraphBefore { get; set; }

		public double ParagraphAfter { get; set; }

		public ParagraphProperty Clone()
		{
			return new ParagraphProperty
			{
				LineSpacing = LineSpacing,
				FixedLineSpacing = FixedLineSpacing,
				ParagraphAfter = ParagraphAfter,
				ParagraphBefore = ParagraphBefore,
				TextAlignment = TextAlignment,
				DefaultRunProperty = DefaultRunProperty?.Clone(),
				MarkerProperty = MarkerProperty?.Clone(),
				Indent = Indent,
				MarginLeft = MarginLeft,
				IndentLevel = IndentLevel,
				Direction = Direction
			};
		}

		public override bool Equals(object o)
		{
			ParagraphProperty paragraphProperty = o as ParagraphProperty;
			if (paragraphProperty == null)
			{
				return false;
			}
			if (!LineSpacing.Equals(paragraphProperty.LineSpacing))
			{
				return false;
			}
			if (!FixedLineSpacing.Equals(paragraphProperty.FixedLineSpacing))
			{
				return false;
			}
			if (!TextAlignment.Equals(paragraphProperty.TextAlignment))
			{
				return false;
			}
			if (!DefaultRunProperty.Equals(paragraphProperty.DefaultRunProperty))
			{
				return false;
			}
			if (!Indent.Equals(paragraphProperty.Indent))
			{
				return false;
			}
			if (!MarginLeft.Equals(paragraphProperty.MarginLeft))
			{
				return false;
			}
			if (!Converter.AreEquals(MarkerProperty, paragraphProperty.MarkerProperty))
			{
				return false;
			}
			if (!ParagraphBefore.Equals(paragraphProperty.ParagraphBefore))
			{
				return false;
			}
			if (!ParagraphAfter.Equals(paragraphProperty.ParagraphAfter))
			{
				return false;
			}
			if (!IndentLevel.Equals(paragraphProperty.IndentLevel))
			{
				return false;
			}
			if (!Direction.Equals(paragraphProperty.Direction))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		public void SetParagraphProperty(ParagraphProperty paragraphProperty)
		{
			MarginLeft = paragraphProperty.MarginLeft;
			Indent = paragraphProperty.Indent;
			if (paragraphProperty.MarkerProperty != null)
			{
				MarkerProperty = paragraphProperty.MarkerProperty.Clone();
			}
			LineSpacing = paragraphProperty.LineSpacing;
			FixedLineSpacing = paragraphProperty.FixedLineSpacing;
			TextAlignment = paragraphProperty.TextAlignment;
			DefaultRunProperty = paragraphProperty.DefaultRunProperty.Clone();
			ParagraphBefore = paragraphProperty.ParagraphBefore;
			ParagraphAfter = paragraphProperty.ParagraphAfter;
			IndentLevel = paragraphProperty.IndentLevel;
			Direction = paragraphProperty.Direction;
		}
	}
}
