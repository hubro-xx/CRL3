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
    public class TestModelManage : CRL.BaseProvider<TestModel>
    {
        public static TestModelManage Instance
        {
            get
            {
                return new TestModelManage();
            }
        }
    }
    [CRL.Attribute.Table(TableName = "TestModel_1")]
    public class TestModel : CRL.IModel
    {
        protected override bool CheckRepeatedInsert
        {
            get
            {
                return false;
            }
        }
        [CRL.Attribute.Field(IsPrimaryKey = true)]
        public int Id
        {
            get;
            set;
        }
        [CRL.Attribute.Field(Length = 50)]
        public string Name
        {
            get;
            set;
        }
        public string Name2
        {
            get;
            set;
        }
        public override string CheckData()
        {
            if (Name!="hubro")
            {
                return "输入的值?";
            }
            return base.CheckData();
        }
    }
    [CRL.Attribute.Table(TableName="table1")]//映射表名为table1
    public class ModelTest:CRL.IModelBase
    {
        protected override bool CheckRepeatedInsert
        {
            get
            {
                return base.CheckRepeatedInsert;
            }
        }
        protected override void OnColumnCreated(string fieldName)
        {
            base.OnColumnCreated(fieldName);
        }
        //创建表时,初始数据,自增暂为自动
        protected override System.Collections.IList GetInitData()
        {
            var list = new List<ModelTest>();
            list.Add(new ModelTest() { BarCode = "123", Price = 10, ProductName="创建表时初始" });
            list.Add(new ModelTest() { BarCode = "456", Price = 10, ProductName = "创建表时初始" });
            return list;
        }
        public override string CheckData()
        {
            if (string.IsNullOrEmpty(BarCode))
            {
                return "BarCode不能为空";
            }
            return base.CheckData();
        }

        [CRL.Attribute.Field(FieldIndexType = CRL.Attribute.FieldIndexType.非聚集唯一)]//非聚集唯一索引
        public string No
        {
            get;
            set;
        }
        [CRL.Attribute.Field(FieldIndexType = CRL.Attribute.FieldIndexType.非聚集)]//非聚集索引
        public string DataType
        {
            get;
            set;
        }
        [CRL.Attribute.Field(Length=100)]//长度为100
        public string BarCode
        {
            get;
            set;
        }
        /// <summary>
        /// 自动关联查询
        /// 值等同为 select Name as ProductName from ProductData where BarCode=ModelTest.ModelTest
        /// </summary>
        [CRL.Attribute.Field(ConstraintType = typeof(ProductData), ConstraintField = "$BarCode=BarCode", ConstraintResultField = "Name")]
        public string ProductName
        {
            get;
            set;
        }
        [CRL.Attribute.Field(ColumnType = "decimal(18,4)")]//强制指定字段类型,如果不指定则默认为decimal(18,2)
        public decimal Price
        {

            get;
            set;
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
