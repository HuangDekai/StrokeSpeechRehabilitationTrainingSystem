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

namespace 脑卒中言语康复训练系统.ViewModels.Dialogs
{
    public class AddSchemeLookViewModel : BindableBase, IDialogHostAware
    {
        public AddSchemeLookViewModel() 
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            AddCommand = new DelegateCommand(Add);
            DeleteCommand = new DelegateCommand(Delete);
        }

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }

        #region 属性
        public string DialogHostName { get; set; }
        private static SqLiteHelper sqlHelper;

        private SchemeLookRaise scheme;
        private ObservableCollection<TrainRaise> trains;
        private int idx = 1;

        /// <summary>
        /// 用于显示项目名称Combox
        /// </summary>
        public ObservableCollection<TrainRaise> Trains
        {
            get { return trains; }
            set { trains = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 用于记录整个训练
        /// </summary>
        public SchemeLookRaise Scheme
        {
            get { return scheme; }
            set { scheme = value; RaisePropertyChanged(); }
        }

        #endregion
        private void Delete()
        {
            if (Scheme.Projects.Count > 1)
            {
                Scheme.Projects.RemoveAt(Scheme.Projects.Count - 1);
            }
        }

        private void Add()
        {
            Scheme.Projects.Add(new SchemeItemRaise
            {
                Order = idx++,
                Quantity = 5,
            });
        }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.Cancel));
            }
        }

        private void Save()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                DateTime time = DateTime.Now;
                Scheme.UpdateTime = time;
                Scheme.CreateTime = time;
                
                InsertSchemeLook(Scheme);
                Scheme.Id = GetLastSchemeLookId();

                foreach (var item in Scheme.Projects)
                {
                    if (item.Quantity <= 0)
                    {
                        item.Quantity = 1;
                    }
                    else if (item.Quantity > 20)
                    {
                        item.Quantity = 20;
                    }
                    item.SchemeLookId = Scheme.Id;
                    item.CreateTime = time;
                    item.UpdateTime = time;
                    InsertSchemeItem(item);
                }
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK));
            }
        }

        public void OnDialogOpend(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("UserId"))
            {
                Scheme = new SchemeLookRaise();
                Scheme.UserId = parameters.GetValue<int>("UserId");
                Scheme.Projects = new ObservableCollection<SchemeItemRaise>();
               
                Scheme.Projects.Add(new SchemeItemRaise
                {
                    Order = idx++,
                    Quantity = 5, 
                });
           
                Trains = GetTrainRaises();
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

        private static ObservableCollection<TrainRaise> GetTrainRaises()
        {
            GetConnetion();
            var tarins = new ObservableCollection<TrainRaise>();
            var reader = sqlHelper.ReadFullTable("Train");
            while (reader.Read())
            {
                tarins.Add(new TrainRaise()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Type = reader.GetString(reader.GetOrdinal("Type")),
                    Content = reader.GetString(reader.GetOrdinal("Content")),
                    CreateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("CreateTime"))),
                    UpdateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("UpdateTime"))),
                });
            }
            reader.Close();
            sqlHelper.CloseConnection();
            return tarins;
        }

        private static void InsertSchemeLook(SchemeLookRaise scheme)
        {
            GetConnetion();
            string[] valuse = new string[]
            {
                scheme.UserId.ToString(),
                scheme.Name,
                scheme.CreateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                scheme.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            };
            string sql = "INSERT INTO SchemeLook(UserId,Name,CreateTime,UpdateTime) VALUES (" +
                "'" + valuse[0] + "'," +
                "'" + valuse[1] + "'," +
                "'" + valuse[2] + "'," +
                "'" + valuse[3] + "'" +
                ")";
            sqlHelper.ExecuteQuery(sql);
            sqlHelper.CloseConnection();
        }

        private static int GetLastSchemeLookId()
        {
            int id = -1;
            GetConnetion();
            string sql = "SELECT Id FROM SchemeLook Order By Id DESC LIMIT 1";
            var reader = sqlHelper.ExecuteQuery(sql);
            if (reader.Read())
            {
                id = reader.GetInt32(reader.GetOrdinal("Id"));
            }
            reader.Close();
            sqlHelper.CloseConnection();
            return id;
        }

        private static void InsertSchemeItem(SchemeItemRaise schemeItem)
        {
            GetConnetion();
            string[] valuse = new string[]
            {
                schemeItem.SchemeLookId.ToString(),
                (schemeItem.SelectId + 1).ToString(),//TrainId
                schemeItem.Quantity.ToString(),
                schemeItem.CreateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                schemeItem.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            };
            string sql = "INSERT INTO SchemeItem(SchemeLookId,TrainId,Quantity,CreateTime,UpdateTime) VALUES (" +
                "'" + valuse[0] + "'," +
                "'" + valuse[1] + "'," +
                "'" + valuse[2] + "'," +
                "'" + valuse[3] + "'," +
                "'" + valuse[4] + "'" +
                ")";
            sqlHelper.ExecuteQuery(sql);
            sqlHelper.CloseConnection();
        }
    }
}
