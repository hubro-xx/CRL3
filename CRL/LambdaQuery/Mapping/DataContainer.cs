using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.LambdaQuery.Mapping
{
    internal class DataContainer
    {
        object[] data;
        int index = 0;
        public DataContainer(object[] _data)
        {
            data = _data;
        }
        public object GetValue()
        {
            var result = data.GetValue(index);
            index++;
            return result;
        }
    }
}
