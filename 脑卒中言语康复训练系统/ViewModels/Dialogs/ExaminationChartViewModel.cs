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
using 脑卒中言语康复训练系统.Common.Tools;
using 脑卒中言语康复训练系统.Shard.Helper;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels.Dialogs
{
    public class ExaminationChartViewModel : BindableBase, IDialogHostAware
    {
        public ExaminationChartViewModel() {
            CancelCommand = new DelegateCommand(Cancel);
            UserInfo = LoginVerificationTool.GetLoginUserInfo();
        }

        #region 属性
        private static SqLiteHelper sqlHelper;
        public string DialogHostName { get; set; }
        private UserInfo userInfo;

        /// <summary>
        /// 存放用户信息
        /// </summary>
        public UserInfo UserInfo
        {
            get { return userInfo; }
            set { userInfo = value; RaisePropertyChanged(); }
        }
        #endregion

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogHost.Close(DialogHostName);
            }
        }
        public void OnDialogOpend(IDialogParameters parameters)
        {

        }
    }
}
