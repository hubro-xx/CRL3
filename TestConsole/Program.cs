using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var list = new List<Test2>();
            for (int i = 0; i < 100; i++)
            {
                list.Add(new Test2() { Name = "234234" });
            }
            var sw = new Stopwatch();
            sw.Start();
            foreach (var item in list)
            {
                var name = item.Name;
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

            sw.Start();
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                //var item = list[i];
                //var name = item.Name;
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            var propType=typeof(type2);
            var aa= propType.GetEnumUnderlyingType();
            Console.ReadLine();
        }
        public enum type2
        {
            wss,
            sss,
        }
        public class Test2
        {
            public string Name;
            public string Name2
            {
                get;
                set;
            }
            public string Name3
            {
                get
                {
                    return "Name3";
                }
            } 
        }
    }
}
