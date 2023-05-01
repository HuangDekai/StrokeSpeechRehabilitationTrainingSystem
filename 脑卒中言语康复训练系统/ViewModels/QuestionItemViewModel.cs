using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Xml.Linq;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.Common.Tools;
using 脑卒中言语康复训练系统.Extensions;
using 脑卒中言语康复训练系统.Models;
using 脑卒中言语康复训练系统.Shard.Helper;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels
{
    public class QuestionItemViewModel : BindableBase, INavigationAware
    {

         public QuestionItemViewModel(IDialogHostService dialogService, IRegionManager regionManager)
        {
            this.dialogService = dialogService;
            this.regionManager = regionManager;
            CancelCommand = new DelegateCommand(Cancel);
            LastCommand = new DelegateCommand(Last);
            NextCommand = new DelegateCommand(Next);
            SelectCommand = new DelegateCommand<OptionRaise>(Select);
            ItemSelectCommand = new DelegateCommand<QuestionRaise>(ItemSelect);
            CommitCommand = new DelegateCommand(Commit);
        }

        #region 属性
        public string DialogHostName { get; set; }
        private static SqLiteHelper sqlHelper;
        private readonly IDialogHostService dialogService;
        private readonly IRegionManager regionManager;
        private IRegionNavigationJournal journal;
        private QuestionRaise currQuestion;
        private int currQuestionIndex;
        private ExaminationRaise examinationRaise;
        private int isShowCommitButton = 2;
        private ExaminationRecord record;

        /// <summary>
        /// 用于记录本次问卷相关信息
        /// </summary>
        public ExaminationRecord Record
        {
            get { return record; }
            set { record = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 是否显示提交按钮,用于给 QuestionItemView 中提交是否显示, 默认=2
        /// 0 - Visible 显示
        /// 1 - Hidden 隐藏但占用空间
        /// 2 - Collapsed 隐藏且不占用空间
        /// </summary>
        public int IsShowCommitButton
        {
            get { return isShowCommitButton; }
            set { isShowCommitButton = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 用于表示被选择的问题的题号, 初始值为0
        /// </summary>
        public int CurrQuestionIndex
        {
            get { return currQuestionIndex; }
            set { currQuestionIndex = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 当前显示的Question
        /// </summary>
        public QuestionRaise CurrQuestion
        {
            get { return currQuestion; }
            set { currQuestion = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 用于存储问卷信息
        /// </summary>
        public ExaminationRaise ExaminationRaise
        {
            get { return examinationRaise; }
            set { examinationRaise = value; RaisePropertyChanged(); }
        }

        #endregion
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand LastCommand { get; set; }
        public DelegateCommand NextCommand { get; set; }
        public DelegateCommand<OptionRaise> SelectCommand { get; set; }
        public DelegateCommand<QuestionRaise> ItemSelectCommand { get; set; }
        public DelegateCommand CommitCommand { get; set; }

        /// <summary>
        /// CommitCommand 绑定方法, 点击按钮提交答题记录
        /// </summary>
        private async void Commit()
        {
            var endTime = DateTime.Now;
            record.EndTime = endTime;
            record.CreateTime = endTime;
            record.UpdateTime = endTime;
            record.UserId = LoginVerificationTool.GetLoginUserId();
            record.ExaminationId = ExaminationRaise.Id;
            double score = 0;
            foreach (var question in ExaminationRaise.Questions)
            {
                score += question.Select.Weight;
            }
            record.Score = score;

            var parameters = new DialogParameters();
            parameters.Add("Title", "温馨提示");
            parameters.Add("Message", "确认要提交量表测评吗?");
            var result = await dialogService.ShowDialog("MessageBoxView", parameters);

            if (result != null && result.Result == ButtonResult.OK)
            {
                InsertExaminationRecord(Record);
                regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("ExaminationView");
            }

            parameters.Add("ExaminationRecordId", Record.Id);
            dialogService.ShowDialog("ExaminationChartView", parameters);
        }

        /// <summary>
        /// ItemSelectCommand 绑定方法, 点击按钮切换到对应的题目
        /// </summary>
        private void ItemSelect(QuestionRaise questionItem)
        {
            CurrQuestion.IsCurrSelect = false;
            CurrQuestion = questionItem;
            CurrQuestionIndex = questionItem.Sort - 1;
            CurrQuestion.IsCurrSelect = true;
        }

        /// <summary>
        /// SelectCommand 绑定方法, 选择选项
        /// </summary>
        private void Select(OptionRaise optionSelected)
        {
            //如果没选过,选中后自动进入下一个问题,否则只改变选项
            if (CurrQuestion.Select == null)
            {
                CurrQuestion.Select = optionSelected;
                CurrQuestion.Select.IsChecked = true;
                Record.QuestionRecords[CurrQuestionIndex].OptionId = optionSelected.Id;
                Next();
            }
            else
            {
                CurrQuestion.Select = optionSelected;
                CurrQuestion.Select.IsChecked = true;
                Record.QuestionRecords[CurrQuestionIndex].OptionId = optionSelected.Id;
            }
        }

        /// <summary>
        /// CancelCommand 绑定方法, 退出界面
        /// </summary>
        private void Cancel()
        {
            if (journal.CanGoBack)
            {
                journal.GoBack();
            }
        }

        /// <summary>
        /// LastCommand 绑定方法, 上一个问题
        /// </summary>
        private void Last()
        {
            if (CurrQuestionIndex > 0)
            {
                CurrQuestion.IsCurrSelect = false;
                CurrQuestionIndex--;
                CurrQuestion = examinationRaise.Questions[CurrQuestionIndex];
                CurrQuestion.IsCurrSelect = true;
            }
            ButtonShowFlush();
        }

        /// <summary>
        /// NextCommand 绑定方法, 下一个问题
        /// </summary>
        private void Next()
        {
            if (CurrQuestionIndex < ExaminationRaise.Quantity - 1)
            {
                CurrQuestion.IsCurrSelect = false;
                CurrQuestionIndex ++;
                CurrQuestion = examinationRaise.Questions[CurrQuestionIndex];
                CurrQuestion.IsCurrSelect = true;
            }
            ButtonShowFlush();
        }

        private void ButtonShowFlush()
        {
            if (IsCheckedAll())
            {
                IsShowCommitButton = 0;
            } else
            {
                IsShowCommitButton = 2;
            }
        }

        /// <summary>
        /// 用于判断是否每个问题都被选过了
        /// </summary>
        /// <returns></returns>
        private bool IsCheckedAll()
        {
            foreach (var question in examinationRaise.Questions)
            {
                if (question.Select == null)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 在导航到页面时被调用
        /// </summary>
        /// <param name="navigationContext">传入的内容</param>
        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("ExaminationInfo"))
            {
                journal = navigationContext.NavigationService.Journal;
                var ExaminationInfo = navigationContext.Parameters.GetValue<Examination>("ExaminationInfo");
                ExaminationRaise = new ExaminationRaise() { 
                    Id = ExaminationInfo.Id, 
                    Name = ExaminationInfo.Name,
                    Content= ExaminationInfo.Content,
                    Normal= ExaminationInfo.Normal,
                    Quantity= ExaminationInfo.Quantity,
                    CreateTime = ExaminationInfo.CreateTime,
                    UpdateTime = ExaminationInfo.UpdateTime,
                };
                GetQuestionList(ExaminationInfo);
                CurrQuestion = examinationRaise.Questions[CurrQuestionIndex];
                InitExaminationRecord(ExaminationRaise);

                //如果未登录,调用返回方法返回之前界面
                var isLogin = await LoginVerification();
                if (!isLogin)
                {
                    Cancel();
                }
            }
        }

        /// <summary>
        /// 该方法用于设置进入时候是否重用原来的页面
        /// </summary>
        /// <param name="navigationContext"></param>
        /// <returns>false - 每次进入该页面都创建一个新实例; true - 重用</returns>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        /// <summary>
        /// 在页面从导航堆栈中移除时被调用，用于保存页面的状态
        /// </summary>
        /// <param name="navigationContext"></param>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
           
        }

        /// <summary>
        /// 初始化问卷记录
        /// </summary>
        /// <param name="examinationRaise"></param>
        private void InitExaminationRecord(ExaminationRaise examinationRaise)
        {
            var time = DateTime.Now;
            Record = new ExaminationRecord();
            Record.StartTime = time;
            Record.QuestionRecords = new ObservableCollection<QuestionRecord>();
            foreach (var item in examinationRaise.Questions)
            {
                var record = new QuestionRecord()
                {
                    QuestionId = item.Id,
                };
                Record.QuestionRecords.Add(record);
            }
        }

        /// <summary>
        /// 用于判断是否登录了,若未登录,进行拦截
        /// </summary>
        /// <returns>登录(true)/未登录(false)</returns>
        private async Task<bool> LoginVerification()
        {
            bool isSuccess = true;
            if (!LoginVerificationTool.IsLogin())
            {
                var parameters = new DialogParameters();
                parameters.Add("Title", "温馨提示");
                parameters.Add("Message", "请先登录再进行量表测评!");
                await dialogService.ShowDialog("MessageBoxView", parameters);
                isSuccess = false;
            }
            return isSuccess;
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
        /// 将 ExaminationInfo 对应的 Question 获取并放入 ExaminationInfo.QuestionList中
        /// </summary>
        private void GetQuestionList(Examination examinationInfo)
        {
            GetConnetion();
            string sql = "select * from Question where ExaminationId = " + examinationInfo.Id;
            var reader = sqlHelper.ExecuteQuery(sql);
            ObservableCollection<QuestionRaise> questions = new ObservableCollection<QuestionRaise>();
            int sort = 1;
            while (reader.Read())
            {
                var questionRaise = new QuestionRaise()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    ExaminationId = examinationInfo.Id,
                    Content = reader.GetString(reader.GetOrdinal("Content")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                    Start = reader.GetInt32(reader.GetOrdinal("Start")),
                    CreateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("CreateTime"))),
                    UpdateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("UpdateTime"))),
                    Sort = sort ++,
                };
                questionRaise.IsCurrSelect = questionRaise.Sort== 1;
                questionRaise.Options = GetOptionList(questionRaise);
                questions.Add(questionRaise);
            }
            reader.Close();
            sqlHelper.CloseConnection();
            ExaminationRaise.Questions = questions;
        }

        /// <summary>
        /// 根据传入的 question 查找对应的 Option
        /// </summary>
        /// <param name="question">传入的 question 实体</param>
        /// <returns></returns>
        private ObservableCollection<OptionRaise> GetOptionList(QuestionRaise questionRaise)
        {
            GetConnetion();
            string sql = "select * from Option limit " + (questionRaise.Start - 1) + "," + questionRaise.Quantity;
            var reader = sqlHelper.ExecuteQuery(sql);
            ObservableCollection<OptionRaise> options = new ObservableCollection<OptionRaise>();
            int sort = 1;
            while (reader.Read())
            {
                var optionRaise = new OptionRaise()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Weight = reader.GetDouble(reader.GetOrdinal("Weight")),
                    Content = reader.GetString(reader.GetOrdinal("Content")),
                    CreateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("CreateTime"))),
                    UpdateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("UpdateTime"))),
                    Sort = sort ++,
                };
                options.Add(optionRaise);
            }
            reader.Close();
            sqlHelper.CloseConnection();
            return options;
        }

        private void InsertExaminationRecord(ExaminationRecord examinationRecord)
        {
            GetConnetion();
            string sql = "INSERT INTO ExaminationRecord(UserId,ExaminationId,Score,StartTime,EndTime,CreateTime,UpdateTime) Values (" + 
                examinationRecord.UserId + "," +
                examinationRecord.ExaminationId + "," +
                examinationRecord.Score + "," +
                "'" + examinationRecord.StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'," +
                "'" + examinationRecord.EndTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'," +
                "'" + examinationRecord.CreateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'," +
                "'" + examinationRecord.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" 
                + ")";
            sqlHelper.ExecuteQuery(sql);

            sql = "SELECT * FROM ExaminationRecord " 
                + "WHERE UserId = " + examinationRecord.UserId + " AND ExaminationId = " + examinationRecord.ExaminationId
                + " Order By Id DESC Limit 1";
            var reader = sqlHelper.ExecuteQuery(sql);
            if (reader.Read())
            {
                examinationRecord.Id = reader.GetInt32(reader.GetOrdinal("Id"));
            }
            reader.Close();

            foreach (var item in examinationRecord.QuestionRecords)
            {
                sql = "INSERT INTO QuestionRecord(QuestionId, ExaminationRecordId, OptionId, CreateTime, UpdateTime) Values (" +
                    item.QuestionId + ","+
                    examinationRecord.Id + "," +
                    item.OptionId + "," +
                    "'" + examinationRecord.CreateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'," +
                    "'" + examinationRecord.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'"
                    + ")";
                sqlHelper.ExecuteQuery(sql);
            }


            sqlHelper.CloseConnection();
        }
    }
}
