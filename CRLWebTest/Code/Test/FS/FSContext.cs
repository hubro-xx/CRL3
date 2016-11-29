using FS.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChloePerformanceTest.FS
{
    public class FSContext : DbContext<FSContext>
    {
        public TableSet<TestEntity> TestEntity { get; set; }
    }
}
