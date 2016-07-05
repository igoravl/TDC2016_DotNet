using System.Waf.Applications;

namespace Waf.Writer.Presentation.Views
{
    public interface IMainView : IView
    {
        ContentViewState ContentViewState { get; set; }
    }
}