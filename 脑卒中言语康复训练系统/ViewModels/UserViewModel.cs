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
            string name = AppDomain.CurrentDomain.BaseDirectory;
            string path = System.IO.Directory.GetParent(name).Parent.Parent.Parent.Parent.FullName;
            sqlHelper = new SqLiteHelper("data source = " + path + "\\脑卒中言语康复训练系统.Shard\\Graduate.db");
            string sql = "select * from UserInfo where Id = 1";
            var reader = sqlHelper.ExecuteQuery(sql);
            reader.Read();

            userInfo = new UserInfo()
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")), 
                Order = reader.GetString(reader.GetOrdinal("Order")),
                Avatar= reader.GetString(reader.GetOrdinal("Avatar")),
                Name = reader.GetString(reader.GetOrdinal("Name")), 
                Gender = reader.GetInt16(reader.GetOrdinal("Gender")), 
                Birth = reader.GetDateTime(reader.GetOrdinal("Birth")), 
                Phone = reader.GetString(reader.GetOrdinal("Phone")), 
                Department = reader.GetString(reader.GetOrdinal("Department")),
                Address = reader.GetString(reader.GetOrdinal("Address")),
                Situation = reader.GetString(reader.GetOrdinal("Situation")),
            };
            CreateTestData();
            UserLoginCommand = new DelegateCommand<string>(UserLogin);
            this.dialogService = dialogService;
        }

        private void UserLogin(string obj)
        {
            dialogService.ShowDialog("UserLoginView", null);
        }

        #region 属性
        private static SqLiteHelper sqlHelper;

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
        
        private readonly IDialogHostService dialogService;

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
