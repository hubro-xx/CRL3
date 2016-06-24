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
using System.Linq.Expressions;
using System.Text;
using CoreHelper;
using CRL.LambdaQuery;
namespace CRL
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
        ParameCollection GetUpdateField<TModel>(TModel obj) where TModel : IModel, new()
        {
            var c = new ParameCollection();
            var fields = TypeCache.GetProperties(typeof(TModel), true);
            if (obj.Changes.Count > 0)//按手动指定更改
            {
                foreach (var item in obj.Changes)
                {
                    var key = item.Key.Replace("$", "");
                    var f = fields[key];
                    if (f == null)
                        continue;
                    if (f.IsPrimaryKey || f.FieldType == Attribute.FieldType.虚拟字段)
                        continue;
                    var value = item.Value;
                    //如果表示值为被追加 名称为$name
                    //使用Cumulation扩展方法后按此处理
                    if (key != item.Key)//按$name=name+'123123'
                    {
                        if (value.ToString().IsNumber())
                        {
                            value = string.Format("{0}+{1}", key, value);
                        }
                        else
                        {
                            value = string.Format("{0}+'{1}'", key, value);
                        }
                    }
                    c[item.Key] = value;
                }
                return c;
            }
            //按对象对比
            var origin = obj.OriginClone;
            if (origin == null)
            {
                throw new Exception("_originClone为空,请确认此对象是由查询创建");
            }
            CheckData(obj);

            foreach (var f in fields.Values)
            {
                if (f.IsPrimaryKey)
                    continue;
                if (!string.IsNullOrEmpty(f.VirtualField))
                {
                    continue;
                }
                var originValue = f.GetValue(origin);
                var currentValue = f.GetValue(obj);
                if (!Object.Equals(originValue, currentValue))
                {
                    c.Add(f.Name, currentValue);
                }
            }
            return c;

        }

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
        public int Update<TModel>(TModel obj) where TModel : IModel, new()
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
            obj.Changes.Clear();
            return n;
        }
        /// <summary>
        /// 指定条件并按对象差异更新
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update<TModel>(Expression<Func<TModel, bool>> expression, TModel model) where TModel : IModel, new()
        {
            var c = GetUpdateField(model);
            if (c.Count == 0)
            {
                return 0;
                //throw new Exception("更新集合为空");
            }
            var n = Update(expression, c);
            model.Changes.Clear();
            return n;
        }
        /// <summary>
        /// 指定条件和参数进行更新
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression">条件</param>
        /// <param name="setValue">值</param>
        /// <returns></returns>
        public int Update<TModel>(Expression<Func<TModel, bool>> expression, ParameCollection setValue) where TModel : IModel, new()
        {
            if (setValue.Count == 0)
            {
                throw new Exception("更新时发生错误,参数值为空 ParameCollection setValue");
            }
            LambdaQuery<TModel> query = new LambdaQuery<TModel>(dbContext, false);
            string condition = query.FormatExpression(expression.Body);
            query.FillParames(this);
            var count = Update<TModel>(setValue, condition);
            System.Threading.Tasks.Task.Run(() =>
            {
                UpdateCacheItem<TModel>(expression, setValue);
            });
            
            //CacheUpdated(typeof(T).Name);
            return count;
        }
        /// <summary>
        /// 按匿名对象更新
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="expression"></param>
        /// <param name="updateValue"></param>
        /// <returns></returns>
        public int Update<TModel>(Expression<Func<TModel, bool>> expression, dynamic updateValue) where TModel : IModel, new()
        {
            var properties = updateValue.GetType().GetProperties();
            var c = new ParameCollection();
            foreach (var p in properties)
            {
                c.Add(p.Name, p.GetValue(updateValue));
            }
            return Update(expression, c);
        }


        /// <summary>
        /// 按完整查询条件进行更新
        /// goup语法不支持,其它支持
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="query"></param>
        /// <param name="updateValue"></param>
        /// <returns></returns>
        public int Update<TModel>(LambdaQuery<TModel> query, ParameCollection updateValue) where TModel : IModel, new()
        {
            if (query.__GroupFields.Count > 0)
            {
                throw new Exception("update不支持group查询");
            }
            if (query.__Relations.Count > 1)
            {
                throw new Exception("update关联不支持多次");
            }
            query._IsRelationUpdate = true;
            var conditions = query.GetQueryConditions(false).Trim();
            conditions = conditions.Substring(5);
            string table = query.QueryTableName;
            table = query.__DBAdapter.KeyWordFormat(table);
            query.FillParames(this);
            var properties = updateValue.GetType().GetProperties();

            if (query.__Relations.Count > 0)
            {
                string setString = ForamtSetValue<TModel>(updateValue);
                var kv = query.__Relations.First();
                var t1 = query.QueryTableName;
                var t2 = TypeCache.GetTableName(kv.Key, query.__DbContext);
                var join = query.__Relations[kv.Key];
                join = join.Substring(join.IndexOf(" on ") + 3);
                string sql = query.__DBAdapter.GetRelationUpdateSql(t1, t2, join + " and " + conditions, setString);
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
        public int Update<TModel, TJoin>(Expression<Func<TModel, TJoin, bool>> expression, ParameCollection updateValue)
            where TModel : IModel, new()
            where TJoin : IModel, new()
        {
            var query = new LambdaQuery<TModel>(dbContext);
            query.Join<TJoin>(expression);
            return Update(query, updateValue);
        }

        #endregion
    }
}
