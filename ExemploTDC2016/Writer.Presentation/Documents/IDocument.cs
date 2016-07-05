using System.ComponentModel;

namespace Waf.Writer.Presentation.Documents
{
    public interface IDocument : INotifyPropertyChanged
    {
        IDocumentType DocumentType { get; }

        string FileName { get; set; }

        bool Modified { get; set; }
    }
}