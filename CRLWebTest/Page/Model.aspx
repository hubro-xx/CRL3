<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Model.aspx.cs" Inherits="WebTest.Page.Model" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<h4>以下演示了对象写法</h4>
<blockquote>
在对数据更新时,会按属性设定长度进行检查,如果超过,会抛出异常,string默认长度为30
</blockquote>
<pre>
    [CRL.Attribute.Table(TableName="table1")]//映射表名为table1,不指定则按类名
    public class ModelTest:CRL.IModelBase
    {
        //创建表时,初始数据
        public override System.Collections.IList GetInitData()
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
        /// 自动关联查询,不建义这样使用
        /// 值等同为 select Name as ProductName from ProductData where BarCode=ModelTest.ModelTest
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
        /// <summary>
        /// 虚拟字段,等同于 year(addtime) as Year
        /// 字段前需加前辍,以在关联查询时区分
        /// </summary>
        [CRL.Attribute.Field(VirtualField = "year($addtime)")]
        public int Year
        {
            get;
            set;
        }
    }
</pre>
<h4>MSSQL数据库,对象和字段类型对应关系如下</h4>
<pre>
    Dictionary<Type, string> dic = new Dictionary<Type, string>();
    //字段类型对应
    dic.Add(typeof(System.String), "nvarchar({0})");
    dic.Add(typeof(System.Decimal), "decimal(18, 2)");
    dic.Add(typeof(System.Double), "float");
    dic.Add(typeof(System.Single), "real");
    dic.Add(typeof(System.Boolean), "bit");
    dic.Add(typeof(System.Int32), "int");
    dic.Add(typeof(System.Int16), "SMALLINT");
    dic.Add(typeof(System.Enum), "int");
    dic.Add(typeof(System.Byte), "SMALLINT");
    dic.Add(typeof(System.DateTime), "datetime");
    dic.Add(typeof(System.UInt16), "SMALLINT");
    dic.Add(typeof(System.Int64), "bigint");
    dic.Add(typeof(System.Object), "nvarchar(30)");
    dic.Add(typeof(System.Byte[]), "varbinary({0})");
    dic.Add(typeof(System.Guid), "nvarchar(50)");
</pre>
</asp:Content>
