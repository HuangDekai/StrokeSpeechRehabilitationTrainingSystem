using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Common.Events
{
    public class UpdateModel
    {
        public bool IsOpen { get; set; }
    }
    class UpdateLoadingEvent : PubSubEvent<UpdateModel>
    {
    }
}
