using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Shard.Models
{
    public class QuestionRecord : BaseModel
    {
		private int questionId;
		private int examinationRecordId;
		private int optionId;

		/// <summary>
		/// 最后选择那一项选项的Id
		/// </summary>
		public int OptionId
		{
			get { return optionId; }
			set { optionId = value; }
		}


		/// <summary>
		/// 所属的记录Id
		/// </summary>
		public int ExaminationRecordId
		{
			get { return examinationRecordId; }
			set { examinationRecordId = value; }
		}

		/// <summary>
		/// 要记录的问题的Id
		/// </summary>
		public int QuestionId
		{
			get { return questionId; }
			set { questionId = value; }
		}

	}
}
