using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.Models
{
    public class QuestionRecordRaise : BaseModelRaise
    {
        private int questionId;
        private int examinationRecordId;
        private int optionId;
        private OptionRaise option;
        private string content;

        /// <summary>
        /// 冗余字段,存放对应问题(questionId)的内容
        /// </summary>
        public string Content
        {
            get { return content; }
            set { content = value; }
        }


        /// <summary>
        /// 冗余字段,存放OptionId对应的问题
        /// </summary>
        public OptionRaise Option
        {
            get { return option; }
            set { option = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 最后选择那一项选项的Id
        /// </summary>
        public int OptionId
        {
            get { return optionId; }
            set { optionId = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 所属的记录Id
        /// </summary>
        public int ExaminationRecordId
        {
            get { return examinationRecordId; }
            set { examinationRecordId = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 要记录的问题的Id
        /// </summary>
        public int QuestionId
        {
            get { return questionId; }
            set { questionId = value; RaisePropertyChanged(); }
        }
    }
}
