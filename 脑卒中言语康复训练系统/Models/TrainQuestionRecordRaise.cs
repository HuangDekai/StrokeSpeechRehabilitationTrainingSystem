using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Models
{
    public class TrainQuestionRecordRaise : BaseModelRaise
    {
        private int trainQuestionId;
        private int trainRecordId;
        private DateTime startTime;
        private DateTime endTime;
        private TimeSpan cost;

        /// <summary>
        /// 冗余字段, EndTime - StartTime, 回答问题时花费时间
        /// </summary>
        public TimeSpan Cost
        {
            get { return cost; }
            set { cost = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 回答该问题时的结束时间
        /// </summary>
        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 回答该问题时的开始时间
        /// </summary>
        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 训练问题记录所属的问题记录的Id
        /// </summary>
        public int TrainRecordId
        {
            get { return trainRecordId; }
            set { trainRecordId = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 训练问题记录对应的问题Id
        /// </summary>
        public int TrainQuestionId
        {
            get { return trainQuestionId; }
            set { trainQuestionId = value; RaisePropertyChanged(); }
        }

    }
}
