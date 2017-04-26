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
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;
namespace CRL.LambdaQuery
{
    public sealed partial class MongoDBLambdaQuery<T> : LambdaQuery<T> where T : IModel, new()
    {         
        /// <summary>
        /// lambda查询
        /// </summary>
        /// <param name="_dbContext"></param>
        public MongoDBLambdaQuery(DbContext _dbContext)
            : base(_dbContext, false)
        {
           
        }

        internal FilterDefinition<T> __MongoDBFilter = new BsonDocument();
        public override LambdaQuery<T> Where(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            if (expression == null)
                return this;
            var crlExpression = FormatExpression(expression.Body);
            var filterData = RouteCRLExpression(crlExpression);
            __MongoDBFilter = __MongoDBFilter & filterData.Filter;
            return this;
        }
        #region 生成filter
        class FilterData
        {
            public FilterDefinition<T> Filter;
            public object Data;
            public CRLExpression.CRLExpressionType Type;
        }
        FilterDefinition<T> getFilter(FilterData left, FilterData right, string expressionType)
        {
            var builder = Builders<T>.Filter;
            var _expressionType = (ExpressionType)Enum.Parse(typeof(ExpressionType), expressionType);
            FilterDefinition<T> filter=null;
            if (left.Type == CRLExpression.CRLExpressionType.Binary || left.Type == CRLExpression.CRLExpressionType.Tree)//表示二元运算
            {
                #region 按条件组合
                switch (_expressionType)
                {
                    case ExpressionType.AndAlso:
                        filter = left.Filter & right.Filter;
                        break;
                    case ExpressionType.OrElse:
                        filter = left.Filter | right.Filter;
                        break;
                    default:
                        throw new InvalidCastException("不支持的运算符");
                }
                #endregion
            }
            else if (left.Type == CRLExpression.CRLExpressionType.MethodCallArgs)
            {
                var methodInfo = left.Data as CRLExpression.MethodCallObj;
                #region 按方法
                var field = methodInfo.MemberName;
                var args = methodInfo.Args;
                switch (methodInfo.MethodName)
                {
                    case "Contains":
                        filter = builder.Regex(field, string.Format("{0}",args.FirstOrDefault()));
                        break;
                    case "StartsWith":
                        filter = builder.Regex(field, string.Format("^{0}", args.FirstOrDefault()));
                        break;
                    case "Like":
                        filter = builder.Regex(field, string.Format("{0}", args.FirstOrDefault()));
                        break;
                    case "LikeLeft":
                        filter = builder.Regex(field, string.Format(".+?{0}", args.FirstOrDefault()));
                        break;
                    case "LikeRight":
                        filter = builder.Regex(field, string.Format("{0}.+", args.FirstOrDefault()));
                        break;
                    case "Between":
                        filter = builder.Gt(field, args[0]) & builder.Lt(field, args[1]);
                        break;
                    case "DateDiff":
                        throw new NotSupportedException(methodInfo.MethodName);
                    case "In":
                        var list = args.FirstOrDefault() as IEnumerable;
                        var list2 = new List<object>();
                        foreach(var s in list)
                        {
                            list2.Add(s);
                        }
                        filter = builder.In(field, list2);
                        break;
                    case "Substring":
                        throw new NotSupportedException(methodInfo.MethodName);
                    case "Equals":
                        filter = builder.Eq(field, args.FirstOrDefault());
                        break;
                    default:
                        throw new NotSupportedException(methodInfo.MethodName);//不支持
                }
                if (_expressionType == System.Linq.Expressions.ExpressionType.Not)
                {
                    filter = builder.Not(filter);
                }
                #endregion
            }
            else//按值
            {
                #region 按值
                object name, value;
                if (left.Type == CRLExpression.CRLExpressionType.Name)
                {
                    name = left.Data;
                    value = right.Data;
                }
                else
                {
                    name = right.Data;
                    value = left.Data;
                }
                switch (_expressionType)
                {
                    case ExpressionType.Equal:
                        filter = builder.Eq(name.ToString(), value);
                        break;
                    case ExpressionType.GreaterThan:
                        filter = builder.Gt(name.ToString(), value);
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        filter = builder.Gte(name.ToString(), value);
                        break;
                    case ExpressionType.LessThan:
                        filter = builder.Lt(name.ToString(), value);
                        break;
                    case ExpressionType.LessThanOrEqual:
                        filter = builder.Lte(name.ToString(), value);
                        break;
                    case ExpressionType.NotEqual:
                        filter = builder.Ne(name.ToString(), value);
                        break;
                    default:
                        throw new InvalidCastException("不支持的运算符");
                }
                #endregion
            }
            return filter;
        }
        FilterData BinaryCRLExpression(CRLExpression.CRLExpression left, CRLExpression.CRLExpression right, string expressionType)
        {
            var parLeft = RouteCRLExpression(left);
            var parRight = RouteCRLExpression(right);
            return new FilterData() { Filter = getFilter(parLeft, parRight, expressionType) };
        }
        FilterData RouteCRLExpression(CRLExpression.CRLExpression exp)
        {
            if (exp.Type == CRLExpression.CRLExpressionType.Binary || exp.Type == CRLExpression.CRLExpressionType.Tree)//表示二元运算
            {
                return BinaryCRLExpression(exp.Left, exp.Right, exp.ExpType);
            }
            else if(exp.Type== CRLExpression.CRLExpressionType.MethodCall)
            {
                var methodInfo = exp.Data as CRLExpression.MethodCallObj;
                var left = new CRLExpression.CRLExpression() { ExpType = methodInfo.ExpressionType.ToString(), Type = CRLExpression.CRLExpressionType.MethodCallArgs, Data = methodInfo };
                var right = new CRLExpression.CRLExpression() { ExpType = methodInfo.ExpressionType.ToString(), Type = CRLExpression.CRLExpressionType.MethodCallArgs, Data = methodInfo.Args };
                return BinaryCRLExpression(left, right, methodInfo.ExpressionType.ToString());
            }
            //按值
            return new FilterData() { Data = exp.Data, Type = exp.Type };
        }

