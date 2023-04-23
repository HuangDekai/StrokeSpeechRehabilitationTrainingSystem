using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Models
{
    public class TrainRaise : BaseModelRaise
    {
        private string name;
        private int quantity;
        private string content;
        private string type;
        private ObservableCollection<TrainQuestionRaise> trainQuestions;
        private ObservableCollection<AbilityRaise> abilities;


        /// <summary>
        /// 冗余字段,用于存储 Ability 要训练的能力
        /// </summary>
        public ObservableCollection<AbilityRaise> Abilities
        {
            get { return abilities; }
            set { abilities = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 冗余字段,用于存储 TrainQuestion 训练问题
        /// </summary>
        public ObservableCollection<TrainQuestionRaise> TrainQuestions
        {
            get { return trainQuestions; }
            set { trainQuestions = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 训练类型,用于打开Type + "View" 的页面
        /// </summary>
        public string Type
        {
            get { return type; }
            set { type = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 训练内容
        /// </summary>
        public string Content
        {
            get { return content; }
            set { content = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 冗余字段,训练题数
        /// </summary>
        public int Qutantity
        {
            get { return quantity; }
            set { quantity = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 训练名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged(); }
        }
    }
}
