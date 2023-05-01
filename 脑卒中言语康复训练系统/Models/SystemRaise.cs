using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Models
{
    public class SystemRaise : BaseModelRaise
    {
		private string name;
		private string logo;

		public string Logo
		{
			get { return logo; }
			set { logo = value; RaisePropertyChanged(); }
		}

		public string Name
		{
			get { return name; }
			set { name = value; RaisePropertyChanged(); }
		}

	}
}
