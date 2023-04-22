using ImTools;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.Extensions;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels
{
    class ExaminationViewModel : BindableBase
    {
		public ExaminationViewModel(IDialogHostService dialogService, IRegionManager regionManager) 
		{ 
            this.dialogService = dialogService;
            this.regionManager = regionManager;
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
                var ExaminationInfo = dialogResult.Parameters.GetValue<Examination>("ExaminationInfo");
                if (ExaminationInfo != null)
                {
                    var navigationParam = new NavigationParameters();
                    navigationParam.Add("ExaminationInfo", ExaminationInfo);
                    parameters.Add("createNewRegionInstance", true);
                    regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("QuestionItemView",navigationParam);
                }
            }
        }

        #region 属性
        private readonly IDialogHostService dialogService;
        private readonly IRegionManager regionManager;

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
