using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.Shard.Helper;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels.Dialogs
{
    public class UserInfoViewModel : BindableBase, IDialogHostAware
    {
        public UserInfoViewModel(IDialogHostService dialogService) 
        {
            this.dialogService = dialogService;

            CancelCommand = new DelegateCommand(Cancel);
            SaveCommand = new DelegateCommand(Save);
            UploadCommand = new DelegateCommand(Upload);    
            
            Genders = new ObservableCollection<short> { -1, 0, 1 };
        }
        #region 属性
        public string DialogHostName { get; set; }
        private static SqLiteHelper sqlHelper;
        private readonly IDialogHostService dialogService;
        private UserInfo currUserInfo;

        private ObservableCollection<short> genders;
        private ObservableCollection<string> departments;
        private string avatar;

        /// <summary>
        /// 用于实时更新切换的头像
        /// </summary>
        public string Avatar
        {
            get { return avatar; }
            set { avatar = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 存放部门列表
        /// </summary>
        public ObservableCollection<string> Departments
        {
            get { return departments; }
            set { departments = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 存放性别列表
        /// </summary>
        public ObservableCollection<short> Genders
        {
            get { return genders; }
            set { genders = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 存放输入进来的用户信息
        /// </summary>
        public UserInfo CurrUserInfo
        {
            get { return currUserInfo; }
            set { currUserInfo = value; RaisePropertyChanged(); }
        }

        #endregion
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand UploadCommand { get; set; }

        private void Upload()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

            if (openFileDialog.ShowDialog() == true)
            {
                var baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                var imageDirectory = Path.Combine(baseDirectory, "Image");
                var avatarDirectory = Path.Combine(imageDirectory, "Avatar");
                Directory.CreateDirectory(avatarDirectory);

                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var extension = Path.GetExtension(openFileDialog.FileName);
                var fileName = $"{timestamp}_{CurrUserInfo.Name}{extension}";
                var destinationPath = Path.Combine(avatarDirectory, fileName);

                File.Copy(openFileDialog.FileName, destinationPath);

                CurrUserInfo.Avatar = fileName;
                Avatar = fileName;
            }
        }

        private void Save()
        {
            GetConnetion();
            
            
            //存在Id,修改
            if (CurrUserInfo != null && CurrUserInfo.Id != 0)
            {
                string[] colNames = { "`Order`", "Avatar", "Name", "Gender", "Birth", "Phone", "Department", "Address", "Situation", "UpdateTime", "CreateTime" };
                string[] colValues = {
                    CurrUserInfo.Order, CurrUserInfo.Avatar, CurrUserInfo.Name, CurrUserInfo.Gender.ToString(),
                    CurrUserInfo.Birth.ToString("yyyy-MM-dd HH:mm:ss.fff"), CurrUserInfo.Phone, CurrUserInfo.Department, CurrUserInfo.Address,
                    CurrUserInfo.Situation,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),CurrUserInfo.CreateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")
                };
                string key = "Id";
                string value = CurrUserInfo.Id.ToString();
                sqlHelper.UpdateValues("UserInfo", colNames, colValues, key, value);
            } 
            //不存在Id,新增
            else
            {
                
                string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                string sql = "Insert INTO UserInfo(`Order`,Avatar,Name,Gender,Birth,Phone,Department,Address,Situation,UpdateTime,CreateTime) " +
                    "VALUES (" +
                    "'" + CurrUserInfo.Order + "'," +
                    "'" + CurrUserInfo.Avatar + "'," +
                    "'" + CurrUserInfo.Name + "'," +
                    "'" + CurrUserInfo.Gender.ToString() + "'," +
                    "'" + CurrUserInfo.Birth.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'," +
                    "'" + CurrUserInfo.Phone + "'," +
                    "'" + CurrUserInfo.Department + "'," +
                    "'" + CurrUserInfo.Address + "'," +
                    "'" + CurrUserInfo.Situation + "'," +
                    "'" + time + "'," +
                    "'" + time + "'" +
                    ")";
                sqlHelper.ExecuteQuery(sql);
            }
            Cancel();
        }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogHost.Close(DialogHostName);
            }
        }

        public void OnDialogOpend(IDialogParameters parameters)
        {
            CurrUserInfo = new UserInfo()
            {
                Avatar = "/Image/Default.png",
                Gender = -1,
                Birth = DateTime.Now,
            };
            if (parameters.ContainsKey("UserInfo"))
            {
                var userInfo = parameters.GetValue<UserInfo>("UserInfo");
                CurrUserInfo = new UserInfo()
                {
                    Id = userInfo.Id,
                    Order = userInfo.Order,
                    Avatar = userInfo.Avatar,
                    Name = userInfo.Name,
                    Gender = userInfo.Gender,
                    Birth = userInfo.Birth,
                    Phone = userInfo.Phone,
                    Department = userInfo.Department,
                    Address = userInfo.Address,
                    Situation = userInfo.Situation,
                    IsShowSelect = userInfo.IsShowSelect,
                    UpdateTime = userInfo.UpdateTime,
                    CreateTime = userInfo.CreateTime,
                };
                Avatar = CurrUserInfo.Avatar;    
            }
            Departments = GetDepartments();
            
        }

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
