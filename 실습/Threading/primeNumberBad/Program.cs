using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    const int MaxCount = 150000;
    const int ThreadCount = 4;

    static bool IsPrimeNumber(int number)
    {
        if (number == 1)
            return false;
        if (number == 2 || number == 3)
            return true;
        for (int i = 2; i < number - 1; i++)
        {
            if (number % i == 0)
                return false;
        }
        return true;
    }

    static void PrintNumbers(List<int> primes)
    {
        foreach (int v in primes)
            Console.WriteLine(v);
    }

    static void Main(string[] args)
    {
        // 각 스레드는 여기서 값을 가져감(동기화 없음 → 데이터 레이스 의도적으로 유지)
        int num = 1;

        var primes = new List<int>(); // 스레드 안전하지 않음(락 없음 → 레이스 유지)

        var t0 = DateTime.Now; // system_clock 유사(Stopwatch 대신)

        // 작동할 워커 스레드
        var threads = new List<Thread>(ThreadCount);

        for (int i = 0; i < ThreadCount; i++)
        {
            var t = new Thread(() =>
            {
                // 각 스레드의 메인 함수
                while (true)
                {
                    int n;
                    n = num;   // 원자성 없음
                    num++;     // 원자성 없음 → 중복/누락 가능

                    if (n >= MaxCount)
                        break;

                    if (IsPrimeNumber(n))
                    {
                        primes.Add(n); // 원자성 없음 → 중복/누락 가능
                    }
                }
            });

            threads.Add(t);
            t.Start();
        }

        // 모든 스레드가 일을 마칠 때까지 기다림
        foreach (var t in threads)
            t.Join();

        var t1 = DateTime.Now;
        var durationMs = (t1 - t0).TotalMilliseconds;
        Console.WriteLine($"Took {durationMs} milliseconds.");

        //PrintNumbers(primes); // 실제 소수 출력(원하면 주석 해제)
    }
}