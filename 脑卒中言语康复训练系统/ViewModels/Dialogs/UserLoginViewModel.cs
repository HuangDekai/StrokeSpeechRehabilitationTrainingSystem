using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 脑卒中言语康复训练系统.Common;

namespace 脑卒中言语康复训练系统.ViewModels.Dialogs
{
    public class UserLoginViewModel : IDialogHostAware
    {
        public UserLoginViewModel() {
            SaveCommand = new DelegateCommand(Save);
            CancelComand = new DelegateCommand(Cancel);
        }

        private void Cancel()
        {
            if(DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogHost.Close(DialogHostName);
            }
        }

        private void Save()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogParameters param = new DialogParameters();
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, param));
            }
        }

        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelComand { get; set; }

        public void OnDialogOpend(IDialogParameters parameters)
        {
        }
    }
}
