//5

class ThreadTestProgram
{
    public static void Main(string[] args)
    {
        Thread thread0 = new Thread(() => Run(0));
        thread0.Start();
        Thread.Sleep(100);
        thread0.Abort();
    }

    public static void Run(int idx)
    {
        try
        {
            int runIdx = idx;
            Console.WriteLine(string.Format("Run {0} Start", runIdx));
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(string.Format("Run {0} :: {1}", runIdx, i));
                Thread.Sleep(10);
            }
            Console.WriteLine(string.Format("Run {0} End", runIdx));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}

