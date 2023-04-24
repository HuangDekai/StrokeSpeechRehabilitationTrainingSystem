using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Models
{
    public class TrainQuestionRaise : BaseModelRaise
    {
        private int trainId;
        private string content;
        private int quantity;
        private int correctAnswerId;
        private AnswerRaise correctAnswer;
        private ObservableCollection<AnswerRaise> answers;

        /// <summary>
        /// 冗余字段,用于存储问题下的选项
        /// </summary>
        public ObservableCollection<AnswerRaise> Answers
        {
            get { return answers; }
            set { answers = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 冗余字段,用于存储正确的答案
        /// </summary>
        public AnswerRaise CorrectAnswer
        {
            get { return correctAnswer; }
            set { correctAnswer = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 正确答案的Id
        /// </summary>
        public int CorrectAnswerId
        {
            get { return correctAnswerId; }
            set { correctAnswerId = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 问题选项数
        /// </summary>
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 问题内容
        /// </summary>
        public string Content
        {
            get { return content; }
            set { content = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 训练项目Id
        /// </summary>
        public int TrainId
        {
            get { return trainId; }
            set { trainId = value; RaisePropertyChanged(); }
        }
    }
}
