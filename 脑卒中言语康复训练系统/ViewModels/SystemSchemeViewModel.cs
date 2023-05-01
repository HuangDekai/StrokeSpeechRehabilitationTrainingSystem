using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 脑卒中言语康复训练系统.Models;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.ViewModels
{
    internal class SystemSchemeViewModel : BindableBase
    {
        public SystemSchemeViewModel() { 
            CreateData();
        }

        private ObservableCollection<SchemeLookRaise> schemeLooks;

        public ObservableCollection<SchemeLookRaise> SchemeLooks
        {
            get { return schemeLooks; }
            set { schemeLooks = value; RaisePropertyChanged(); }
        }

        private void CreateData()
        {
            SchemeLooks = new ObservableCollection<SchemeLookRaise>();

            for (int i = 0; i < 5; i++)
            {
                ObservableCollection<SchemeItemRaise> list = new ObservableCollection<SchemeItemRaise>();
                for (int j = 1; j <= 10 + i; j++)
                {
                    list.Add(new SchemeItemRaise { Order = i, Name = "Test" + i, Level = i, Quantity = 5 });
                }
                SchemeLooks.Add(new SchemeLookRaise { Name = "Project" + i, Projects = list});
            }
        }
    }
}
