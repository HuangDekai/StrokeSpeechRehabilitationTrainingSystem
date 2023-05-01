using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using 脑卒中言语康复训练系统.Common.Tools;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.Models;
using 脑卒中言语康复训练系统.Shard.Helper;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace 脑卒中言语康复训练系统.ViewModels.Trains
{
    public class IncompleteImageMatchingTrainViewModel : BindableBase, INavigationAware
    {
        public IncompleteImageMatchingTrainViewModel(IDialogHostService dialogService, IRegionManager regionManager) 
        {
            this.dialogService = dialogService;
            this.regionManager = regionManager;

            CancelCommand = new DelegateCommand(Cancel);
            ReplayCommand = new DelegateCommand(Replay);
            NextCommand = new DelegateCommand(Next);
            PauseCommand = new DelegateCommand(Pause);
            LeftImageFocusedCommand = new DelegateCommand(LeftImageFocused);
            RightImageFocusedCommand = new DelegateCommand(RightImageFocused);
            ImageUnFocusedCommand = new DelegateCommand(ImageUnFocused);
            SelecteCommand = new DelegateCommand<object>(Select);
            CommitCommand = new DelegateCommand(Commit);

            synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
        }

        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand NextCommand { get; set; }
        public DelegateCommand ReplayCommand { get; set; }
        public DelegateCommand CommitCommand { get; set; }
        public DelegateCommand PauseCommand { get; set; }
        public DelegateCommand LeftImageFocusedCommand { get; set; }
        public DelegateCommand RightImageFocusedCommand { get; set; }
        public DelegateCommand ImageUnFocusedCommand { get; set; }
        public DelegateCommand<object> SelecteCommand { get; set; }

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
            if (res != null && res.Result == ButtonResult.OK)
            {
                InsertTrainRecord();
                Cancel();
            }
        }

        /// <summary>
        /// PauseCommand 绑定方法, 暂停
        /// </summary>
        private async void Pause()
        {
            synthesizer.SpeakAsyncCancelAll();
            var parameters = new DialogParameters();
            parameters.Add("Title", "暂停中");
            parameters.Add("Message", "正在暂停中,是否开始答题?");
            parameters.Add("ButtonText", "开始");
            var res = await dialogService.ShowDialog("MessageBoxOnlySureView", parameters);
            CurrTrainRecord.TrainQuestionRecords[CurrItemIndex - 1].StartTime = DateTime.Now;

            Replay();
        }

        private void Select(object obj)
        {
            if (IsEnable)
            {
                IsEnable = false;

                var selected = obj as AnswerRaise;
                if (selected != null)
                {
                    int score = 0;
                    if (selected.IsCorrect)
                    {
                        score = 1;
                        synthesizer.Speak("答对了");
                    }
                    else
                    {
                        synthesizer.Speak("回答错误,继续努力");
                    }
                    CurrTrainRecord.TrainQuestionRecords[CurrItemIndex - 1].Score = score;

                    Next();
                }
            }
        }

        /// <summary>
        /// 鼠标离开图片区域,显示效果切换
        /// </summary>
        private void ImageUnFocused()
        {
            LeftElevation = "Dp1";
            RightElevation = "Dp1";
        }

        /// <summary>
        /// 鼠标进入左边图片区域,显示效果切换
        /// </summary>
        private void LeftImageFocused()
        {
            LeftElevation = "Dp24";
            RightElevation = "Dp1";
        }
        /// <summary>
        /// 鼠标进入右边图片区域,显示效果切换
        /// </summary>
        private void RightImageFocused()
        {
            LeftElevation = "Dp1";
            RightElevation = "Dp24";
        }

        /// <summary>
        /// 进入下一个问题
        /// </summary>
        private void Next()
        {
            CurrTrainRecord.TrainQuestionRecords[CurrItemIndex - 1].EndTime = DateTime.Now;
            if (CurrItemIndex < MaxItemIndex)
            {
                synthesizer.SpeakAsyncCancelAll();
                CurrItemIndex++;
                CurrQuestion = TrainInfo.TrainQuestions[CurrItemIndex - 1];
                SpliteImage();
                IsBtnGroupShow = 1;
                if (CurrItemIndex == MaxItemIndex)
                {
                    IsNextShow = 2;
                    IsCommitShow = 0;
                }
                synthesizer.SpeakAsync("请选择");
                StartTimer();
            }
            else if (CurrItemIndex == MaxItemIndex)
            {
                Commit();
            }
        }

        /// <summary>
        /// 重播该问题
        /// </summary>
        private void Replay()
        {
            IsBtnGroupShow = 1;
            CurrTrainRecord.TrainQuestionRecords[CurrItemIndex - 1].Retry++;
            synthesizer.SpeakAsync("请选择匹配上图的图片");
        }

        /// <summary>
        /// CancelCommand 绑定方法, 退出界面
        /// </summary>
        private void Cancel()
        {
            if (journal.CanGoBack)
            {
                synthesizer.SpeakAsyncCancelAll();
                journal.GoBack();
            }
        }

        /// <summary>
        /// 用于绑定 synthesizer.SpeakCompleted 事件, 在异步语音读完后执行
        /// </summary>
        private void Synthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            IsBtnGroupShow = 0;
            CurrTrainRecord.TrainQuestionRecords[CurrItemIndex - 1].StartTime = DateTime.Now;
        }

        #region 属性
        private static SqLiteHelper sqlHelper;
        private readonly IDialogHostService dialogService;
        private readonly IRegionManager regionManager;
        private IRegionNavigationJournal journal;
        //语音播放器
        private SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        private TrainRaise trainInfo;
        private TrainQuestionRaise currQuestion;
        private int currItemIndex = 1;
        private int maxItemIndex;
        private TrainRecordRaise currTrainRecord;
        private int isBtnGroupShow = 1;
        private int isNextShow;
        private int isCommitShow = 2;
        private string leftElevation = "Dp1";
        private string rightElevation = "Dp1";
        private bool isEnable = true;
        private TransformedBitmap titleImage;
        private TransformedBitmap answerImage;

        public TransformedBitmap AnswerImage
        {
            get { return answerImage; }
            set { answerImage = value; RaisePropertyChanged(); }
        }


        public TransformedBitmap TitleImage
        {
            get { return titleImage; }
            set { titleImage = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 用于控制图片的鼠标事件,防止多次点击
        /// </summary>
        public bool IsEnable
        {
            get { return isEnable; }
            set { isEnable = value; RaisePropertyChanged(); }
        }


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

        /// <summary>
        /// 右边图片悬浮显示
        /// </summary>
        public string RightElevation
        {
            get { return rightElevation; }
            set { rightElevation = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 左边图片悬浮显示
        /// </summary>
        public string LeftElevation
        {
            get { return leftElevation; }
            set { leftElevation = value; RaisePropertyChanged(); }
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
        /// 用于存放训练记录
        /// </summary>
        public TrainRecordRaise CurrTrainRecord
        {
            get { return currTrainRecord; }
            set { currTrainRecord = value; RaisePropertyChanged(); }
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

        #region 页面进入退出时操作
        /// <summary>
        /// 进入页面是否重用实例 - 否
        /// </summary>
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
            synthesizer.SpeakAsyncCancelAll();
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
                CurrTrainRecord = InitTrainRecord(TrainInfo);
                SpliteImage();

                synthesizer.SpeakAsync("请选择匹配上图的图片");
            }
        }
        #endregion

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
                CurrTrainRecord.Id = trainRecordId;
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
        private TrainRecordRaise InitTrainRecord(TrainRaise trainInfo)
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

        /// <summary>
        /// 防止多次点击的计时器
        /// </summary>
        private void StartTimer()
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.5);
            timer.Tick += (sender, args) =>
            {
                IsEnable = true;
                ((DispatcherTimer)sender).Stop();
            };
            timer.Start();
        }

        private void SpliteImage()
        {
            string fileName = CurrQuestion.CorrectAnswer.Picture;
            var baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/");
            var imagePath = Path.Combine("Image", fileName);
            var imagePathRelativeToBaseDirectory = Path.Combine(baseDirectory, imagePath);
            fileName = imagePathRelativeToBaseDirectory.ToString();
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(fileName);
            image.EndInit();

            // 定义矩形区域
            Int32Rect rect1 = new Int32Rect(0, 0, image.PixelWidth / 2, image.PixelHeight);
            Int32Rect rect2 = new Int32Rect(image.PixelWidth / 2, 0, image.PixelWidth / 2, image.PixelHeight);

            // 创建CroppedBitmap
            CroppedBitmap cb1 = new CroppedBitmap(image, rect1);
            CroppedBitmap cb2 = new CroppedBitmap(image, rect2);

            // 创建TransformedBitmap
            TitleImage = new TransformedBitmap(cb1, new ScaleTransform(-1, 1));
            AnswerImage = new TransformedBitmap(cb2, new ScaleTransform(-1, 1));

        }
    }
}
