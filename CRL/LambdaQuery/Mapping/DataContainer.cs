using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CRL.LambdaQuery.Mapping
{
    class DataContainer
    {
        object[] data;
        int index = 0;
        Type dataType;
        public DataContainer(object[] _data, Type _dataType)
        {
            dataType = _dataType;
            data = _data;
        }
        public T GetValue<T>()
        {
            var result = data.GetValue(index);
            index++;
            if (result is DBNull)
            {
                return default(T);
            }
            try
            {
                return (T)result;
            }
            catch
            {
                throw new CRLException(string.Format("将值 {0} 转换成类型{1}.{2}时失败,请检查对象类型和数据表字段类型是否一致", result + " " + typeof(T), dataType));
            }
        }
    }
}
