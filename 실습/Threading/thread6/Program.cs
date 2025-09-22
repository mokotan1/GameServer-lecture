using System;
using System.Threading;

namespace ThreadTest
{
    class ThreadTestProgram
    {
        public static void Main(string[] args)
        {
            Thread thread = new Thread(() => Run(0)); //“스레드가 시작될 때 이 코드를 실행해라” Thread thread = new Thread(new ThreadStart(Run0));
            thread.Start();
            Run(1);
        }

        public static void Run(int idx)
        {
            Console.WriteLine(string.Format("Run {0} Start", idx));
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(string.Format("Run {0} :: {1}", idx, i));
            }
            Console.WriteLine(string.Format("Run {0} End", idx));
        }
    }
}