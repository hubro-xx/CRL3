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

namespace CRL.DBExtend.MongoDB
{
    public sealed partial class MongoDB
    {



        public override void BeginTran()
        {
            return;
        }

        public override void RollbackTran()
        {
            return;
        }

        public override void CommitTran()
        {
            return;
        }

        internal override void CheckTableCreated(Type type)
        {
            return;
        }

       

        public override Dictionary<TKey, TValue> ExecDictionary<TKey, TValue>(string sql, params Type[] types)
        {
            throw new NotSupportedException();//不支持
        }

        public override List<dynamic> ExecDynamicList(string sql, params Type[] types)
        {
            throw new NotSupportedException();//不支持
        }

        public override List<T> ExecList<T>(string sql, params Type[] types)
        {
            throw new NotSupportedException();//不支持
        }


        public override T ExecObject<T>(string sql, params Type[] types)
        {
            throw new NotSupportedException();//不支持
        }

        public override object ExecScalar(string sql, params Type[] types)
        {
            throw new NotSupportedException();//不支持
        }

        public override T ExecScalar<T>(string sql, params Type[] types)
        {
            throw new NotSupportedException();//不支持
        }

        public override int Execute(string sql, params Type[] types)
        {
            throw new NotSupportedException();//不支持
        }

        public override object GetOutParam(string name)
        {
            throw new NotSupportedException();//不支持
        }

        public override T GetOutParam<T>(string name)
        {
            throw new NotSupportedException();//不支持
        }

        public override int GetReturnValue()
        {
            throw new NotSupportedException();//不支持
        }


        public override int Run(string sp)
        {
            throw new NotSupportedException();//不支持
        }

        public override List<dynamic> RunDynamicList(string sp)
        {
            throw new NotSupportedException();//不支持
        }

        public override List<T> RunList<T>(string sp)
        {
            throw new NotSupportedException();//不支持
        }

        public override T RunObject<T>(string sp)
        {
            throw new NotSupportedException();//不支持
        }

        public override object RunScalar(string sp)
        {
            throw new NotSupportedException();//不支持
        }
    }
}
