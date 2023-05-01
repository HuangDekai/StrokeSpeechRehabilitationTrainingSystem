using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.Models;
using 脑卒中言语康复训练系统.Shard.Helper;

namespace 脑卒中言语康复训练系统.ViewModels.Dialogs
{
    class SystemNameUpdateViewModel : BindableBase, IDialogHostAware
    {
        public SystemNameUpdateViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            QuerySystem();
            Name = (string)SystemRaise.Name.Clone();
        }

        public string DialogHostName { get; set; }
        private static SqLiteHelper sqlHelper;
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged(); }
        }

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        private SystemRaise systemRaise;

        public SystemRaise SystemRaise
        {
            get { return systemRaise; }
            set { systemRaise = value; RaisePropertyChanged(); }
        }


        public void OnDialogOpend(IDialogParameters parameters)
        {
            
        }

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
                UpdateSystem();
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK));
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

        private void QuerySystem()
        {
            GetConnetion();
            var reader = sqlHelper.ReadFullTable("System");
            while (reader.Read())
            {
                SystemRaise = new SystemRaise()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    CreateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("CreateTime"))),
                    UpdateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("UpdateTime"))),
                };
            }
        }

        private void UpdateSystem()
        {
            GetConnetion();
            string[] colName = { "Name", "UpdateTime" };
            string[] values = { Name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") };
            sqlHelper.UpdateValues("System", colName, values, "Id", 0.ToString());
        }

    }
}
