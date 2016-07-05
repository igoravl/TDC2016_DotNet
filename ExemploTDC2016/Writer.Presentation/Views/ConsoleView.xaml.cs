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

        private void InputBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                {
                    RunCommand();
                    break;
                }
            }
        }

        private void RunCommand()
        {
            var vm = (ConsoleViewModel) DataContext;
            vm.Invoke(inputBox.Text);
            inputBox.Text = string.Empty;
        }
    }
}