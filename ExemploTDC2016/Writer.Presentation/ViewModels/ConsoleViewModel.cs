using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Waf.Writer.Presentation.Controllers;
using Waf.Writer.Presentation.Services;
using Waf.Writer.Presentation.Views;

namespace Waf.Writer.Presentation.ViewModels
{
    [Export]
    public class ConsoleViewModel : ViewModel<IConsoleView>
    {
        private readonly IPowerShellService _powerShellService;
        private readonly Brush _defaultColor = new SolidColorBrush(Colors.White);
        private readonly Brush _errorColor = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush _warningColor = new SolidColorBrush(Colors.Yellow);
        private readonly SolidColorBrush _verboseColor = new SolidColorBrush(Colors.Gray);
        private readonly FlowDocument _document = new FlowDocument();
        private ConsoleController _controller;

        [ImportingConstructor]
        public ConsoleViewModel(IConsoleView view, IPowerShellService powerShellService) : base(view)
        {
            _powerShellService = powerShellService;

            _powerShellService.ErrorOutputted += (sender, e) => WriteError(e.Text);
            _powerShellService.WarningOutputted += (sender, e) => WriteWarning(e.Text);
            _powerShellService.VerboseOutputted += (sender, e) => WriteVerbose(e.Text);

            var style = (Style) Application.Current.FindResource("ConsoleWindowDocument");
            _document.Style = style;

            style = (Style) Application.Current.FindResource("ConsoleWindowParagraph");
            _document.Resources.Add(typeof(Paragraph), style);
        }

        public void Write(string text, Brush color)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var textLines = text.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            var p = new Paragraph();

            for (var i = 0; i < textLines.Length - 1; i++)
            {
                p.Inlines.Add(new Run(textLines[i]) {Foreground = color});
                p.Inlines.Add(new LineBreak());
            }

            p.Inlines.Add(new Run(textLines.Last()) {Foreground = color});

            _document.Blocks.Add(p);

            ViewCore.ScrollToEnd();
        }

        public void Write(string text)
        {
            Write(text, _defaultColor);
        }

        public void WriteWarning(string text)
        {
            Write(text, _warningColor);
        }

        public void WriteError(string text)
        {
            Write(text, _errorColor);
        }

        public void WriteVerbose(string text)
        {
            Write(text, _verboseColor);
        }

        public void Clear()
        {
        }

        public void Invoke(string command)
        {
            Write($"PS> {command}");

            foreach (var r in _powerShellService.Invoke(command))
            {
                Write(r.ToString());
            }

        }

        public FlowDocument Document { get { return _document; } }
    }
}