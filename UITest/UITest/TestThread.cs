using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UITest
{
    class TestThread
    {
        public static void DoWork()
        {
            Console.WriteLine("Static thread procedure.");
        }
        public int Data;
        public void DoMoreWork()
        {
            Console.WriteLine("Instance thread procedure. Data={0}", Data);
        }
    }
}
