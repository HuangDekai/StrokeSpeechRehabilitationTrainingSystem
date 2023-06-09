﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Shard.Models
{
    /// <summary>
    /// 用于 QuestionView 展示问卷内容
    /// </summary>
    public class Examination : BaseModel
    {
        private string name;

        private string normal;

        private string content;

        private int quantity;

        private List<Question> questionList;

        /// <summary>
        /// 冗余字段, 用于存储试卷下的问题
        /// </summary>
        public List<Question> QuestionList
        {
            get { return questionList; }
            set { questionList = value; }
        }


        /// <summary>
        /// 问卷题目数量
        /// </summary>
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }


        /// <summary>
        /// 问卷内容, 问卷描述
        /// </summary>
        public string Content
        {
            get { return content; }
            set { content = value; }
        }


        /// <summary>
        /// 正常区间
        /// </summary>
        public string Normal
        {
            get { return normal; }
            set { normal = value; }
        }


        /// <summary>
        /// 问卷名
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

    }
}
