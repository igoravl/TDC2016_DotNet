using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows.Documents;

namespace Waf.Writer.Presentation.Views
{
    public interface IConsoleView: IView
    {
        void ScrollToEnd();
    }
}
