using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.Packaging;
using System.Waf.Applications;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;
using Waf.Writer.Presentation.Documents;
using Waf.Writer.Presentation.Services;
using Waf.Writer.Presentation.ViewModels;

namespace Waf.Writer.Presentation.Controllers
{
    /// <summary>
    ///     Responsible for the print related commands and the PrintPreview.
    /// </summary>
    [Export]
    internal class PrintController
    {
        private const string PackagePath = "pack://temp.xps";
        private readonly DelegateCommand closePrintPreviewCommand;

        private readonly IFileService fileService;
        private readonly DelegateCommand printCommand;
        private readonly IPrintDialogService printDialogService;
        private readonly DelegateCommand printPreviewCommand;
        private readonly ExportFactory<PrintPreviewViewModel> printPreviewViewModelFactory;
        private readonly ShellViewModel shellViewModel;
        private Package package;
        private object previousView;
        private XpsDocument xpsDocument;


        [ImportingConstructor]
        public PrintController(IFileService fileService, IPrintDialogService printDialogService,
            ShellViewModel shellViewModel, ExportFactory<PrintPreviewViewModel> printPreviewViewModelFactory)
        {
            this.fileService = fileService;
            this.printDialogService = printDialogService;
            this.shellViewModel = shellViewModel;
            this.printPreviewViewModelFactory = printPreviewViewModelFactory;
            printPreviewCommand = new DelegateCommand(ShowPrintPreview, CanShowPrintPreview);
            printCommand = new DelegateCommand(PrintDocument, CanPrintDocument);
            closePrintPreviewCommand = new DelegateCommand(ClosePrintPreview);

            PropertyChangedEventManager.AddHandler(fileService, FileServicePropertyChanged, "");
        }


        public void Initialize()
        {
            shellViewModel.PrintPreviewCommand = printPreviewCommand;
            shellViewModel.ClosePrintPreviewCommand = closePrintPreviewCommand;
            shellViewModel.PrintCommand = printCommand;
        }

        public void Shutdown()
        {
            if (xpsDocument != null)
            {
                xpsDocument.Close();
            }
        }

        private bool CanShowPrintPreview()
        {
            return fileService.ActiveDocument != null && !shellViewModel.IsPrintPreviewVisible;
        }

        private void ShowPrintPreview()
        {
            // We have to clone the FlowDocument before we use different pagination settings for the export.        
            var document = fileService.ActiveDocument as RichTextDocument;
            var clone = document.CloneContent();
            clone.ColumnWidth = double.PositiveInfinity;

            // Create a package for the XPS document
            var packageStream = new MemoryStream();
            package = Package.Open(packageStream, FileMode.Create, FileAccess.ReadWrite);

            // Create a XPS document with the path "pack://temp.xps"
            PackageStore.AddPackage(new Uri(PackagePath), package);
            xpsDocument = new XpsDocument(package, CompressionOption.SuperFast, PackagePath);

            // Serialize the XPS document
            var serializer = new XpsSerializationManager(new XpsPackagingPolicy(xpsDocument), false);
            var paginator = ((IDocumentPaginatorSource) clone).DocumentPaginator;
            serializer.SaveAsXaml(paginator);

            // Get the fixed document sequence
            var documentSequence = xpsDocument.GetFixedDocumentSequence();

            // Create and show the print preview view
            var printPreviewViewModel = printPreviewViewModelFactory.CreateExport().Value;
            printPreviewViewModel.Document = documentSequence;
            previousView = shellViewModel.ContentView;
            shellViewModel.ContentView = printPreviewViewModel.View;
            shellViewModel.IsPrintPreviewVisible = true;
            printPreviewCommand.RaiseCanExecuteChanged();
        }

        private bool CanPrintDocument()
        {
            return fileService.ActiveDocument != null;
        }

        private void PrintDocument()
        {
            if (printDialogService.ShowDialog())
            {
                // We have to clone the FlowDocument before we use different pagination settings for the export.        
                var document = (RichTextDocument) fileService.ActiveDocument;
                var clone = document.CloneContent();

                printDialogService.PrintDocument(((IDocumentPaginatorSource) clone).DocumentPaginator,
                    fileService.ActiveDocument.FileName);
            }
        }

        private void ClosePrintPreview()
        {
            // Remove the package from the store
            PackageStore.RemovePackage(new Uri(PackagePath));

            xpsDocument.Close();
            package.Close();

            shellViewModel.ContentView = previousView;
            previousView = null;
            shellViewModel.IsPrintPreviewVisible = false;
            printPreviewCommand.RaiseCanExecuteChanged();
        }

        private void FileServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ActiveDocument")
            {
                printPreviewCommand.RaiseCanExecuteChanged();
                printCommand.RaiseCanExecuteChanged();
            }
        }
    }
}