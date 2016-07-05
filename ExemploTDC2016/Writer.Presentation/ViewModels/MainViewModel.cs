﻿using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Waf.Applications;
using Waf.Writer.Presentation.Documents;
using Waf.Writer.Presentation.Services;
using Waf.Writer.Presentation.Views;

namespace Waf.Writer.Presentation.ViewModels
{
    [Export]
    public class MainViewModel : ViewModel<IMainView>
    {
        private readonly IShellService shellService;
        private IDocument activeDocument;
        private object activeDocumentView;
        private object startView;


        [ImportingConstructor]
        public MainViewModel(IMainView view, IShellService shellService, IFileService fileService)
            : base(view)
        {
            this.shellService = shellService;
            FileService = fileService;
            DocumentViews = new ObservableCollection<object>();

            CollectionChangedEventManager.AddHandler(DocumentViews, DocumentViewsCollectionChanged);
            PropertyChangedEventManager.AddHandler(fileService, FileServicePropertyChanged, "");
        }


        public IFileService FileService { get; }

        public object StartView
        {
            get { return startView; }
            set { SetProperty(ref startView, value); }
        }

        public ObservableCollection<object> DocumentViews { get; }

        public object ActiveDocumentView
        {
            get { return activeDocumentView; }
            set { SetProperty(ref activeDocumentView, value); }
        }


        private void DocumentViewsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!DocumentViews.Any())
            {
                ViewCore.ContentViewState = ContentViewState.StartViewVisible;
            }
            else
            {
                ViewCore.ContentViewState = ContentViewState.DocumentViewVisible;
            }
        }

        private void FileServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActiveDocument")
            {
                if (activeDocument != null)
                {
                    PropertyChangedEventManager.RemoveHandler(activeDocument, ActiveDocumentPropertyChanged, "");
                }

                activeDocument = FileService.ActiveDocument;

                if (activeDocument != null)
                {
                    PropertyChangedEventManager.AddHandler(activeDocument, ActiveDocumentPropertyChanged, "");
                }

                UpdateShellServiceDocumentName();
            }
        }

        private void ActiveDocumentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FileName")
            {
                UpdateShellServiceDocumentName();
            }
        }

        private void UpdateShellServiceDocumentName()
        {
            if (FileService.ActiveDocument != null)
            {
                shellService.DocumentName = Path.GetFileName(FileService.ActiveDocument.FileName);
            }
            else
            {
                shellService.DocumentName = null;
            }
        }
    }
}