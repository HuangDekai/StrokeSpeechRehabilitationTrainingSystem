using MaterialDesignThemes.Wpf;
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
using 脑卒中言语康复训练系统.Models;
using 脑卒中言语康复训练系统.Shard.Helper;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels.Dialogs
{
    public class TrainQuestionCoverViewModel : BindableBase, IDialogHostAware
    {
        public TrainQuestionCoverViewModel() { 
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }
        
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogHost.Close(DialogHostName);
            }
        }

        private void Save()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                var parameters = new DialogParameters();
                parameters.Add("TrainInfo", TrainInfo);
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, parameters));
            }
        }

        #region 属性
        public string DialogHostName { get; set; }
        private static SqLiteHelper sqlHelper;

        private string name;

        /// <summary>
        /// 训练名, 由 TrainView 及其 Model 传入
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged(); }
        }

        private TrainRaise trainInfo;

        /// <summary>
        /// 训练实体,存储训练相关信息
        /// </summary>
        public TrainRaise TrainInfo
        {
            get { return trainInfo; }
            set { trainInfo = value; RaisePropertyChanged(); }
        }

        #endregion

        /// <summary>
        /// 在 Dialog 被打开时
        /// </summary>
        /// <param name="parameters">传入的参数</param>
        public void OnDialogOpend(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("Name"))
            {
                Name = parameters.GetValue<string>("Name");
                GetTrain();
            }
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
        public void GetTrain()
        {
            GetConnetion();
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
        }

        public ObservableCollection<AbilityRaise> GetAbilityRaise(TrainRaise train)
        {
            GetConnetion();
            string sql = "SELECT A.* FROM Train T " +
                "LEFT JOIN Train_Ability TA ON T.Id = TA.TrainId " +
                "LEFT JOIN Ability A ON TA.AbilityId = A.Id WHERE T.Name = '" + train.Name +"'";
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
    }
}
