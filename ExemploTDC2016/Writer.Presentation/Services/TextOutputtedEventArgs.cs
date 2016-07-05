using System;

namespace Waf.Writer.Presentation.Services
{
    public class TextOutputtedEventArgs : EventArgs
    {
        public TextOutputtedEventArgs(string text)
        {
            Text = text;
        }

        public string Text { get; private set; }
    }
}