using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.Models
{
    public class SchemeLookRaise : BaseModelRaise
    {
        private string name;
        private int userId;
        private ObservableCollection<SchemeItemRaise> projects;

        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        /// <summary>
        /// 冗余字段,存储所属的训练项目
        /// </summary>
        public ObservableCollection<SchemeItemRaise> Projects
        {
            get { return projects; }
            set { projects = value; RaisePropertyChanged(); }
        }


        /// <summary>
        /// 方案名
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged(); }
        }
    }
}
