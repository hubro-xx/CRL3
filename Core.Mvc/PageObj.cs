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
using System.Text;

namespace Core.Mvc
{
    public class PageObj<T> : List<T>, IEnumerable<T>, IEnumerable
    {
        public PageObj(IEnumerable<T> allItems, int pageIndex, int total, int pageSize)
        {
            AddRange(allItems);
            PageIndex = pageIndex;
            Total = total;
            PageSize = pageSize;
        }
        public int PageIndex
        {
            get;
            set;
        }
        public int Total
        {
            get;
            set;
        }
        public int PageSize
        {
            get;
            set;
        }
    }
}
