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
using System.Web.UI;
using System.Web.UI.WebControls;
using CRL.Sharding;
using CRL;
namespace WebTest.Page
{
    public partial class Sharding : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        string error;
        protected void Button1_Click(object sender, EventArgs e)
        {
            var n = CRL.Sharding.DB.DataBaseManage.Instance.Count(b => b.Id > 0);
            if (n > 0)
            {
                return;
            }
            CRL.Sharding.DB.DataBaseManage.Instance.CleanData();
            //创建库分组
            var db = new CRL.Sharding.DB.DataBase();
            db.Name = "db1";
            db.MaxMainDataTotal = 10;
            CRL.Sharding.DB.DataBaseManage.Instance.Create(db);
            CRL.Sharding.DB.DataBaseManage.Instance.Create(db);
            //创建表
            var dbList = CRL.Sharding.DB.DataBaseManage.Instance.QueryList();
            foreach(var item in dbList)
            {
                var table = new CRL.Sharding.DB.Table();
                table.TableName = "MemberSharding";
                table.IsMainTable = true;
                CRL.Sharding.DB.TableManage.Instance.Create(item, table, out error);

                var table2 = new CRL.Sharding.DB.Table();
                table2.TableName = "OrderSharding";
                table2.IsMainTable = false;
                table2.MaxPartDataTotal = 5;
                CRL.Sharding.DB.TableManage.Instance.Create(item, table2, out error);

                //创建分区
                CRL.Sharding.DB.TablePartManage.Instance.Create(table2, out error);
            }
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            var m = new Code.Sharding.MemberSharding();
            m.Id = Convert.ToInt32(TextBox1.Text);
            var location = CRL.Sharding.DBService.GetLocation("MemberSharding", m.Id);
            m.Name = location.ToString();
            Code.Sharding.MemberManage.Instance.SetLocation(m.Id).Add(m);

            var order = new Code.Sharding.OrderSharding();
            order.MemberId = m.Id;
            var location2 = CRL.Sharding.DBService.GetLocation("OrderSharding", m.Id);
            order.Remark = location2.ToString();
            Code.Sharding.OrderManage.Instance.SetLocation(m.Id).Add(order);
            Label1.Text = "插入会员编号" + m.Id + "," + location + " 订单" + location2;
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            var id  = Convert.ToInt32(TextBox1.Text);
            var list = Code.Sharding.OrderManage.Instance.SetLocation(id).QueryList(b => b.MemberId == id);
            GridView1.DataSource = list;
            GridView1.DataBind();
            //Label1.Text = "";
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            var id = Convert.ToInt32(TextBox1.Text);
            var list = Code.Sharding.MemberManage.Instance.SetLocation(id).QueryList(b => b.Id == id);
            GridView1.DataSource = list;
            GridView1.DataBind();
            //Label1.Text = "";
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            var id = Convert.ToInt32(TextBox1.Text);
            var orderManage = Code.Sharding.OrderManage.Instance.SetLocation(id);
            var query = orderManage.GetLambdaQuery();
            query.ShardingUnion(UnionType.UnionAll);
            var list = query.ToList();
            GridView1.DataSource = list;
            GridView1.DataBind();
        }

       
    }
}
