﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waf.Writer.Presentation.Documents
{
    public interface IDocumentType
    {
        string Description { get; }

        string FileExtension { get; }


        bool CanNew();
        
        IDocument New();

        bool CanOpen();

        IDocument Open(string fileName);

        bool CanSave(IDocument document);

        void Save(IDocument document, string fileName);
    }
}
