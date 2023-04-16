using ImTools;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels
{
    class ExaminationViewModel : BindableBase
    {
		public ExaminationViewModel(IDialogHostService dialogService) 
		{ 
            this.dialogService = dialogService;
            ToExaminationCommand = new DelegateCommand<string>(ToExamination);
            CreateData();
        }

        private async void ToExamination(Object obj)
        {
            var parameters = new DialogParameters();
            parameters.Add("Name", obj.ToString());
            var dialogResult = await dialogService.ShowDialog("QuestionCoverView", parameters);
            if (dialogResult != null && dialogResult.Result == ButtonResult.OK)
            {
                
            }
        }

        #region 属性
        private readonly IDialogHostService dialogService;

        private ObservableCollection<ExaminationLook> examinationLooks;
        public ObservableCollection<ExaminationLook> ExaminationLooks
        {
            get { return examinationLooks; }
            set { examinationLooks = value; RaisePropertyChanged(); }
        }
        #endregion
        public DelegateCommand<string> ToExaminationCommand { get; private set; }

        private void CreateData()
		{
			ExaminationLooks = new ObservableCollection<ExaminationLook>();
            ExaminationLooks.Add(new ExaminationLook() { Name = "容纳他人量表"});
            for (int i = 0; i < 10; i++)
			{
				ExaminationLooks.Add(new ExaminationLook() { Name = "容纳他人量表" + i });
			}
		}
	}
}
