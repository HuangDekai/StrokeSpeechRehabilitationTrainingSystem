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
    public class DepartmentNameUpdateViewModel : BindableBase, IDialogHostAware
    {
        public DepartmentNameUpdateViewModel() 
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }
        public string DialogHostName { get; set; }
        private static SqLiteHelper sqlHelper;
        private string name;
        private DepartmentRaise department;

        public DepartmentRaise Department
        {
            get { return department; }
            set { department = value; RaisePropertyChanged(); }
        }

        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged(); }
        }

        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public void OnDialogOpend(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("Department"))
            {
                Department = parameters.GetValue<DepartmentRaise>("Department");
                if (Department != null)
                {
                    Name = (string)Department.Name.Clone();
                } else
                {
                    Department = new DepartmentRaise();
                    Department.Id = -1;
                }
            }
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
                if (Department.Id != -1)
                {
                    UpdateDepartment();
                }
                else
                {
                    AddDepartment();
                }
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

        private void UpdateDepartment()
        {
            GetConnetion();
            string[] colName = { "Name", "UpdateTime" };
            string[] values = { Name, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") };
            sqlHelper.UpdateValues("Department", colName, values, "Id", Department.Id.ToString());
        }

        private void AddDepartment()
        {
            GetConnetion();
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string sql = "Insert into Department(Name, Createtime, Updatetime) Values (" +
                "'" + Name + "'," +
                "'" + time + "'," +
                "'" + time + "')";
            sqlHelper.ExecuteQuery(sql);
        }
    }
}
