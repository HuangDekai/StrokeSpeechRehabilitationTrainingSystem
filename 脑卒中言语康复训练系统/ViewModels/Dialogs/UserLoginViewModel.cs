using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.Shard.Helper;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels.Dialogs
{
    public class UserLoginViewModel : BindableBase,IDialogHostAware
    {
        public UserLoginViewModel() {
            
            getUserInfoData(1,10);
            getUserInfoDataRowNum(10);
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            LastPageCommand = new DelegateCommand(LastPage);
            NextPageCommand = new DelegateCommand(NextPage);
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

        private void LastPage()
        {
            if (CurrPage > 1)
            {
                CurrPage--;
                getUserInfoData(CurrPage, 10);
            }
        }

        private void NextPage()
        {
            if (CurrPage < PageNum)
            {
                CurrPage++;
                getUserInfoData(CurrPage, 10);
            }
        }

        #region 属性
        public string DialogHostName { get; set; }

        /// <summary>
        /// 用于操作 SQLite 数据库
        /// </summary>
        private static SqLiteHelper sqlHelper;

        private ObservableCollection<UserInfo> userInfos;

        /// <summary>
        /// 存放用户登录列表
        /// </summary>
        public ObservableCollection<UserInfo> UserInfos
        {
            get { return userInfos; }
            set { userInfos = value; RaisePropertyChanged(); }
        }


        private int currPage = 1;

        /// <summary>
        /// 用户登录列表当前页数
        /// </summary>
        public int CurrPage
        {
            get { return currPage; }
            set { currPage = value; RaisePropertyChanged(); }
        }


        private int pageNum;

        /// <summary>
        /// 用户登录列表最大页数
        /// </summary>
        public int PageNum
        {
            get { return pageNum; }
            set { pageNum = value; RaisePropertyChanged(); }
        }


        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand LastPageCommand { get; set; }
        public DelegateCommand NextPageCommand { get; set; }

        #endregion

        public void OnDialogOpend(IDialogParameters parameters)
        {
        }

        public static void getConnetion() {
            string name = AppDomain.CurrentDomain.BaseDirectory;
            string path = System.IO.Directory.GetParent(name).Parent.Parent.Parent.Parent.FullName;
            sqlHelper = new SqLiteHelper("data source = " + path + "\\脑卒中言语康复训练系统.Shard\\Graduate.db");
        }

        public void getUserInfoDataRowNum(int pageSize)
        {
            getConnetion();
            string sql = "select count(*) as PageNum from UserInfo;";
            var reader = sqlHelper.ExecuteQuery(sql);
            PageNum = 1;
            if (reader != null)
            {
                reader.Read();
                PageNum = reader.GetInt32(0) / pageSize + 1;
            }
            reader.Close();
        }

        public void getUserInfoData(int pageIndex, int pageSize)
        {
            getConnetion();
            UserInfos= new ObservableCollection<UserInfo>();
            int start = (pageIndex - 1) * pageSize;
            string sql = "select *  from UserInfo limit " + start + "," + pageSize;
            var reader = sqlHelper.ExecuteQuery(sql);
            int cnt = 10;
            while (reader.Read())
            {
                var userInfo = new UserInfo()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Order = reader.GetString(reader.GetOrdinal("Order")),
                    Avatar = reader.GetString(reader.GetOrdinal("Avatar")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Gender = reader.GetInt16(reader.GetOrdinal("Gender")),
                    Birth = reader.GetDateTime(reader.GetOrdinal("Birth")),
                    Phone = reader.GetString(reader.GetOrdinal("Phone")),
                    Department = reader.GetString(reader.GetOrdinal("Department")),
                    Address = reader.GetString(reader.GetOrdinal("Address")),
                    Situation = reader.GetString(reader.GetOrdinal("Situation")),
                };
                UserInfos.Add(userInfo);
                cnt--;
            }
            reader.Close();
            
            while (cnt > 0)
            {
                UserInfos.Add(new UserInfo() { Id = -1, Gender = -1, Birth = DateTime.MinValue});
                cnt--;
            }
        }
    }
}
