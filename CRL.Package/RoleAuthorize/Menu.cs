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
using System.Text;

namespace CRL.Package.RoleAuthorize
{
    /// <summary>
    /// 菜单
    /// </summary>
    [Attribute.Table(TableName="Menu")]
    public sealed class Menu : Category.Category
    {
        protected override System.Collections.IList GetInitData()
        {
            var list = new List<Menu>();
            list.Add(new Menu() { Name = "会员管理", SequenceCode = "01", DataType = 1 });
            list.Add(new Menu() { Name = "会员查询", SequenceCode = "0101", ParentCode = "01", DataType = 1, Url = "/Member/List" });

            list.Add(new Menu() { Name = "系统设置", SequenceCode = "02", DataType = 1 });
            list.Add(new Menu() { Name = "参数设置", SequenceCode = "0201", ParentCode = "02", DataType = 1, Url = "/DicConfig/List" });
            list.Add(new Menu() { Name = "分类管理", SequenceCode = "0202", ParentCode = "02", DataType = 1, Url = "/category/List" });
            list.Add(new Menu() { Name = "广告管理", SequenceCode = "0203", ParentCode = "02", DataType = 1, Url = "/Advert/List" });
            list.Add(new Menu() { Name = "文章管理", SequenceCode = "0204", ParentCode = "02", DataType = 1, Url = "/Article/List" });

            return list;
        }

        [Attribute.Field(Length = 200)]
        public string Url
        {
            get;
            set;
        }
        /// <summary>
        /// 只在设置权限时使用
        /// </summary>
        public bool Que;
        private bool showInNav = true;
        /// <summary>
        /// 是否在导航菜单中显示
        /// </summary>
        public bool ShowInNav
        {
            get { return showInNav; }
            set { showInNav = value; }
        }
        public bool Enabled = true;
        public string GetPadding()
        {
            string str = "";
            int n = SequenceCode.Length / 2;
            if (n > 1)
            {
                str = "└";
            }
            for (int i = 1; i < n;i++ )
            {
                str += "────";
            }
            return str;
        }
        public override string ToString()
        {
            return string.Format("{0} {1}",SequenceCode,Name);
        }
        [CRL.Attribute.Field(MapingField = false)]
        public int Hit
        {
            get;
            set;
        }
    }
}
