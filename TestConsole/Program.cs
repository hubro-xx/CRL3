using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var type = typeof(Test2);
            var pro = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.GetProperty);
            Console.WriteLine(pro.Count());
            Console.ReadLine();
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
