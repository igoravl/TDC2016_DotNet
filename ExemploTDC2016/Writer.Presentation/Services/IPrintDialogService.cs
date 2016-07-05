using System.Windows.Documents;

namespace Waf.Writer.Presentation.Services
{
    public interface IPrintDialogService
    {
        bool ShowDialog();

        void PrintDocument(DocumentPaginator documentPaginator, string description);
    }
}