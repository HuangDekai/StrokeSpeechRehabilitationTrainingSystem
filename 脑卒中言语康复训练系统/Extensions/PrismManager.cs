using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 脑卒中言语康复训练系统.Extensions
{
    /// <summary>
    /// 用于存放各个页面的 RegionName, 便于复用和修改
    /// </summary>
    public class PrismManager
    {
        /// <summary>
        /// 首页区域
        /// </summary>
        public static readonly string MainViewRegionName = "MainViewRegion";

        /// <summary>
        /// 设置页区域
        /// </summary>
        public static readonly string SettingsViewRegionName = "SettingsViewRegion";
    }
}
