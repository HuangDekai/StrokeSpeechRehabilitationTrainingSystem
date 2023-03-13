using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Shard.Models
{
	/// <summary>
	/// 测评记录, 用于 UserView 中的测评数据展示
	/// </summary>
    public class ExaminationRecord : BaseModel
    {
		private string name;
		private string normal;
		private double score;
		private TimeSpan cost;
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
		/// 测评用时
		/// </summary>
		public TimeSpan Cost
		{
			get { return cost; }
			set { cost = value; }
		}

		/// <summary>
		/// 测评得分
		/// </summary>
		public double Score
		{
			get { return score; }
			set { score = value; }
		}



		/// <summary>
		/// 正常区间
		/// </summary>
		public string Normal
		{
			get { return normal; }
			set { normal = value; }
		}


		/// <summary>
		/// 测评名称
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

	}
}
