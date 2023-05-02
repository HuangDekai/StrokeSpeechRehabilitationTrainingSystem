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
using 脑卒中言语康复训练系统.Common.Tools;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.Extensions;
using 脑卒中言语康复训练系统.Models;
using 脑卒中言语康复训练系统.Shard.Helper;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels
{
    internal class SystemSchemeViewModel : BindableBase
    {
        public SystemSchemeViewModel(IRegionManager regionManager, IDialogHostService dialogService) {
            this.regionManager = regionManager;
            this.dialogService = dialogService;
            GetSchemeLooks();

            SelectCommand = new DelegateCommand<object>(select);
            DeleteCommand = new DelegateCommand<object>(delete);
            AddCommand = new DelegateCommand(add);
        }

        public DelegateCommand<object> SelectCommand { get; set; }
        public DelegateCommand<object> DeleteCommand { get; set; }
        public DelegateCommand AddCommand { get; set; }

        public async void delete(object obj)
        {
            var isLogin = await LoginVerification();
            if (!isLogin)
            {
                return;
            }
            SchemeLookRaise scheme = obj as SchemeLookRaise;
            var param = new DialogParameters();
            param.Add("Title", "温馨提示");
            param.Add("Message", "是否确认删除[" + scheme.Name + "]?");
            var res = await dialogService.ShowDialog("MessageBoxView", param);
            if (res != null && res.Result == ButtonResult.OK)
            {
                DeleteSchemeLook(scheme.Id);
                GetSchemeLooks();
            }
        }

        public async void add()
        {
            var isLogin = await LoginVerification();
            if (!isLogin)
            {
                return;
            }
            var param = new DialogParameters();
            param.Add("UserId", 0);
            await dialogService.ShowDialog("AddSchemeLookView", param);
            GetSchemeLooks();
        }

        public async void select(object obj)
        {
            var isLogin = await LoginVerification();
            if (!isLogin)
            {
                return;
            }
            if (obj != null)
            {
                var navigationParam = new NavigationParameters();
                var schemes = obj as SchemeLookRaise;

                var trainInfo = GetTrainInfo(SchemeLooks[SelectIdx].Projects[0].Name);
                navigationParam.Add("TrainInfo", trainInfo);
                navigationParam.Add("Scheme", schemes);
                navigationParam.Add("NextTrain", 1);
                navigationParam.Add("MaxItemIndex", schemes.Projects[0].Quantity);
                regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(schemes.Projects[0].Type + "View", navigationParam);
            }
        }

        private static SqLiteHelper sqlHelper;
        private readonly IRegionManager regionManager;
        private readonly IDialogHostService dialogService;
        private ObservableCollection<SchemeLookRaise> schemeLooks;
        private int selectIdx = 0;

        public int SelectIdx
        {
            get { return selectIdx; }
            set { selectIdx = value; RaisePropertyChanged(); }
        }


        public ObservableCollection<SchemeLookRaise> SchemeLooks
        {
            get { return schemeLooks; }
            set { schemeLooks = value; RaisePropertyChanged(); }
        }

        private void GetSchemeLooks()
        {
            int userId = LoginVerificationTool.GetLoginUserId();

            SchemeLooks = new ObservableCollection<SchemeLookRaise>();
            GetConnetion();
            string sql = "SELECT * FROM SchemeLook WHERE UserId = " + 0;
            var reader = sqlHelper.ExecuteQuery(sql);
            while (reader.Read())
            {
                var schemelook = new SchemeLookRaise()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    UpdateTime = reader.GetDateTime(reader.GetOrdinal("UpdateTime")),
                    CreateTime = reader.GetDateTime(reader.GetOrdinal("CreateTime")),
                };
                schemelook.Projects = GetSchemeItems(schemelook);
                SchemeLooks.Add(schemelook);
            }
            reader.Close();
            sqlHelper.CloseConnection();
        }

        private ObservableCollection<SchemeItemRaise> GetSchemeItems(SchemeLookRaise schemeLook)
        {
            var projects = new ObservableCollection<SchemeItemRaise>();
            GetConnetion();
            string sql = "SELECT * FROM SchemeItem LEFT JOIN Train ON SchemeItem.TrainId = Train.Id WHERE SchemeLookId = " + schemeLook.Id;
            var reader = sqlHelper.ExecuteQuery(sql);
            int order = 1;
            while (reader.Read())
            {
                projects.Add(new SchemeItemRaise()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Order = order++,
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                    Type = reader.GetString(reader.GetOrdinal("Type")),
                    SchemeLookId = reader.GetInt32(reader.GetOrdinal("SchemeLookId")),
                    UpdateTime = reader.GetDateTime(reader.GetOrdinal("UpdateTime")),
                    CreateTime = reader.GetDateTime(reader.GetOrdinal("CreateTime")),
                });
            }
            reader.Close();
            sqlHelper.CloseConnection();
            return projects;
        }

        /// <summary>
        /// 获取SQLite Connection
        /// </summary>
        private static void GetConnetion()
        {
            string name = AppDomain.CurrentDomain.BaseDirectory;
            string path = System.IO.Directory.GetParent(name).FullName;
            sqlHelper = new SqLiteHelper("data source = " + path + "\\Graduate.db");
        }


        /// <summary>
        /// 获取对应名的Train信息
        /// </summary>
        public TrainRaise GetTrainInfo(string Name)
        {
            GetConnetion();
            TrainRaise trainInfo = null;
            string sql = "select * from Train where Name = '" + Name + "'";
            var reader = sqlHelper.ExecuteQuery(sql);
            if (reader.Read())
            {
                trainInfo = new TrainRaise()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Content = reader.GetString(reader.GetOrdinal("Content")),
                    Type = reader.GetString(reader.GetOrdinal("Type")),
                    CreateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("CreateTime"))),
                    UpdateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("UpdateTime"))),
                };
            }
            reader.Close();
            sqlHelper.CloseConnection();

            trainInfo.Abilities = GetAbilityRaise(trainInfo);
            return trainInfo;
        }

        public ObservableCollection<AbilityRaise> GetAbilityRaise(TrainRaise train)
        {
            GetConnetion();
            string sql = "SELECT A.* FROM Train T " +
                "LEFT JOIN Train_Ability TA ON T.Id = TA.TrainId " +
                "LEFT JOIN Ability A ON TA.AbilityId = A.Id WHERE T.Name = '" + train.Name + "'";
            var reader = sqlHelper.ExecuteQuery(sql);
            var abilities = new ObservableCollection<AbilityRaise>();
            while (reader.Read())
            {
                var ability = new AbilityRaise()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Content = reader.GetString(reader.GetOrdinal("Content")),
                    CreateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("CreateTime"))),
                    UpdateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("UpdateTime"))),
                };
                abilities.Add(ability);
            }
            reader.Close();
            sqlHelper.CloseConnection();
            return abilities;
        }

        private void DeleteSchemeLook(int id)
        {
            GetConnetion();
            string sql = "delete from SchemeLook Where id = " + id;
            sqlHelper.ExecuteQuery(sql);
            sql = "delete from SchemeItem Where SchemeLookId = " + id;
            sqlHelper.ExecuteQuery(sql);
            sqlHelper.CloseConnection();
        }

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
    }
}
