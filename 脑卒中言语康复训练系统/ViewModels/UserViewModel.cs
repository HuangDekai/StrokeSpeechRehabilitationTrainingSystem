using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels
{
    internal class UserViewModel : BindableBase
    {
        public UserViewModel(IDialogHostService dialogService) 
        {
            userInfo = new UserInfo()
            {
                Id = 1, 
                Order = "test00001020",
                Avatar= "../Image/Default.png",
                Name = "测试", Gender = 1, Birth = new DateTime(1998, 10, 20), Phone = "18775712345", Department = "康复治疗科第一科室",
                Address = "上海市徐汇区翻斗大街翻斗花园二号楼1001室",
                Situation = "中度失语，无听理解障碍和语义理解障碍，存在找词困难和命名障碍，存在语句杂乱症状",
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
