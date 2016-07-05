using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Waf.Writer.Presentation.ViewModels;

namespace Waf.Writer.Presentation.Services
{
    [Export(typeof(IPowerShellService))]
    public class PowerShellService : IPowerShellService
    {
        private const string InitScript = @"
Function Init($viewModel)
{
    Add-Type -AssemblyName PresentationFramework
    Add-Type -AssemblyName System.Windows.Forms
    Add-Type -AssemblyName System.Windows.Controls.Ribbon
    Add-Type -AssemblyName System.Xaml
    Add-Type -AssemblyName WindowsBase

    $global:App = $viewModel
} ";
        private PowerShell _powerShellHost;
        private readonly ShellViewModel _shellViewModel;
        private bool _isInitialized;

        public event EventHandler<TextOutputtedEventArgs> WarningOutputted;
        public event EventHandler<TextOutputtedEventArgs> ErrorOutputted;
        public event EventHandler<TextOutputtedEventArgs> VerboseOutputted;

        [ImportingConstructor]
        public PowerShellService(ShellViewModel shellViewModel)
        {
            _shellViewModel = shellViewModel;
        }

        public void Initialize()
        {
            LoadPowerShell();
            RunAutoExec();

            _isInitialized = true;
        }

        public void Reload()
        {
            if (_isInitialized)
            {
                LoadPowerShell();
            }
            else
            {
                Initialize();
            }
        }

        private void LoadPowerShell()
        {
            var iss = InitialSessionState.CreateDefault();
            iss.ThreadOptions = PSThreadOptions.UseCurrentThread;
            iss.ApartmentState = ApartmentState.STA;

            var rs = RunspaceFactory.CreateRunspace(iss);
            rs.Open();

            Runspace.DefaultRunspace = rs;

            _powerShellHost = PowerShell.Create();
            _powerShellHost.Runspace = rs;
            _powerShellHost.AddScript(InitScript);
            _powerShellHost.Invoke();
            _powerShellHost.Streams.Warning.DataAdded += Warning_DataAdded;
            _powerShellHost.Streams.Error.DataAdded += Error_DataAdded;
            _powerShellHost.Streams.Verbose.DataAdded += Verbose_DataAdded;

            _powerShellHost.AddCommand("Init").AddParameter("viewModel", _shellViewModel);
            _powerShellHost.Invoke();

            try
            {
                var exeDir = AppDomain.CurrentDomain.BaseDirectory;
                var scriptsDir = Path.Combine(exeDir, "scripts");

                foreach (var scriptFile in Directory.GetFiles(scriptsDir, "*.ps1"))
                {
                    var contents = File.ReadAllText(scriptFile);
                    _powerShellHost.AddScript(contents);
                }

                _powerShellHost.Invoke();
            }
            finally
            {
                _powerShellHost.Commands.Clear();
            }
        }

        public void RunAutoExec()
        {
            try
            {
                _powerShellHost.AddScript("Get-Command AutoExec");
                var cmd = _powerShellHost.Invoke();

                if (cmd.Count <= 0) return;

                _powerShellHost.AddCommand("AutoExec");
                _powerShellHost.Invoke();
            }
            finally 
            {
                _powerShellHost.Commands.Clear();
            }
        }

        private void Error_DataAdded(object sender, DataAddedEventArgs e)
        {
            var data = ((PSDataCollection<ErrorRecord>)sender)[e.Index];
            ErrorOutputted?.Invoke(this, new TextOutputtedEventArgs(data.ToString()));
        }

        private void Warning_DataAdded(object sender, DataAddedEventArgs e)
        {
            var data = ((PSDataCollection<WarningRecord>) sender)[e.Index];
            WarningOutputted?.Invoke(this, new TextOutputtedEventArgs(data.ToString()));
        }

        private void Verbose_DataAdded(object sender, DataAddedEventArgs e)
        {
            var data = ((PSDataCollection<VerboseRecord>)sender)[e.Index];
            VerboseOutputted?.Invoke(this, new TextOutputtedEventArgs(data.ToString()));
        }

        public IEnumerable<object> Invoke(string script)
        {
            _powerShellHost.AddScript(script).AddCommand("Out-String");
            var result = _powerShellHost.Invoke();
            _powerShellHost.Commands.Clear();

            return result;
        }
    }
}
