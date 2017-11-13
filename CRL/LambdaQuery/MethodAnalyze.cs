/**
* CRL 快速开发框架 V4.0
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
    internal delegate string MethodHandler(CRLExpression.MethodCallObj methodInfo,ref int parIndex, AddParameHandler addParame);
    internal delegate void AddParameHandler(string name, object value);
    internal class MethodAnalyze
    {
        public MethodAnalyze(DBAdapter.DBAdapterBase _dBAdapter)
        {
            //var table = TypeCache.GetTable(typeof(T));
            dBAdapter = _dBAdapter;
            if (dBAdapter == null)
            {
                throw new CRLException("dBAdapter尚未初始化");
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
                methodDic.Add("Length", Length);
                methodDic.Add("Len", Length);
                methodDic.Add("Trim", Trim);
                methodDic.Add("TrimStart", TrimStart);
                methodDic.Add("TrimEnd", TrimEnd);
                methodDic.Add("In", In);
                methodDic.Add("Substring", Substring);
                methodDic.Add("COUNT", Count);
                methodDic.Add("Count", Count);
                methodDic.Add("SUM", Sum);
                methodDic.Add("MAX", Max);
                methodDic.Add("MIN", Min);
                methodDic.Add("AVG", AVG);
                methodDic.Add("Equals", Equals);
                methodDic.Add("StartsWith", StartsWith);
                methodDic.Add("IsNullOrEmpty", IsNullOrEmpty);
                methodDic.Add("ToString", CaseToType);
                methodDic.Add("ToInt32", CaseToType);
                methodDic.Add("ToDecimal", CaseToType);
                methodDic.Add("ToDouble", CaseToType);
                methodDic.Add("ToBoolean", CaseToType);
                methodDic.Add("ToDateTime", CaseToType);
                methodDic.Add("ToInt16", CaseToType);
                methodDic.Add("Parse", CaseToType);
                methodDic.Add("ToSingle", CaseToType);
                methodDic.Add("ToUpper", ToUpper);
                methodDic.Add("ToLower", ToLower);
                methodDic.Add("IsNull", IsNull);
                methodDic.Add("Replace", Replace);
            }
            return methodDic;
        }
        string GetParamName(string name, object index)
        {
            return dBAdapter.GetParamName(name, index);
        }
        public string IsNull(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            var args = methodInfo.Args.First();
            string parName = GetParamName("isnull", parIndex);
            parIndex += 1;
            addParame(parName, args);
            return dBAdapter.IsNull(field, parName);
        }
        public string ToUpper(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            return dBAdapter.ToUpperFormat(field);
        }
        public string ToLower(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            return dBAdapter.ToLowerFormat(field);
        }

        public string Length(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            return dBAdapter.LengthFormat(field);
        }
        public string Trim(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            return dBAdapter.Trim(field);
        }
        public string TrimStart(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            return dBAdapter.TrimStart(field);
        }
        public string TrimEnd(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            return dBAdapter.TrimEnd(field);
        }
        public string IsNullOrEmpty(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            var isNot = methodInfo.ExpressionType == ExpressionType.Not;
            var notStr = dBAdapter.IsNotFormat(isNot);
            var result = string.Format("({0} {1} null {3} {0}{2}'')", field, notStr, isNot ? "!=" : "=", isNot ? "and" : "or");
            return result;
        }
        public string CaseToType(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            if (string.IsNullOrEmpty(field))//按转换常量算
            {
                string parName = GetParamName("case", parIndex);
                parIndex += 1;
                addParame(parName, methodInfo.Args.First());
                field = parName;
            }
            return dBAdapter.CastField(field, methodInfo.ReturnType);
        }

        public string Substring(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            var nodeType = methodInfo.ExpressionType;
            var args = methodInfo.Args;
            if (args.Count < 2)
            {
                throw new CRLException("Substring扩展方法需要两个参数,index,length");
            }
            return dBAdapter.SubstringFormat(field, (int)args[0], (int)args[1]);
            //return string.Format(" SUBSTRING({0},{1},{2})", field, args[0], args[1]);
        }
        public string StringLike(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            var nodeType = methodInfo.ExpressionType;
            var args = methodInfo.Args;
            return StringLikeFull(methodInfo, ref parIndex, addParame, "%{0}%");
        }
        public string StringLikeLeft(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            //string str = args[0].ToString();
            //str = str.Replace("%", "");
            return StringLikeFull(methodInfo,ref parIndex, addParame, "%{0}");
        }
        public string StringLikeRight(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            //string str = args[0].ToString();
            //str = str.Replace("%","");
            return StringLikeFull(methodInfo, ref parIndex, addParame, "{0}%");
        }
        string StringLikeFull(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame,string likeFormat)
        {
            var field = methodInfo.MemberQueryName;
            var nodeType = methodInfo.ExpressionType;
            var args = methodInfo.Args[0];
            string parName = GetParamName("like", parIndex);
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
        public string StringContains(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            var nodeType = methodInfo.ExpressionType;
            var args = methodInfo.Args;
            string parName = GetParamName("contains", parIndex);
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
        public string Between(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            var nodeType = methodInfo.ExpressionType;
            var args = methodInfo.Args;
            string parName = GetParamName("between", parIndex);
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
            string parName2 = GetParamName("between", parIndex);
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
        public string DateTimeDateDiff(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            var nodeType = methodInfo.ExpressionType;
            var args = methodInfo.Args;
            string parName = GetParamName("dateDiff", parIndex);
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
            //DateDiff(2015/2/5 17:59:44,t1.AddTime,DateDiff1)>1 
            return dBAdapter.DateDiffFormat(field, args[0].ToString(), parName);
            //return string.Format("DateDiff({0},{1},{2}){3}", args[0], field, parName, args[2]);
        }
        public string Replace(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            var args = methodInfo.Args;
            string parName = GetParamName("repd", parIndex);
            var args1 = args[0];
            if (args1 is ExpressionValueObj)
            {
                parName = args1.ToString();
            }
            else
            {
                addParame(parName, args1);
            }
            var args2= args[1];
            parIndex += 1;
            string parName2 = GetParamName("repd", parIndex);
            addParame(parName2, args2);
            parIndex += 1;
            //DateDiff(2015/2/5 17:59:44,t1.AddTime,DateDiff1)>1 
            return dBAdapter.Replace(field, parName, parName2);
        }
        #region 函数
        public string Count(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            return string.Format("count({0})", field);
        }

        public string Sum(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            return string.Format("sum({0})", field);
        }
        public string Max(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            return string.Format("max({0})", field);
        }
        public string Min(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            return string.Format("min({0})", field);
        }
        public string AVG(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            return string.Format("avg({0})", field);
        }
        #endregion
        string InFormat(object value, ref int parIndex, AddParameHandler addParame)
        {
            string str = "";
            var par2 = value;
            if (par2 is string)
            {
                string parName = GetParamName("in", parIndex);
                addParame(parName, value);
                str = parName;
            }
            else if (par2 is string[])
            {
                var list = par2 as Array;
                if (list.Length == 0)
                {
                    throw new CRLException("in 参数为空");
                }
                var max = list.Length > 1000;//超出直接拼字符串
                foreach (var s in list)
                {
                    string parName;
                    if (max)
                    {
                        parName = string.Format("'{0}'", s);
                    }
                    else
                    {
                        parName = GetParamName("in", parIndex);
                        addParame(parName, s);
                        parIndex += 1;
                    }
                    str += string.Format("{0},", parName);
                }
                if (str.Length > 1)
                {
                    str = str.Substring(0, str.Length - 1);
                }
            }
            else//按数字
            {
                var list = par2 as Array;
                if (list.Length == 0)
                {
                    throw new CRLException("in 参数为空");
                }
                var max = list.Length > 1000;//超出直接拼字符串
                foreach (var s in list)
                {
                    string parName;
                    if (max)
                    {
                        parName = s.ToString();
                    }
                    else
                    {
                        parName = GetParamName("in", parIndex);
                        addParame(parName, (int)s);
                        parIndex += 1;
                    }
                    str += string.Format("{0},", parName);
                }
                if (str.Length > 1)
                {
                    str = str.Substring(0, str.Length - 1);
                }
            }
            return str;
        }
        public string In(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            var nodeType = methodInfo.ExpressionType;
            var args = methodInfo.Args;
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
        public string Equals(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            var nodeType = methodInfo.ExpressionType;
            var args = methodInfo.Args;
            string parName = GetParamName("equalEnum", parIndex);
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
            var operate = ExpressionVisitor.ExpressionTypeCast(nodeType);
            return string.Format("{0}{2}{1}", field, parName, operate);
            //if (nodeType == ExpressionType.Equal)
            //{
            //    return string.Format("{0}={1}", field, parName);
            //}
            //else
            //{
            //    return string.Format("{0}!={1}", field, parName);
            //}
        }
        public string StartsWith(CRLExpression.MethodCallObj methodInfo, ref int parIndex, AddParameHandler addParame)
        {
            var field = methodInfo.MemberQueryName;
            var nodeType = methodInfo.ExpressionType;
            var args = methodInfo.Args;
            var par = args[0].ToString();
            string parName = GetParamName("startsWith", parIndex);
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

            var operate = ExpressionVisitor.ExpressionTypeCast(nodeType);
            return string.Format("{0}{2}{1}", str, parName, operate);
            //if (nodeType == ExpressionType.Equal)
            //{
            //    return string.Format("{0}={1}", str, parName);
            //}
            //else
            //{
            //    return string.Format("{0}!={1}", str, parName);
            //}
        }
    }
}
