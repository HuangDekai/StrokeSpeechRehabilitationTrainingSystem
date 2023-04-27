using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.Common.Tools;
using 脑卒中言语康复训练系统.Models;
using 脑卒中言语康复训练系统.Shard.Helper;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels.Dialogs
{
    public class ExaminationChartViewModel : BindableBase, IDialogHostAware
    {
        public ExaminationChartViewModel() 
        {
            CancelCommand = new DelegateCommand(Cancel);
            UserInfo = LoginVerificationTool.GetLoginUserInfo();
        }

        #region 属性
        private static SqLiteHelper sqlHelper;
        public string DialogHostName { get; set; }
        private UserInfo userInfo;
        private ExaminationRecordRaise record;

        /// <summary>
        /// 存放记录信息
        /// </summary>
        public ExaminationRecordRaise Record
        {
            get { return record; }
            set { record = value; RaisePropertyChanged(); }
        }

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
        
        /// <summary>
        /// CancelCommand 绑定方法,退出按钮点击执行逻辑
        /// </summary>
        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogHost.Close(DialogHostName);
            }
        }

        /// <summary>
        /// 打开界面时候执行
        /// </summary>
        public void OnDialogOpend(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("ExaminationRecordId"))
            {
                int ExaminationRecordId = parameters.GetValue<int>("ExaminationRecordId");
                Record = GetExaminationRecord(ExaminationRecordId);
                var a = 0;
            }
        }

        /// <summary>
        /// 获取SQLite Connection
        /// </summary>
        private static void GetConnetion()
        {
            string name = AppDomain.CurrentDomain.BaseDirectory;
            string path = System.IO.Directory.GetParent(name).Parent.Parent.Parent.Parent.FullName;
            sqlHelper = new SqLiteHelper("data source = " + path + "\\脑卒中言语康复训练系统.Shard\\Graduate.db");
        }

        /// <summary>
        /// 通过 id 获取 ExaminationRecord 记录
        /// </summary>
        /// <param name="id">ExaminationRecord 的 id </param>
        private ExaminationRecordRaise GetExaminationRecord(int id)
        {
            var record = new ExaminationRecordRaise();
            GetConnetion();
            string sql = "SELECT * FROM ExaminationRecord JOIN Examination ON ExaminationRecord.ExaminationId = Examination.Id WHERE ExaminationRecord.Id = " + id;
            var reader = sqlHelper.ExecuteQuery(sql);
            if (reader.Read())
            {
                record.Id = id;
                record.UserId = reader.GetInt32(reader.GetOrdinal("UserId"));
                record.ExaminationId = reader.GetInt32(reader.GetOrdinal("ExaminationId"));
                record.Name = reader.GetString(reader.GetOrdinal("Name"));
                record.Content = reader.GetString(reader.GetOrdinal("Content"));
                record.Score = reader.GetDouble(reader.GetOrdinal("Score"));
                record.StartTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("StartTime")));
                record.EndTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("EndTime")));
                record.Cost = record.EndTime - record.StartTime;
                record.CreateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("CreateTime")));
                record.UpdateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("UpdateTime")));
            }
            reader.Close();
            sqlHelper.CloseConnection();

            record.QuestionRecords = GetAllQuestionRecord(record);
            return record;
        }

        /// <summary>
        /// 根据 record 获取其对应的答题记录
        /// </summary>
        /// <param name="record">量表记录</param>
        /// <returns>答题记录</returns>
        private ObservableCollection<QuestionRecordRaise> GetAllQuestionRecord(ExaminationRecordRaise record)
        {
            var questionRecords = new ObservableCollection<QuestionRecordRaise>();
            GetConnetion();
            string sql = "SELECT QR.*, O.*, Q.* FROM QuestionRecord QR JOIN Option O ON QR.OptionId = O.Id JOIN Question Q ON QR.QuestionId = Q.Id " +
                "WHERE ExaminationRecordId = " + record.Id;
            var reader = sqlHelper.ExecuteQuery(sql);
            int i = 1;
            while (reader.Read())
            {
                var questionRecord = new QuestionRecordRaise()
                {
                    Id = i++,
                    QuestionId = reader.GetInt32(reader.GetOrdinal("QuestionId")),
                    ExaminationRecordId = reader.GetInt32(reader.GetOrdinal("ExaminationRecordId")),
                    OptionId = reader.GetInt32(reader.GetOrdinal("OptionId")),
                    Option = new OptionRaise()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("OptionId")),
                        Content = reader.GetString(reader.GetOrdinal("Content")),
                        Weight = reader.GetDouble(reader.GetOrdinal("Weight")),
                    },
                    Content = reader.GetValue(13) != null ? reader.GetValue(13).ToString() : "",
                    UpdateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("UpdateTime"))),
                    CreateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("CreateTime"))),
                };
                questionRecords.Add(questionRecord);
            }
            reader.Close();
            sqlHelper.CloseConnection();
            return questionRecords;
        }
    }
}
