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
using System.Text;
using System.Reflection;
using System.Data;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Diagnostics;
using CRL.LambdaQuery.Mapping;
using System.Collections.Concurrent;

namespace CRL
{
    /// <summary>
    /// 基本方法
    /// </summary>
    public class Base
    {
        //static internal bool UseEmitCreater = true;
        internal static Expression<Func<TModel, bool>> GetQueryIdExpression<TModel>(object id)
        {
            var type = typeof(TModel);
            var table = TypeCache.GetTable(type);
            if (table.PrimaryKey.PropertyType != id.GetType())
            {
                throw new CRLException("参数类型与主键类型定义不一致");
            }
            var parameter = Expression.Parameter(type, "b");
            //创建常数 
            var constant = Expression.Constant(id);
            MemberExpression member = Expression.PropertyOrField(parameter, table.PrimaryKey.MemberName);
            var body = Expression.Equal(member, constant);
            //获取Lambda表达式
            var lambda = Expression.Lambda<Func<TModel, Boolean>>(body, parameter);
            //var pr = lambda.Compile();
            return lambda;
        }
        /// <summary>
        /// 按程序集查找定义过的MODEL
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static Dictionary<Attribute.TableAttribute,AbsDBExtend> GetAllModel(Type baseType)
        {
            //var assemblies = System.Web.Compilation.BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assembyle = System.Reflection.Assembly.GetAssembly(baseType);
            Type[] types = assembyle.GetTypes();
            var findTypes = new Dictionary<Attribute.TableAttribute,AbsDBExtend>();
            var typeCRL = typeof(CRL.IProvider);
            foreach (var type in types)
            {
                if (typeCRL.IsAssignableFrom(type))
                {
                    if (type.ContainsGenericParameters)
                    {
                        continue;
                    }
                        IProvider obj;
                    try
                    {
                        obj = System.Activator.CreateInstance(type) as IProvider;
                    }
                    catch (Exception ero)
                    {
                        throw new Exception(ero.Message+ type);
                    }
                    var table = TypeCache.GetTable(obj.ModelType);
                    var pro = type.GetProperty("DBExtend", BindingFlags.Instance | BindingFlags.NonPublic);
                    var db = pro.GetValue(obj) as AbsDBExtend;
                    if (!findTypes.ContainsKey(table))
                    {
                        findTypes.Add(table, db);
                    }
                }
            }
            return findTypes;
        }

        /// <summary>
        /// SQL语句转换为存储过程
        /// </summary>
        /// <param name="template"></param>
        /// <param name="dbContext"></param>
        /// <param name="sql"></param>
        /// <param name="procedureName"></param>
        /// <param name="templateParame"></param>
        /// <returns></returns>
        internal static string SqlToProcedure(string template, DbContext dbContext, string sql, string procedureName, Dictionary<string, string> templateParame = null)
        {
            var adpater = DBAdapter.DBAdapterBase.GetDBAdapterBase(dbContext);
            template = template.Trim();
            Regex r = new Regex(@"\@(\w+)", RegexOptions.IgnoreCase);//like @parame
            Match m;
            List<string> pars = new List<string>();
            for (m = r.Match(sql); m.Success; m = m.NextMatch())
            {
                string par = m.Groups[1].ToString();
                if (!pars.Contains(par))
                {
                    pars.Add(par);
                }
            }
            //string template = Properties.Resources.pageTemplate.Trim();
            sql = sql.Replace("'", "''");//单引号过滤
            template = template.Replace("{name}", procedureName);
            template = template.Replace("{sql}", sql);
            string parames = "";
            //构造参数
            if (dbContext.DBHelper.Params != null)
            {
                foreach (var p in dbContext.DBHelper.Params)
                {
                    string key = p.Key.Replace("@", "");
                    var t = p.Value.GetType();
                    var par = adpater.GetDBColumnType(t);
                    if (t == typeof(System.String))
                    {
                        par = string.Format(par, 500);
                    }
                    parames += adpater.SpParameFormat(key, par, false);
                }
            }
            if (dbContext.DBHelper.OutParams != null)
            {
                foreach (var p in dbContext.DBHelper.OutParams)
                {
                    string key = p.Key;
                    var t = p.Value.GetType();
                    var par = adpater.GetDBColumnType(t);
                    parames += adpater.SpParameFormat(key, par, true);
                }
            }
            if (parames.Length > 0)
            {
                parames = "(" + parames.Substring(0, parames.Length - 1) + ")";
            }
            template = template.Replace("{parame}", parames);
            if (templateParame != null)
            {
                foreach (var item in templateParame)
                {
                    var value = item.Value;
                    value = value.Replace("'", "''");//单引号过滤
                    template = template.Replace("{" + item.Key + "}", value);
                }
            }
            
            template = adpater.GetCreateSpScript(procedureName, template);
            return template;
        }
        /// <summary>
        /// 获取当前版本
        /// </summary>
        /// <returns></returns>
        public static string GetVersion()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            return assembly.Version.ToString();
        }
        //internal static string FormatFieldPrefix(DBAdapter.DBAdapterBase dBAdapter, Type type, string fieldName)
        //{
        //    return "{" + type.FullName + "}" + dBAdapter.KeyWordFormat(fieldName);
        //}
        public static void Dispose()
        {
            MemoryDataCache.CacheService.StopWatch();
            ExistsTableCache.ColumnBackgroundCheck.Stop();
        }
        #region callContext
        internal const string UseCRLContextFlagName = "__CRLContextFlagName";
        internal const string CRLContextName = "__TransDbContext";
        internal const string AllDBExtendName = "__AllDBExtend";
        internal const string SQLRunningtimeName = "__SQLRunningtime";
        internal const string UseTransactionScopeName = "__TransactionScopeName";
        internal const string ContextUrlName = "__ContextUrlName";
        /// <summary>
        /// 获取当前调用所有的数据访问会话
        /// 可用此检查代码调用深度
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCallDBContext()
        {
            var allList = CallContext.GetData<List<string>>(AllDBExtendName);
            if (allList == null)
            {
                allList = new List<string>();
                CallContext.SetData(AllDBExtendName, allList);
            }
            return allList;
        }
        //contenxt name sql
        static ConcurrentDictionary<string, Dictionary<int, SqlInfo>> allSqlCache = new ConcurrentDictionary<string, Dictionary<int, SqlInfo>>();

