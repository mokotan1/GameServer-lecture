using System;
using System.Threading;

class Program
{
    static readonly object L1 = new object();
    static readonly object L2 = new object();

    static void Main()
    {
        var t1 = new Thread(Thread1);
        var t2 = new Thread(Thread2);

        t1.Start();
        t2.Start();

        // 교착에 걸리면 여기서 영원히 반환되지 않음
        t1.Join();
        t2.Join();

        Console.WriteLine("끝"); // 출력되지 않음
    }

    static void Thread1()
    {
        Console.WriteLine("T1: lock L1");
        lock (L1)
        {
            Thread.Sleep(100); // 스케줄링을 유도해 재현성 ↑
            Console.WriteLine("T1: lock L2 (대기)");
            lock (L2)
            {
                Console.WriteLine("T1: got both");
            }
        }
    }

    static void Thread2()
    {
        Console.WriteLine("T2: lock L2");
        lock (L2)
        {
            Thread.Sleep(100);
            Console.WriteLine("T2: lock L1 (대기)");
            lock (L1)
            {
                Console.WriteLine("T2: got both");
            }
        }
    }
}
