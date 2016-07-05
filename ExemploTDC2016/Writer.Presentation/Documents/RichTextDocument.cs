using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace Waf.Writer.Presentation.Documents
{
    public class RichTextDocument : Document
    {
        public RichTextDocument(RichTextDocumentType documentType) : this(documentType, new FlowDocument())
        {
        }

        public RichTextDocument(RichTextDocumentType documentType, FlowDocument content) : base(documentType)
        {
            Content = content;
        }


        public FlowDocument Content { get; }


        public FlowDocument CloneContent()
        {
            var clone = new FlowDocument();

            using (var stream = new MemoryStream())
            {
                var source = new TextRange(Content.ContentStart, Content.ContentEnd);
                source.Save(stream, DataFormats.Xaml);
                var target = new TextRange(clone.ContentStart, clone.ContentEnd);
                target.Load(stream, DataFormats.Xaml);
            }

            return clone;
        }
    }
}