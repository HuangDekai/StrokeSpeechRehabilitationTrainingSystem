using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Shard.Models
{
    public class Ability : BaseModel
    {
		private string content;

		/// <summary>
		/// 能力内容
		/// </summary>
		public string Content
		{
			get { return content; }
			set { content = value; }
		}


	}
}
