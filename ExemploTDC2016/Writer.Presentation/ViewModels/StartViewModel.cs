using System.ComponentModel.Composition;
using System.Waf.Applications;
using Waf.Writer.Presentation.Services;
using Waf.Writer.Presentation.Views;

namespace Waf.Writer.Presentation.ViewModels
{
    [Export]
    public class StartViewModel : ViewModel<IStartView>
    {
        [ImportingConstructor]
        public StartViewModel(IStartView view, IFileService fileService) : base(view)
        {
            FileService = fileService;
        }


        public IFileService FileService { get; }
    }
}