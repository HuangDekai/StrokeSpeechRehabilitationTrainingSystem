using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Models
{
    public class BaseModelRaise : BindableBase
    {
        private int id;
        private DateTime createTime;
        private DateTime updateTime;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public DateTime CreateTime
        {
            get { return createTime; }
            set { createTime = value; }
        }

        public DateTime UpdateTime
        {
            get { return updateTime; }
            set { updateTime = value; }
        }
    }
}
