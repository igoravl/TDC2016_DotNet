﻿using System.Linq;
using System.Waf.Applications;
using Waf.Writer.Presentation.ViewModels;
using Waf.Writer.Presentation.Views;

namespace Waf.Writer.Presentation.DesignData
{
    public class SampleStartViewModel : StartViewModel
    {
        public SampleStartViewModel() : base(new MockStartView(), new MockFileService())
        {
            ((MockFileService) FileService).RecentFileList = new RecentFileList();
            FileService.RecentFileList.AddFile(@"C:\Users\Admin\My Documents\Document 1.rtf");
            FileService.RecentFileList.AddFile(@"C:\Users\Admin\My Documents\WPF Application Framework (WAF).rtf");
            FileService.RecentFileList.AddFile(@"C:\Users\Admin\My Documents\WAF Writer\Readme.rtf");
            FileService.RecentFileList.RecentFiles.First().IsPinned = true;
        }


        private class MockStartView : MockView, IStartView
        {
        }
    }
}