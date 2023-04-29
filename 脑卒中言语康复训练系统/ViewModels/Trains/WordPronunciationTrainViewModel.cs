using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.Common.Tools;
using 脑卒中言语康复训练系统.Models;
using 脑卒中言语康复训练系统.Shard.Helper;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels.Trains
{
    public class WordPronunciationTrainViewModel : BindableBase, INavigationAware
    {
        public WordPronunciationTrainViewModel(IDialogHostService dialogService, IRegionManager regionManager) 
        {
            this.dialogService= dialogService;
            this.regionManager= regionManager;

            CancelCommand = new DelegateCommand(Cancel);
            NextCommand = new DelegateCommand(NextButtonFuc);
            ReplayCommand = new DelegateCommand(ReplayButtonFuc);
            CommitCommand = new DelegateCommand(Commit);
            PauseCommand = new DelegateCommand(Pause);
            synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
        }

        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand NextCommand { get; set; }
        public DelegateCommand ReplayCommand { get; set; }
        public DelegateCommand CommitCommand { get; set; }
        public DelegateCommand PauseCommand { get; set; }

        /// <summary>
        /// PauseCommand 绑定方法, 暂停
        /// </summary>
        private async void Pause()
        {
            isClickPause = true;
            recognitionEngine.RecognizeAsyncStop();
            synthesizer.SpeakAsyncCancelAll();
            var parameters = new DialogParameters();
            parameters.Add("Title", "暂停中");
            parameters.Add("Message", "正在暂停中,是否开始答题?");
            parameters.Add("ButtonText", "开始");
            var res = await dialogService.ShowDialog("MessageBoxOnlySureView", parameters);
            IsBtnGroupShow = 0;
            isClickPause = false;
            Replay();
        }

        /// <summary>
        /// CommitCommand 绑定方法, 点击按钮提交问卷,使用Cancel()离开页面
        /// </summary>
        private async void Commit()
        {
            CurrTrainRecord.EndTime = DateTime.Now;
            var parameters = new DialogParameters();
            parameters.Add("Title", "温馨提示");
            parameters.Add("Message", "是否确认完成答题?");
            var res = await dialogService.ShowDialog("MessageBoxView", parameters);
            if (res.Result == ButtonResult.OK)
            {
                InsertTrainRecord();
                isNormalOut = true;
                Cancel();
            }
        }

        /// <summary>
        /// ReplayCommand 绑定方法, 点击按钮重播该问题
        /// </summary>
        private void ReplayButtonFuc()
        {
            recognitionEngine.RecognizeAsyncStop();
            isNext = false;
            isRecognized = true;
        }

        /// <summary>
        /// 重播该问题
        /// </summary>
        private void Replay()
        {
            CurrTrainRecord.TrainQuestionRecords[CurrItemIndex - 1].Retry ++;
            recognitionEngine.RecognizeAsyncStop();
            synthesizer.SpeakAsync(CurrQuestion.Content);
        }

        /// <summary>
        /// NextCommand 绑定方法, 点击按钮进入下一个问题
        /// </summary>
        private void NextButtonFuc()
        {
            isClickNext = true;
            recognitionEngine.RecognizeAsyncStop();
            isNext = true;
            isRecognized = true;
        }

        /// <summary>
        /// 进入下一个问题
        /// </summary>
        private void Next()
        {
            if (CurrItemIndex < MaxItemIndex)
            {
                CurrItemIndex++;
                CurrQuestion = TrainInfo.TrainQuestions[CurrItemIndex - 1];
                IsBtnGroupShow = 1;
                if (CurrItemIndex == MaxItemIndex)
                {
                    IsNextShow = 2;
                    IsCommitShow = 0;
                }
                recognitionEngine.RecognizeAsyncStop();
                synthesizer.SpeakAsync(CurrQuestion.Content);
            }
            
        }

        /// <summary>
        /// CancelCommand 绑定方法, 退出界面
        /// </summary>
        private void Cancel()
        {
            recognitionEngine.RecognizeAsyncStop();
            if (journal.CanGoBack)
            {
                journal.GoBack();
            }
        }

        /// <summary>
        /// 用于绑定 synthesizer.SpeakCompleted 事件, 在异步语音读完后执行
        /// </summary>
        private void Synthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            if (!isLeave && !isClickPause)
            {
                IsBtnGroupShow = 0;
                synthesizer.Speak("请回答");
                CurrTrainRecord.TrainQuestionRecords[CurrItemIndex - 1].StartTime = DateTime.Now;
                recognitionEngine.RecognizeAsync(RecognizeMode.Single);
            }
        }

        #region 属性
        private static SqLiteHelper sqlHelper;
        private readonly IDialogHostService dialogService;
        private readonly IRegionManager regionManager;
        private IRegionNavigationJournal journal;
        //语音播放器
        private SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        //创建语音识别引擎
        SpeechRecognitionEngine recognitionEngine = new SpeechRecognitionEngine();

        private TrainRaise trainInfo;
        private TrainQuestionRaise currQuestion;
        private int currItemIndex = 1;
        private int maxItemIndex;
        private int isBtnGroupShow = 1;
        private string speechResult;
        private bool isNext = false;
        private bool isRecognized = false;
        private bool isLeave = false;
        private bool isClickNext = false;
        private bool isClickPause = false;
        private int isNextShow;
        private int isCommitShow = 2;
        private bool isNormalOut = false;

        /// <summary>
        /// 用于控制提交按钮是否显示,0-展示,1-隐藏但占位,2-隐藏且不占位, 默认2
        /// </summary>
        public int IsCommitShow
        {
            get { return isCommitShow; }
            set { isCommitShow = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 用于控制下一题按钮是否显示,0-展示,1-隐藏但占位,2-隐藏且不占位
        /// </summary>
        public int IsNextShow
        {
            get { return isNextShow; }
            set { isNextShow = value; RaisePropertyChanged(); }
        }

        private TrainRecordRaise currTrainRecord;

        /// <summary>
        /// 用于存放训练记录
        /// </summary>
        public TrainRecordRaise CurrTrainRecord
        {
            get { return currTrainRecord; }
            set { currTrainRecord = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 用于存放语音识别结果
        /// </summary>
        public string SpeechResult
        {
            get { return speechResult; }
            set { speechResult = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 用于控制右侧按钮是否显示,0-展示,1-隐藏
        /// </summary>
        public int IsBtnGroupShow
        {
            get { return isBtnGroupShow; }
            set { isBtnGroupShow = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 最大题目数量
        /// </summary>
        public int MaxItemIndex
        {
            get { return maxItemIndex; }
            set { maxItemIndex = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 当前题目,默认1
        /// </summary>
        public int CurrItemIndex
        {
            get { return currItemIndex; }
            set { currItemIndex = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 用于页面展示当前的问题
        /// </summary>
        public TrainQuestionRaise CurrQuestion
        {
            get { return currQuestion; }
            set { currQuestion = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 存储整个训练的所有信息
        /// </summary>
        public TrainRaise TrainInfo
        {
            get { return trainInfo; }
            set { trainInfo = value; RaisePropertyChanged(); }
        }

        #endregion

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
            isLeave = true;
            recognitionEngine.RecognizeAsyncStop();
            synthesizer.SpeakAsyncCancelAll();

            if (isNormalOut)
            {
                var param = new DialogParameters();
                param.Add("TrainRecordId", CurrTrainRecord.Id);
                dialogService.ShowDialog("ResultChartView", param);
            }
        }

        /// <summary>
        /// 在导航到页面时被调用
        /// </summary>
        /// <param name="navigationContext">传入的内容</param>
        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("TrainInfo"))
            {
                journal = navigationContext.NavigationService.Journal;
                TrainInfo = navigationContext.Parameters.GetValue<TrainRaise>("TrainInfo");
                MaxItemIndex = navigationContext.Parameters.ContainsKey("MaxItemIndex") ? navigationContext.Parameters.GetValue<int>("MaxItemIndex") : 5;

                //如果未登录,调用返回方法返回之前界面
                var isLogin = await LoginVerification();
                if (!isLogin)
                {
                    Cancel();
                }

                GetTrainQuestions();
                CurrQuestion = TrainInfo.TrainQuestions[0];
                CurrTrainRecord = initTrainRecord(TrainInfo);
                InitSpeechRecognitionEngine(TrainInfo);
                synthesizer.SpeakAsync(CurrQuestion.Content);
            }
        }

        #region 数据库操作
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
        /// 将 trainInfo 对应的 TrainQuestion 获取并放入 TrainInfo.TrainQuestions中
        /// </summary>
        private void GetTrainQuestions()
        {
            GetConnetion();
            string sql = "select * from TrainQuestion where TrainId = " + TrainInfo.Id + " ORDER BY RANDOM() LIMIT " + MaxItemIndex;
            var reader = sqlHelper.ExecuteQuery(sql);
            ObservableCollection<TrainQuestionRaise> trainQuestions = new ObservableCollection<TrainQuestionRaise>();

            while (reader.Read())
            {
                var trainQuestionRaise = new TrainQuestionRaise()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    TrainId = TrainInfo.Id,
                    Content = reader.GetString(reader.GetOrdinal("Content")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                    CorrectAnswerId = reader.GetInt32(reader.GetOrdinal("CorrectAnswerId")),
                    CreateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("CreateTime"))),
                    UpdateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("UpdateTime"))),
                };
                trainQuestionRaise.Answers = GetAnswers(trainQuestionRaise);

                trainQuestions.Add(trainQuestionRaise);
            }
            reader.Close();
            sqlHelper.CloseConnection();

            TrainInfo.TrainQuestions = trainQuestions;
            foreach (var item in TrainInfo.TrainQuestions)
            {
                item.CorrectAnswer = item.Answers[0];
                Shuffle<AnswerRaise>(item.Answers);
            }
            Shuffle<TrainQuestionRaise>(TrainInfo.TrainQuestions);
        }

        /// <summary>
        /// 根据传入的 trainQuestion 查找对应的 Answer
        /// </summary>
        /// <param name="trainQuestion">传入的 question 实体</param>
        /// <returns></returns>
        private ObservableCollection<AnswerRaise> GetAnswers(TrainQuestionRaise trainQuestion)
        {
            GetConnetion();

            string sql = "select * from Answer where Id = " + trainQuestion.CorrectAnswerId;
            var reader = sqlHelper.ExecuteQuery(sql);
            ObservableCollection<AnswerRaise> answers = new ObservableCollection<AnswerRaise>();
            if (reader.Read())
            {
                var answerRaise = new AnswerRaise()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Picture = reader.GetString(reader.GetOrdinal("Picture")),
                    Content = reader.GetString(reader.GetOrdinal("Content")),
                    GroupId = reader.GetInt32(reader.GetOrdinal("GroupId")),
                    IsCorrect = true,
                    CreateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("CreateTime"))),
                    UpdateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("UpdateTime"))),
                };

                answers.Add(answerRaise);
            }
            reader.Close();

            if (trainQuestion.Quantity > 1)
            {
                sql = "SELECT * FROM Answer WHERE Id <> " + answers[0].Id + " AND GroupId = " + answers[0].GroupId + " ORDER BY RANDOM() LIMIT " + (trainQuestion.Quantity - 1);
                reader = sqlHelper.ExecuteQuery(sql);
                while (reader.Read())
                {
                    var answerRaise = new AnswerRaise()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Picture = reader.GetString(reader.GetOrdinal("Picture")),
                        Content = reader.GetString(reader.GetOrdinal("Content")),
                        IsCorrect = false,
                        CreateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("CreateTime"))),
                        UpdateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("UpdateTime"))),
                    };

                    answers.Add(answerRaise);
                }
            }
            reader.Close();
            sqlHelper.CloseConnection();
            return answers;
        }

        /// <summary>
        /// 根据 CurrTrainRecord 插入训练记录
        /// </summary>
        private void InsertTrainRecord()
        {
            GetConnetion();
            DateTime dateTime = DateTime.Now;
            
            string sql = "INSERT INTO TrainRecord(UserId,TrainId,StartTime,EndTime,CreateTime,UpdateTime) VALUES (" +
                CurrTrainRecord.UserId + "," +
                CurrTrainRecord.TrainId + "," +
                "'" + CurrTrainRecord.StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'," +
                "'" + CurrTrainRecord.EndTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'," +
                "'" + dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'," +
                "'" + dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" +
                ")";
            sqlHelper.ExecuteQuery(sql);

            sql = "SELECT * FROM TrainRecord WHERE UserId = " + CurrTrainRecord.UserId + " AND TrainId = " + CurrTrainRecord.TrainId + " ORDER BY id DESC LIMIT 1";
            var reader = sqlHelper.ExecuteQuery(sql);
            if (reader.Read())
            {
                var trainRecordId = reader.GetInt32(reader.GetOrdinal("Id"));
                CurrTrainRecord.Id= trainRecordId;
                reader.Close();

                foreach (var record in CurrTrainRecord.TrainQuestionRecords)
                {
                    sql = "INSERT INTO TrainQuestionRecord(TrainQuestionId,TrainRecordId,Retry,Score,StartTime,EndTime,CreateTime,UpdateTime) VALUES (" +
                    record.TrainQuestionId + "," +
                    trainRecordId + "," +
                    record.Retry + "," +
                    record.Score + "," +
                    "'" + record.StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'," +
                    "'" + record.EndTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'," +
                    "'" + dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'," +
                    "'" + dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'" +
                    ")";
                    sqlHelper.ExecuteQuery(sql);
                }
                
            }

            sqlHelper.CloseConnection();
        }
        #endregion


        /// <summary>
        /// 初始化TTS引擎,根据trainRaise限定词语保证识别率
        /// </summary>
        /// <param name="trainRaise">带有回答内容的TrainRaise</param>
        private void InitSpeechRecognitionEngine(TrainRaise trainRaise)
        {
            //创建一组语音识别的语法约束选择
            Choices choices = new Choices();
            HashSet<string> speechNames = new HashSet<string>();
            //添加语音识别关键字
            foreach (var question in trainRaise.TrainQuestions)
            {
                foreach (var answer in question.Answers)
                {
                    if (!speechNames.Contains(answer.Content))
                    {
                        speechNames.Add(answer.Content);
                        choices.Add(answer.Content);
                    }
                }
            }
            //以编程的方式为语音生成约束
            GrammarBuilder gb = new GrammarBuilder(choices);
            //grammarbuilder封装对象
            Grammar grm = new Grammar(gb);
            recognitionEngine.LoadGrammarAsync(grm);
            //设置过期时间
            recognitionEngine.InitialSilenceTimeout = TimeSpan.FromSeconds(20);
            //创建语音接收事件
            recognitionEngine.SpeechRecognized += (s, e) => {
                isRecognized = true;
                if (e.Result.Confidence >= 0.5)
                {
                    SpeechResult = e.Result.Text;
                    if (SpeechResult.Equals(CurrQuestion.CorrectAnswer.Content))
                    {
                        CorrectAnwer();
                        isNext = true;
                    } else
                    {
                        ErrorAnwer();
                    }
                }
                else
                {
                    RecognitionFailed();
                }
            };
            recognitionEngine.AudioStateChanged += (s, e) =>
            {
                //离开页面
                if (isLeave)
                {
                    
                }
                //点击暂停
                else if (isClickPause)
                {

                }
                //Timeout
                else if (e.AudioState == AudioState.Stopped && !isNext && !isRecognized)
                {
                    RecognitionFailed();
                    Replay();
                }
                //识别失败情况
                else if (e.AudioState == AudioState.Stopped && !isNext)
                {
                    Replay();
                    isRecognized = false;
                }
                //识别成功
                else if (e.AudioState == AudioState.Stopped && isNext)
                {
                    CurrTrainRecord.TrainQuestionRecords[CurrItemIndex - 1].Score = isClickNext ? 0 : 1;
                    CurrTrainRecord.TrainQuestionRecords[CurrItemIndex - 1].EndTime = DateTime.Now;
                    //如果是最后一题答对了
                    if (CurrItemIndex >= MaxItemIndex)
                    {
                        Commit();
                    }
                    else
                    {
                        Next();
                        isNext = false;
                        isRecognized = false;
                        isClickNext = false;
                    }
                }
            };
            //音频输入
            recognitionEngine.SetInputToDefaultAudioDevice();
        }

        /// <summary>
        /// 语音识别失败执行该函数
        /// </summary>
        private void RecognitionFailed()
        {
            synthesizer.Rate = 1;
            synthesizer.Speak("识别失败,请重试");
            synthesizer.Rate = 0;
        }

        /// <summary>
        /// 答案正确执行该函数
        /// </summary>
        private void CorrectAnwer()
        {
            synthesizer.Rate = 1;
            synthesizer.Speak("答对了");
            synthesizer.Rate = 0;
        }

        /// <summary>
        /// 答案错误执行该函数
        /// </summary>
        private void ErrorAnwer()
        {
            synthesizer.Rate = 1;
            synthesizer.Speak("回答错误,再来一次");
            synthesizer.Rate = 0;
        }

        /// <summary>
        /// 打乱一个 ObservableCollection 的顺序
        /// </summary>
        public static void Shuffle<T>(ObservableCollection<T> collection)
        {
            Random rng = new Random();
            int n = collection.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = collection[k];
                collection[k] = collection[n];
                collection[n] = value;
            }
        }

        #region 登录校验
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
                parameters.Add("Message", "请先登录再进行训练!");
                await dialogService.ShowDialog("MessageBoxView", parameters);
                isSuccess = false;
            }
            return isSuccess;
        }
        #endregion

        #region 训练记录相关
        /// <summary>
        /// 初始化训练记录
        /// </summary>
        /// <param name="trainInfo">初始化好的训练信息</param>
        /// <returns>初始化好的训练记录</returns>
        private TrainRecordRaise initTrainRecord(TrainRaise trainInfo)
        {
            
            var tarinRecord = new TrainRecordRaise
            {
                TrainId = trainInfo.Id,
                UserId = LoginVerificationTool.GetLoginUserId(),
                StartTime = DateTime.Now,
                TrainQuestionRecords = new ObservableCollection<TrainQuestionRecordRaise>()
            };
            foreach (var question in trainInfo.TrainQuestions)
            {
                var questionRecord = new TrainQuestionRecordRaise
                {
                    TrainQuestionId = question.Id,
                };
                tarinRecord.TrainQuestionRecords.Add(questionRecord);
            }
            return tarinRecord;
        }
        #endregion
    }
}
