

class Program
{
    static void MethodExample1()
    {
        Thread.Sleep(5000);

        for (int i = 0; i < 3; i++)
        {
            Console.WriteLine("MethodExample1()의 loop " + i);
        }
    }

    static void MethodExample2()
    {
        Thread.Sleep(3000);

        for (int i = 0; i < 3; i++)
        {
            Console.WriteLine("MethodExample2()의 loop " + i);
        }

    }

    static void MethodExample3()
    {
        Thread.Sleep(3000);

        for (int i = 0; i < 3; i++)
        {
            Console.WriteLine("MethodExample3()의 loop " + i);
        }

    }

    static void Main(string[] args)
    {

        Thead t1 = new Thead(MethodExample1);
        Thead t2 = new Thead(MethodExample2);
        Thead t3 = new Thead(MethodExample3);


        t1.Start();
        t2.Start();
        t3.Start();
    }
}