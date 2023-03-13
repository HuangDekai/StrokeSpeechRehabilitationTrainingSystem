using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels
{
    class ExaminationViewModel : BindableBase
    {
		public ExaminationViewModel() 
		{ 
			CreateData();
		}

		private ObservableCollection<ExaminationLook> examinationLooks;

		public ObservableCollection<ExaminationLook> ExaminationLooks
		{
			get { return examinationLooks; }
			set { examinationLooks = value; RaisePropertyChanged(); }
		}

		private void CreateData()
		{
			ExaminationLooks = new ObservableCollection<ExaminationLook>();
			for (int i = 0; i < 10; i++)
			{
				ExaminationLooks.Add(new ExaminationLook() { Name = "容纳他人量表" + i });
			}
		}
	}
}
