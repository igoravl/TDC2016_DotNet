using System.Waf.Applications;

namespace Waf.Writer.Presentation.Views
{
    public interface ISaveChangesView : IView
    {
        void ShowDialog(object owner);

        void Close();
    }
}