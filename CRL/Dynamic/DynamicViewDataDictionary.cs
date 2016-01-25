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
            this._viewDataThunk = viewDataThunk;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.ViewData.Keys;
        }

        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            result = this.ViewData[binder.Name.ToLower()];
            return true;
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
