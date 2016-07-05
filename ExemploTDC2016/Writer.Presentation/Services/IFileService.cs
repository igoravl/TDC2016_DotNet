using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Waf.Writer.Presentation.Documents;
using System.Windows.Input;
using System.Waf.Applications;

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
