﻿using System;
using System.ComponentModel.Composition;
using System.Linq;
using Waf.Writer.Presentation.Services;

namespace Waf.Writer.Presentation.Services
{
    [Export(typeof(IEnvironmentService))]
    internal class EnvironmentService : IEnvironmentService
    {
        private readonly Lazy<string> documentFileName = new Lazy<string>(() => Environment.GetCommandLineArgs().ElementAtOrDefault(1));

        
        public string DocumentFileName { get { return documentFileName.Value; } }
    }
}
