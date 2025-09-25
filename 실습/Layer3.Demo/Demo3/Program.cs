using System; // C#의 기본 클래스 및 기본 형식을 정의하는 System 네임스페이스를 사용합니다. Console 같은 기능을 쓰기 위해 필요합니다.
using System.Net.NetworkInformation; // Ping, PingReply, IPStatus 등 네트워크 진단 관련 클래스를 사용하기 위해 필요합니다.

// PingExample이라는 이름의 클래스를 정의합니다.
class PingExample
{
    // C# 프로그램의 시작점인 Main 메서드입니다.
    static void Main(string[] args)
    {
        // 사용자에게 핑을 보낼 IP 주소나 호스트 이름을 입력하라는 메시지를 출력합니다.
        Console.Write("Enter IP address or host to ping: ");
        // 사용자의 입력을 한 줄 읽어옵니다. Console.ReadLine()은 null을 반환할 수 있습니다.
        string? address = Console.ReadLine();

        // 입력값이 null이거나 비어있는지 확인하여 null 참조 예외를 방지합니다.
        if (string.IsNullOrEmpty(address))
        {
            // 입력이 유효하지 않으면 메시지를 출력하고 프로그램을 종료합니다.
            Console.WriteLine("Address cannot be empty.");
            return;
        }

        // try-catch 블록을 사용하여 Ping.Send 과정에서 발생할 수 있는 예외(예: 잘못된 호스트 이름)를 처리합니다.
        try
        {
            // Ping 요청을 보내기 위한 Ping 객체를 생성합니다.
            Ping pingSender = new Ping();
            // 입력받은 주소로 Ping 요청을 보내고, 그 결과를 PingReply 객체로 받습니다.
            PingReply reply = pingSender.Send(address);

            // Ping 요청에 대한 응답 상태가 'Success'인지 확인합니다.
            if (reply.Status == IPStatus.Success)
            {
                // 성공 메시지와 함께 상세 정보를 출력합니다.
                Console.WriteLine($"Ping to {address} successful.");
                // 왕복 시간(ms)을 출력합니다.
                Console.WriteLine($"Roundtrip time: {reply.RoundtripTime} ms");
                // reply.Options가 null이 아닌지 확인하여 null 참조 예외(CS8602)를 방지합니다.
                if (reply.Options != null)
                {
                    // TTL(Time to Live) 값을 출력합니다.
                    Console.WriteLine($"TTL (Time to Live): {reply.Options.Ttl}");
                }
                // 응답으로 받은 데이터 버퍼의 크기를 출력합니다.
                Console.WriteLine($"Buffer size: {reply.Buffer.Length} bytes");
            }
            // Ping 요청이 실패했을 경우,
            else
            {
                // 실패한 이유(상태)를 콘솔에 출력합니다.
                Console.WriteLine($"Ping to {address} failed: {reply.Status}");
            }
        }
        // Ping 관련 예외가 발생했을 경우 처리합니다.
        catch (PingException ex)
        {
            Console.WriteLine($"Ping error: {ex.Message}");
        }
        // 그 외 다른 모든 예외를 처리합니다.
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }
}
