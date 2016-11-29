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
using System.Text;
using System.Threading.Tasks;

namespace CRL.DBAdapter
{
    internal class MongoDBAdapter : DBAdapterBase
    {
        public MongoDBAdapter(DbContext _dbContext)
            : base(_dbContext)
        {
        }
        public override CoreHelper.DBType DBType
        {
            get { return CoreHelper.DBType.MongoDB; }
        }
        internal override bool CanCompileSP
        {
            get
            {
                return false;
            }
        }
        public override string GetColumnType(Attribute.FieldAttribute info, out string defaultValue)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<Type, string> FieldMaping()
        {
            //todo
            Dictionary<Type, string> dic = new Dictionary<Type, string>();
            //字段类型对应
            dic.Add(typeof(System.String), "String");
            dic.Add(typeof(System.Decimal), "Decimal");
            dic.Add(typeof(System.Double), "Double");
            dic.Add(typeof(System.Single), "Single");
            dic.Add(typeof(System.Boolean), "Boolean");
            dic.Add(typeof(System.Int32), "Integer");
            dic.Add(typeof(System.Int16), "Integer");
            dic.Add(typeof(System.Enum), "Integer");
            dic.Add(typeof(System.Byte), "Binary data");
            dic.Add(typeof(System.DateTime), "Date");
            dic.Add(typeof(System.UInt16), "Integer");
            dic.Add(typeof(System.Int64), "Integer");
            dic.Add(typeof(System.Object), "Object");
            dic.Add(typeof(System.Byte[]), "Binary data");
            dic.Add(typeof(System.Guid), "nvarchar(50)");
            return dic;
        }

        public override string GetColumnIndexScript(Attribute.FieldAttribute filed)
        {
            throw new NotImplementedException();
        }

        public override string GetCreateColumnScript(Attribute.FieldAttribute field)
        {
            throw new NotImplementedException();
        }

        public override string GetCreateSpScript(string spName, string script)
        {
            throw new NotImplementedException();
        }

        public override void CreateTable(List<Attribute.FieldAttribute> fields, string tableName)
        {
            throw new NotImplementedException();
        }

        public override void BatchInsert(System.Collections.IList details, bool keepIdentity = false)
        {
            throw new NotImplementedException();
        }

        public override string GetTableFields(string tableName)
        {
            throw new NotImplementedException();
        }

        public override object InsertObject(IModel obj)
        {
            throw new NotImplementedException();
        }

        public override string GetSelectTop(string fields, string query, string sort, int top)
        {
            throw new NotImplementedException();
        }

        public override string GetWithNolockFormat(bool v)
        {
            throw new NotImplementedException();
        }

        public override string GetAllSPSql()
        {
            throw new NotImplementedException();
        }

        public override string GetAllTablesSql()
        {
            throw new NotImplementedException();
        }

        public override string SpParameFormat(string name, string type, bool output)
        {
            throw new NotImplementedException();
        }

        public override string KeyWordFormat(string value)
        {
            return value;
        }

        public override string TemplateGroupPage
        {
            get { throw new NotImplementedException(); }
        }

        public override string TemplatePage
        {
            get { throw new NotImplementedException(); }
        }

        public override string TemplateSp
        {
            get { throw new NotImplementedException(); }
        }

        public override string SqlFormat(string sql)
        {
            throw new NotImplementedException();
        }
        public override string CastField(string field, Type fieldType)
        {
            throw new NotImplementedException();
        }
    }
}
