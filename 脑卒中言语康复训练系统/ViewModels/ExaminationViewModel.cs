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
using 脑卒中言语康复训练系统.Common.Tools;
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

        public DelegateCommand<string> ToExaminationCommand { get; private set; }

        /// <summary>
        /// ToExaminationCommand 绑定的方法, 进入 QuestionCoverView 这个 Dialog, 若在 QuestionCoverView 确认, 则进入 QuestionItemView 问卷界面
        /// </summary>
        /// <param name="obj"></param>
        private async void ToExamination(Object obj)
        {
            var isLogin = await LoginVerification();
            if (!isLogin)
            {
                return;
            }
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

        /// <summary>
        /// 用于判断是否登录了,若未登录,进行拦截
        /// </summary>
        /// <returns>登录(true)/未登录(false)</returns>
        private async Task<bool> LoginVerification()
        {
            bool isSuccess = true;
            if (!LoginVerificationTool.IsLogin())
            {
                var parameters = new DialogParameters();
                parameters.Add("Title", "温馨提示");
                parameters.Add("Message", "请先登录再进行量表测评!");
                await dialogService.ShowDialog("MessageBoxView", parameters);
                isSuccess = false;
            }
            return isSuccess;
        }

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
