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
		private int userId;
		private DateTime startTime;
		private DateTime endTime;
        private int sort;
		private int examinationId;

		/// <summary>
		/// 对应的问卷的Id
		/// </summary>
		public int ExaminationId
		{
			get { return examinationId; }
			set { examinationId = value; }
		}


		/// <summary>
		/// 冗余字段,用于表中排序序号
		/// </summary>
		public int Sort
        {
            get { return sort; }
            set { sort = value; }
        }

        /// <summary>
        /// 测评开始时间
        /// </summary>
        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        /// <summary>
        /// 测评结束时间
        /// </summary>
        public DateTime EndTime
		{
			get { return endTime; }
			set { endTime = value; }
		}

		/// <summary>
		/// 用户ID, 关联 UserInfo 的 Id
		/// </summary>
		public int UserId
		{
			get { return userId; }
			set { userId = value; }
		}


        /// <summary>
        /// 训练内容,冗余字段
        /// </summary>
        public string Content
		{
			get { return content; }
			set { content = value; }
		}


		/// <summary>
		/// 测评用时,冗余字段
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
