using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Shard.Models
{
    public class TrainQuestion : BaseModel
    {
		private int trainId;
		private string content;
		private int quantity;
		private int correctAnswerId;

		/// <summary>
		/// 正确答案的Id
		/// </summary>
		public int CorrectAnswerId
		{
			get { return correctAnswerId; }
			set { correctAnswerId = value; }
		}

		/// <summary>
		/// 问题选项数
		/// </summary>
		public int Quantity
		{
			get { return quantity; }
			set { quantity = value; }
		}

		/// <summary>
		/// 问题内容
		/// </summary>
		public string Content
		{
			get { return content; }
			set { content = value; }
		}

		/// <summary>
		/// 训练项目Id
		/// </summary>
		public int TrainId
		{
			get { return trainId; }
			set { trainId = value; }
		}

	}
}
