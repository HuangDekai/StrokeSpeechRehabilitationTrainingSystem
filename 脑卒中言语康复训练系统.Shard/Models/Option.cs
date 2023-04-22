using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Shard.Models
{
	/// <summary>
	/// 选项实体
	/// </summary>
    public class Option : BaseModel
    {
		private double weight;
		private string content;
		private int sort;

		/// <summary>
		/// 冗余字段, 用于排序
		/// </summary>
		public int Sort
		{
			get { return sort; }
			set { sort = value; }
		}


		/// <summary>
		/// 内容,选项的题目内容
		/// </summary>
		public string Content
		{
			get { return content; }
			set { content = value; }
		}

		/// <summary>
		/// 权重,该选项的分数
		/// </summary>
		public double Weight
		{
			get { return weight; }
			set { weight = value; }
		}

	}
}
