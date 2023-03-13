using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Shard.Models
{
    public class UserManagement
    {
		private int id;
		private string name;
		private string order;
		private DateTime birth;
		private short gender;
		private string department;
		private string phone;

		public int Id
		{
			get { return id; }
			set { id = value; }
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
		/// 所属科室
		/// </summary>
		public string Department
		{
			get { return department; }
			set { department = value; }
		}


		/// <summary>
		/// 性别 0-女 1-男
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
		/// 用户编号
		/// </summary>
		public string Order
		{
			get { return order; }
			set { order = value; }
		}


		/// <summary>
		/// 用户姓名
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

	}
}
