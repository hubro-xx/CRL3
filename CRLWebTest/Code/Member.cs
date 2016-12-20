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
using System.Web;

namespace WebTest.Code
{
    /// <summary>
    /// 会员
    /// </summary>
    public class Member:CRL.Package.Person.Member
    {
        protected override System.Collections.IList GetInitData()
        {
            var list = new List<Member>();
            list.Add(new Member() { Name = "hubro" });
            list.Add(new Member() { Name = "hubro2" });
            return list;
        }
        ///// <summary>
        ///// 虚拟字段,等同于 year($addtime) as Year
        ///// 字段前需加前辍,以在关联查询时区分
        ///// </summary>
        //[CRL.Attribute.Field(VirtualField = "year($addtime)")]
        //public int Year
        //{
        //    get;
        //    set;
        //}
    }
}
