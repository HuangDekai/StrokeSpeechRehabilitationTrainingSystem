using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Models
{
	/// <summary>
	/// 选项实体
	/// </summary>
    public class OptionRaise : BaseModelRaise
    {
		private double weight;
		private string content;
		private int sort;
		private bool isChecked;

		/// <summary>
		/// 冗余字段, 用于判断选项是否选中
		/// </summary>
		public bool IsChecked
		{
			get { return isChecked; }
			set { isChecked = value; RaisePropertyChanged(); }
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
