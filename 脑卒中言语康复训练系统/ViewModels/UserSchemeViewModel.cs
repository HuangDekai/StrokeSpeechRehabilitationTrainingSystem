using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml.Linq;
using 脑卒中言语康复训练系统.Extensions;
using 脑卒中言语康复训练系统.Models;
using 脑卒中言语康复训练系统.Shard.Helper;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels
{
    class UserSchemeViewModel : BindableBase
    {
        public UserSchemeViewModel(IRegionManager regionManager) {
            this.regionManager = regionManager;
            CreateData();

            SelectCommand = new DelegateCommand<object>(select);
        }

        public DelegateCommand<object> SelectCommand { get; set; }

        public void select(object obj)
        {
            if (obj != null)
            {
                var navigationParam = new NavigationParameters();
                var schemes = obj as SchemeLookRaise;

                var trainInfo = GetTrainInfo(SchemeLooks[SelectIdx].Projects[0].Name);
                navigationParam.Add("TrainInfo", trainInfo);
                navigationParam.Add("Scheme", schemes);
                navigationParam.Add("NextTrain", 1);
                navigationParam.Add("MaxItemIndex", schemes.Projects[0].Quantity);
                regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(SchemeLooks[SelectIdx].Projects[0].Type + "View", navigationParam);
            }
        }

        private static SqLiteHelper sqlHelper;
        private readonly IRegionManager regionManager;
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

        private void CreateData()
        {
            SchemeLooks = new ObservableCollection<SchemeLookRaise>();

            ObservableCollection<SchemeItemRaise> list = new ObservableCollection<SchemeItemRaise>();
            list.Add(new SchemeItemRaise { Order = 1, Name = "看词选图训练", Type = "WordMatchingTrain", Quantity = 1 });
            list.Add(new SchemeItemRaise { Order = 1, Name = "看词选图训练", Type = "WordMatchingTrain", Quantity = 2 });
            list.Add(new SchemeItemRaise { Order = 1, Name = "残缺图片匹配训练", Type = "IncompleteImageMatchingTrain", Quantity = 3 });
            list.Add(new SchemeItemRaise { Order = 1, Name = "字词发音训练", Type = "WordPronunciationTrain", Quantity = 1 });
            list.Add(new SchemeItemRaise { Order = 1, Name = "听理解训练", Type = "AuditoryComprehensionTrain", Quantity = 5 });
            

            SchemeLooks.Add(new SchemeLookRaise { Name = "系统项目", Projects = list });
            
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
    }
}
