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
    internal class SystemSchemeViewModel : BindableBase
    {
        public SystemSchemeViewModel() { 
            CreateData();
        }

        private ObservableCollection<SchemeLook> schemeLooks;

        public ObservableCollection<SchemeLook> SchemeLooks
        {
            get { return schemeLooks; }
            set { schemeLooks = value; RaisePropertyChanged(); }
        }

        private void CreateData()
        {
            SchemeLooks = new ObservableCollection<SchemeLook>();

            List<SchemeItem> list = new List<SchemeItem>();
            for (int i = 1; i < 6; i++)
            {
                list.Add(new SchemeItem { Order = i, Name = "Test" + i, Level = i, Quantity=5});
            }

            for (int i = 0; i < 5; i++)
            {
                SchemeLooks.Add(new SchemeLook { Name = "Project" + i, Projects = list});
            }
        }
    }
}
