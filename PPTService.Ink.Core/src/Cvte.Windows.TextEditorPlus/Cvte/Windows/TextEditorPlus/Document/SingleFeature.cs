using System;
using System.Collections;
using System.Collections.Generic;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class SingleFeature : IFeature, IReadOnlyCollection<string>, IEnumerable<string>, IEnumerable
	{
		public string FeatureText { get; }

		public int Count => 1;

		public SingleFeature(string featureText)
		{
			FeatureText = featureText;
		}

		internal Feature ToFeature()
		{
			return new Feature(FeatureText);
		}

		public IEnumerator<string> GetEnumerator()
		{
			yield return FeatureText;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override string ToString()
		{
			return FeatureText;
		}

		public bool HasFeature(IFeature feature)
		{
			if (feature == this)
			{
				return true;
			}
			foreach (string item in feature)
			{
				if (string.Equals(item, FeatureText, StringComparison.Ordinal))
				{
					return true;
				}
			}
			return false;
		}
	}
}
