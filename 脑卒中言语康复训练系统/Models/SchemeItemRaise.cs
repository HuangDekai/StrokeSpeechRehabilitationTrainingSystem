using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Models
{
    public class SchemeItemRaise : BaseModelRaise
    {
        private int order;

        private string name;

        private int level;

        private int quantity;

        private string type;

        private int schemeLookId;
        private int selectId = 0;

        /// <summary>
        /// 冗余字段,存放选择的训练Id, 数字+1为对应的TrainId
        /// </summary>
        public int SelectId
        {
            get { return selectId; }
            set { selectId = value; RaisePropertyChanged(); }
        }


        public int SchemeLookId
        {
            get { return schemeLookId; }
            set { schemeLookId = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 冗余字段,训练的类型,用于启动项目
        /// </summary>
        public string Type
        {
            get { return type; }
            set { type = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 序号
        /// </summary>
        public int Order
        {
            get { return order; }
            set { order = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 难度
        /// </summary>
        public int Level
        {
            get { return level; }
            set { level = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 项目名称,冗余字段,用于标识相关的训练项目名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged(); }
        }

    }
}
