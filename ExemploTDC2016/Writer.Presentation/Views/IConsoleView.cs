using System.Waf.Applications;

namespace Waf.Writer.Presentation.Views
{
    public interface IConsoleView : IView
    {
        void ScrollToEnd();
    }
}