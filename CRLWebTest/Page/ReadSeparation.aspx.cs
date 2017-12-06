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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebTest.Page
{
    public partial class ReadSeparation : System.Web.UI.Page
    {
        public List<Code.ProductData> dataMaster;
        public List<Code.ProductData> dataRead;
        protected void Page_Load(object sender, EventArgs e)
        {
            CRL.SettingConfig.UseReadSeparation = true;
            Bind();
        }
        protected override void OnUnload(EventArgs e)
        {
            CRL.SettingConfig.UseReadSeparation = false;
        }

        void Bind()
        {
            using (var cx = new CRL.CRLDbConnectionScope())//事务默认为从主库读
            {
                dataMaster = Code.ProductDataManage.Instance.QueryList(b => b.Id < 5);
            }
            dataRead = Code.ProductDataManage.Instance.QueryList(b => b.Id < 5);
            GridView1.DataSource = dataMaster;
            GridView1.DataBind();
            GridView2.DataSource = dataRead;
            GridView2.DataBind();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var item = Code.ProductDataManage.Instance.QueryItem(2);
            item.ProductName = "更改主库数据为" + DateTime.Now.Second;
            Code.ProductDataManage.Instance.Update(item);
            Bind();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            var item = Code.ProductDataManage.Instance.QueryItem(2);
            Response.Write("从库数据1为" + item.ProductName);
        }
    }
}
