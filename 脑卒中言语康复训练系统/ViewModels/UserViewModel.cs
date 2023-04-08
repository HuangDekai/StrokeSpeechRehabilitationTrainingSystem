using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.Shard.Helper;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels
{
    internal class UserViewModel : BindableBase
    {
        public UserViewModel(IDialogHostService dialogService) 
        {
            CreateTestData();
            UserLoginCommand = new DelegateCommand<string>(UserLogin);
            this.dialogService = dialogService;
        }


        /// <summary>
        /// 获取SQLite Connection
        /// </summary>
        public static void getConnetion()
        {
            string name = AppDomain.CurrentDomain.BaseDirectory;
            string path = System.IO.Directory.GetParent(name).Parent.Parent.Parent.Parent.FullName;
            sqlHelper = new SqLiteHelper("data source = " + path + "\\脑卒中言语康复训练系统.Shard\\Graduate.db");
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
            }
        }

        #region 属性
        private static SqLiteHelper sqlHelper;

        private readonly IDialogHostService dialogService;

        /// <summary>
        /// 用于用户信息展示
        /// </summary>
        private UserInfo userInfo;
        public UserInfo UserInfo
        {
            get { return userInfo; }
            set { userInfo = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 用于测评记录展示
        /// </summary>
        private ObservableCollection<ExaminationRecord> examinationRecordCollection;
        public ObservableCollection<ExaminationRecord> ExaminationRecordCollection
        {
            get { return examinationRecordCollection; }
            set { examinationRecordCollection = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 用于训练记录展示
        /// </summary>
        private ObservableCollection<TrainRecord> trainRecordCollection;

        public ObservableCollection<TrainRecord> TrainRecordCollection
        {
            get { return trainRecordCollection; }
            set { trainRecordCollection = value; RaisePropertyChanged(); }
        }
        #endregion

        public DelegateCommand<string> UserLoginCommand { get; private set; }

        void CreateTestData()
        {
            examinationRecordCollection = new ObservableCollection<ExaminationRecord>();
            trainRecordCollection = new ObservableCollection<TrainRecord>();

            DateTime start = DateTime.Now;
            for (int i = 0; i < 10; i++)
            {
                examinationRecordCollection.Add(
                    new ExaminationRecord() {
                        Id=i,
                        Name="训练"+i, 
                        Content="test content"+i,
                        Cost=DateTime.Now - start, 
                        Normal="60分以上", 
                        Score=60+i, 
                        CreateTime = start, 
                        UpdateTime=start
                    }
                );
            }
            for (int i = 0; i < 10; i++)
            {
                trainRecordCollection.Add(
                    new TrainRecord()
                    {
                        Id=i,
                        Name="测评" + i,
                        Content = "test content sadfasfsadfsadfsdafsadfsadfasdfsadfsadfsadf" + i,
                        Cost=DateTime.Now - start,
                        CreateTime=start,
                        UpdateTime=start
                    }    
                );
            }
        }

    }
}
