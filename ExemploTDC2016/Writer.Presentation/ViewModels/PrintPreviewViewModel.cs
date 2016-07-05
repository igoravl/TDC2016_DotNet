using System.ComponentModel.Composition;
using System.Windows.Documents;
using Waf.Writer.Presentation.Services;
using Waf.Writer.Presentation.Views;

namespace Waf.Writer.Presentation.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class PrintPreviewViewModel : ZoomViewModel<IPrintPreviewView>
    {
        private IDocumentPaginatorSource document;
        
        
        [ImportingConstructor]
        public PrintPreviewViewModel(IPrintPreviewView view, IShellService shellService)
            : base(view, shellService)
        {
        }


        public IDocumentPaginatorSource Document
        {
            get { return document; }
            set { SetProperty(ref document, value); }
        }


        protected override void FitToWidthCore()
        {
            base.FitToWidthCore();
            ViewCore.FitToWidth();
        }
    }
}

