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
		private int userId;
        private DateTime startTime;
        private DateTime endTime;
		private int sort;

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
        /// 训练结束时间
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
		/// 训练时间,冗余字段,需要结束时间-开始时间
		/// </summary>
		public TimeSpan Cost
		{
			get { return cost; }
			set { cost = value; }
		}


		/// <summary>
		/// 训练内容, 冗余字段, 需要根据 TrainRecord.TrainId = Train.Id 从 Train 中获取
		/// </summary>
		public string Content
		{
			get { return content; }
			set { content = value; }
		}


        /// <summary>
        /// 训练名称, 冗余字段, 需要根据 TrainRecord.TrainId = Train.Id 从 Train 中获取
        /// </summary>
        public string Name
		{
			get { return name; }
			set { name = value; }
		}

	}
}
