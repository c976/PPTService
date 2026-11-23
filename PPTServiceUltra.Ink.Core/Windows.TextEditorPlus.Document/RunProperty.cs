using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.ComponentModel;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document.AttachedProperty;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public class RunProperty
	{
		private FontVariants _fontVariants;

		private double _fontSize = SystemFonts.CaptionFontSize;

		private FontFamily _fontFamily = DefaultFontFamily;

		private FontStyle _fontStyle = FontStyles.Normal;

		private FontWeight _fontWeight = FontWeights.Normal;

		private Brush _background = Brushes.Transparent;

		private Brush _foreground = Brushes.Black;

		private double _opacity = 1.0;

		private TextDecorationCollection _textDecoration = new TextDecorationCollection();

		private EffectArgsCollection _effectsArgs = new EffectArgsCollection();

		private string _hyperlink;

		private DashStyle _underlineDashStyle = DashStyles.Solid;

		private Brush _underlineBrush;

		private BaselineAlignment _baselineAlignment = BaselineAlignment.Baseline;

		[CanBeNull]
		private RenderRunProperty _renderRunProperty;

		private static FontFamily DefaultFontFamily => SystemFonts.CaptionFontFamily;

		internal AttachedRunPropertyCollection RunTimeAttachedRunPropertyCollection { get; } = new AttachedRunPropertyCollection();

		public double FontSize
		{
			get
			{
				return _fontSize;
			}
			set
			{
				double num = value.CoerceValue(1.0, 65536.0);
				double fontSize = _fontSize;
				_fontSize = num;
				if (!Converter.AreEquals(fontSize, num))
				{
					RaiseOnTextRunPropertyChanged();
				}
			}
		}

		public FontStyle FontStyle
		{
			get
			{
				return _fontStyle;
			}
			set
			{
				FontStyle fontStyle = _fontStyle;
				_fontStyle = value;
				if (!value.Equals(fontStyle))
				{
					RaiseOnTextRunPropertyChanged();
				}
			}
		}

		public FontWeight FontWeight
		{
			get
			{
				return _fontWeight;
			}
			set
			{
				FontWeight fontWeight = _fontWeight;
				_fontWeight = value;
				if (!value.Equals(fontWeight))
				{
					RaiseOnTextRunPropertyChanged();
				}
			}
		}

		public FontFamily FontFamily
		{
			get
			{
				return _fontFamily;
			}
			set
			{
				FontFamily fontFamily = _fontFamily;
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				_fontFamily = value;
				if (!Converter.AreEquals(fontFamily, _fontFamily))
				{
					RaiseOnTextRunPropertyChanged();
				}
			}
		}

		public FontVariants FontVariants
		{
			get
			{
				return _fontVariants;
			}
			set
			{
				if (value != FontVariants.Normal && value != FontVariants.Subscript && value != FontVariants.Superscript)
				{
					throw new InvalidEnumArgumentException("value", (int)value, typeof(FontVariants));
				}
				FontVariants fontVariants = _fontVariants;
				_fontVariants = value;
				if (!value.Equals(fontVariants))
				{
					RaiseOnTextRunPropertyChanged();
				}
			}
		}

		public Brush Background
		{
			get
			{
				return _background;
			}
			set
			{
				Brush background = _background;
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				_background = value;
				if (!Converter.AreEquals(background, _background))
				{
					RaiseOnTextRunPropertyChanged();
				}
			}
		}

		public Brush Foreground
		{
			get
			{
				return _foreground;
			}
			set
			{
				Brush foreground = _foreground;
				if (value == null)
				{
					if (DebuggingProperties.IsDebug)
					{
						throw new ArgumentNullException("value");
					}
					value = InnerTextContext.DefaultForeground;
				}
				_foreground = value;
				if (!Converter.AreEquals(foreground, _foreground))
				{
					RaiseOnTextRunPropertyChanged();
				}
			}
		}

		public TextDecorationCollection TextDecoration
		{
			get
			{
				return _textDecoration;
			}
			set
			{
				TextDecorationCollection textDecoration = _textDecoration;
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				_textDecoration = value;
				if (!_textDecoration.IsFrozen && _textDecoration.Count == 1 && Converter.AreEquals(value, TextDecorations.Underline))
				{
					_textDecoration = TextDecorations.Underline;
				}
				if (!Converter.AreEquals(textDecoration, _textDecoration))
				{
					RaiseOnTextRunPropertyChanged();
				}
			}
		}

		public EffectArgsCollection EffectsArgs
		{
			get
			{
				return _effectsArgs;
			}
			set
			{
				EffectArgsCollection effectsArgs = _effectsArgs;
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				_effectsArgs = value;
				if (!Converter.AreEquals(effectsArgs, _effectsArgs))
				{
					RaiseOnTextRunPropertyChanged();
				}
			}
		}

		public double Opacity
		{
			get
			{
				return _opacity;
			}
			set
			{
				double opacity = value.CoerceValue(0.0, 1.0);
				double opacity2 = _opacity;
				_opacity = opacity;
				if (!Converter.AreEquals(opacity2, _opacity))
				{
					RaiseOnTextRunPropertyChanged();
				}
			}
		}

		public string Hyperlink
		{
			get
			{
				return _hyperlink;
			}
			set
			{
				string hyperlink = _hyperlink;
				_hyperlink = value;
				if (!Converter.AreEquals(hyperlink, _hyperlink))
				{
					RaiseOnTextRunPropertyChanged();
				}
			}
		}

		public bool IsHyperlinkHit { get; set; }

		public DashStyle UnderlineDashStyle
		{
			get
			{
				return _underlineDashStyle;
			}
			set
			{
				DashStyle underlineDashStyle = _underlineDashStyle;
				_underlineDashStyle = value;
				if (!Converter.AreEquals(underlineDashStyle, _underlineDashStyle))
				{
					RaiseOnTextRunPropertyChanged();
				}
			}
		}

		public Brush UnderlineBrush
		{
			get
			{
				return _underlineBrush;
			}
			set
			{
				Brush underlineBrush = _underlineBrush;
				_underlineBrush = value;
				if (!Converter.AreEquals(underlineBrush, _underlineBrush))
				{
					RaiseOnTextRunPropertyChanged();
				}
			}
		}

		public BaselineAlignment BaselineAlignment
		{
			get
			{
				return _baselineAlignment;
			}
			set
			{
				BaselineAlignment baselineAlignment = _baselineAlignment;
				_baselineAlignment = value;
				if (!value.Equals(baselineAlignment))
				{
					RaiseOnTextRunPropertyChanged();
				}
			}
		}

		[CanBeNull]
		internal RenderRunProperty RenderRunProperty => _renderRunProperty ?? (_renderRunProperty = new RenderRunProperty(this));

		[Obsolete("不符合事件命名规范，请使用 TextRunPropertyChanged 代替，true")]
		public event EventHandler OnTextRunPropertyChanged;

		public event EventHandler TextRunPropertyChanged;

		public RunProperty Clone()
		{
			RunProperty runProperty = new RunProperty();
			CopyRunProperty(this, runProperty);
			return runProperty;
		}

		public void SetRunProperty(RunProperty runProp)
		{
			FontVariants = runProp.FontVariants;
			FontSize = runProp.FontSize;
			Opacity = runProp.Opacity;
			FontStyle = runProp.FontStyle;
			BaselineAlignment = runProp.BaselineAlignment;
			FontWeight = runProp.FontWeight;
			if (runProp.FontFamily != null)
			{
				FontFamily = runProp.FontFamily;
			}
			Background = PropertyCloningTool.Clone(runProp.Background);
			Foreground = PropertyCloningTool.Clone(runProp.Foreground);
			TextDecoration = PropertyCloningTool.Clone(runProp.TextDecoration);
			UnderlineBrush = PropertyCloningTool.Clone(runProp.UnderlineBrush);
			UnderlineDashStyle = PropertyCloningTool.Clone(runProp.UnderlineDashStyle);
			EffectsArgs = runProp.EffectsArgs.Clone();
			RunTimeAttachedRunPropertyCollection.Reset(runProp.RunTimeAttachedRunPropertyCollection.Clone());
			if (runProp.Hyperlink != null)
			{
				Hyperlink = runProp.Hyperlink;
			}
			IsHyperlinkHit = runProp.IsHyperlinkHit;
		}

		protected static void CopyRunProperty(RunProperty source, RunProperty dest)
		{
			dest._fontVariants = source.FontVariants;
			dest._fontSize = source.FontSize;
			dest._opacity = source.Opacity;
			dest._fontStyle = source.FontStyle;
			dest._baselineAlignment = source.BaselineAlignment;
			dest._fontWeight = source.FontWeight;
			if (source.FontFamily != null)
			{
				dest._fontFamily = source.FontFamily;
			}
			dest._background = PropertyCloningTool.Clone(source.Background);
			dest._foreground = PropertyCloningTool.Clone(source.Foreground);
			dest._textDecoration = PropertyCloningTool.Clone(source.TextDecoration);
			dest._underlineBrush = PropertyCloningTool.Clone(source.UnderlineBrush);
			dest._underlineDashStyle = PropertyCloningTool.Clone(source.UnderlineDashStyle);
			dest._effectsArgs = source.EffectsArgs.Clone();
			dest.RunTimeAttachedRunPropertyCollection.Reset(source.RunTimeAttachedRunPropertyCollection.Clone());
			if (source.Hyperlink != null)
			{
				dest._hyperlink = source.Hyperlink;
			}
			dest.IsHyperlinkHit = source.IsHyperlinkHit;
		}

		public override bool Equals(object o)
		{
			RunProperty runProperty = o as RunProperty;
			if (runProperty == null)
			{
				return false;
			}
			if (!Converter.AreEquals(FontSize, runProperty.FontSize))
			{
				return false;
			}
			if (!Converter.AreEquals(FontFamily, runProperty.FontFamily))
			{
				return false;
			}
			if (!Converter.AreEquals(Foreground, runProperty.Foreground))
			{
				return false;
			}
			if (!RunTimeAttachedRunPropertyCollection.Equals(runProperty.RunTimeAttachedRunPropertyCollection))
			{
				return false;
			}
			if (!Converter.AreEquals(FontVariants, runProperty.FontVariants))
			{
				return false;
			}
			if (!Converter.AreEquals(FontStyle, runProperty.FontStyle))
			{
				return false;
			}
			if (!Converter.AreEquals(Opacity, runProperty.Opacity))
			{
				return false;
			}
			if (!Converter.AreEquals(FontWeight, runProperty.FontWeight))
			{
				return false;
			}
			if (!Converter.AreEquals(TextDecoration, runProperty.TextDecoration))
			{
				return false;
			}
			if (!Converter.AreEquals(UnderlineDashStyle, runProperty.UnderlineDashStyle))
			{
				return false;
			}
			if (!Converter.AreEquals(UnderlineBrush, runProperty.UnderlineBrush))
			{
				return false;
			}
			if (!Converter.AreEquals(IsHyperlinkHit, runProperty.IsHyperlinkHit))
			{
				return false;
			}
			if (!Converter.AreEquals(Hyperlink, runProperty.Hyperlink))
			{
				return false;
			}
			if (!Converter.AreEquals(EffectsArgs, runProperty.EffectsArgs))
			{
				return false;
			}
			if (!Converter.AreEquals(BaselineAlignment, runProperty.BaselineAlignment))
			{
				return false;
			}
			if (!Converter.AreEquals(Background, runProperty.Background))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		private void RaiseOnTextRunPropertyChanged()
		{
			OnTextRunPropertyChanged?.Invoke(this, EventArgs.Empty);
			TextRunPropertyChanged?.Invoke(this, EventArgs.Empty);
			InvalidateRenderRunProperty();
		}

		internal void InvalidateRenderRunProperty()
		{
			_renderRunProperty = null;
		}
	}
}
