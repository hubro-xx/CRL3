using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryConsole
{
    public enum TransType
    {
        /// <summary>
        /// 入库
        /// </summary>
        In = 0,
        /// <summary>
        /// 出库
        /// </summary>
        Out = 1
    }
    /// <summary>
    /// 产品渠道
    /// </summary>
    public enum ProductChannel
    {
        自采,
        其它
    }

    /// <summary>
    /// 继承IModelBase实现业务类
    /// 默认主健为ID,不可更改
    /// </summary>
    //[CRL.Attribute.Table(TableName = "ProductData")]//映射表名
    public class ProductData : CRL.IModelBase
    {
        /// <summary>
        /// 实现数据约束
        /// </summary>
        /// <returns></returns>
        public override string CheckData()
        {
            if (string.IsNullOrEmpty(BarCode))
            {
                return "BarCode不能为空";
            }
            if (Number < 0)
            {
                return "Number不能小于0";
            }
            return "";
        }

        /// <summary>
        /// 接口用户
        /// </summary>
        [CRL.Attribute.Field(FieldIndexType = CRL.Attribute.FieldIndexType.非聚集)]
        public string InterFaceUser
        {
            get;
            set;
        }
        public DateTime? Date2
        {
            get;
            set;
        }
        public int UserId
        {
            get;
            set;
        }
        [CRL.Attribute.Field(FieldIndexType = CRL.Attribute.FieldIndexType.非聚集)]
        public string BarCode
        {
            get;
            set;
        }
        /// <summary>
        /// 方向
        /// </summary>
        [CRL.Attribute.Field(FieldIndexType = CRL.Attribute.FieldIndexType.非聚集)]
        public TransType TransType
        {
            get;
            set;
        }
        public string ProductId
        {
            get;
            set;
        }
        /// <summary>
        /// 映射字段名为ProductName1
        /// </summary>
        //[CRL.Attribute.Field(MapingName = "ProductName1")]
        public string ProductName
        {
            get;
            set;
        }
        /// <summary>
        /// 供货商ID
        /// </summary>
        public string SupplierId
        {
            get;
            set;
        }
        public string SupplierName
        {
            get;
            set;
        }
        public string CategoryName
        {
            get;
            set;
        }
        /// <summary>
        /// 进价
        /// </summary>
        public decimal PurchasePrice
        {
            get;
            set;
        }
        /// <summary>
        /// 存本次付款额
        /// </summary>
        public decimal SoldPrice
        {
            get;
            set;
        }
        [CRL.Attribute.Field(Length = 20)]
        public string Style
        {
            get;
            set;
        }
        [CRL.Attribute.Field(Length = 4000)]
        public string Remark
        {
            get;
            set;
        }
        public bool IsTop
        {
            get;
            set;
        }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal Number
        {
            get;
            set;
        }
        public ProductChannel ProductChannel
        {
            get;
            set;
        }
        public bool Show;
        ///// <summary>
        ///// 自动关联字段
        ///// </summary>
        //[CRL.Attribute.Field(ConstraintType = typeof(Order), ConstraintField = "$InterFaceUser=Channel", ConstraintResultField = "OrderId")]
        //public string name2
        //{
        //    get;
        //    set;
        //}
        //public int bb
        //{
        //    get;
        //    set;
        //}
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

    public class ProductDataManage : CRL.BaseProvider<ProductData>
    {
        protected override CRL.LambdaQuery.LambdaQuery<ProductData> CacheQuery()
        {
            return GetLambdaQuery().Where(b => b.Id < 1000).Expire(5);
        }
        /// <summary>
        /// 对象被更新时,是否通知缓存服务器
        /// </summary>
        protected override bool OnUpdateNotifyCacheServer
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// 是否从远程查询缓存
        /// </summary>
        protected override bool QueryCacheFromRemote
        {
            get
            {
                return false;
            }
        }


        /// <summary>
        /// 实例访问入口
        /// </summary>
        public static ProductDataManage Instance
        {
            get { return new ProductDataManage(); }
        }

        public List<ProductData> QueryDayProduct(DateTime date)
        {
            return new List<ProductData>();
        }

        public void DynamicQueryTest()
        {
            //此方法演示根据结果集返回动态对象
            string sql = "select top 10 Id,ProductId,ProductName1 from ProductData";
            var helper = DBExtend;
            var list = helper.ExecDynamicList(sql);
            //添加引用 Miscorsoft.CSharp程序集
            foreach (dynamic item in list)
            {
                var id = item.Id;
                var productId = item.ProductId;
            }
        }
    }
}
