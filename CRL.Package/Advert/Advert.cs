using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CRL.Package.Advert
{
    /// <summary>
    /// 广告实体
    /// </summary>
    public class Advert : IModelBase
    {
        public override string CheckData()
        {
            return "";
        }
        [Attribute.Field(FieldIndexType = Attribute.FieldIndexType.非聚集)]
        public string CategoryCode
        {
            get;
            set;
        }
        /**
         *关联示例
        //取出关联字段
        [CRL.Attribute.Field(Constraint = "DataType=0", ConstraintField = "$CategoryCode=SequenceCode", ConstraintType = typeof(CRL.Category.ICategory), ConstraintResultField = "Name")]
        public string CategoryName
        {
            get;
            set;
        }
        //取出关联对象
        [CRL.Attribute.Field(Constraint = "DataType=0", ConstraintField = "$CategoryCode=SequenceCode")]
        public CRL.Category.ICategory Category
        {
            get;
            set;
        }
        **/
        [Attribute.Field(Length=200)]
        public string Url
        {
            get;
            set;
        }
        [Attribute.Field(Length = 200)]
        public string Title
        {
            get;
            set;
        }
        [Attribute.Field(Length = 200)]
        public string ImageUrl
        {
            get;
            set;
        }
        public int Width
        {
            get;
            set;
        }
        public int Height
        {
            get;
            set;
        }
        public string TagData
        {
            get;
            set;
        }
        public bool Disable
        {
            get;
            set;
        }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime
        {
            get;
            set;
        }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime
        {
            get;
            set;
        }
        public int Sort
        {
            get;
            set;
        }
    }
}
