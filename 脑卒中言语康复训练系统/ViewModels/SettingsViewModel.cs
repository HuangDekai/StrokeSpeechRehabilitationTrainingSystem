using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.Models;
using 脑卒中言语康复训练系统.Shard.Helper;

namespace 脑卒中言语康复训练系统.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        public SettingsViewModel(IDialogHostService dialogService) 
        { 
            this.dialogService = dialogService;

            ToUpdateCommand = new DelegateCommand<object>(ToUpdate);
            ToAddCommand = new DelegateCommand(ToAdd);
            ToUpdateSystemNameCommand = new DelegateCommand(ToUpdateSystemName);
            DeleteCommand = new DelegateCommand<object>(Delete);
            UploadCommand = new DelegateCommand(Upload);

            Departments = GetDepartments();
            SystemRaise = GetSystemRaise();
            
        }

        public DelegateCommand ToUpdateSystemNameCommand { get; set; }
        public DelegateCommand<object> ToUpdateCommand { get; set; }
        public DelegateCommand ToAddCommand { get; set; }
        public DelegateCommand<object> DeleteCommand { get; set; }
        public DelegateCommand UploadCommand { get; set; }

        private async void Upload()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            string fileName = null;
            if (openFileDialog.ShowDialog() == true)
            {
                var baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                var imageDirectory = Path.Combine(baseDirectory, "Image");
                Directory.CreateDirectory(imageDirectory);

                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var extension = Path.GetExtension(openFileDialog.FileName);
                string logoName = "logo";
                fileName = $"{timestamp}_{logoName}{extension}";
                var destinationPath = Path.Combine(imageDirectory, fileName);

                File.Copy(openFileDialog.FileName, destinationPath);
            }
            if (fileName != null)
            {
                var param = new DialogParameters();
                param.Add("Title", "温馨提示");
                param.Add("Message", "确认修改Logo吗?");
                var res = await dialogService.ShowDialog("MessageBoxView", param);
                if (res != null && res.Result == ButtonResult.OK)
                {
                    SystemRaise.Logo = fileName;
                    UploadLogo();
                }
                var p = new DialogParameters();
                p.Add("Title", "温馨提示");
                p.Add("Message", "单位LOGO修改成功,下次打开时将全局生效");
                await dialogService.ShowDialog("MessageBoxOnlySureView",p);
            }
        }

        private async void ToUpdateSystemName()
        {
            var param = new DialogParameters();
            var res = await dialogService.ShowDialog("SystemNameUpdateView", param);
            if (res != null && res.Result == ButtonResult.OK)
            {
                var p = new DialogParameters();
                p.Add("Title", "温馨提示");
                p.Add("Message", "单位名称修改成功,下次打开时将全局生效");
                await dialogService.ShowDialog("MessageBoxOnlySureView", p);
                SystemRaise = GetSystemRaise();
            }
        }

        private async void Delete(object obj)
        {
            var partment = obj as DepartmentRaise;
            var param = new DialogParameters();
            param.Add("Title", "删除");
            param.Add("Message", "确定要删除 [" + partment.Name + "] ?");
            var res = await dialogService.ShowDialog("MessageBoxView", param);
            if (res != null && res.Result == ButtonResult.OK)
            {
                DeleteDepartment(partment.Id);
            }
            Departments = GetDepartments();
        }

        private async void ToAdd()
        {
            var param = new DialogParameters();
            param.Add("Department", null);
            var res = await dialogService.ShowDialog("DepartmentNameUpdateView", param);
            if (res != null && res.Result == ButtonResult.OK)
            {
                Departments = GetDepartments();
            }
        }

        private async void ToUpdate(object obj)
        {
            var param = new DialogParameters();
            param.Add("Department", obj as DepartmentRaise);
            var res = await dialogService.ShowDialog("DepartmentNameUpdateView", param);
            if (res != null && res.Result == ButtonResult.OK)
            {
                Departments = GetDepartments();
            }
        }

        #region 属性
        private static SqLiteHelper sqlHelper;

        private readonly IDialogHostService dialogService;

        private ObservableCollection<DepartmentRaise> departments;
        private SystemRaise systemRaise;

        public SystemRaise SystemRaise
        {
            get { return systemRaise; }
            set { systemRaise = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 用于显示部门列表
        /// </summary>
        public ObservableCollection<DepartmentRaise> Departments
        {
            get { return departments; }
            set { departments = value; RaisePropertyChanged(); }
        }

        #endregion

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
        /// 获取部门
        /// </summary>
        public ObservableCollection<DepartmentRaise> GetDepartments()
        {
            GetConnetion();
            var departments = new ObservableCollection<DepartmentRaise>();
            
            string sql = "select * from Department;";
            var reader = sqlHelper.ExecuteQuery(sql);
            int i = 1;
            while (reader.Read())
            {
                departments.Add(new DepartmentRaise
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Sort = i ++,
                    CreateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("CreateTime"))),
                    UpdateTime = Convert.ToDateTime(reader.GetString(reader.GetOrdinal("UpdateTime"))),
                });
            }
            reader.Close();
            sqlHelper.CloseConnection();

            return departments;
        }

        public void DeleteDepartment(int id)
        {
            GetConnetion();
            string sql = "Delete From Department Where Id = " + id;
            sqlHelper.ExecuteQuery(sql);
        }

        public SystemRaise GetSystemRaise()
        {
            GetConnetion();
            var reader = sqlHelper.ReadFullTable("System");
            SystemRaise raise = null;
            while (reader.Read())
            {
                raise = new SystemRaise()
                {
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Logo = reader.GetString(reader.GetOrdinal("Logo")),
                };
            }
            return raise;
        }

        public void UploadLogo()
        {
            GetConnetion();
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string[] cols = {"Logo","UpdateTime"};
            string[] values = { SystemRaise.Logo, time };
            sqlHelper.UpdateValues("System", cols, values, "Id", 0.ToString());
        }
    }
}