        [Serializable]
        public class SqlInfo
        {
            /// <summary>
            /// sql
            /// </summary>
            public string SQL
            {
                get;set;
            }
            /// <summary>
            /// ms
            /// </summary>
            public long Time
            {
                get; set;
            }
            public int RowCount
            {
                get;set;
            }
        }
        public static Dictionary<int, SqlInfo> GetSQLRunningtime(out bool useContext)
        {
            var list = new Dictionary<int, SqlInfo>();
            string key = CallContext.GetData<string>(ContextUrlName);
            if (string.IsNullOrEmpty(key))
            {
                useContext = false;
                return list;
            }
            var a = allSqlCache.TryGetValue(key, out list);
            if (list == null)
            {
                list = new Dictionary<int, SqlInfo>();
                allSqlCache.TryAdd(key, list);
            }
            useContext = true;
            return list;
        }
        internal static void SaveSQLRunningtme(string sql, long n, int rowCount = 1)
        {
            bool useContext;
            var dic = GetSQLRunningtime(out useContext);
            if (!useContext)
            {
                return;
            }
            SqlInfo item;
            var hash = sql.GetHashCode();
            var a = dic.TryGetValue(hash, out item);
            if (a)
            {
                if (item.Time < n)
                {
                    item.Time = n;
                }
                if (item.RowCount < rowCount)
                {
                    item.RowCount = rowCount;
                }
            }
            else
            {
                dic.Add(hash, new SqlInfo() { SQL = sql, Time = n, RowCount = rowCount });
            }
        }
        #endregion
        public static Dictionary<string, int> GetTempCacheCount()
        {
            var dic = new Dictionary<string, int>();
            //dic.Add("表达式二元运算缓存", LambdaQuery.ExpressionVisitor.BinaryExpressionCache.Count);
            //dic.Add("表达式方法解析缓存", LambdaQuery.ExpressionVisitor.MethodCallExpressionCache.Count);
            //dic.Add("表达式属性解析缓存", LambdaQuery.ExpressionVisitor.MemberExpressionCache.Count);
            dic.Add("对象字段筛选缓存", LambdaQuery.LambdaQueryBase.queryFieldCache.Count);
            dic.Add("对象映射列缓存", ObjectConvert.columnCache.Count);
            dic.Add("IModel对象缓存", MemoryDataCache.CacheService.CacheCount);
            dic.Add("SQL查询监视缓存", allSqlCache.Count());
            return dic;
        }

    }
}
