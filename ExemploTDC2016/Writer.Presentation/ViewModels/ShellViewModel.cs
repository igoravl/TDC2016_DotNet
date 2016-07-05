using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Waf.Applications;
using System.Waf.Applications.Services;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;
using Waf.Writer.Presentation.Properties;
using Waf.Writer.Presentation.Services;
using Waf.Writer.Presentation.Views;

namespace Waf.Writer.Presentation.ViewModels
{
    [Export]
    public class ShellViewModel : ViewModel<IShellView>
    {
        private readonly DelegateCommand aboutCommand;
        private readonly DelegateCommand englishCommand;
        private readonly DelegateCommand germanCommand;
        private readonly IMessageService messageService;
        private readonly IPowerShellService _powerShellService;
        private readonly ICommand showConsoleCommand;
        private ICommand closePrintPreviewCommand;
        private double consoleHeight;
        private object consoleView;
        private object contentView;
        private ICommand exitCommand;
        private bool isConsoleVisible;
        private bool isPrintPreviewVisible;
        private ICommand printCommand;
        private ICommand editMacrosCommand;
        private ICommand printPreviewCommand;
        private ObservableCollection<RibbonGroup> ribbonGroups = new ObservableCollection<RibbonGroup>();

        [ImportingConstructor]
        public ShellViewModel(IShellView view, IMessageService messageService, IPresentationService presentationService,
            IShellService shellService, IFileService fileService)
            : base(view)
        {
            this.messageService = messageService;
            ShellService = shellService;
            FileService = fileService;
            englishCommand = new DelegateCommand(() => SelectLanguage(new CultureInfo("en-US")));
            germanCommand = new DelegateCommand(() => SelectLanguage(new CultureInfo("de-DE")));
            aboutCommand = new DelegateCommand(ShowAboutMessage);
            editMacrosCommand = new DelegateCommand(EditMacros);

            showConsoleCommand = new DelegateCommand(ShowConsole);

            view.Closing += ViewClosing;
            view.Closed += ViewClosed;

            // Restore the window size when the values are valid.
            if (Settings.Default.Left >= 0 && Settings.Default.Top >= 0 && Settings.Default.Width > 0 &&
                Settings.Default.Height > 0
                && Settings.Default.Left + Settings.Default.Width <= presentationService.VirtualScreenWidth
                && Settings.Default.Top + Settings.Default.Height <= presentationService.VirtualScreenHeight)
            {
                ViewCore.Left = Settings.Default.Left;
                ViewCore.Top = Settings.Default.Top;
                ViewCore.Height = Settings.Default.Height;
                ViewCore.Width = Settings.Default.Width;
            }
            ViewCore.IsMaximized = Settings.Default.IsMaximized;

            IsConsoleVisible = Settings.Default.ConsoleVisible;

            ConsoleHeight = Settings.Default.ConsoleHeight;
        }

        private void EditMacros()
        {
            const string isePath = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell_ise.exe";

            var exeDir = AppDomain.CurrentDomain.BaseDirectory;
            var scriptsDir = Path.Combine(exeDir, "scripts");
            var scripts = string.Join(",", Directory.GetFiles(scriptsDir, "*.ps1"));

            Process.Start(isePath, $"-file {scripts}");
        }


        public string Title
        {
            get { return ApplicationInfo.ProductName; }
        }

        public IShellService ShellService { get; }

        public IFileService FileService { get; }

        public CultureInfo NewLanguage { get; private set; }

        public ICommand EnglishCommand
        {
            get { return englishCommand; }
        }

        public ICommand GermanCommand
        {
            get { return germanCommand; }
        }

        public ICommand AboutCommand
        {
            get { return aboutCommand; }
        }

        public ICommand PrintPreviewCommand
        {
            get { return printPreviewCommand; }
            set { SetProperty(ref printPreviewCommand, value); }
        }

        public ICommand ClosePrintPreviewCommand
        {
            get { return closePrintPreviewCommand; }
            set { SetProperty(ref closePrintPreviewCommand, value); }
        }

        public ICommand PrintCommand
        {
            get { return printCommand; }
            set { SetProperty(ref printCommand, value); }
        }

        public ICommand ExitCommand
        {
            get { return exitCommand; }
            set { SetProperty(ref exitCommand, value); }
        }

        public object ContentView
        {
            get { return contentView; }
            set { SetProperty(ref contentView, value); }
        }

        public object ConsoleView
        {
            get { return consoleView; }
            set { SetProperty(ref consoleView, value); }
        }

        public bool IsPrintPreviewVisible
        {
            get { return isPrintPreviewVisible; }
            set { SetProperty(ref isPrintPreviewVisible, value); }
        }

        public ObservableCollection<RibbonGroup> RibbonGroups
        {
            get { return ribbonGroups; }
            set { SetProperty(ref ribbonGroups, value); }
        }

        public bool IsConsoleVisible
        {
            get { return isConsoleVisible; }
            set { SetProperty(ref isConsoleVisible, value); }
        }

        public double ConsoleHeight
        {
            get { return consoleHeight; }
            set { SetProperty(ref consoleHeight, value); }
        }

        public ICommand EditMacrosCommand
        {
            get { return editMacrosCommand; }
            set { editMacrosCommand = value; }
        }

        public event CancelEventHandler Closing;


        public void Show()
        {
            ViewCore.Show();
        }

        public void Close()
        {
            ViewCore.Close();
        }

        private void SelectLanguage(CultureInfo uiCulture)
        {
            if (!uiCulture.Equals(CultureInfo.CurrentUICulture))
            {
                messageService.ShowMessage(ShellService.ShellView, Resources.RestartApplication + "\n\n" +
                                                                   Resources.ResourceManager.GetString(
                                                                       "RestartApplication", uiCulture));
            }
            NewLanguage = uiCulture;
        }

        private void ShowAboutMessage()
        {
            messageService.ShowMessage(ShellService.ShellView,
                string.Format(CultureInfo.CurrentCulture, Resources.AboutText,
                    ApplicationInfo.ProductName, ApplicationInfo.Version));
        }

        private void ShowConsole()
        {
        }

        protected virtual void OnClosing(CancelEventArgs e)
        {
            if (Closing != null)
            {
                Closing(this, e);
            }
        }

        private void ViewClosing(object sender, CancelEventArgs e)
        {
            OnClosing(e);
        }

        private void ViewClosed(object sender, EventArgs e)
        {
            Settings.Default.Left = ViewCore.Left;
            Settings.Default.Top = ViewCore.Top;
            Settings.Default.Height = ViewCore.Height;
            Settings.Default.Width = ViewCore.Width;
            Settings.Default.IsMaximized = ViewCore.IsMaximized;
            Settings.Default.ConsoleVisible = IsConsoleVisible;
            Settings.Default.ConsoleHeight = ConsoleHeight;
        }
    }
}