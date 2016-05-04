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
using System.Collections;

namespace CRL.Package.ReceiveAddress
{
    /// <summary>
    /// 收货地址维护
    /// </summary>
    public class AddressBusiness<TType, TModel> : BaseProvider<TModel>
        where TType : class
        where TModel : Address,new()
    {
        public static AddressBusiness<TType, TModel> Instance
        {
            get { return new AddressBusiness<TType, TModel>(); }
        }

        /// <summary>
        /// 获取用户收货地址
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<TModel> QueryUserAddress(int userId)
        {
            var query = GetLambdaQuery();
            query = query.Where(b => b.UserId == userId).OrderBy(b => b.DefaultAddress, true);
            return query.ToList();
        }
        /// <summary>
        /// 设为默认
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        public void SetDefault(int userId,int id)
        {
            ParameCollection setValue = new ParameCollection();
            setValue["DefaultAddress"] = 0;
            Update((b=>b.UserId==userId), setValue);//去掉默认

            setValue["DefaultAddress"] = 1;
            Update(b => b.Id == id, setValue);//设为默认
        }
    }
}
