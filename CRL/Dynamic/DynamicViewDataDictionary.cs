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

namespace CRL.Dynamic
{
    internal sealed class DynamicViewDataDictionary : System.Dynamic.DynamicObject
    {
        // Fields
        private Dictionary<string, object> _viewDataThunk;

        // Methods
        public DynamicViewDataDictionary(Dictionary<string, object> viewDataThunk)
        {
            viewDataThunk = viewDataThunk ?? new Dictionary<string, object>();

            this._viewDataThunk = viewDataThunk;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.ViewData.Keys;
        }

        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            var a = ViewData.TryGetValue(binder.Name.ToLower(), out result);
            if (!a)
            {
                throw new CRLException(string.Format("动态字典Bag不存在索引值:{0}", binder.Name));
            }
            return a;
        }

        public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
        {
            this.ViewData[binder.Name.ToLower()] = value;
            return true;
        }

        // Properties
        private Dictionary<string, object> ViewData
        {
            get
            {
                return this._viewDataThunk;
            }
        }
    }

}
