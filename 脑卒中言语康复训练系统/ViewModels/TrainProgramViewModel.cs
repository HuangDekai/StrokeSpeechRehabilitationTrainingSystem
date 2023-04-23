using Prism.Commands;
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
using 脑卒中言语康复训练系统.Models;
using 脑卒中言语康复训练系统.Shard.Helper;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels
{
    public class TrainProgramViewModel : BindableBase
    {
        TrainProgramViewModel(IDialogHostService dialogService, IRegionManager regionManager) 
        {
            this.dialogService= dialogService;
            this.regionManager = regionManager;
            ToTrainCommand = new DelegateCommand<string>(ToTrain);
            GetTrains();
        }

        public DelegateCommand<string> ToTrainCommand { get; private set; }

        private async void ToTrain(Object obj)
        {
            var isLogin = await LoginVerification();
            if (!isLogin)
            {
                return;
            }
            var parameters = new DialogParameters();
            parameters.Add("Name", obj.ToString());

            var dialogResult = await dialogService.ShowDialog("TrainQuestionCoverView", parameters);
            if (dialogResult != null && dialogResult.Result == ButtonResult.OK)
            {
                var TrainInfo = dialogResult.Parameters.GetValue<TrainRaise>("TrainInfo");
                if (TrainInfo != null)
                {
                    var navigationParam = new NavigationParameters();
                    navigationParam.Add("ExaminationInfo", TrainInfo);
                    parameters.Add("createNewRegionInstance", true);
                    string trainViewName = TrainInfo.Type + "View";
                    regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(trainViewName, navigationParam);
                }
            }
        }

        #region 属性
        private static SqLiteHelper sqlHelper;
        private readonly IDialogHostService dialogService;
        private readonly IRegionManager regionManager;
        private ObservableCollection<Train> trains;
        
        /// <summary>
        /// 用于存储展示的训练信息
        /// </summary>
        public ObservableCollection<Train> Trains
        {
            get { return trains; }
            set { trains = value; RaisePropertyChanged(); }
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

        /// <summary>
        /// 获取SQLite Connection
        /// </summary>
        private static void GetConnetion()
        {
            string name = AppDomain.CurrentDomain.BaseDirectory;
            string path = System.IO.Directory.GetParent(name).Parent.Parent.Parent.Parent.FullName;
            sqlHelper = new SqLiteHelper("data source = " + path + "\\脑卒中言语康复训练系统.Shard\\Graduate.db");
        }

        /// <summary>
        /// 从数据库中获取存有的量表名称并赋值给 Trains
        /// </summary>
        private void GetTrains()
        {
            Trains = new ObservableCollection<Train>();
            GetConnetion();
            var reader = sqlHelper.ReadFullTable("Train");

            while (reader.Read())
            {
                Trains.Add(new Train() { Name = reader.GetString(reader.GetOrdinal("Name")) });
            }
            reader.Close();
            sqlHelper.CloseConnection();
        }
    }
}
