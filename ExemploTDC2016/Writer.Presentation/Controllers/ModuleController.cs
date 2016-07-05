using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Threading;
using System.Waf.Applications;
using Waf.Writer.Presentation.Properties;
using Waf.Writer.Presentation.Services;
using Waf.Writer.Presentation.ViewModels;

namespace Waf.Writer.Presentation.Controllers
{
    /// <summary>
    ///     Responsible for the application lifecycle.
    /// </summary>
    [Export(typeof(IModuleController)), Export]
    internal class ModuleController : IModuleController
    {
        private readonly ConsoleViewModel _consoleViewModel;
        private readonly IEnvironmentService _environmentService;
        private readonly DelegateCommand _exitCommand;
        private readonly FileController _fileController;
        private readonly MainViewModel _mainViewModel;
        private readonly PrintController _printController;
        private readonly ShellViewModel _shellViewModel;
        private readonly StartViewModel _startViewModel;

        [ImportingConstructor]
        public ModuleController(IEnvironmentService environmentService, IPresentationService presentationService,
            ShellService shellService,
            Lazy<FileController> fileController, Lazy<RichTextDocumentController> richTextDocumentController,
            Lazy<PrintController> printController,
            Lazy<ShellViewModel> shellViewModel, Lazy<MainViewModel> mainViewModel, Lazy<StartViewModel> startViewModel,
            Lazy<IPowerShellService> powerShellService,
            Lazy<ConsoleViewModel> consoleViewModel)
        {
            // Upgrade the settings from a previous version when the new version starts the first time.
            if (Settings.Default.IsUpgradeNeeded)
            {
                Settings.Default.Upgrade();
                Settings.Default.IsUpgradeNeeded = false;
            }

            // Initializing the cultures must be done first. Therefore, we inject the Controllers and ViewModels lazy.
            InitializeCultures();
            presentationService.InitializeCultures();

            _environmentService = environmentService;
            _fileController = fileController.Value;
            //this._richTextDocumentController = richTextDocumentController.Value;
            _printController = printController.Value;
            _shellViewModel = shellViewModel.Value;
            _mainViewModel = mainViewModel.Value;
            _startViewModel = startViewModel.Value;
            _consoleViewModel = consoleViewModel.Value;

            shellService.ShellView = _shellViewModel.View;
            _shellViewModel.Closing += ShellViewModelClosing;
            _exitCommand = new DelegateCommand(Close);

            powerShellService.Value.Initialize();
        }

        public void Initialize()
        {
            _shellViewModel.ExitCommand = _exitCommand;
            _mainViewModel.StartView = _startViewModel.View;

            _printController.Initialize();
            _fileController.Initialize();
        }

        public void Run()
        {
            _shellViewModel.ContentView = _mainViewModel.View;
            _shellViewModel.ConsoleView = _consoleViewModel.View;

            if (!string.IsNullOrEmpty(_environmentService.DocumentFileName))
            {
                _fileController.Open(_environmentService.DocumentFileName);
            }

            _shellViewModel.Show();
        }

        public void Shutdown()
        {
            _fileController.Shutdown();
            _printController.Shutdown();

            if (_shellViewModel.NewLanguage != null)
            {
                Settings.Default.UICulture = _shellViewModel.NewLanguage.Name;
            }
            try
            {
                Settings.Default.Save();
            }
            catch (Exception)
            {
                // When more application instances are closed at the same time then an exception occurs.
            }
        }

        private static void InitializeCultures()
        {
            if (!string.IsNullOrEmpty(Settings.Default.Culture))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Settings.Default.Culture);
            }
            if (!string.IsNullOrEmpty(Settings.Default.UICulture))
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.UICulture);
            }
        }

        private void Close()
        {
            _shellViewModel.Close();
        }

        private void ShellViewModelClosing(object sender, CancelEventArgs e)
        {
            // Try to close all documents and see if the user has already saved them.
            e.Cancel = !_fileController.CloseAll();
        }
    }
}