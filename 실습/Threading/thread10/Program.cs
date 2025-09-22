//여러 개의 스레드를 두고 작동하는 프로그램의 경우에, 여러 스레드가 자원이나 변수 등을 공유하는 경우가 많다

//6

class ThreadTestProgram
{
    public class Villige
    {
        public int population = 1000;

        public void AddVillager()
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

    public static void Main(string[] args)
    {
        Villige manager = new Villige();
        for (int i = 0; i < 10; i++)
        {
            new Thread(new ThreadStart(manager.AddVillager)).Start();
        }
    }
}
