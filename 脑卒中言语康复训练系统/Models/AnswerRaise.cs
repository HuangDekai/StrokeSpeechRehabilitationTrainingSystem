using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Models
{
    public class AnswerRaise : BaseModelRaise
    {
        private string content;
        private string picture;
        private int groupId;

        /// <summary>
        /// 分组Id, 用于给答案分类
        /// </summary>
        public int GroupId
        {
            get { return groupId; }
            set { groupId = value; }
        }

        private bool isCorrect;

        /// <summary>
        /// 冗余字段,用于判断该答案是不是正确答案
        /// </summary>
        public bool IsCorrect
        {
            get { return isCorrect; }
            set { isCorrect = value; }
        }


        /// <summary>
        /// 图片相对地址
        /// </summary>
        public string Picture
        {
            get { return picture; }
            set { picture = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 答案内容
        /// </summary>
        public string Content
        {
            get { return content; }
            set { content = value; RaisePropertyChanged(); }
        }
    }
}
