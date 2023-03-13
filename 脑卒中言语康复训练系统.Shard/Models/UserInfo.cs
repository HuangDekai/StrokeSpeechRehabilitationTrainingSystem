using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Shard.Models
{
    public class UserInfo : BaseModel
    {
        private string order;
        private string avatar;
        private string name;
        private short gender;
        private DateTime birth;
        private string phone;
        private string department;
        private string address;
        private string situation;

        /// <summary>
        /// 用户编号
        /// </summary>
        public string Order
        {
            get { return order; }
            set { order = value; }
        }

        /// <summary>
        /// 头像地址
        /// </summary>
        public string Avatar
        {
            get { return avatar; }
            set { avatar = value; }
        }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// 性别
        /// </summary>
        public short Gender 
        { 
            get { return gender; } 
            set { gender = value; } 
        }

        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime Birth
        {
            get { return birth; }
            set { birth = value; }
        }

        /// <summary>
        /// 电话
        /// </summary>
        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }


        /// <summary>
        /// 科室
        /// </summary>
        public string Department
        {
            get { return department; }
            set { department = value; }
        }

        /// <summary>
        /// 住址
        /// </summary>
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        /// <summary>
        /// 情况概述
        /// </summary>
        public string Situation
        {
            get { return situation; }
            set { situation = value; }
        }
    }
}
