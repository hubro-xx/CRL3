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
                    var obj = System.Activator.CreateInstance(type) as IProvider;
                    var table = TypeCache.GetTable(obj.ModelType);
                    var pro = type.GetProperty("DBExtend", BindingFlags.Instance | BindingFlags.NonPublic);
                    var db = pro.GetValue(obj) as AbsDBExtend;
                    findTypes.Add(table, db);
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
            foreach (var p in dbContext.DBHelper.Params)
            {
                string key = p.Key.Replace("@","");
                var t = p.Value.GetType();
                var par = adpater.GetDBColumnType(t);
                if (t == typeof(System.String))
                {
                    par = string.Format(par, 500);
                }
                parames += adpater.SpParameFormat(key, par, false);
            }
            foreach (var p in dbContext.DBHelper.OutParams)
            {
                string key = p.Key;
                var t = p.Value.GetType();
                var par = adpater.GetDBColumnType(t);
                parames += adpater.SpParameFormat(key, par, true);
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
        /// <summary>
        /// 获取当前调用所有的数据访问会话
        /// 可用此检查代码调用深度
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCallDBContext()
        {
            var allKey = "AllDBExtend";
            var allList = CallContext.GetData<List<string>>(allKey);
            if (allList == null)
            {
                allList = new List<string>();
            }
            return allList;
        }
        public static Dictionary<string, int> GetTempCacheCount()
        {
            var dic = new Dictionary<string, int>();
            dic.Add("表达式二元运算缓存", LambdaQuery.ExpressionVisitor.BinaryExpressionCache.Count);
            dic.Add("表达式方法解析缓存", LambdaQuery.ExpressionVisitor.MethodCallExpressionCache.Count);
            dic.Add("表达式属性解析缓存", LambdaQuery.ExpressionVisitor.MemberExpressionCache.Count);
            dic.Add("表达式字段筛选缓存", LambdaQuery.LambdaQueryBase._GetSelectFieldCache.Count);
            dic.Add("对象映射列缓存", ObjectConvert.columnCache.Count);
            return dic;
        }
        internal const string UseCRLContextFlagName = "CRLContextFlagName";
        internal const string CRLContextName = "TransDbContext";
    }
}
