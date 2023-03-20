using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Shard.Models
{
    /// <summary>
    /// 用户方案/系统方案
    /// </summary>
    public class SchemeItem
    {
		private int order;

		private string name;

		private int level;

		private int quantity;

		/// <summary>
		/// 序号
		/// </summary>
		public int Order
		{
			get { return order; }
			set { order = value; }
		}

		/// <summary>
		/// 数量
		/// </summary>
		public int Quantity
		{
			get { return quantity; }
			set { quantity = value; }
		}


		/// <summary>
		/// 难度
		/// </summary>
		public int Level
		{
			get { return level; }
			set { level = value; }
		}


		/// <summary>
		/// 项目名称
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

	}
}
