using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRL.ExistsTableCache
{
    [Serializable]
    internal class DataBase
    {
        public string Name
        {
            get;
            set;
        }
        public List<Table> Tables
        {
            get;
            set;
        }
    }
}
