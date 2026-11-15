using System;
using System.Windows;
using System.Windows.Forms;
using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	internal class DataObjectAdapter
	{
		private readonly System.Windows.Forms.IDataObject _formsDataObject;

		private readonly System.Windows.IDataObject _mediaDataObject;

		internal DataObjectAdapter([NotNull] System.Windows.Forms.IDataObject dataObject)
		{
			if (dataObject == null)
			{
				throw new ArgumentNullException("dataObject");
			}
			_formsDataObject = dataObject;
		}

		internal DataObjectAdapter([NotNull] System.Windows.IDataObject dataObject)
		{
			if (dataObject == null)
			{
				throw new ArgumentNullException("dataObject");
			}
			_mediaDataObject = dataObject;
		}

		internal bool GetDataPresent(string stringFormat)
		{
			if (string.IsNullOrEmpty(stringFormat))
			{
				return false;
			}
			return _formsDataObject?.GetDataPresent(stringFormat) ?? _mediaDataObject.GetDataPresent(stringFormat);
		}

		internal object GetData(string stringFormat)
		{
			if (string.IsNullOrEmpty(stringFormat))
			{
				return null;
			}
			object obj = _formsDataObject?.GetData(stringFormat);
			if (obj == null)
			{
				System.Windows.IDataObject mediaDataObject = _mediaDataObject;
				if (mediaDataObject == null)
				{
					return null;
				}
				obj = mediaDataObject.GetData(stringFormat);
			}
			return obj;
		}
	}
}
