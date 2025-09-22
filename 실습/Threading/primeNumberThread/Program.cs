using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

class Program
{
    const int MaxCount = 150000;
    const int ThreadCount = 4;

    static bool IsPrimeNumber(int n)
    {
        if (n <= 1) return false;
        if (n <= 3) return true;
        if (n % 2 == 0) return n == 2;
        for (int i = 3; i * i <= n; i += 2)
            if (n % i == 0) return false;
        return true;
    }

    static void PrintNumbers(IEnumerable<int> primes)
    {
        foreach (var v in primes) Console.WriteLine(v);
    }

    static void Main(string[] args)
    {
        int num = 1;                        // 공유 카운터

        var primes = new List<int>();
        var gate = new object();            // 병합용 게이트(락)

        var sw = Stopwatch.StartNew();      // 정확한 경과 시간 측정

        var threads = new List<Thread>(ThreadCount);

        for (int i = 0; i < ThreadCount; i++)
        {
            var t = new Thread(() =>
            {
                var local = new List<int>();    // 스레드별 로컬 버퍼

                while (true)
                {
                    // 가벼운 원자적 티켓 발급
                    int n = Interlocked.Increment(ref num) - 1;

                    // 포함 경계(1..MaxCount 모두 검사)
                    if (n > MaxCount) break;

                    if (IsPrimeNumber(n))
                        local.Add(n);
                }

                // 한 번에 병합하여 락 경합 최소화
                if (local.Count > 0)
                {
                    lock (gate)
                    {
                        primes.AddRange(local);
                    }
                }
            });

            t.Start();
            threads.Add(t);
        }

        foreach (var t in threads) t.Join();

        sw.Stop();
        Console.WriteLine($"Took {sw.ElapsedMilliseconds} milliseconds.");

        // 필요 시 정렬/출력
        // primes.Sort();
        // PrintNumbers(primes);
    }
}
