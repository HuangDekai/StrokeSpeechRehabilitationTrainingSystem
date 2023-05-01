using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 脑卒中言语康复训练系统.Models;
using 脑卒中言语康复训练系统.Shard.Helper;

namespace 脑卒中言语康复训练系统.Common.Tools
{
    public class TrainQueryTool
    {
        private static SqLiteHelper sqlHelper;

        /// <summary>
        /// 获取SQLite Connection
        /// </summary>
        private static void GetConnetion()
        {
            string name = AppDomain.CurrentDomain.BaseDirectory;
            string path = System.IO.Directory.GetParent(name).FullName;
            sqlHelper = new SqLiteHelper("data source = " + path + "\\Graduate.db");
        }

        private static void CloseConnetion()
        {
            sqlHelper.CloseConnection();
        }

        public static TrainRaise GetTrainRaiseByTrainType(string type)
        {
            GetConnetion();
            string sql = "select * from Train Where Type = '" + type+"'";
            var reader = sqlHelper.ExecuteQuery(sql);
            TrainRaise res = new TrainRaise();
            while (reader.Read())
            {
                res.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                res.Name= reader.GetString(reader.GetOrdinal("Name"));
                res.Content = reader.GetString(reader.GetOrdinal("Content"));
                res.CreateTime = reader.GetDateTime(reader.GetOrdinal("CreateTime"));
                res.UpdateTime = reader.GetDateTime(reader.GetOrdinal("UpdateTime"));
            }
            reader.Close();
            CloseConnetion();
            return res;
        }
    }
}
