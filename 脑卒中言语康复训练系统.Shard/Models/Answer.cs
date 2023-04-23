using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Shard.Models
{
    public class Answer : BaseModel
    {
		private string content;
		private string picture;

		/// <summary>
		/// 图片相对地址
		/// </summary>
		public string Picture
		{
			get { return picture; }
			set { picture = value; }
		}

		/// <summary>
		/// 答案内容
		/// </summary>
		public string Content
        {
			get { return content; }
			set { content = value; }
		}

	}
}
