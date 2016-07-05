﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace Waf.Writer.Presentation.Services
{
    public interface IPrintDialogService
    {
        bool ShowDialog();
        
        void PrintDocument(DocumentPaginator documentPaginator, string description);
    }
}
