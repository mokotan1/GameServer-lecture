using System;
using System.Threading;

class Thread3
{
    static void DoSomething()
    {
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine($"$ DoSomething : {i}");
            Thread.Sleep(10); // 올바른 Thread.Sleep 사용
        }
    }

    static void Main(string[] args)
    {
        Thread t1 = new Thread(new ThreadStart(DoSomething)); // 올바른 스레드 생성

        Console.WriteLine("시작.......");

        t1.Start();

        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine($"Main thread : {i}"); // 메인 스레드 진행 상황을 보여주기 위해 추가
            Thread.Sleep(10);
        }

        Console.WriteLine("기다려......");
        t1.Join();

        Console.WriteLine("끝");
    }
}