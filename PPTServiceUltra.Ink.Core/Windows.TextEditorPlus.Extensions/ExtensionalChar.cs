using System;
using System.Windows;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Storage;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Extensions
{
	public abstract class ExtensionalChar : ICharObject, IEquatable<string>, ICloneable, IEquatable<ExtCharSaveInfo>, ICharFeature
	{
		public abstract Point LogicalCenter { get; set; }

		public abstract Size Size { get; set; }

		public virtual bool IsClickSelectionEnable { get; set; }

		public virtual bool IsNeedRotationUnderVertical { get; set; }

		public virtual bool HitTestEnable { get; }

		public abstract UIElement Render(RunProperty runProperty = null);

		public abstract void StyleAdapter(PropertyChangedArgs propertyChangedArgs);

		public virtual void UpdateRender()
		{
		}

		public abstract ExtCharSaveInfo GetSaveInfo();

		public abstract void SetSaveInfo(ExtCharSaveInfo extCharSaveInfo);

		public abstract ExtensionalChar DeepClone();

		public bool Equals([NotNull] ExtCharSaveInfo other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			ExtCharSaveInfo saveInfo = GetSaveInfo();
			string text = TextContext.StorageParser.Deparse(saveInfo);
			string value = TextContext.StorageParser.Deparse(other);
			return text.Equals(value);
		}

		public virtual string ToText()
		{
			return '\ufffd'.ToString();
		}

		public object Clone()
		{
			return DeepClone();
		}

		public virtual bool Equals(string saveInfoString)
		{
			return false;
		}
	}
}
