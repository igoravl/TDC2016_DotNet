using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Input;
using Waf.Writer.Presentation.ViewModels;

namespace Waf.Writer.Presentation.Views
{
    /// <summary>
    ///     Interaction logic for ConsoleView.xaml
    /// </summary>
    [Export(typeof(IConsoleView)), PartCreationPolicy(CreationPolicy.Shared)]
    public partial class ConsoleView : UserControl, IConsoleView
    {
        public ConsoleView()
        {
            InitializeComponent();
        }

        public void ScrollToEnd()
        {
            ScrollViewer.ScrollToEnd();
        }

        private void InputBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.Enter:
                {
                    RunCommand();
                    break;
                }
                case Key.Up:
                {
                    HistoryUp();
                    break;
                }
                case Key.Down:
                {
                    HistoryDown();
                    break;
                }
            }
        }

        private void HistoryUp()
        {
            var vm = (ConsoleViewModel) DataContext;
            inputBox.Text = vm.PowerShellService.History.Up();
        }

        private void HistoryDown()
        {
            var vm = (ConsoleViewModel)DataContext;
            inputBox.Text = vm.PowerShellService.History.Down();
        }

        private void RunCommand()
        {
            var vm = (ConsoleViewModel) DataContext;
            var command = inputBox.Text;

            vm.Invoke(command);

            inputBox.Text = string.Empty;

            vm.PowerShellService.History.Add(command);
        }
    }
}