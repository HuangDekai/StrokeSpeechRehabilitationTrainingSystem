using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.Shard.Helper;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels.Dialogs
{
    public class QuestionCoverViewModel : BindableBase, IDialogHostAware
    {
        public QuestionCoverViewModel() { 
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
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
                var parameters = new DialogParameters();
                parameters.Add("ExaminationInfo", ExaminationInfo);
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, parameters));
            }
        }

        #region 属性
        public string DialogHostName { get; set; }

        private static SqLiteHelper sqlHelper;

        private string name;

        /// <summary>
        /// 问卷名, 由 ExaminationView 及其 Model 传入
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged(); }
        }

        private Examination examinationInfo;

        /// <summary>
        /// 根据问卷名在数据库中获取的问卷详情
        /// </summary>
        public Examination ExaminationInfo
        {
            get { return examinationInfo; }
            set { examinationInfo = value; RaisePropertyChanged(); }
        }

        #endregion

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        /// <summary>
        /// 在 Dialog 被打开时
        /// </summary>
        /// <param name="parameters">传入的参数</param>
        public void OnDialogOpend(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("Name"))
            {
                Name = parameters.GetValue<string>("Name");
                GetExamination();
            }
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
        /// 获取对应名的Examination信息
        /// </summary>
        public void GetExamination()
        {
            GetConnetion();
            string sql = "select * from Examination where Name = '" + Name + "'";
            var reader = sqlHelper.ExecuteQuery(sql);
            while (reader.Read())
            {
                examinationInfo = new Examination()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Normal = reader.GetString(reader.GetOrdinal("Normal")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                    Content = reader.GetString(reader.GetOrdinal("Content")),
                    CreateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("CreateTime"))),
                    UpdateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("UpdateTime"))),
                };
            }
            reader.Close();
            sqlHelper.CloseConnection();
        }
    }
}
