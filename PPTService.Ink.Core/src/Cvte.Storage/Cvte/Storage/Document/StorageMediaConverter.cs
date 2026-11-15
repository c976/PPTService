using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Document
{
	
	
	public class StorageMediaConverter< TInTarget,  TOutTarget>
	{
		private readonly ICompoundDocumentAdapter<TInTarget> _inputDocumentAdapter;

		private readonly ICompoundDocumentAdapter<TOutTarget> _outputDocumentAdapter;

		public ICompoundDocumentConverter DocumentConverter { get; set; }

		public StorageMediaConverter(ICompoundDocumentAdapter<TInTarget> inputDocumentAdapter, ICompoundDocumentAdapter<TOutTarget> outputDocumentAdapter)
		{
			if (inputDocumentAdapter == null)
			{
				throw new ArgumentNullException("inputDocumentAdapter");
			}
			_inputDocumentAdapter = inputDocumentAdapter;
			if (outputDocumentAdapter == null)
			{
				throw new ArgumentNullException("outputDocumentAdapter");
			}
			_outputDocumentAdapter = outputDocumentAdapter;
		}

		public bool Convert(TInTarget inTarget, TOutTarget outTarget)
		{
			CompoundDocument document;
			if (!_inputDocumentAdapter.TryLoad(inTarget, out document))
			{
				return false;
			}
			if (DocumentConverter != null)
			{
				CompoundDocument document2 = DocumentConverter.Convert(document);
				_outputDocumentAdapter.Save(document2, outTarget);
				return true;
			}
			_outputDocumentAdapter.Save(document, outTarget);
			return true;
		}
	}
}
