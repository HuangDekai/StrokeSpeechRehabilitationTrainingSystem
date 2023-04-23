using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Shard.Models
{
    public class Train : BaseModel
    {
		private string name;
		private int quantity;
		private string content;

		/// <summary>
		/// 训练内容
		/// </summary>
		public string Content
		{
			get { return content; }
			set { content = value; }
		}

		/// <summary>
		/// 训练题数
		/// </summary>
		public int Qutantity
		{
			get { return quantity; }
			set { quantity = value; }
		}

		/// <summary>
		/// 训练名称
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

	}
}
