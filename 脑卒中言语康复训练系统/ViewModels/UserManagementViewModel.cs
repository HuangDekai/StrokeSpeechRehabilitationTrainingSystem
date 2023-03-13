using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels
{
    internal class UserManagementViewModel : BindableBase
    {
        public UserManagementViewModel() 
        {
            CreateData();
        }

        private ObservableCollection<UserManagement> userManagements;

        public ObservableCollection<UserManagement> UserManagements
        {
            get { return userManagements; }
            set { userManagements = value; RaisePropertyChanged(); }
        }

        private void CreateData()
        {
            userManagements = new ObservableCollection<UserManagement>();
            for (int i = 0; i < 30; i++)
            {
                userManagements.Add(new UserManagement
                {
                    Id=i,
                    Name="test"+i,
                    Order="test"+i,
                    Birth=new DateTime(1998,i%11+1,i%29+1),
                    Gender= (short)(i%2),
                    Phone=(138777712345+i).ToString(),
                    Department="康复医疗科",
                });
            }
        }
    }


}
