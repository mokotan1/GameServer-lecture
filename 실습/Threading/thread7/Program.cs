//3

using System;

using System.Threading;

namespace ThreadTest
{
    class ThreadTestProgram
    {
        public static void Main(string[] args)
        {
            Thread thread0 = new Thread(() => Run(0));
            thread0.Start();
            Thread thread1 = new Thread(() => Run(1));
            thread1.Start();
        }

        public static void Run(int idx)
        {
            Console.WriteLine(string.Format("Run {0} Start", idx));
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(string.Format("Run {0} :: {1}", idx, i));
                Thread.Sleep(10);
            }
            Console.WriteLine(string.Format("Run {0} End", idx));
        }
    }
}