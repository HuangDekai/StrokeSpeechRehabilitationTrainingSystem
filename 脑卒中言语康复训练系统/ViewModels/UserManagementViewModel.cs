using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.Common.Tools;
using 脑卒中言语康复训练系统.Shard.Helper;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels
{
    internal class UserManagementViewModel : BindableBase
    {
        public UserManagementViewModel(IDialogHostService dialogService) 
        {
            this.dialogService = dialogService;

            QueryCommand = new DelegateCommand(Query);
            UpdateCommand = new DelegateCommand<Object>(Update);
            DeleteCommand = new DelegateCommand<object>(Delete);
            AddCommand = new DelegateCommand(Add);

            UserManagements = GetUserInfoList("","");
            Departments = GetDepartments();
        }

        public DelegateCommand QueryCommand { get; set; }
        public DelegateCommand<Object> UpdateCommand { get; set; }
        public DelegateCommand<Object> DeleteCommand { get; set; }
        public DelegateCommand AddCommand { get; set; }

        private async void Add()
        {
            var param = new DialogParameters();
            await dialogService.ShowDialog("UserInfoView", param);

            UserManagements = GetUserInfoList("", "");
        }

        private async void Delete(Object SelectedItem)
        {
            var user = (UserInfo)SelectedItem;

            if (LoginVerificationTool.IsLogin())
            {
                if (LoginVerificationTool.GetLoginUserId() == user.Id)
                {
                    var param = new DialogParameters();
                    param.Add("Title", "警告");
                    param.Add("Message", "无法删除正在登录的用户!");
                    await dialogService.ShowDialog("MessageBoxOnlySureView", param);
                    return;
                }
            }

            var parameters = new DialogParameters();
            parameters.Add("Title", "温馨提示");
            parameters.Add("Message", "是否确定删除该用户?");
            var result = await dialogService.ShowDialog("MessageBoxView", parameters);
            if (result != null && result.Result == ButtonResult.OK)
            {
                GetConnetion();
                string sql = "DELETE FROM UserInfo WHERE Id = " + user.Id;
                sqlHelper.ExecuteQuery(sql);
                sqlHelper.CloseConnection();
            }

            UserManagements = GetUserInfoList("", "");
        }

        private void Query()
        {
            UserManagements = GetUserInfoList(QueryName,QueryDepartment);
        }

        private async void Update(object SelectedItem)
        {
            var param = new DialogParameters();
            param.Add("UserInfo", (UserInfo)SelectedItem);
            await dialogService.ShowDialog("UserInfoView", param);

            UserManagements = GetUserInfoList("", "");
        }

        #region 属性
        /// <summary>
        /// 用于操作 SQLite 数据库
        /// </summary>
        private static SqLiteHelper sqlHelper;
        private readonly IDialogHostService dialogService;

        private ObservableCollection<UserInfo> userManagements;
        private ObservableCollection<string> departments;
        private string queryDepartment;
        private string queryName;

        /// <summary>
        /// 用户搜索时候输入的用户名
        /// </summary>
        public string QueryName
        {
            get { return queryName; }
            set { queryName = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 用户搜索时候显示的部门列表
        /// </summary>
        public string QueryDepartment
        {
            get { return queryDepartment; }
            set { queryDepartment = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 存放 ComboBox 里的部门
        /// </summary>
        public ObservableCollection<string> Departments
        {
            get { return departments; }
            set { departments = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 存放用户列表
        /// </summary>
        public ObservableCollection<UserInfo> UserManagements
        {
            get { return userManagements; }
            set { userManagements = value; RaisePropertyChanged(); }
        }

        #endregion

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        public static void GetConnetion()
        {
            string name = AppDomain.CurrentDomain.BaseDirectory;
            string path = System.IO.Directory.GetParent(name).Parent.Parent.Parent.Parent.FullName;
            sqlHelper = new SqLiteHelper("data source = " + path + "\\脑卒中言语康复训练系统.Shard\\Graduate.db");
        }

        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <param name="pageIndex">要查询第几页</param>
        /// <param name="pageSize">每页有多少行</param>
        /// <returns>用户列表</returns>
        private ObservableCollection<UserInfo> GetUserInfoList(string name, string department)
        {
            GetConnetion();
            var userInfoList = new ObservableCollection<UserInfo>();
            string sql = "select *  from UserInfo ";
            if (!String.IsNullOrEmpty(name))
            {
                sql += "where name like'%" + name + "%'";
            }
            if (!String.IsNullOrEmpty(department))
            {
                if (String.IsNullOrEmpty(name))
                {
                    sql += "where";
                }
                else
                {
                    sql += " AND";
                }
                sql += " department='" + department + "'";
            }
            var reader = sqlHelper.ExecuteQuery(sql);
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
                    IsShowSelect = 0,
                };
                userInfoList.Add(userInfo);
            }
            reader.Close();
            sqlHelper.CloseConnection();

            return userInfoList;
        }

        /// <summary>
        /// 获取部门, 更新下拉列表
        /// </summary>
        public ObservableCollection<string> GetDepartments()
        {
            GetConnetion();
            var departments = new ObservableCollection<string>();
            departments.Add("");
            string sql = "select distinct department from UserInfo;";
            var reader = sqlHelper.ExecuteQuery(sql);
            while (reader.Read())
            {
                departments.Add(reader.GetString(0));
            }
            reader.Close();
            sqlHelper.CloseConnection();

            return departments;
        }


    }


}
