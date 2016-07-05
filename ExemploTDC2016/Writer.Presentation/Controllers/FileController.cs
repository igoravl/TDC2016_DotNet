﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Waf.Applications;
using System.Waf.Applications.Services;
using Waf.Writer.Presentation.Documents;
using Waf.Writer.Presentation.Properties;
using Waf.Writer.Presentation.Services;
using Waf.Writer.Presentation.ViewModels;

namespace Waf.Writer.Presentation.Controllers
{
    /// <summary>
    ///     Responsible for the file related commands.
    /// </summary>
    [Export]
    internal class FileController
    {
        private readonly DelegateCommand closeCommand;
        private readonly List<IDocumentType> documentTypes;
        private readonly IFileDialogService fileDialogService;
        private readonly FileService fileService;
        private readonly IMessageService messageService;
        private readonly DelegateCommand newCommand;
        private readonly DelegateCommand openCommand;
        private readonly RecentFileList recentFileList;
        private readonly DelegateCommand saveAsCommand;
        private readonly ExportFactory<SaveChangesViewModel> saveChangesViewModelFactory;
        private readonly DelegateCommand saveCommand;
        private readonly IShellService shellService;
        private IDocument lastActiveDocument;


        [ImportingConstructor]
        public FileController(IMessageService messageService, IFileDialogService fileDialogService,
            IShellService shellService,
            FileService fileService, ExportFactory<SaveChangesViewModel> saveChangesViewModelFactory)
        {
            this.messageService = messageService;
            this.fileDialogService = fileDialogService;
            this.shellService = shellService;
            this.fileService = fileService;
            this.saveChangesViewModelFactory = saveChangesViewModelFactory;
            documentTypes = new List<IDocumentType>();
            newCommand = new DelegateCommand(NewCommand);
            openCommand = new DelegateCommand(OpenCommand);
            closeCommand = new DelegateCommand(CloseCommand, CanCloseCommand);
            saveCommand = new DelegateCommand(SaveCommand, CanSaveCommand);
            saveAsCommand = new DelegateCommand(SaveAsCommand, CanSaveAsCommand);

            this.fileService.NewCommand = newCommand;
            this.fileService.OpenCommand = openCommand;
            this.fileService.CloseCommand = closeCommand;
            this.fileService.SaveCommand = saveCommand;
            this.fileService.SaveAsCommand = saveAsCommand;

            recentFileList = Settings.Default.RecentFileList;
            if (recentFileList == null)
            {
                recentFileList = new RecentFileList();
            }
            this.fileService.RecentFileList = recentFileList;

            PropertyChangedEventManager.AddHandler(fileService, FileServicePropertyChanged, "");
        }


        private ReadOnlyObservableCollection<IDocument> Documents
        {
            get { return fileService.Documents; }
        }

        private IDocument ActiveDocument
        {
            get { return fileService.ActiveDocument; }
            set { fileService.ActiveDocument = value; }
        }


        public void Initialize()
        {
            documentTypes.Add(new RichTextDocumentType());
            documentTypes.Add(new XpsExportDocumentType());
        }

        internal void Register(DocumentType documentType)
        {
            documentTypes.Add(documentType);
        }

        public void Shutdown()
        {
            Settings.Default.RecentFileList = recentFileList;
        }


        public IDocument Open(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("The argument fileName must not be null or empty.");
            }
            return OpenCore(fileName);
        }

        public bool CloseAll()
        {
            if (!CanDocumentsClose(Documents))
            {
                return false;
            }

            ActiveDocument = null;
            while (Documents.Any())
            {
                fileService.RemoveDocument(Documents.First());
            }
            return true;
        }

        private void NewCommand()
        {
            New(documentTypes.First());
        }

        private void OpenCommand(object commandParameter)
        {
            var fileName = commandParameter as string;
            if (!string.IsNullOrEmpty(fileName))
            {
                Open(fileName);
            }
            else
            {
                Open();
            }
        }

        private bool CanCloseCommand()
        {
            return ActiveDocument != null;
        }

        private void CloseCommand()
        {
            Close(ActiveDocument);
        }

        private bool CanSaveCommand()
        {
            return ActiveDocument != null && ActiveDocument.Modified;
        }

        private void SaveCommand()
        {
            Save(ActiveDocument);
        }

        private bool CanSaveAsCommand()
        {
            return ActiveDocument != null;
        }

        private void SaveAsCommand()
        {
            SaveAs(ActiveDocument);
        }

