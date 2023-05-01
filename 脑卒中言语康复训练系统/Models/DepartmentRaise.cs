using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Models
{
    public class DepartmentRaise : BaseModelRaise
    {
		private string name;
		private int sort;

		/// <summary>
		/// 冗余字段,用于排序
		/// </summary>
		public int Sort
		{
			get { return sort; }
			set { sort = value; RaisePropertyChanged(); }
		}

		public string Name
		{
			get { return name; }
			set { name = value; RaisePropertyChanged(); }
		}

	}
}
