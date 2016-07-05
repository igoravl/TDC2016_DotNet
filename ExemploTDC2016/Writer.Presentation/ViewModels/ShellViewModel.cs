﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
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
        private readonly IMessageService messageService;
        private readonly IShellService shellService;
        private readonly IFileService fileService;
        private readonly DelegateCommand englishCommand;
        private readonly DelegateCommand germanCommand;
        private readonly DelegateCommand aboutCommand;
        private ICommand printPreviewCommand;
        private ICommand closePrintPreviewCommand;
        private ICommand printCommand;
        private ICommand exitCommand;
        private object contentView;
        private object consoleView;
        private bool isPrintPreviewVisible;
        private CultureInfo newLanguage;
        private readonly ICommand showConsoleCommand;
        private ObservableCollection<RibbonGroup> ribbonGroups = new ObservableCollection<RibbonGroup>();
        private bool isConsoleVisible;
        private double consoleHeight;

        [ImportingConstructor]
        public ShellViewModel(IShellView view, IMessageService messageService, IPresentationService presentationService,
            IShellService shellService, IFileService fileService)
            : base(view)
        {
            this.messageService = messageService;
            this.shellService = shellService;
            this.fileService = fileService;
            this.englishCommand = new DelegateCommand(() => SelectLanguage(new CultureInfo("en-US")));
            this.germanCommand = new DelegateCommand(() => SelectLanguage(new CultureInfo("de-DE")));
            this.aboutCommand = new DelegateCommand(ShowAboutMessage);

            this.showConsoleCommand = new DelegateCommand(ShowConsole);

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

        public string Title
        {
            get { return ApplicationInfo.ProductName; }
        }

        public IShellService ShellService
        {
            get { return shellService; }
        }

        public IFileService FileService
        {
            get { return fileService; }
        }

        public CultureInfo NewLanguage
        {
            get { return newLanguage; }
        }

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
                messageService.ShowMessage(shellService.ShellView, Resources.RestartApplication + "\n\n" +
                                                                   Resources.ResourceManager.GetString(
                                                                       "RestartApplication", uiCulture));
            }
            newLanguage = uiCulture;
        }

        private void ShowAboutMessage()
        {
            messageService.ShowMessage(shellService.ShellView,
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