using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using 脑卒中言语康复训练系统.Common;
using 脑卒中言语康复训练系统.ViewModels;
using 脑卒中言语康复训练系统.ViewModels.Dialogs;
using 脑卒中言语康复训练系统.ViewModels.Trains;
using 脑卒中言语康复训练系统.Views;
using 脑卒中言语康复训练系统.Views.Dialogs;
using 脑卒中言语康复训练系统.Views.Trains;

namespace 脑卒中言语康复训练系统
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }

        /// <summary>
        /// 调用 MainWindow 里的 service.Configure() 初始化, 实现默认首页
        /// </summary>
        protected override void OnInitialized()
        {
            var service = App.Current.MainWindow.DataContext as IConfigurationService;
            if(service != null)
            {
                service.Configure();
            }
            base.OnInitialized();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IDialogHostService, DialogHostService>();

            containerRegistry.RegisterForNavigation<UserLoginView, UserLoginViewModel>();
            containerRegistry.RegisterForNavigation<MessageBoxView, MessageBoxViewModel>();
            containerRegistry.RegisterForNavigation<QuestionCoverView, QuestionCoverViewModel>();
            containerRegistry.RegisterForNavigation<QuestionItemView, QuestionItemViewModel>();
            containerRegistry.RegisterForNavigation<TrainQuestionCoverView, TrainQuestionCoverViewModel>();
            containerRegistry.RegisterForNavigation<MessageBoxOnlySureView, MessageBoxOnlySureViewModel>();
            containerRegistry.RegisterForNavigation<ResultChartView, ResultChartViewModel>();
            containerRegistry.RegisterForNavigation<ExaminationChartView,ExaminationChartViewModel>();
            containerRegistry.RegisterForNavigation<UserInfoView, UserInfoViewModel>();
            containerRegistry.RegisterForNavigation<DepartmentNameUpdateView, DepartmentNameUpdateViewModel>();
            containerRegistry.RegisterForNavigation<SystemNameUpdateView, SystemNameUpdateViewModel>();

            containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
            containerRegistry.RegisterForNavigation<UserView, UserViewModel>();
            containerRegistry.RegisterForNavigation<UserManagementView, UserManagementViewModel>();
            containerRegistry.RegisterForNavigation<ExaminationView, ExaminationViewModel>();
            containerRegistry.RegisterForNavigation<SystemSchemeView, SystemSchemeViewModel>();
            containerRegistry.RegisterForNavigation<UserSchemeView, UserSchemeViewModel>();
            containerRegistry.RegisterForNavigation<TrainProgramView, TrainProgramViewModel>();
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();
            
            containerRegistry.RegisterForNavigation<WordPronunciationTrainView, WordPronunciationTrainViewModel>();
            containerRegistry.RegisterForNavigation<AuditoryComprehensionTrainView, AuditoryComprehensionTrainViewModel>();
            containerRegistry.RegisterForNavigation<WordMatchingTrainView, WordMatchingTrainViewModel>();
            containerRegistry.RegisterForNavigation<IncompleteImageMatchingTrainView, IncompleteImageMatchingTrainViewModel>();
        }
    }
}
