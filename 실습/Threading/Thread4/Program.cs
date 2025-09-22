namespace ConsoleApp109
{
    class Program
    {
        static void DoSomething()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"DoSomething : {i}");
                Thread.Sleep(10);
            }
        }

        static void Main(string[] args)
        {
            Thread t1 = new Thread(new ThreadStart(DoSomething));

            Console.WriteLine("시작......");
            t1.Start();

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(i);
                Thread.Sleep(10);
            }

            Console.WriteLine("기다려.....");
            t1.Join();

            Console.WriteLine("끝");
        }
    }
}
