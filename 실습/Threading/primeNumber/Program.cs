using System;
using System.Collections.Generic;
using System.Diagnostics;

class Program
{
    const int MaxCount = 150000;

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
        {
            Console.WriteLine(v);
        }
    }

    static void Main(string[] args)
    {
        var primes = new List<int>();

        // start clock
        var sw = Stopwatch.StartNew();

        for (int i = 1; i <= MaxCount; i++)
        {
            if (IsPrimeNumber(i))
            {
                primes.Add(i);
            }
        }

        // end clock
        sw.Stop();
        Console.WriteLine($"Took {sw.ElapsedMilliseconds} milliseconds.");

        // PrintNumbers(primes); // actually print out prime numbers
    }
}