        private void FileServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActiveDocument")
            {
                if (lastActiveDocument != null)
                {
                    PropertyChangedEventManager.RemoveHandler(lastActiveDocument, ActiveDocumentPropertyChanged, "");
                }

                lastActiveDocument = fileService.ActiveDocument;

                if (lastActiveDocument != null)
                {
                    PropertyChangedEventManager.AddHandler(lastActiveDocument, ActiveDocumentPropertyChanged, "");
                }

                UpdateCommands();
            }
        }

        private void ActiveDocumentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Modified")
            {
                UpdateCommands();
            }
        }

        private void UpdateCommands()
        {
            closeCommand.RaiseCanExecuteChanged();
            saveCommand.RaiseCanExecuteChanged();
            saveAsCommand.RaiseCanExecuteChanged();
        }


        internal IDocument New(IDocumentType documentType)
        {
            if (documentType == null)
            {
                throw new ArgumentNullException("documentType");
            }
            if (!documentTypes.Contains(documentType))
            {
                throw new ArgumentException("documentType is not an item of the DocumentTypes collection.");
            }
            var document = documentType.New();
            fileService.AddDocument(document);
            ActiveDocument = document;
            return document;
        }

        private IDocument Open()
        {
            var fileTypes = (from d in documentTypes
                where d.CanOpen()
                select new FileType(d.Description, d.FileExtension)
                ).ToArray();
            if (!fileTypes.Any())
            {
                throw new InvalidOperationException("No DocumentType is registered that supports the Open operation.");
            }

            var result = fileDialogService.ShowOpenFileDialog(shellService.ShellView, fileTypes);
            if (result.IsValid)
            {
                return OpenCore(result.FileName, result.SelectedFileType);
            }
            return null;
        }

        private void Save(IDocument document)
        {
            if (Path.IsPathRooted(document.FileName))
            {
                var saveTypes = documentTypes.Where(d => d.CanSave(document)).ToArray();
                var documentType =
                    saveTypes.First(d => d.FileExtension == Path.GetExtension(document.FileName));
                SaveCore(documentType, document, document.FileName);
            }
            else
            {
                SaveAs(document);
            }
        }

        private void SaveAs(IDocument document)
        {
            var fileTypes = (from d in documentTypes
                where d.CanSave(document)
                select new FileType(d.Description, d.FileExtension)
                ).ToArray();
            if (!fileTypes.Any())
            {
                throw new InvalidOperationException("No DocumentType is registered that supports the Save operation.");
            }

            FileType selectedFileType;
            if (File.Exists(document.FileName))
            {
                var saveTypes = documentTypes.Where(d => d.CanSave(document)).ToArray();
                var documentType =
                    saveTypes.First(d => d.FileExtension == Path.GetExtension(document.FileName));
                selectedFileType = fileTypes.Where(
                    f => f.Description == documentType.Description && f.FileExtension == documentType.FileExtension)
                    .First();
            }
            else
            {
                selectedFileType = fileTypes.First();
            }
            var fileName = Path.GetFileNameWithoutExtension(document.FileName);

            var result = fileDialogService.ShowSaveFileDialog(shellService.ShellView, fileTypes,
                selectedFileType, fileName);
            if (result.IsValid)
            {
                var documentType = GetDocumentType(result.SelectedFileType);
                SaveCore(documentType, document, result.FileName);
            }
        }

        private void Close(IDocument document)
        {
            if (!CanDocumentsClose(new[] {document}))
            {
                return;
            }

            if (ActiveDocument == document)
            {
                ActiveDocument = null;
            }
            fileService.RemoveDocument(document);
        }

        private bool CanDocumentsClose(IEnumerable<IDocument> documentsToClose)
        {
            var modifiedDocuments = documentsToClose.Where(d => d.Modified).ToArray();
            if (!modifiedDocuments.Any())
            {
                return true;
            }

            // Show the save changes view to the user
            var saveChangesViewModel = saveChangesViewModelFactory.CreateExport().Value;
            saveChangesViewModel.Documents = modifiedDocuments;

            var dialogResult = saveChangesViewModel.ShowDialog(shellService.ShellView);

            if (dialogResult == true)
            {
                foreach (var document in modifiedDocuments)
                {
                    Save(document);
                }
            }

            return dialogResult != null;
        }


        private IDocument OpenCore(string fileName, FileType fileType = null)
        {
            // Check if document is already opened
            var document = Documents.SingleOrDefault(d => d.FileName == fileName);
            if (document == null)
            {
                IDocumentType documentType;
                if (fileType != null)
                {
                    documentType = GetDocumentType(fileType);
                }
                else
                {
                    documentType = documentTypes.FirstOrDefault(dt => dt.FileExtension == Path.GetExtension(fileName));
                    if (documentType == null)
                    {
                        Trace.TraceError(string.Format(CultureInfo.InvariantCulture,
                            "The extension of the file '{0}' is not supported.", fileName));
                        messageService.ShowError(shellService.ShellView,
                            string.Format(CultureInfo.CurrentCulture, Resources.FileExtensionNotSupported, fileName));
                        return null;
                    }
                }

                try
                {
                    document = documentType.Open(fileName);
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                    messageService.ShowError(shellService.ShellView,
                        string.Format(CultureInfo.CurrentCulture, Resources.CannotOpenFile, fileName));
                    if (e is FileNotFoundException)
                    {
                        var recentFile = recentFileList.RecentFiles.FirstOrDefault(x => x.Path == fileName);
                        if (recentFile != null)
                        {
                            recentFileList.Remove(recentFile);
                        }
                    }
                    return null;
                }

                fileService.AddDocument(document);
                recentFileList.AddFile(document.FileName);
            }
            ActiveDocument = document;
            return document;
        }

        private void SaveCore(IDocumentType documentType, IDocument document, string fileName)
        {
            try
            {
                documentType.Save(document, fileName);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                messageService.ShowError(shellService.ShellView,
                    string.Format(CultureInfo.CurrentCulture, Resources.CannotSaveFile, fileName));
            }

            if (documentType.CanOpen())
            {
                recentFileList.AddFile(fileName);
            }
        }

        private IDocumentType GetDocumentType(FileType fileType)
        {
            var documentType = (from d in documentTypes
                where d.Description == fileType.Description
                      && d.FileExtension == fileType.FileExtension
                select d).First();
            return documentType;
        }
    }
}