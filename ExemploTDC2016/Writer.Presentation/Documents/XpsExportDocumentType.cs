﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.IO.Packaging;
using System.IO;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;
using System.Windows;
using Waf.Writer.Presentation.Properties;

namespace Waf.Writer.Presentation.Documents
{
    public class XpsExportDocumentType : DocumentType
    {
        public XpsExportDocumentType() : base(Resources.XpsDocuments, ".xps")
        {
        }

        
        public override bool CanSave(IDocument document) { return document is RichTextDocument; }

        protected override void SaveCore(IDocument document, string fileName)
        {
            // We have to clone the FlowDocument before we use different pagination settings for the export.        
            RichTextDocument richTextDocument = document as RichTextDocument;
            FlowDocument clone = richTextDocument.CloneContent();
            clone.ColumnWidth = double.PositiveInfinity;

            using (Package package = Package.Open(fileName, FileMode.Create))
            using (XpsDocument xpsDocument = new XpsDocument(package, CompressionOption.Maximum))
            {
                XpsSerializationManager serializer = new XpsSerializationManager(new XpsPackagingPolicy(xpsDocument), false);
                DocumentPaginator paginator = ((IDocumentPaginatorSource)clone).DocumentPaginator;
                serializer.SaveAsXaml(paginator);
            }
        }
    }
}
