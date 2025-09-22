class Program
{
  static void MethodExample1()
  {
    Thread.Sleep(5000);
    for (int loop = 0; loop < 3; loop++)
    {
      Console.WriteLine("MethodExample1()의 loop: " + loop);
    }
  }
  
  static void MethodExample2()
  {
    Thread.Sleep(3000);
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
    // 세 개의 Thread 객체
    Thread t1 = new Thread(MethodExample1);
    Thread t2 = new Thread(MethodExample2);
    Thread t3 = new Thread(MethodExample3);

    // 스레드 시작
    t1.Start();
    t2.Start();
    t3.Start();
  }
}