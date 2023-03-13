using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Shard.Models
{
    public class TrainRecord : BaseModel
    {
		private string name;
		private string content;
		private TimeSpan cost;


		/// <summary>
		/// 训练时间
		/// </summary>
		public TimeSpan Cost
		{
			get { return cost; }
			set { cost = value; }
		}


		/// <summary>
		/// 训练内容
		/// </summary>
		public string Content
		{
			get { return content; }
			set { content = value; }
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
