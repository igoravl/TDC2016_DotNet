using System;
using System.Collections.Generic;

namespace Waf.Writer.Presentation.Services
{
    public interface IPowerShellService
    {
        event EventHandler<TextOutputtedEventArgs> WarningOutputted;
        event EventHandler<TextOutputtedEventArgs> ErrorOutputted;
        event EventHandler<TextOutputtedEventArgs> VerboseOutputted;
        void Initialize();
        void Reload();
        void RunAutoExec();
        IEnumerable<object> Invoke(string script);
    }
}