        #endregion
        internal SortDefinition<T> _MongoDBSort = new BsonDocument();
        public override LambdaQuery<T> OrderBy<TResult>(System.Linq.Expressions.Expression<Func<T, TResult>> expression, bool desc = true)
        {
            //var sortBuild = Builders<T>.Sort;
            var parameters = expression.Parameters.Select(b => b.Type).ToArray();
            var field = GetSelectField(false, expression.Body, false, parameters).fields.First();
            if (desc)
            {
                _MongoDBSort = _MongoDBSort.Descending(field.MemberName);
            }
            else
            {
                _MongoDBSort = _MongoDBSort.Ascending(field.MemberName);
            }
            return this;
        }

        public override LambdaQuery<T> OrderByPrimaryKey(bool desc)
        {
            //var sortBuild = Builders<T>.Sort;
            var field = TypeCache.GetTable(typeof(T)).PrimaryKey;
            if (desc)
            {
                _MongoDBSort = _MongoDBSort.Descending(field.MemberName);
            }
            else
            {
                _MongoDBSort = _MongoDBSort.Ascending(field.MemberName);
            }
            return this;
        }

        public override LambdaQuery<T> Or(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            var crlExpression = FormatExpression(expression.Body);
            var filterData = RouteCRLExpression(crlExpression);
            __MongoDBFilter = __MongoDBFilter | filterData.Filter;
            return this;
        }
        #region NotSupported

        internal override string GetQueryFieldString()
        {
            return "";
        }

        internal override string GetQueryConditions(bool withTableName = true)
        {
            return "";
        }

        internal override string GetOrderBy()
        {
            return ""; throw new NotImplementedException();
        }

        internal override string GetQuery()
        {
            return "";
        }
        #endregion
    }
}
