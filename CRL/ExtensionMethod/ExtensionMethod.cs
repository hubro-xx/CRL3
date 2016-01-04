using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections;
using CRL.LambdaQuery;
namespace CRL
{
    #region 比较时间格式
    /// <summary>
    /// 比较时间格式
    /// </summary>
    public enum DatePart
    {
        /// <summary>
        /// 年
        /// </summary>
        yy,
        /// <summary>
        /// 季度
        /// </summary>
        qq,
        /// <summary>
        /// 月
        /// </summary>
        mm,
        /// <summary>
        /// 年中的日
        /// </summary>
        dy,
        /// <summary>
        /// 日
        /// </summary>
        dd,
        /// <summary>
        /// 周
        /// </summary>
        ww,
        /// <summary>
        /// 星期
        /// </summary>
        dw,
        /// <summary>
        /// 小时
        /// </summary>
        hh,
        /// <summary>
        /// 分
        /// </summary>
        mi,
        /// <summary>
        /// 秒
        /// </summary>
        ss,
        /// <summary>
        /// 毫秒
        /// </summary>
        ms,
        /// <summary>
        /// 微妙
        /// </summary>
        mcs,
        /// <summary>
        /// 纳秒
        /// </summary>
        ns
    }
    #endregion
    /// <summary>
    /// 查询扩展方法,请引用CRL命名空间
    /// </summary>
    public static partial class ExtensionMethod
    {

        #region 查询方法
        /// <summary>
        /// Like("%key%")
        /// 如果包函%通配符,则按通配符算
        /// </summary>
        /// <param name="s"></param>
        /// <param name="likeString"></param>
        /// <returns></returns>
        public static bool Like(this string s, string likeString)
        {
            if (string.IsNullOrEmpty(likeString))
                throw new Exception("参数值不能为空:likeString");
            return s.IndexOf(likeString) > -1;
        }
        /// <summary>
        /// Like("%key")
        /// </summary>
        /// <param name="s"></param>
        /// <param name="likeString"></param>
        /// <returns></returns>
        public static bool LikeLeft(this string s, string likeString)
        {
            if (string.IsNullOrEmpty(likeString))
                throw new Exception("参数值不能为空:likeString");
            return s.IndexOf(likeString) > -1;
        }
        /// <summary>
        /// Like("key%")
        /// </summary>
        /// <param name="s"></param>
        /// <param name="likeString"></param>
        /// <returns></returns>
        public static bool LikeRight(this string s, string likeString)
        {
            if (string.IsNullOrEmpty(likeString))
                throw new Exception("参数值不能为空:likeString");
            return s.IndexOf(likeString) > -1;
        }

        /// <summary>
        /// DateDiff
        /// </summary>
        /// <param name="time"></param>
        /// <param name="format">DatePart</param>
        /// <param name="compareTime">比较的时间</param>
        /// <returns></returns>
        public static int DateDiff(this DateTime time, DatePart format, DateTime compareTime)
        {
            return 1;
        }
        /// <summary>
        /// DateDiff
        /// </summary>
        /// <param name="time"></param>
        /// <param name="format"></param>
        /// <param name="compareTime"></param>
        /// <returns></returns>
        public static int DateDiff(this Nullable<DateTime> time, DatePart format, DateTime compareTime)
        {
            return 1;
        }
        /// <summary>
        /// 表示COUNT
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static int COUNT(this object origin)
        {
            return 0;
        }
        #endregion

        #region 对象转换
        /// <summary>
        /// 转换共同属性的对象
        /// </summary>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDest ToType<TDest>(this object source)
            where TDest : class,new()
        {
            var simpleTypes = typeof(TDest).GetProperties().ToList();
            simpleTypes.RemoveAll(b => b.SetMethod == null);
            List<PropertyInfo> complexTypes = source.GetType().GetProperties().ToList();
            complexTypes.RemoveAll(b => b.Name == "Item" || b.SetMethod == null);
            TDest obj = new TDest();
            foreach (var info in simpleTypes)
            {
                var complexInfo = complexTypes.Find(b => b.Name == info.Name);
                if (complexInfo != null)
                {
                    object value = complexInfo.GetValue(source, null);
                    value = ObjectConvert.ConvertObject(info.PropertyType, value);
                    info.SetValue(obj, value, null);
                }
            }
            return obj;
        }
        /// <summary>
        /// 转换为共同属性的集合
        /// </summary>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<TDest> ToType<TDest>(this IEnumerable source)
            where TDest : class,new()
        {
            var simpleTypes = typeof(TDest).GetProperties().ToList();
            simpleTypes.RemoveAll(b => b.SetMethod == null);
            List<PropertyInfo> complexTypes = null;
            List<TDest> list = new List<TDest>();
            foreach (var item in source)
            {
                TDest obj = new TDest();
                if (complexTypes == null)
                {
                    complexTypes = item.GetType().GetProperties().ToList();
                    complexTypes.RemoveAll(b => b.Name == "Item" || b.SetMethod == null);
                }
                foreach (var info in simpleTypes)
                {
                    var complexInfo = complexTypes.Find(b => b.Name == info.Name);
                    if (complexInfo != null)
                    {
                        object value = complexInfo.GetValue(item,null);
                        value = ObjectConvert.ConvertObject(info.PropertyType, value);
                        info.SetValue(obj, value, null);
                    }
                }
                list.Add(obj);
            }
            return list;
        }
        #endregion

        #region IEnumerable Find
        /// <summary>
        /// Find
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T Find<T>(this IEnumerable<T> source,Func<T, bool> predicate)
        {
            return source.Where(predicate).FirstOrDefault();
        }
        /// <summary>
        /// FindAll
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<T> FindAll<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.Where(predicate).ToList();
        }
        #endregion
        /// <summary>
        /// 枚举转换为INT
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static int ToInt(this Enum e)
        {
            return Convert.ToInt32(e);
        }
    }
}
