using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.Extensions;
using 脑卒中言语康复训练系统.Models;
using 脑卒中言语康复训练系统.Shard.Helper;

namespace 脑卒中言语康复训练系统.ViewModels
{
    class MainViewModel : BindableBase, IConfigurationService
    {
        public MainViewModel(IRegionManager regionManager)
        {
            MenuBars = new ObservableCollection<MenuBar>();
            SystemRaise = GetSystemRaise();

            NavigateCommand = new DelegateCommand<MenuBar>(Navigate);
            this.regionManager = regionManager;
            GoBackCommand = new DelegateCommand(() =>
            {
                if (journal != null && journal.CanGoBack)
                {
                    journal.GoBack();
                }
            });

            GoForwardCommand = new DelegateCommand(() =>
            {
                if (journal != null && journal.CanGoForward)
                {
                    journal.GoForward();
                }
            });
        }

        private void Navigate(MenuBar obj)
        {
            if (obj.NameSpace == null || string.IsNullOrWhiteSpace(obj.NameSpace))
            {
                return;
            }
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(obj.NameSpace, callback =>
            {
                journal = callback.Context.NavigationService.Journal;
            });
        }

        public DelegateCommand<MenuBar> NavigateCommand { get; private set; }
        public DelegateCommand GoBackCommand { get; private set; }
        public DelegateCommand GoForwardCommand { get; private set; }

        private ObservableCollection<MenuBar> menuBars;
        private static SqLiteHelper sqlHelper;
        private readonly IRegionManager regionManager;
        private IRegionNavigationJournal journal;
        private SystemRaise systemRaise;

        public SystemRaise SystemRaise
        {
            get { return systemRaise; }
            set { systemRaise = value; RaisePropertyChanged(); }
        }


        public ObservableCollection<MenuBar> MenuBars
        {
            get { return menuBars; }
            set
            {
                menuBars = value;
                //通过RaisePropertyChanged来激发属性变更的事件，通知UI
                RaisePropertyChanged();
            }
        }

        void CreateMenuBar()
        {
            MenuBars.Add(new MenuBar { Icon = "AccountOutline", Title = "当前用户", NameSpace = "UserView" });
            MenuBars.Add(new MenuBar { Icon = "ListBoxOutline", Title = "训练项目", NameSpace = "TrainProgramView" });
            MenuBars.Add(new MenuBar { Icon = "AccountDetailsOutline", Title = "用户方案", NameSpace = "UserSchemeView" });
            MenuBars.Add(new MenuBar { Icon = "NotebookPlus", Title = "系统方案", NameSpace = "SystemSchemeView" });
            MenuBars.Add(new MenuBar { Icon = "ClipboardTextMultipleOutline", Title = "量表评估", NameSpace = "ExaminationView" });
            MenuBars.Add(new MenuBar { Icon = "AccountGroupOutline", Title = "用户管理", NameSpace = "UserManagementView" });
            MenuBars.Add(new MenuBar { Icon = "Cog", Title = "系统设置", NameSpace = "SettingsView" });
        }

        /// <summary>
        /// 配置首页初始化参数
        /// </summary>
        public void Configure()
        {
            CreateMenuBar();
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("UserView");
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
    }
}
