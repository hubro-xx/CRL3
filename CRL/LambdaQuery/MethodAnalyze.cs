/**
* CRL 快速开发框架 V3.1
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CRL.LambdaQuery
{
    internal delegate string MethodHandler(string field, ExpressionType nodeType,ref int parIndex, AddParameHandler addParame, params object[] args);
    internal delegate void AddParameHandler(string name, object value);
    internal class MethodAnalyze
    {
        public MethodAnalyze(DBAdapter.DBAdapterBase _dBAdapter)
        {
            //var table = TypeCache.GetTable(typeof(T));
            dBAdapter = _dBAdapter;
            if (dBAdapter == null)
            {
                throw new Exception("dBAdapter尚未初始化");
            }
        }

        DBAdapter.DBAdapterBase dBAdapter;
        static Dictionary<DBAdapter.DBAdapterBase, MethodAnalyze> methodHandlers = new Dictionary<DBAdapter.DBAdapterBase, MethodAnalyze>();
        public static Dictionary<string, MethodHandler> GetMethos(DBAdapter.DBAdapterBase _dBAdapter)
        {
            if(!methodHandlers.ContainsKey(_dBAdapter))
            {
                methodHandlers.Add(_dBAdapter, new MethodAnalyze(_dBAdapter));
            }
            return methodHandlers[_dBAdapter].Methods();
        }
        Dictionary<string, MethodHandler> methodDic;
        public Dictionary<string, MethodHandler> Methods()
        {
            if (methodDic == null)
            {
                methodDic = new Dictionary<string, MethodHandler>();
                methodDic.Add("Like", StringLike);
                methodDic.Add("LikeLeft", StringLikeLeft);
                methodDic.Add("LikeRight", StringLikeRight);
                methodDic.Add("Contains", StringContains);
                methodDic.Add("Between", Between);
                methodDic.Add("DateDiff", DateTimeDateDiff);
                methodDic.Add("In", In);
                methodDic.Add("Substring", Substring);
                methodDic.Add("COUNT", Count);
                methodDic.Add("SUM", Sum);
                methodDic.Add("MAX", Max);
                methodDic.Add("MIN", Min);
                methodDic.Add("AVG", AVG);
                methodDic.Add("Equals", Equals);
                methodDic.Add("StartsWith", StartsWith);
            }
            return methodDic;
        }


        public string Substring(string field, ExpressionType nodeType, ref int parIndex, AddParameHandler addParame, object[] args)
        {
            return dBAdapter.SubstringFormat(field, (int)args[0], (int)args[1]);
            //return string.Format(" SUBSTRING({0},{1},{2})", field, args[0], args[1]);
        }
        public string StringLike(string field, ExpressionType nodeType, ref int parIndex, AddParameHandler addParame, object[] args)
        {
            var args1 = args[0];
            return StringLikeFull(field, nodeType, ref parIndex, addParame, "%{0}%", args1);
        }
        public string StringLikeLeft(string field, ExpressionType nodeType, ref int parIndex, AddParameHandler addParame, object[] args)
        {
            //string str = args[0].ToString();
            //str = str.Replace("%", "");
            var args1 = args[0];
            return StringLikeFull(field, nodeType, ref parIndex, addParame, "%{0}", args1);
        }
        public string StringLikeRight(string field, ExpressionType nodeType, ref int parIndex, AddParameHandler addParame, object[] args)
        {
            //string str = args[0].ToString();
            //str = str.Replace("%","");
            var args1 = args[0];
            return StringLikeFull(field, nodeType, ref parIndex, addParame, "{0}%", args1);
        }
        string StringLikeFull(string field, ExpressionType nodeType, ref int parIndex, AddParameHandler addParame, string likeFormat,object args)
        {
            string parName = string.Format("@like{0}" , parIndex);
            parIndex += 1;

            if (args is ExpressionValueObj)
            {
                parName = args.ToString();
                //if (!parName.ToString().Contains("%"))
                //{
                //    parName = string.Format(likeFormat, parName);
                //}
            }
            else
            {
                if (!args.ToString().Contains("%"))
                {
                    args = string.Format(likeFormat, args);
                }
                addParame(parName, args);
            }
            if (nodeType == ExpressionType.Equal)
            {
                return dBAdapter.StringLikeFormat(field, parName);
            }
            else
            {
                return dBAdapter.StringNotLikeFormat(field, parName);
            }
            //return string.Format("{0} LIKE {1}", field, parName);
        }
        public string StringContains(string field, ExpressionType nodeType, ref int parIndex, AddParameHandler addParame, object[] args)
        {
            string parName = string.Format("@contrains{0}", parIndex);
            var args1 = args[0];
            if (args1 is ExpressionValueObj)
            {
                parName = args1.ToString();
            }
            else
            {
                addParame(parName, args1);
            }
            parIndex += 1;
            if (nodeType == ExpressionType.Equal)
            {
                return dBAdapter.StringContainsFormat(field, parName);
            }
            else
            {
                return dBAdapter.StringNotContainsFormat(field, parName);
            }
            //return string.Format("CHARINDEX({1},{0})>0", field, parName);
        }
        public string Between(string field, ExpressionType nodeType, ref int parIndex, AddParameHandler addParame, object[] args)
        {
            string parName = string.Format("@between{0}", parIndex);
            var args1 = args[0];
            var args2 = args[1];
            if (args1 is ExpressionValueObj)
            {
                parName = args1.ToString();
            }
            else
            {
                addParame(parName, args1);
            }
            parIndex += 1;
            string parName2 = string.Format("@between{0}", parIndex);
            if (args2 is ExpressionValueObj)
            {
                parName2 = args2.ToString();
            }
            else
            {
                addParame(parName2, args2);
            }
            parIndex += 1;
            if (nodeType == ExpressionType.Equal)
            {
                return dBAdapter.BetweenFormat(field, parName, parName2);
            }
            else
            {
                return dBAdapter.NotBetweenFormat(field, parName, parName2);
            }
            //return string.Format("{0} between {1} and {2}", field, parName, parName2);
        }
        public string DateTimeDateDiff(string field, ExpressionType nodeType, ref int parIndex, AddParameHandler addParame, object[] args)
        {
            string parName = string.Format("@DateDiff{0}", parIndex);
            var args1 = args[0];
            if (args1 is ExpressionValueObj)
            {
                parName = args1.ToString();
            }
            else
            {
                addParame(parName, args[1]);
            }
            parIndex += 1;
            //DateDiff(2015/2/5 17:59:44,t1.AddTime,@DateDiff1)>1 
            return dBAdapter.DateDiffFormat(field, args[0].ToString(), parName);
            //return string.Format("DateDiff({0},{1},{2}){3}", args[0], field, parName, args[2]);
        }
        #region 函数
        /// <summary>
        /// 表示COUNT此字段
        /// </summary>
        /// <param name="field"></param>
        /// <param name="parIndex"></param>
        /// <param name="addParame"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string Count(string field, ExpressionType nodeType, ref int parIndex, AddParameHandler addParame, object[] args)
        {
            return string.Format("count({0})", field);
        }
        /// <summary>
        /// 表示SUM此字段
        /// </summary>
        /// <param name="field"></param>
        /// <param name="parIndex"></param>
        /// <param name="addParame"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public string Sum(string field, ExpressionType nodeType, ref int parIndex, AddParameHandler addParame, object[] args)
        {
            return string.Format("sum({0})", field);
        }
        public string Max(string field, ExpressionType nodeType, ref int parIndex, AddParameHandler addParame, object[] args)
        {
            return string.Format("max({0})", field);
        }
        public string Min(string field, ExpressionType nodeType, ref int parIndex, AddParameHandler addParame, object[] args)
        {
            return string.Format("min({0})", field);
        }
        public string AVG(string field, ExpressionType nodeType, ref int parIndex, AddParameHandler addParame, object[] args)
        {
            return string.Format("avg({0})", field);
        }
        #endregion
        string InFormat(object value, ref int parIndex, AddParameHandler addParame)
        {
            string str = "";
            var par2 = value;
            if (par2 is string)
            {
                string parName = string.Format("@in{0}", parIndex);
                addParame(parName, value);
                str = parName;
            }
            else if (par2 is string[])
            {
                IEnumerable list = par2 as IEnumerable;
                foreach (var s in list)
                {
                    string parName = string.Format("@in{0}", parIndex);
                    addParame(parName, s);
                    parIndex += 1;
                    str += string.Format("{0},", parName);
                }
                if (str.Length > 1)
                {
                    str = str.Substring(0, str.Length - 1);
                }
            }
            else//按数字
            {
                IEnumerable list = par2 as IEnumerable;
                foreach (var s in list)
                {
                    string parName = string.Format("@in{0}", parIndex);
                    addParame(parName, (int)s);
                    parIndex += 1;
                    str += string.Format("{0},", parName);
                }
                if (str.Length > 1)
                {
                    str = str.Substring(0, str.Length - 1);
                }
            }
            return str;
        }
        public string In(string field, ExpressionType nodeType, ref int parIndex, AddParameHandler addParame, object[] args)
        {
            string str = InFormat(args[0], ref parIndex, addParame);
            if (nodeType == ExpressionType.Equal)
            {
                return dBAdapter.InFormat(field, str);
            }
            else
            {
                return dBAdapter.NotInFormat(field, str);
            }
            //return string.Format("{0} IN ({1})", field, str);
        }
        public string Equals(string field, ExpressionType nodeType, ref int parIndex, AddParameHandler addParame, object[] args)
        {
            string parName = string.Format("@equalEnum{0}", parIndex);
            parIndex += 1;
            var args1 = args[0];
            if (args1 is ExpressionValueObj)
            {
                parName = args1.ToString();
            }
            else
            {
                args1 = ObjectConvert.ConvertObject(args1.GetType(), args1);
                addParame(parName, args1);
            }
            if (nodeType == ExpressionType.Equal)
            {
                return string.Format("{0}={1}", field, parName);
            }
            else
            {
                return string.Format("{0}!={1}", field, parName);
            }
        }
        public string StartsWith(string field, ExpressionType nodeType, ref int parIndex, AddParameHandler addParame, object[] args)
        {
            var par = args[0].ToString();
            string parName = string.Format("@StartsWith{0}", parIndex);
            parIndex += 1;
            var args1 = args[0];
            if (args1 is ExpressionValueObj)
            {
                parName = args1.ToString();
            }
            else
            {
                addParame(parName, par);
            }
            var str = dBAdapter.SubstringFormat(field, 0, par.Length);
            if (nodeType == ExpressionType.Equal)
            {
                return string.Format("{0}={1}", str, parName);
            }
            else
            {
                return string.Format("{0}!={1}", str, parName);
            }
        }

    }
}
