using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.Models
{
    public class ExaminationRecordRaise : BaseModelRaise
    {
        private string name;
        private string normal;
        private double score;
        private TimeSpan cost;
        private string content;
        private int userId;
        private DateTime startTime;
        private DateTime endTime;
        private int sort;
        private int examinationId;
        private ObservableCollection<QuestionRecordRaise> questionRecords;

        /// <summary>
        /// 冗余字段,存储各个的问题记录
        /// </summary>
        public ObservableCollection<QuestionRecordRaise> QuestionRecords
        {
            get { return questionRecords; }
            set { questionRecords = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 对应的问卷的Id
        /// </summary>
        public int ExaminationId
        {
            get { return examinationId; }
            set { examinationId = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 冗余字段,用于表中排序序号
        /// </summary>
        public int Sort
        {
            get { return sort; }
            set { sort = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 测评开始时间
        /// </summary>
        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 测评结束时间
        /// </summary>
        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 用户ID, 关联 UserInfo 的 Id
        /// </summary>
        public int UserId
        {
            get { return userId; }
            set { userId = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 训练内容,冗余字段
        /// </summary>
        public string Content
        {
            get { return content; }
            set { content = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 测评用时,冗余字段
        /// </summary>
        public TimeSpan Cost
        {
            get { return cost; }
            set { cost = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 测评得分
        /// </summary>
        public double Score
        {
            get { return score; }
            set { score = value; RaisePropertyChanged(); }
        }



        /// <summary>
        /// 正常区间
        /// </summary>
        public string Normal
        {
            get { return normal; }
            set { normal = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 测评名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged(); }
        }
    }
}
