﻿using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using 脑卒中言语康复训练系统.Common;
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
            NextCommand = new DelegateCommand(Next);
            ReplayCommand = new DelegateCommand(Replay);
            synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
        }

        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand NextCommand { get; set; }
        public DelegateCommand ReplayCommand { get; set; }

        /// <summary>
        /// ReplayCommand 绑定方法, 重播该问题
        /// </summary>
        private void Replay()
        {
            recognitionEngine.RecognizeAsyncStop();
            synthesizer.SpeakAsync(CurrQuestion.Content);
        }

        /// <summary>
        /// NextCommand 绑定方法, 下一个问题
        /// </summary>
        private void Next()
        {
            if (CurrItemIndex < MaxItemIndex)
            {
                CurrItemIndex++;
                CurrQuestion = TrainInfo.TrainQuestions[CurrItemIndex - 1];
                IsBtnGroupShow = 1;
                recognitionEngine.RecognizeAsyncStop();
                synthesizer.SpeakAsync(CurrQuestion.Content);
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
        /// 用于绑定 synthesizer.SpeakCompleted 事件, 在异步语音读完后执行
        /// </summary>
        private void Synthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {

            IsBtnGroupShow = 0;
            synthesizer.Speak("请回答");
            recognitionEngine.RecognizeAsync(RecognizeMode.Single);
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
        }

        /// <summary>
        /// 在导航到页面时被调用
        /// </summary>
        /// <param name="navigationContext">传入的内容</param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("TrainInfo"))
            {
                journal = navigationContext.NavigationService.Journal;
                TrainInfo = navigationContext.Parameters.GetValue<TrainRaise>("TrainInfo");
                MaxItemIndex = navigationContext.Parameters.ContainsKey("MaxItemIndex") ? navigationContext.Parameters.GetValue<int>("MaxItemIndex") : 5;
                GetTrainQuestions();
                CurrQuestion = TrainInfo.TrainQuestions[0];
                InitSpeechRecognitionEngine(TrainInfo);
                synthesizer.SpeakAsync(CurrQuestion.Content);
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
            //创建语音接收事件
            recognitionEngine.SpeechRecognized += (s, e) => {
                if (e.Result.Confidence >= 0.5)
                {
                    SpeechResult = e.Result.Text;
                    if (SpeechResult.Equals(CurrQuestion.CorrectAnswer.Content))
                    {
                        CorrectAnwer();
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
            Replay();
        }

        /// <summary>
        /// 答案正确执行该函数
        /// </summary>
        private void CorrectAnwer()
        {
            synthesizer.Rate = 1;
            synthesizer.Speak("答对了");
            synthesizer.Rate = 0;
            Next();
        }

        /// <summary>
        /// 答案错误执行该函数
        /// </summary>
        private void ErrorAnwer()
        {
            synthesizer.Rate = 1;
            synthesizer.Speak("回答错误,再来一次");
            synthesizer.Rate = 0;
            Replay();
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

    }
}
