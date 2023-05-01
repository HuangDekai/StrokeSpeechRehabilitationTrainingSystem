using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.Shard.Helper;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels.Dialogs
{
    public class UserLoginViewModel : BindableBase,IDialogHostAware
    {
        public UserLoginViewModel() {
            
            getUserInfoData(1,10);
            getUserInfoDataRowNum(10,"","");
            getDepartments();
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            LastPageCommand = new DelegateCommand(LastPage);
            NextPageCommand = new DelegateCommand(NextPage);
            QueryCommand = new DelegateCommand(Query);
            SelectCommand = new DelegateCommand<Object>(Select);
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
                param.Add("LoginUser", LoginUser);
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, param));
            }
        }

        private void LastPage()
        {
            if (CurrPage > 1)
            {
                CurrPage--;
                getUserInfoData(CurrPage, 10, QueryName, QueryDepartment);
            }
        }

        private void NextPage()
        {
            if (CurrPage < PageNum)
            {
                CurrPage++;
                getUserInfoData(CurrPage, 10, QueryName, QueryDepartment);
            }
        }

        private void Query()
        {
            getUserInfoData(1, 10, QueryName, QueryDepartment);
            getUserInfoDataRowNum(10, QueryName, QueryDepartment);
        }
        
        private void Select(object SelectedItem)
        {
            LoginUser = (UserInfo)SelectedItem;
            Save();
        }

        #region 属性
        public string DialogHostName { get; set; }

        private UserInfo loginUser;

        /// <summary>
        /// 用于传递选择的登录用户给 UserView
        /// </summary>
        public UserInfo LoginUser
        {
            get { return loginUser; }
            set { loginUser = value; }
        }


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

        private ObservableCollection<string> departments;

        /// <summary>
        /// 存放 ComboBox 里的部门
        /// </summary>
        public ObservableCollection<string> Departments
        {
            get { return departments; }
            set { departments = value; RaisePropertyChanged(); }
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

        private string queryName;

        /// <summary>
        /// 用户搜索时候输入的用户名
        /// </summary>
        public string QueryName
        {
            get { return queryName; }
            set { queryName = value; RaisePropertyChanged(); }
        }

        private string queryDepartment;

        /// <summary>
        /// 用户搜索时候显示的部门列表
        /// </summary>
        public string QueryDepartment
        {
            get { return queryDepartment; }
            set { queryDepartment = value; RaisePropertyChanged(); }
        }


        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand LastPageCommand { get; set; }
        public DelegateCommand NextPageCommand { get; set; }
        public DelegateCommand QueryCommand { get; set; }
        public DelegateCommand<Object> SelectCommand { get; set; }

        #endregion

        public void OnDialogOpend(IDialogParameters parameters)
        {

        }

        /// <summary>
        /// 获取SQLite Connection
        /// </summary>
        private static void GetConnetion()
        {
            string name = AppDomain.CurrentDomain.BaseDirectory;
            string path = System.IO.Directory.GetParent(name).FullName;
            sqlHelper = new SqLiteHelper("data source = " + path + "\\Graduate.db");
        }


        /// <summary>
        /// 查询并更新页面中的当前页和最大页
        /// </summary>
        /// <param name="pageSize">每页有多少行</param>
        /// <param name="name">查询名字</param>
        /// <param name="department">查询部门</param>
        public void getUserInfoDataRowNum(int pageSize, string name, string department)
        {
            GetConnetion();
            string sql = "select count(*) as PageNum from UserInfo";
            if (!String.IsNullOrEmpty(name))
            {
                sql += " where name like '%" + name+"%'";
            }
            if (!String.IsNullOrEmpty(department))
            {
                if (String.IsNullOrEmpty(name))
                {
                    sql += " where";
                } else
                {
                    sql += " AND";
                }
                sql += " department='" + department+"'";
            }
            var reader = sqlHelper.ExecuteQuery(sql);
            CurrPage = 1;
            if (reader != null)
            {
                reader.Read();
                PageNum = reader.GetInt32(0) / pageSize + 1;
            }
            reader.Close();
            sqlHelper.CloseConnection();
        }

        /// <summary>
        /// 分页查询并更新页面中的用户列表
        /// </summary>
        /// <param name="pageIndex">要查询第几页</param>
        /// <param name="pageSize">每页有多少行</param>
        /// <param name="name">查询名字</param>
        /// <param name="department">查询部门</param>
        public void getUserInfoData(int pageIndex, int pageSize, string name="", string department="")
        {
            GetConnetion();
            UserInfos = new ObservableCollection<UserInfo>();
            int start = (pageIndex - 1) * pageSize;
            string sql = "select *  from UserInfo ";
            if (!String.IsNullOrEmpty(name))
            {
                sql += "where name like'%" + name+"%'";
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
                sql += " department='" + department+"'";
            }
            sql += " limit " + start + "," + pageSize;
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
                    IsShowSelect = 0,
                };
                UserInfos.Add(userInfo);
                cnt--;
            }
            reader.Close();
            sqlHelper.CloseConnection();

            while (cnt > 0)
            {
                UserInfos.Add(new UserInfo() { Id = -1, Gender = -1, Birth = DateTime.MinValue, IsShowSelect=1 });
                cnt--;
            }


        }

        /// <summary>
        /// 获取部门, 更新下拉列表
        /// </summary>
        public void getDepartments()
        {
            
            GetConnetion();
            Departments = new ObservableCollection<string>();
            Departments.Add("");
            string sql = "select Name from Department;";
            var reader = sqlHelper.ExecuteQuery(sql);
            while (reader.Read())
            {
                Departments.Add( reader.GetString(0));
            }
            reader.Close();
            sqlHelper.CloseConnection();
        }
    }
}
