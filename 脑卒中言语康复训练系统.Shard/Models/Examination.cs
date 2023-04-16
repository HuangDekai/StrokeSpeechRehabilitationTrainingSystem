using System;
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
