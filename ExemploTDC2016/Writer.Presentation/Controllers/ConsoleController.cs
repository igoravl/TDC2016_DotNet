using System.ComponentModel.Composition;
using Waf.Writer.Presentation.ViewModels;

namespace Waf.Writer.Presentation.Controllers
{
    [Export]
    public class ConsoleController
    {
        private readonly ConsoleViewModel _consoleViewModel;
        private readonly ShellViewModel _shellViewModel;

        [ImportingConstructor]
        public ConsoleController(ShellViewModel shellViewModel, ConsoleViewModel consoleViewModel)
        {
            _shellViewModel = shellViewModel;
            _consoleViewModel = consoleViewModel;
        }
    }
}