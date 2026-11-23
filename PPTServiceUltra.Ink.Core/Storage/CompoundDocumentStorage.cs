using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Ink.Core.Storage.Encryption;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public class CompoundDocumentStorage< TModel,  TTarget> : IStorage<TModel, TTarget> where TModel : CompoundDocumentStorageModel
	{
		private readonly ParserProvider _parserProvider;

		private readonly FileProvider _fileProvider;

		private readonly ReferenceProvider _referenceProvider;

		protected IFileContentParserManager ContentParserManager => _fileProvider.ContentParserManager;

		private ICompoundDocumentAdapter<TTarget> DocumentAdapter { get; }

		private IReferenceManager ReferenceManager { get; }

		private CompoundDocumentStorageSetting Setting { get; }

		private CompoundDocument CurrentDocument
		{
			get
			{
				return DocumentStateProvider.CurrentDocument;
			}
			set
			{
				DocumentStateProvider.CurrentDocument = value;
			}
		}

		private DocumentStateProvider<TModel> DocumentStateProvider { get; }

		private CompoundDocumentSaveInfo DocumentInfo { get; set; }

		private TTarget Target { get; set; }

		public bool Saved => CurrentDocument != null;

		[CanBeNull]
		public string Hash => CurrentDocument?.Hash;

		public bool SupportsDelta { get; set; }

		public CompoundDocumentStorage([NotNull] ISaveInfoParserManager saveInfoParserManager, [NotNull] ICompoundDocumentAdapter<TTarget> documentAdapter, [NotNull] IReferenceManager referenceManager)
			: this(saveInfoParserManager, documentAdapter, referenceManager, (CompoundDocumentStorageSetting)null, (IFileContentParserManager)null, (DocumentStateManager<TModel>)null)
		{
		}

		public CompoundDocumentStorage([NotNull] ISaveInfoParserManager saveInfoParserManager, [NotNull] ICompoundDocumentAdapter<TTarget> documentAdapter, [NotNull] IReferenceManager referenceManager, [CanBeNull] CompoundDocumentStorageSetting setting, [CanBeNull] IFileContentParserManager fileContentParserManager = null, [CanBeNull] DocumentStateManager<TModel> documentStateManager = null)
		{
			DocumentAdapter = documentAdapter;
			ReferenceManager = referenceManager;
			Setting = setting ?? new CompoundDocumentStorageSetting();
			_parserProvider = new ParserProvider(saveInfoParserManager);
			_fileProvider = new FileProvider(fileContentParserManager ?? new FileContentParserManager());
			_referenceProvider = new ReferenceProvider(ReferenceManager);
			DocumentStateProvider = new DocumentStateProvider<TModel>(documentStateManager);
		}

		public void UpdateBaseDocument()
		{
			DocumentStateProvider.UpdateBaseDocument();
		}

		public TModel Open(TTarget target)
		{
			if (!DocumentAdapter.IsLegal(target))
			{
				throw new BadDocumentException();
			}
			string version;
			if (!DocumentAdapter.TryGetVersion(target, out version))
			{
				throw new UnknowDocumentVersionException
				{
					Version = version
				};
			}
			CompoundDocument document;
			if (!DocumentAdapter.TryLoad(target, out document))
			{
				throw new IncompleteDocumentException();
			}
			CurrentDocument = document;
			CurrentDocument.Version = version;
			Target = target;
			IStorageLogger logger;
			TModel val = DocumentReader.Read<TModel>(CurrentDocument, _parserProvider, _referenceProvider, _fileProvider, out logger);
			DocumentInfo = val.Document;
			if (!logger.WarningExceptions.Any() && !logger.Errors.Any())
			{
				return val;
			}
			SaveReport(logger);
			throw new DocumentFormatException(val, logger.WarningExceptions, logger.Errors);
		}

		public void SaveAs(TModel model, TTarget target)
		{
			OnSaving(model.Document, true);
			Target = target;
			IStorageLogger logger;
			CurrentDocument = Build(model, out logger);
			DocumentInfo = model.Document;
			if (!logger.Errors.Any())
			{
				DocumentAdapter.Save(CurrentDocument, target);
				return;
			}
			SaveReport(logger);
			throw new DocumentFormatException(logger.Errors);
		}

		public void SaveAsDelta(TModel model, TTarget target)
		{
			OnSaving(model.Document, true);
			Target = target;
			if (!SupportsDelta || !TrySaveDeltaAs(model, DocumentStateProvider.GetBaseModel(), target))
			{
				IStorageLogger logger;
				CurrentDocument = Build(model, out logger);
				DocumentInfo = model.Document;
				if (logger.Errors.Any())
				{
					SaveReport(logger);
					throw new DocumentFormatException(logger.Errors);
				}
				DocumentAdapter.Save(CurrentDocument, target);
			}
		}

		public void Save(TModel model)
		{
			if (!Saved)
			{
				throw new InvalidOperationException("文件未保存，请调用SaveAs方法");
			}
			OnSaving(model.Document, false);
			IStorageLogger logger;
			CompoundDocument compoundDocument = Build(model, out logger);
			if (!logger.Errors.Any())
			{
				DocumentAdapter.Save(CurrentDocument, Target, compoundDocument);
				CurrentDocument = compoundDocument;
				return;
			}
			SaveReport(logger);
			throw new DocumentFormatException(logger.Errors);
		}

		public void SaveDelta(TModel model)
		{
			if (!Saved)
			{
				throw new InvalidOperationException("文件未保存，请调用SaveAs方法");
			}
			OnSaving(model.Document, false);
			if (!SupportsDelta || !TrySaveDelta(model, DocumentStateProvider.GetBaseModel()))
			{
				IStorageLogger logger;
				CompoundDocument compoundDocument = Build(model, out logger);
				if (logger.Errors.Any())
				{
					SaveReport(logger);
					throw new DocumentFormatException(logger.Errors);
				}
				DocumentAdapter.Save(CurrentDocument, Target, compoundDocument);
				CurrentDocument = compoundDocument;
			}
		}

		public void New()
		{
			CurrentDocument = null;
			DocumentInfo = null;
			ReferenceManager.Clear();
			Target = default(TTarget);
		}

		protected virtual void OnSaving(CompoundDocumentSaveInfo documentInfo, bool isNew)
		{
			if (!isNew && DocumentInfo != null)
			{
				documentInfo.Creator = DocumentInfo.Creator;
				documentInfo.CreatedAppVersion = DocumentInfo.CreatedAppVersion;
				documentInfo.CreatedDateTime = DocumentInfo.CreatedDateTime;
				documentInfo.CreatedDocumentVersion = DocumentInfo.CreatedDocumentVersion;
			}
		}

		private void SaveReport(IStorageLogger logger)
		{
			if (Setting.ReportEnable)
			{
				logger.SaveReport(Setting.ReportPath);
			}
		}

		protected CompoundDocument Build(TModel model, out IStorageLogger logger)
		{
			return DocumentBuilder.Build(model, _parserProvider, _referenceProvider, _fileProvider, Setting.CachePath, out logger);
		}

		private bool TrySaveDelta(TModel model, TModel originModel)
		{
			CompoundDocument document;
			if (TrySaveDeltaInner(model, originModel, out document))
			{
				DocumentAdapter.Save(CurrentDocument, Target, document);
				CurrentDocument = document;
				return true;
			}
			return false;
		}

		private bool TrySaveDeltaAs(TModel model, TModel originModel, TTarget target)
		{
			CompoundDocument document;
			if (TrySaveDeltaInner(model, originModel, out document))
			{
				CurrentDocument = document;
				DocumentAdapter.Save(CurrentDocument, Target);
				return true;
			}
			return false;
		}

		private bool TrySaveDeltaInner(TModel model, TModel originModel, out CompoundDocument document)
		{
			document = null;
			if (model == null || originModel == null)
			{
				return false;
			}
			if (DocumentStateProvider.BaseDocument == null)
			{
				return false;
			}
			ValueTuple<StorableNode, List<MetadataSaveInfo>> valueTuple = DeltaSaveUtils.BuildUpdatedNode(model, originModel, DocumentStateProvider.BaseDocument, _parserProvider, _referenceProvider, _fileProvider);
			StorableNode item = valueTuple.Item1;
			List<MetadataSaveInfo> item2 = valueTuple.Item2;
			if (item == null)
			{
				return false;
			}
			document = DocumentBuilder.BuildDocumentFromStorableNode(model, item, _parserProvider, _referenceProvider, _fileProvider, Setting.CachePath, item2);
			if (document != null)
			{
				return true;
			}
			return false;
		}
	}
}
