/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CRL;
namespace CRL.Package.Business.TradeType
{
    public class TradeTypeManage : Category.CategoryBusiness<TradeTypeManage, TradeType>
    {
        public new static TradeTypeManage Instance
        {
            get { return new TradeTypeManage(); }
        }
        public string GetTradeTypeName(int tradeCode, Enum dataType)
        {
            return GetTradeTypeName(tradeCode, Convert.ToInt32(dataType));
        }
        /// <summary>
        /// 按账户类型获取交易类型名称
        /// </summary>
        /// <param name="tradeCode"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public string GetTradeTypeName(int tradeCode, int dataType)
        {
            string t = tradeCode.ToString();
            t = t.Length % 2 > 0 ? t.Substring(0, 1) : t.Substring(0, 2);//取账户类型
            var result = GetAllCache(Convert.ToInt32(t)).Where(b => int.Parse(b.TradeCode) == tradeCode);
            if (result.Count()==0)
                return tradeCode.ToString();
            return result.First().Name;
        }
        /// <summary>
        /// 添加一个交易类型
        /// 格式:帐户类型+交易方向+SequenceCode
        /// </summary>
        /// <param name="parentCode"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public TradeType AddTrade(string parentCode, TradeType item)
        {
            if (item.DataType == 0)
            {
                throw new Exception("item.DataType == 0");
            }
            var obj = Add(parentCode, item);
            string tradeCode = string.Format("{0}{1}{2}", item.DataType.ToString().PadLeft(2, '0'), Convert.ToInt32(item.TradeDirection).ToString().PadLeft(2, '0'), item.SequenceCode);
            CRL.ParameCollection c = new CRL.ParameCollection();
            c["tradeCode"] = tradeCode;
            Update(b => b.Id == obj.Id, c);
            return obj;
        }
        /// <summary>
        /// 导出所有交易类型
        /// </summary>
        /// <returns></returns>
        public string ExportClass(Type transactionType)
        {
            string str = @"
/// <summary>
/// 交易类型,由数据导出生成
/// from Aika.Core.Business.TradeType.TradeTypeManage.ExportClass()
/// </summary>
public class GlobalTradeType
    {";
            foreach (var type in Enum.GetValues(transactionType))
            {
                int dataType = (int)type;
                string typeName = type.ToString();
                var list = QueryList(b => b.DataType == dataType && b.TradeDirection == TradeDirection.收入 && b.ParentCode != "").OrderBy(b => b.SequenceCode);
                var list2 = QueryList(b => b.DataType == dataType && b.TradeDirection == TradeDirection.支出 && b.ParentCode != "").OrderBy(b => b.SequenceCode);

                str += @"
#region {0}
public class {0}
{
#region 收入
/// <summary>
/// 收入
/// </summary>
public enum 收入
    {";
                str = str.Replace("{0}", typeName);
                foreach (var item in list)
                {
                    string name = item.Name.Replace(" ", "");
                    str += string.Format(@"
/// <summary>
/// {2} {3}
/// </summary>
{0}={1},
", name, item.TradeCode, item.TradeCode, item.Remark);
                }
                str += @"   }
#endregion
";

                str += @"
#region 支出
/// <summary>
/// 支出
/// </summary>
public enum 支出
    {";
                foreach (var item in list2)
                {
                    string name = item.Name.Replace(" ", "");
                    str += string.Format(@"
/// <summary>
/// {2}
/// </summary>
{0}={1},
", name, item.TradeCode, item.TradeCode);
                }
                str += @"}
}
#endregion
#endregion
";
            }
            str += @"
}";
            //ExportToFile();
            return str;
        }
    }
}
