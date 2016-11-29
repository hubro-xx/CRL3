using FS.Infrastructure;
using FS.Sql.Map.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChloePerformanceTest
{
    public abstract class ClassP
    {
        public Dictionary<string, string> pr1 = new Dictionary<string, string>();
    }
    public class TestEntity : ClassP
    {
        [Chloe.Entity.Column(IsPrimaryKey = true)]
        [Chloe.Entity.AutoIncrement]
        public int Id { get; set; }
        public byte? F_Byte { get; set; }
        public Int16? F_Int16 { get; set; }
        public int? F_Int32 { get; set; }
        public long? F_Int64 { get; set; }
        public double? F_Double { get; set; }
        public float? F_Float { get; set; }
        public decimal? F_Decimal { get; set; }
        public bool F_Bool { get; set; }
        public DateTime? F_DateTime { get; set; }
        public Guid? F_Guid { get; set; }
        public string F_String { get; set; }
    }


}
