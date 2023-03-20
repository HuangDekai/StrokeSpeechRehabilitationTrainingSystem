using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Shard.Models
{
    public class SchemeLook
    {
        private string name;

        private List<SchemeItem> projects;

        public List<SchemeItem> Projects
        {
            get { return projects; }
            set { projects = value; }
        }


        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
