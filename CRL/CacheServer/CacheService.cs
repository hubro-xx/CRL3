using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.CacheServer
{
    /// <summary>
    /// 查询服务
    /// </summary>
    public class CacheService
    {
        /// <summary>
        /// 按json格式查询
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string Deal(string command)
        {
            var commandObj = Command.FromJson(command);
            //var obj = LambdaQuery.CRLQueryExpression.FromJson(commandObj.Data);
            return Deal(commandObj);
        }
        /// <summary>
        /// 按CRLExpression 查询
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        internal static string Deal(CacheServer.Command command)
        {
            if (command.CommandType == CommandType.获取配置)
            {
                return CoreHelper.StringHelper.SerializerToJson(CacheServerSetting.ServerTypeSetting);
            }
            var type = command.ObjectType;
            if (!CacheServerSetting.CacheServerDealDataRules.ContainsKey(type))
            {
                return "error,服务端未找到SettingConfig.CacheServerDealDataRules对应的处理:" + type;
            }
            var handler = CacheServerSetting.CacheServerDealDataRules[type];
            var data = handler(command);
            return CoreHelper.StringHelper.SerializerToJson(data);
        }
    }
}
