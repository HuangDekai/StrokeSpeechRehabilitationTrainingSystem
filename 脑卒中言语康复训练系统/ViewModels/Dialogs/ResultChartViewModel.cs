using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.Common.Tools;
using 脑卒中言语康复训练系统.Models;
using 脑卒中言语康复训练系统.Shard.Helper;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels.Dialogs
{
    internal class ResultChartViewModel : BindableBase, IDialogHostAware
    {
        public ResultChartViewModel()
        {
            CancelCommand = new DelegateCommand(Cancel);
            UserInfo = LoginVerificationTool.GetLoginUserInfo();
        }

        #region 属性
        private static SqLiteHelper sqlHelper;
        private UserInfo userInfo;
        private TrainRecordRaise currTrainRecord;
        private ISeries[] reactionSeries;
        private LabelVisual reactionTitle;
        private ISeries[] retrySeries;
        private LabelVisual retryTitle;
        private string trainTimeCost;
        private int correct;
        private double correctRate;

        /// <summary>
        /// 正确率, Correct / (Correct + Retry)
        /// </summary>
        public double CorrectRate
        {
            get { return correctRate; }
            set { correctRate = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 作对题目数,即每个回答记录里的 Score 之和
        /// </summary>
        public int Correct
        {
            get { return correct; }
            set { correct = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 就是 CurrTrainRecord.Cost.ToString(@"hh\:mm\:ss")
        /// </summary>
        public string TrainTimeCost
        {
            get { return trainTimeCost; }
            set { trainTimeCost = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 折线图X轴设定
        /// </summary>
        public Axis[] XAxes { get; set; } =
        {
            new Axis
            {
                // forces the step of the axis to be at least 1
                MinStep = 1,
            }
        };

        /// <summary>
        /// 反应时间图表标题数据
        /// </summary>
        public LabelVisual RetryTitle
        {
            get { return retryTitle; }
            set { retryTitle = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 反应时间图表数据
        /// </summary>
        public ISeries[] RetrySeries
        {
            get { return retrySeries; }
            set { retrySeries = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 反应时间图表标题数据
        /// </summary>
        public LabelVisual ReactionTitle
        {
            get { return reactionTitle; }
            set { reactionTitle = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 反应时间图表数据
        /// </summary>
        public ISeries[] ReactionSeries
        {
            get { return reactionSeries; }
            set { reactionSeries = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 用于存放记录信息
        /// </summary>
        public TrainRecordRaise CurrTrainRecord
        {
            get { return currTrainRecord; }
            set { currTrainRecord = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 用于存放用户信息
        /// </summary>
        public UserInfo UserInfo
        {
            get { return userInfo; }
            set { userInfo = value; RaisePropertyChanged(); }
        }

        #endregion
        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogHost.Close(DialogHostName);
            }
        }

        public void OnDialogOpend(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("TrainRecordId"))
            {
                int TrainRecordId = parameters.GetValue<int>("TrainRecordId");
                CurrTrainRecord = GetTrainRecordRaise(TrainRecordId);

                InitChart(CurrTrainRecord);
            }
        }

        /// <summary>
        /// 在 CurrTrainRecord 被赋值之后, 将里面的数据放到 Reactions 和 Replays 中便于图表显示
        /// </summary>
        private void InitChart(TrainRecordRaise trainRecord)
        {
            TrainTimeCost = trainRecord.Cost.ToString(@"hh\:mm\:ss");
            var retryTime = 0;
            foreach (var item in trainRecord.TrainQuestionRecords)
            {
                Correct += item.Score;
                retryTime += item.Retry;
            }
            CorrectRate = double.Parse(((double)(Correct * 100) / (Correct + retryTime)).ToString("0.00"));

            ReactionTitle = new LabelVisual
            {
                TextSize = 18,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };
            RetryTitle = new LabelVisual
            {
                TextSize = 18,
                Padding = new LiveChartsCore.Drawing.Padding(15),
                Paint = new SolidColorPaint(SKColors.DarkSlateGray)
            };

            ReactionSeries = new ISeries[1];
            var points = new LogarithmicPoint[CurrTrainRecord.TrainQuestionRecords.Count];
            for (int i = 0; i < CurrTrainRecord.TrainQuestionRecords.Count; i++)
            {
                points[i] = new LogarithmicPoint { X = i + 1, Y = CurrTrainRecord.TrainQuestionRecords[i].Cost.TotalSeconds};
            }
            ReactionSeries[0] = new LineSeries<LogarithmicPoint>
            {
                Mapping = (logPoint, chartPoint) =>
                {
                    // for the x coordinate, we use the X property of the LogaritmicPoint instance
                    chartPoint.SecondaryValue = logPoint.X;
                    // but for the Y coordinate, we will map to the logarithm of the value
                    chartPoint.PrimaryValue = logPoint.Y;
                },
                Values = points,
                Name = "ReactionTime",
                Fill = null,
            };

            RetrySeries = new ISeries[1];
            var retryPoints = new LogarithmicPoint[CurrTrainRecord.TrainQuestionRecords.Count];
            for (int i = 0; i < CurrTrainRecord.TrainQuestionRecords.Count; i++)
            {
                retryPoints[i] = new LogarithmicPoint { X = i + 1, Y = CurrTrainRecord.TrainQuestionRecords[i].Retry };
            }
            RetrySeries[0] = new LineSeries<LogarithmicPoint>
            {
                Mapping = (logPoint, chartPoint) =>
                {
                    // for the x coordinate, we use the X property of the LogaritmicPoint instance
                    chartPoint.SecondaryValue = logPoint.X;
                    // but for the Y coordinate, we will map to the logarithm of the value
                    chartPoint.PrimaryValue = logPoint.Y;
                },
                Values = retryPoints,
                Name = "Retry",
                Fill = null,
                LineSmoothness = 0
            };
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

        private static TrainRecordRaise GetTrainRecordRaise(int id)
        {
            var trainRecord = new TrainRecordRaise();
            GetConnetion();
            string sql = "SELECT * FROM TrainRecord JOIN Train ON TrainRecord.TrainId = Train.Id WHERE TrainRecord.Id = " + id;
            var reader = sqlHelper.ExecuteQuery(sql);
            if (reader.Read())
            {
                trainRecord.Id = id;
                trainRecord.UserId = reader.GetInt32(reader.GetOrdinal("UserId"));
                trainRecord.TrainId = reader.GetInt32(reader.GetOrdinal("TrainId"));
                trainRecord.Name = reader.GetString(reader.GetOrdinal("Name"));
                trainRecord.Content = reader.GetString(reader.GetOrdinal("Content"));
                trainRecord.StartTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("StartTime")));
                trainRecord.EndTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("EndTime")));
                trainRecord.Cost = trainRecord.EndTime - trainRecord.StartTime;
                trainRecord.CreateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("CreateTime")));
                trainRecord.UpdateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("UpdateTime")));
            }
            reader.Close();
            sqlHelper.CloseConnection();

            trainRecord.TrainQuestionRecords = GetAllTrainQuestionRecordRaise(trainRecord);
            return trainRecord;
        }

        /// <summary>
        /// 获取对应 trainRecord 下的所有 TrainQuestionRecord 并返回
        /// </summary>
        /// <param name="trainRecord">带有Id的trainRecord</param>
        /// <returns>trainRecord.TrainQuestionRecords</returns>
        private static ObservableCollection<TrainQuestionRecordRaise> GetAllTrainQuestionRecordRaise(TrainRecordRaise trainRecord)
        {
            var questionRecords = new ObservableCollection<TrainQuestionRecordRaise>();
            GetConnetion();
            string sql = "SELECT * FROM TrainQuestionRecord WHERE TrainRecordId = " + trainRecord.Id;
            var reader = sqlHelper.ExecuteQuery(sql);
            while (reader.Read())
            {
                var record = new TrainQuestionRecordRaise()
                {
                    TrainRecordId = reader.GetInt32(reader.GetOrdinal("TrainRecordId")),
                    TrainQuestionId = reader.GetInt32(reader.GetOrdinal("TrainQuestionId")),
                    Retry = reader.GetInt32(reader.GetOrdinal("Retry")),
                    Score = reader.GetInt32(reader.GetOrdinal("Score")),
                    StartTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("StartTime"))),
                    EndTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("EndTime"))),
                    UpdateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("UpdateTime"))),
                    CreateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("CreateTime"))),
                };
                record.Cost = record.EndTime - record.StartTime;
                questionRecords.Add(record);
            }
            reader.Close();
            sqlHelper.CloseConnection();
            return questionRecords;
        }
    }
}
