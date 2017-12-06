/**
* CRL 快速开发框架 V4.5
* Copyright (c) 2016 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL3
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
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

    /// <summary>
    /// 查询扩展方法,请引用CRL命名空间
    /// </summary>
    public static partial class ExtensionMethod
    {
        #region Between
        /// <summary>
        /// 表示Between
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool Between(this int origin, int begin, int end)
        {
            return origin > begin && origin < end;
        }
        /// <summary>
        /// 表示Between
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static bool Between(this int? origin, int begin, int end) 
        {
            return origin > begin && origin < end;
        }
        public static bool Between(this decimal origin, decimal begin, decimal end)
        {
            return origin > begin && origin < end;
        }
        public static bool Between(this decimal? origin, decimal begin, decimal end)
        {
            return origin > begin && origin < end;
        }
        public static bool Between(this double origin, double begin, double end)
        {
            return origin > begin && origin < end;
        }
        public static bool Between(this double? origin, double begin, double end)
        {
            return origin > begin && origin < end;
        }
        public static bool Between(this float origin, float begin, float end)
        {
            return origin > begin && origin < end;
        }
        public static bool Between(this float? origin, float begin, float end)
        {
            return origin > begin && origin < end;
        }
        public static bool Between(this long origin, long begin, long end)
        {
            return origin > begin && origin < end;
        }
        public static bool Between(this long? origin, long begin, long end)
        {
            return origin > begin && origin < end;
        }
        public static bool Between(this DateTime origin, DateTime begin, DateTime end)
        {
            return origin > begin && origin < end;
        }
        public static bool Between(this DateTime? origin, DateTime begin, DateTime end)
        {
            return origin > begin && origin < end;
        }
        #endregion
        /// <summary>
        /// DateDiff
        /// </summary>
        /// <param name="time"></param>
        /// <param name="format">DatePart</param>
        /// <param name="compareTime">比较的时间</param>
        /// <returns></returns>
        public static double DateDiff(this DateTime time, DatePart format, DateTime compareTime)
        {
            var ts = compareTime - time;
            double val=0;
            switch (format)
            {
                case DatePart.dd:
                    val = ts.TotalDays;
                    break;
                case DatePart.dw:
                    val = ts.TotalDays / 7;
                    break;
                case DatePart.dy:
                    val = ts.TotalDays;
                    break;
                case DatePart.hh:
                    val = ts.TotalHours;
                    break;
                case DatePart.mi:
                    val = ts.TotalMinutes;
                    break;
                case DatePart.mm:
                    val = ts.TotalDays / 30;
                    break;
                case DatePart.ms:
                    val = ts.TotalMilliseconds;
                    break;
                case DatePart.qq:
                    val = ts.TotalDays / 90;
                    break;
                case DatePart.ss:
                    val = ts.TotalSeconds;
                    break;
                case DatePart.ww:
                    val = ts.TotalDays / 7;
                    break;
                case DatePart.yy:
                    val = ts.TotalDays / 365;
                    break;
            }
            return val;
        }
        /// <summary>
        /// DateDiff
        /// </summary>
        /// <param name="time"></param>
        /// <param name="format"></param>
        /// <param name="compareTime"></param>
        /// <returns></returns>
        public static double DateDiff(this DateTime? time, DatePart format, DateTime compareTime)
        {
            return DateDiff(time.Value, format, compareTime);
        }
    }
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
        ///// <summary>
        ///// 微妙
        ///// </summary>
        //mcs,
        ///// <summary>
        ///// 纳秒
        ///// </summary>
        //ns
    }
    #endregion
}
