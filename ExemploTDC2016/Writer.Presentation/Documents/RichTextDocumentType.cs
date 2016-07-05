using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using Waf.Writer.Presentation.Properties;

namespace Waf.Writer.Presentation.Documents
{
    public class RichTextDocumentType : DocumentType
    {
        private int documentCount;


        public RichTextDocumentType() : base(Resources.RichTextDocuments, ".rtf")
        {
        }


        public override bool CanNew()
        {
            return true;
        }

        public override bool CanOpen()
        {
            return true;
        }

        public override bool CanSave(IDocument document)
        {
            return document is RichTextDocument;
        }

        protected override IDocument NewCore()
        {
            var document = new RichTextDocument(this);
            document.FileName = string.Format(CultureInfo.CurrentCulture, Resources.DocumentFileName,
                ++documentCount, FileExtension);
            return document;
        }

        protected override IDocument OpenCore(string fileName)
        {
            var flowDocument = new FlowDocument();
            var range = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
            using (var stream = new FileStream(fileName, FileMode.Open))
            {
                range.Load(stream, DataFormats.Rtf);
            }

            var document = new RichTextDocument(this, flowDocument);
            documentCount++;
            return document;
        }

        protected override void SaveCore(IDocument document, string fileName)
        {
            var flowDocument = ((RichTextDocument) document).Content;
            var range = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                range.Save(stream, DataFormats.Rtf);
            }
        }
    }
}