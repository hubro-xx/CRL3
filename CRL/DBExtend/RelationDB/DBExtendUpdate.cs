/**
* CRL 快速开发框架 V4.0
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CoreHelper;
using CRL.LambdaQuery;
namespace CRL.DBExtend.RelationDB
{
    public sealed partial class DBExtend
    {
        /// <summary>
        /// 格式化为更新值查询
        /// </summary>
        /// <param name="setValue"></param>
        /// <returns></returns>
        string ForamtSetValue<T>(ParameCollection setValue) where T : IModel
        {
            string tableName = TypeCache.GetTableName(typeof(T), dbContext);
            string setString = "";
            foreach (var pair in setValue)
            {
                string name = pair.Key;
                object value = pair.Value;
                value = ObjectConvert.CheckNullValue(value);

                if (name.StartsWith("$"))//直接按值拼接 c2["$SoldCount"] = "SoldCount+" + num;
                {
                    name = name.Substring(1, name.Length - 1);
                    setString += string.Format(" {0}={1},", _DBAdapter.KeyWordFormat(name), value);
                }
                else
                {
                    if (value.ToString().Contains("$"))//当是关联更新
                    {
                        //右边字段需加前辍
                        value = value.ToString().Replace("$", "t2.");
                        name = string.Format("t1.{0}", name);
                    }
                    else
                    {
                        string parame = string.Format("@{0}", name, dbContext.parIndex);
                        AddParam(name, value);
                        dbContext.parIndex += 1;
                        value = parame;
                    }
                    setString += string.Format(" {0}={1},", name, value);

                }
            }
            setString = setString.Substring(0, setString.Length - 1);
            return setString;
        }
        #region update


        /// <summary>
        /// 指定拼接条件更新
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="setValue"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        internal int Update<TModel>(ParameCollection setValue, string where) where TModel : IModel,new()
        {
            CheckTableCreated<TModel>();
            Type type = typeof(TModel);
            string table = TypeCache.GetTableName(type, dbContext);
            string setString = ForamtSetValue<TModel>(setValue);
            string sql = _DBAdapter.GetUpdateSql(table, setString, where);
            sql = _DBAdapter.SqlFormat(sql);
            int n = dbHelper.Execute(sql);
            ClearParame();
            return n;
        }
        
        /// <summary>
        /// 按对象差异更新,由主键确定记录
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int Update<TModel>(TModel obj)
        {
            var c = GetUpdateField(obj);
            if (c.Count == 0)
            {
                return 0;
                //throw new Exception("更新集合为空");
            }
            var primaryKey = TypeCache.GetTable(obj.GetType()).PrimaryKey;
            var keyValue = primaryKey.GetValue(obj);
            string where = string.Format("{0}=@{0}", primaryKey.Name);
            AddParam(primaryKey.Name, keyValue);
            int n = Update<TModel>(c, where);
            UpdateCacheItem(obj, c);
            if (n == 0)
            {
                throw new Exception("更新失败,找不到主键为 " + keyValue + " 的记录");
            }
            obj.CleanChanges();
            return n;
        }

        /// <summary>
        /// 按完整查询条件进行更新
        /// goup语法不支持,其它支持
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="updateValue"></param>
        /// <returns></returns>
        public override int Update<TModel>(LambdaQuery<TModel> query, ParameCollection updateValue)
        {
            var query1 = query as RelationLambdaQuery<TModel>;
            if (query1.__GroupFields.Count > 0)
            {
                throw new Exception("update不支持group查询");
            }
            if (query1.__Relations.Count > 1)
            {
                throw new Exception("update关联不支持多次");
            }
            if (updateValue.Count == 0)
            {
                throw new ArgumentNullException("更新时发生错误,参数值为空 ParameCollection setValue");
            }
            query1._IsRelationUpdate = true;
            var conditions = query1.GetQueryConditions(false).Trim();
            if (conditions.Length > 5)
            {
                conditions = conditions.Substring(5);
            }

            string table = query1.QueryTableName;
            table = query1.__DBAdapter.KeyWordFormat(table);
            query1.FillParames(this);
            var properties = updateValue.GetType().GetProperties();

            if (query1.__Relations.Count > 0)
            {
                string setString = ForamtSetValue<TModel>(updateValue);
                var kv = query1.__Relations.First();
                var t1 = query1.QueryTableName;
                var t2 = TypeCache.GetTableName(kv.Key, query1.__DbContext);
                var join = query1.__Relations[kv.Key];
                join = join.Substring(join.IndexOf(" on ") + 3);
                if (!string.IsNullOrEmpty(conditions))
                {
                    join += " and ";
                }
                string sql = query1.__DBAdapter.GetRelationUpdateSql(t1, t2, join + conditions, setString);
                return Execute(sql);
            }
            else
            {
                conditions = conditions.Replace("t1.","");
            }
            return Update<TModel>(updateValue, conditions);
        }

        /// <summary>
        /// 关联更新
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TJoin"></typeparam>
        /// <param name="expression"></param>
        /// <param name="updateValue"></param>
        /// <returns></returns>
        public override int Update<TModel, TJoin>(Expression<Func<TModel, TJoin, bool>> expression, ParameCollection updateValue)
        {
            var query = new RelationLambdaQuery<TModel>(dbContext);
            query.Join<TJoin>(expression);
            return Update(query, updateValue);
        }

        #endregion
    }
}
