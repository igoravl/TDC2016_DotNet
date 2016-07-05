using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;

namespace Waf.Writer.Presentation.Views
{
    public interface IMainView : IView
    {
        ContentViewState ContentViewState { get; set; }
    }
}
