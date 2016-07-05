using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Waf.Writer.Presentation.ViewModels;

namespace Waf.Writer.Presentation.Views
{
    /// <summary>
    /// Interaction logic for ConsoleView.xaml
    /// </summary>
    [Export(typeof(IConsoleView)), PartCreationPolicy(CreationPolicy.Shared)]
    public partial class ConsoleView : UserControl, IConsoleView
    {
        public ConsoleView()
        {
            InitializeComponent();
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

        public void ScrollToEnd()
        {
            ScrollViewer.ScrollToEnd();
        }
    }
}
