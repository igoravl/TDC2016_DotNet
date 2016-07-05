using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.Writer.Presentation.Documents;
using Waf.Writer.Presentation.Views;

namespace Waf.Writer.Presentation.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class SaveChangesViewModel : ViewModel<ISaveChangesView>
    {
        private readonly DelegateCommand noCommand;
        private readonly DelegateCommand yesCommand;
        private bool? dialogResult;
        private IReadOnlyList<IDocument> documents;


        [ImportingConstructor]
        public SaveChangesViewModel(ISaveChangesView view) : base(view)
        {
            yesCommand = new DelegateCommand(() => Close(true));
            noCommand = new DelegateCommand(() => Close(false));
        }


        public static string Title
        {
            get { return ApplicationInfo.ProductName; }
        }

        public ICommand YesCommand
        {
            get { return yesCommand; }
        }

        public ICommand NoCommand
        {
            get { return noCommand; }
        }

        public IReadOnlyList<IDocument> Documents
        {
            get { return documents; }
            set { SetProperty(ref documents, value); }
        }


        public bool? ShowDialog(object owner)
        {
            ViewCore.ShowDialog(owner);
            return dialogResult;
        }

        private void Close(bool? dialogResult)
        {
            this.dialogResult = dialogResult;
            ViewCore.Close();
        }
    }
}