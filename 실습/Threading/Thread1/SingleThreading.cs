class Program
{
    static void MethodExample1()
    {
        for (int loop = 0; loop < 3; loop++)
        {
            Console.WriteLine("MethodExample1()의 loop: " + loop);
        }
    }

    static void MethodExample2()
    {
        for (int loop = 0; loop < 3; loop++)
        {
            Console.WriteLine("MethodExample2()의 loop: " + loop);
        }
    }

    static void MethodExample3()
    {
        for (int loop = 0; loop < 3; loop++)
        {
            Console.WriteLine("MethodExample3()의 loop: " + loop);
        }
    }

    static void Main(string[] args)
    {
        MethodExample1();
        MethodExample2();
        MethodExample3();
    }
}