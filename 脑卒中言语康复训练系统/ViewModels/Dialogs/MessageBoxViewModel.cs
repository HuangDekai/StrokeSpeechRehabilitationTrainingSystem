using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 脑卒中言语康复训练系统.Common;

namespace 脑卒中言语康复训练系统.ViewModels.Dialogs
{
    public class MessageBoxViewModel : BindableBase, IDialogHostAware
    {
        public MessageBoxViewModel() { 
            
            SaveCommand = new DelegateCommand(Save);
            CancelCommand= new DelegateCommand(Cancel);
        }

        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        private string title;
        private string message;

        /// <summary>
        /// MessageBox的Title
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; RaisePropertyChanged();}
        }

        /// <summary>
        /// MessageBox的消息
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; RaisePropertyChanged();}
        }


        public void OnDialogOpend(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("Title"))
            {
                Title = parameters.GetValue<string>("Title");
            }
            if (parameters.ContainsKey("Message"))
            {
                Message = parameters.GetValue<string>("Message");
            }
        }
        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogHost.Close(DialogHostName);
            }
        }

        private void Save()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK));
            }
        }
    }
}
