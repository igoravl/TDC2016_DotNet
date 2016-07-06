using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Waf.Writer.Presentation.Services
{
    public class HistoryList: ObservableCollection<string>
    {
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            CurrentItem = Count - 1;
        }

        public int CurrentItem { get; set; }

        public string Up()
        {
            if (CurrentItem >= 0 && CurrentItem < Count)
            {
                return this[CurrentItem--];
            }

            CurrentItem = 0;
            return string.Empty;
        }

        public string Down()
        {
            if (CurrentItem >= -1 && CurrentItem < (Count-1))
            {
                return this[++CurrentItem];
            }

            CurrentItem = Count;
            return string.Empty;
        }
    }
}
