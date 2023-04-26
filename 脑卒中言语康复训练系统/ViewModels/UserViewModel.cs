using DryIoc;
using ImTools;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.Common.Tools;
using 脑卒中言语康复训练系统.Shard.Helper;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels
{
    internal class UserViewModel : BindableBase, INavigationAware
    {
        public UserViewModel(IDialogHostService dialogService) 
        {
            UserLoginCommand = new DelegateCommand<string>(UserLogin);
            UserLogoutCommand = new DelegateCommand(UserLogout);
            SelectTrainRecordCommand = new DelegateCommand<object>(SelectTrainRecord);
            this.dialogService = dialogService;
        }


        /// <summary>
        /// 获取SQLite Connection
        /// </summary>
        public static void GetConnetion()
        {
            string name = AppDomain.CurrentDomain.BaseDirectory;
            string path = System.IO.Directory.GetParent(name).Parent.Parent.Parent.Parent.FullName;
            sqlHelper = new SqLiteHelper("data source = " + path + "\\脑卒中言语康复训练系统.Shard\\Graduate.db");
        }

        /// <summary>
        /// 点击训练记录里的详情按钮绑定的方法
        /// </summary>
        /// <param name="obj">对应行的记录</param>
        private void SelectTrainRecord(object obj)
        {
            var param = new DialogParameters();
            param.Add("TrainRecordId", ((TrainRecord)obj).Id);
            dialogService.ShowDialog("ResultChartView",param);
        }

        /// <summary>
        /// 点击登录按钮绑定的方法
        /// </summary>
        /// <param name="obj">暂时无用</param>
        private async void UserLogin(string obj)
        {
            var dialogResult = await dialogService.ShowDialog("UserLoginView", null);
            if (dialogResult != null)
            {
                UserInfo = dialogResult.Parameters.GetValue<UserInfo>("LoginUser");
                GetTrainRecord(UserInfo);
                GetExaminationRecord(UserInfo);

                //如果未登录,登录后切换按钮为 "切换用户"和"注销登录"
                if (LoginBtnIsShow == 0)
                {
                    SwitchBtnShow();
                }

                //登录用户写入数据库中
                LoginVerificationTool.Login(UserInfo.Id);
            }
        }

        /// <summary>
        /// 点击注销登录按钮绑定的方法
        /// </summary>
        private async void UserLogout()
        {
            var parameters = new DialogParameters();
            parameters.Add("Title", "退出登录");
            parameters.Add("Message", "您确定要退出登录用户 [" + UserInfo.Name + "] ?");
            var dialogResult = await dialogService.ShowDialog("MessageBoxView", parameters);
            if (dialogResult != null && dialogResult.Result == ButtonResult.OK)
            {
                SwitchBtnShow();
                UserInfo = null;
                TrainRecordCollection.Clear();
                ExaminationRecordCollection.Clear();

                //用户退出登录数据从数据库中移除
                LoginVerificationTool.Logout();
            }
            
        }

        /// <summary>
        /// 切换登录按钮显示
        /// </summary>
        private void SwitchBtnShow()
        {
            if (LoginBtnIsShow == 0)
            {
                LoginBtnIsShow = 2;
                ChangeBtnIsShow = 0;
                LogoutBtnIsShow= 0;
            } else
            {
                LoginBtnIsShow = 0;
                ChangeBtnIsShow = 1;
                LogoutBtnIsShow = 1;
            }
        }

        /// <summary>
        /// 根据用户信息里的用户id查询用户训练记录
        /// </summary>
        /// <param name="userInfo">包含Id的UserInfo实体</param>
        private void GetTrainRecord(UserInfo userInfo)
        {
            if (userInfo == null)
            {
                throw new ArgumentNullException("UserInfo为空");
            }
            TrainRecordCollection = new ObservableCollection<TrainRecord>();
            GetConnetion();
            string sql = "select * from TrainRecord join Train on TrainRecord.TrainId = Train.Id where TrainRecord.UserId = " + userInfo.Id;
            var reader = sqlHelper.ExecuteQuery(sql);
            int i = 1;
            while (reader.Read())
            {
                var trainRecord = new TrainRecord()
                {
                    Sort= i++,
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Content = reader.GetString(reader.GetOrdinal("Content")),
                    StartTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("StartTime"))),
                    EndTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("EndTime"))),
                    CreateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("CreateTime"))),
                    UpdateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("UpdateTime"))),
                };
                trainRecord.Cost = trainRecord.EndTime- trainRecord.StartTime;
                TrainRecordCollection.Add(trainRecord);
            }
            reader.Close();
            sqlHelper.CloseConnection();
        }

        /// <summary>
        /// 根据用户信息里的用户id查询用户测评记录
        /// </summary>
        /// <param name="userInfo">包含Id的UserInfo实体</param>
        private void GetExaminationRecord(UserInfo userInfo)
        {
            if (userInfo == null)
            {
                throw new ArgumentNullException("UserInfo为空");
            }
            ExaminationRecordCollection = new ObservableCollection<ExaminationRecord>();
            GetConnetion();
            string sql = "select * from ExaminationRecord " +
                         "join Examination " +
                         "on ExaminationRecord.ExaminationId = Examination.Id " +
                         "where ExaminationRecord.UserId = " + userInfo.Id;
            var reader = sqlHelper.ExecuteQuery(sql);
            int i = 1;
            while (reader.Read())
            {
                var examinationRecord = new ExaminationRecord()
                {
                    Sort = i++,
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    ExaminationId = reader.GetInt32(reader.GetOrdinal("ExaminationId")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Normal  = reader.GetString(reader.GetOrdinal("Normal")),
                    Score = reader.GetDouble(reader.GetOrdinal("Score")),
                    Content = reader.GetString(reader.GetOrdinal("Content")),
                    StartTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("StartTime"))),
                    EndTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("EndTime"))),
                    CreateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("CreateTime"))),
                    UpdateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("UpdateTime"))),
                };
                examinationRecord.Cost = examinationRecord.EndTime - examinationRecord.StartTime;
                ExaminationRecordCollection.Add(examinationRecord);
            }
            reader.Close();
            sqlHelper.CloseConnection();
        }

        /// <summary>
        /// 获取用户信息,用于去更新用户信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns>用户信息 / null</returns>
        private UserInfo GetUserInfo(int userId)
        {
            GetConnetion();
            string sql = "select * from UserInfo where id = " + userId;
            UserInfo userInfo = null; 
            var reader = sqlHelper.ExecuteQuery(sql);
            if (reader.Read())
            {
                userInfo = new UserInfo()
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
            }
            reader.Close();
            sqlHelper.CloseConnection();

            return userInfo;
        }

        /// <summary>
        /// 进入当前页面时
        /// </summary>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (LoginVerificationTool.IsLogin())
            {
                UserInfo = GetUserInfo(LoginVerificationTool.GetLoginUserId());
                GetExaminationRecord(UserInfo);
                GetTrainRecord(UserInfo);
                LoginBtnIsShow = 2;
                ChangeBtnIsShow = 0;
                LogoutBtnIsShow = 0;
            }
        }

        /// <summary>
        /// 是否重用当前页面
        /// </summary>
        /// <returns>true 是</returns>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        /// <summary>
        /// 离开页面时操作
        /// </summary>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        #region 属性
        private static SqLiteHelper sqlHelper;

        private readonly IDialogHostService dialogService;

        private int loginBtnIsShow = 0;

        /// <summary>
        /// 是否显示选择按钮,用于给 UserView 中登录是否显示
        /// 0 - Visible 显示
        /// 1 - Hidden 隐藏但占用空间
        /// 2 - Collapsed 隐藏且不占用空间
        /// </summary>
        public int LoginBtnIsShow
        {
            get { return loginBtnIsShow; }
            set { loginBtnIsShow = value; RaisePropertyChanged(); }
        }

        private int changeBtnIsShow = 1;

        /// <summary>
        /// 是否显示选择按钮,用于给 UserView 中切换用户是否显示
        /// 0 - Visible 显示
        /// 1 - Hidden 隐藏但占用空间
        /// 2 - Collapsed 隐藏且不占用空间
        /// </summary>
        public int ChangeBtnIsShow
        {
            get { return changeBtnIsShow; }
            set { changeBtnIsShow = value; RaisePropertyChanged();}
        }

        private int logoutBtnIsShow = 1;

        /// <summary>
        /// 是否显示选择按钮,用于给 UserView 中注销用户是否显示
        /// 0 - Visible 显示
        /// 1 - Hidden 隐藏但占用空间
        /// 2 - Collapsed 隐藏且不占用空间
        /// </summary>
        public int LogoutBtnIsShow
        {
            get { return logoutBtnIsShow; }
            set { logoutBtnIsShow = value; RaisePropertyChanged();}
        }



        private UserInfo userInfo;
        /// <summary>
        /// 用于用户信息展示
        /// </summary>
        public UserInfo UserInfo
        {
            get { return userInfo; }
            set { userInfo = value; RaisePropertyChanged(); }
        }


        private ObservableCollection<ExaminationRecord> examinationRecordCollection;
        /// <summary>
        /// 用于测评记录展示
        /// </summary>
        public ObservableCollection<ExaminationRecord> ExaminationRecordCollection
        {
            get { return examinationRecordCollection; }
            set { examinationRecordCollection = value; RaisePropertyChanged(); }
        }


        private ObservableCollection<TrainRecord> trainRecordCollection;
        /// <summary>
        /// 用于训练记录展示
        /// </summary>
        public ObservableCollection<TrainRecord> TrainRecordCollection
        {
            get { return trainRecordCollection; }
            set { trainRecordCollection = value; RaisePropertyChanged(); }
        }
        #endregion

        public DelegateCommand<string> UserLoginCommand { get; private set; }
        public DelegateCommand UserLogoutCommand { get; private set; }
        public DelegateCommand<object> SelectTrainRecordCommand { get; private set; }
    }
}
