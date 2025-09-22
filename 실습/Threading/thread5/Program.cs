using System;
using System.Threading;

namespace ThreadTest
{
    class ThreadTestProgram
    {
        public static void Main(string[] args)
        {
            Run(0);
            Run(1);
        }

        public static void Run(int idx)
        {
            Console.WriteLine(string.Format("Run {0} Start", idx));

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(string.Format("Run {0} :: {1}", idx, i));
            }
            Console.WriteLine(string.Format("Run {0} End", idx));
        }
    }
}