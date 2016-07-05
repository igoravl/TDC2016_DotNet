using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.Writer.Presentation.Documents;

namespace Waf.Writer.Presentation.Services
{
    public interface IFileService : INotifyPropertyChanged
    {
        ReadOnlyObservableCollection<IDocument> Documents { get; }

        IDocument ActiveDocument { get; set; }

        RecentFileList RecentFileList { get; }

        ICommand NewCommand { get; }

        ICommand OpenCommand { get; }

        ICommand CloseCommand { get; }

        ICommand SaveCommand { get; }

        ICommand SaveAsCommand { get; }
    }
}