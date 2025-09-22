using System;
using System.Threading;

//7

class ThreadTestProgram
{
    public class Vilige
    {
        public int population = 1000;

        public object populationLock = new object();

        public void AddHuman()
        {
            lock (populationLock)
            {
                population++;

                for (int i = 0; i < population; i++)
                {
                    for (int j = 0; j < population; j++)
                    {

                    }
                }
                // 추가된 주민에게 주민번호 주기
                Console.WriteLine(string.Format("새 주민의 주민번호 :: {0}", population));
            }
        }
    }

    public static void Main(string[] args)
    {
        Vilige manager = new Vilige();
        for (int i = 0; i < 10; i++)
        {
            new Thread(new ThreadStart(manager.AddHuman)).Start();
        }
    }
}

