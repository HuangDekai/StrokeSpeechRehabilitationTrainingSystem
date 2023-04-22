using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Shard.Models
{
    /// <summary>
    /// 题目实体类
    /// </summary>
    public class Question : BaseModel
    {
        private int examinationId;
        private string content;
        private int quantity;
        private int start;
        private int sort;
        private int select;
        private List<Option> optionList;

        /// <summary>
        /// 冗余字段, 用于存储问题下的选项
        /// </summary>
        public List<Option> OptionList
        {
            get { return optionList; }
            set { optionList = value; }
        }

        /// <summary>
        /// 冗余字段, 用于存储选择了哪个选项, 0 - 没选, > 0 - 选项id
        /// </summary>
        public int Select
        {
            get { return select; }
            set { select = value; }
        }

        /// <summary>
        /// 冗余字段, 用于排序
        /// </summary>
        public int Sort
        {
            get { return sort; }
            set { sort = value; }
        }

        /// <summary>
        /// 选项开始位置, 即选项会在[Start, Start + Quantity)位置
        /// </summary>
        public int Start
        {
            get { return start; }
            set { start = value; }
        }

        /// <summary>
        /// 题目选项数量
        /// </summary>
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        /// <summary>
        /// 题目内容
        /// </summary>
        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        /// <summary>
        /// 所属试卷Id
        /// </summary>
        public int ExaminationId
        {
            get { return examinationId; }
            set { examinationId = value; }
        }

    }
}
