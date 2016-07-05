using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Controls;
using Waf.Writer.Presentation.ViewModels;

namespace Waf.Writer.Presentation.Views
{
    [Export(typeof(IStartView))]
    public partial class StartView : UserControl, IStartView
    {
        private readonly Lazy<StartViewModel> viewModel;

        public StartView()
        {
            InitializeComponent();

            viewModel = new Lazy<StartViewModel>(() => this.GetViewModel<StartViewModel>());
            newButton.IsVisibleChanged += NewButtonIsVisibleChanged;
        }


        private StartViewModel ViewModel
        {
            get { return viewModel.Value; }
        }


        private void NewButtonIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (newButton.IsVisible)
            {
                newButton.Focus();
            }
        }

        private void OpenContextMenuHandler(object sender, RoutedEventArgs e)
        {
            var recentFile = (RecentFile) ((FrameworkElement) sender).DataContext;
            ViewModel.FileService.OpenCommand.Execute(recentFile.Path);
        }

        private void PinContextMenuHandler(object sender, RoutedEventArgs e)
        {
            var recentFile = (RecentFile) ((FrameworkElement) sender).DataContext;
            recentFile.IsPinned = true;
        }

        private void UnpinContextMenuHandler(object sender, RoutedEventArgs e)
        {
            var recentFile = (RecentFile) ((FrameworkElement) sender).DataContext;
            recentFile.IsPinned = false;
        }

        private void RemoveContextMenuHandler(object sender, RoutedEventArgs e)
        {
            var recentFile = (RecentFile) ((FrameworkElement) sender).DataContext;
            ViewModel.FileService.RecentFileList.Remove(recentFile);
        }
    }
}