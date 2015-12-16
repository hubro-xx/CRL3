using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.CacheServer
{
    public enum CommandType
    {
        查询,
        更新,
        获取配置
    }
    public class Command
    {
        /// <summary>
        /// 对象类型,FullName
        /// </summary>
        public string ObjectType
        {
            get;
            set;
        }
        public CommandType CommandType
        {
            get;
            set;
        }
        /// <summary>
        /// json data
        /// </summary>
        public string Data
        {
            get;
            set;
        }
        public static Command FromJson(string json)
        {
            var result = (Command)CoreHelper.StringHelper.SerializerFromJSON(System.Text.Encoding.UTF8.GetBytes(json), typeof(Command));
            return result;
        }
    }
}
