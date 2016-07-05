using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Waf.Applications;
using Waf.Writer.Presentation.ViewModels;

namespace Waf.Writer.Presentation.Controllers
{
    [Export]
    public class ConsoleController
    {
        private readonly ShellViewModel _shellViewModel;
        private readonly ConsoleViewModel _consoleViewModel;
        [ImportingConstructor]
        public ConsoleController(ShellViewModel shellViewModel, ConsoleViewModel consoleViewModel)
        {
            _shellViewModel = shellViewModel;
            _consoleViewModel = consoleViewModel;
        }

    }
}
