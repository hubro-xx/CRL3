using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class LinqToDBContext:DataContext
    {
        public LinqToDBContext()
        {
        }
        public ITable<TestEntity> TestEntitys { get { return this.GetTable<TestEntity>(); } }
    }
}
