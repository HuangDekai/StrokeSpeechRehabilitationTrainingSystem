using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using 脑卒中言语康复训练系统.Shard.Helper;
using 脑卒中言语康复训练系统.Shard.Models;

namespace 脑卒中言语康复训练系统.Common.Tools
{
    public class LoginVerificationTool
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

        /// <summary>
        /// 获取对应名的登录用户Id信息
        /// </summary>
        public static int GetLoginUserId()
        {
            GetConnetion();
            string sql = "select LoginUserId from LoginUser";
            var reader = sqlHelper.ExecuteQuery(sql);
            int id = -1;
            if (reader.Read())
            {
                id = reader.GetInt32(reader.GetOrdinal("LoginUserId"));
            }
            reader.Close();
            CloseConnetion();
            return id;
        }

        /// <summary>
        /// 用于判断用户是否登录
        /// </summary>
        /// <returns>是true / 否false</returns>
        public static bool IsLogin()
        {
            return GetLoginUserId() != -1;
        }

        /// <summary>
        /// 获取登录用户信息
        /// </summary>
        /// <returns>登录用户信息 / null</returns>
        public static UserInfo GetLoginUserInfo()
        {
            UserInfo userInfo = null;
            if (IsLogin())
            {
                int id = GetLoginUserId();

                GetConnetion();
                string sql = "select *  from UserInfo Where Id = " + id;
                var reader = sqlHelper.ExecuteQuery(sql);
                if(reader.Read())
                {
                    userInfo = new UserInfo()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Order = reader.GetString(reader.GetOrdinal("Order")),
                        Avatar = reader.GetString(reader.GetOrdinal("Avatar")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Gender = reader.GetInt16(reader.GetOrdinal("Gender")),
                        Birth = reader.GetDateTime(reader.GetOrdinal("Birth")),
                        Phone = reader.GetString(reader.GetOrdinal("Phone")),
                        Department = reader.GetString(reader.GetOrdinal("Department")),
                        Address = reader.GetString(reader.GetOrdinal("Address")),
                        Situation = reader.GetString(reader.GetOrdinal("Situation")),
                        IsShowSelect = 0,
                    };
                }
                CloseConnetion();
            }
            return userInfo;
        }

        /// <summary>
        /// 登录,写入表
        /// </summary>
        /// <param name="UserId">登录用户Id</param>
        /// <returns>是(true)/否(false)登录成功</returns>
        public static bool Login(int UserId)
        {
            bool isSuccess = false;
            if (!IsLogin())
            {
                GetConnetion();
                string sql = "INSERT INTO LoginUser Values (" + UserId + ")";
                sqlHelper.ExecuteQuery(sql);
                isSuccess = true;
                CloseConnetion();
            }
            return isSuccess;
        }

        public static bool Logout()
        {
            bool isSuccess = false;
            if (IsLogin())
            {
                GetConnetion();
                string sql = "DELETE FROM LoginUser";
                sqlHelper.ExecuteQuery(sql);
                isSuccess = true;
                CloseConnetion();
            }
            return isSuccess;
        }
    }
}
