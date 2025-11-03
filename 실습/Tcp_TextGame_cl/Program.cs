using System;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;

class Client
{
    TcpClient client;
    NetworkStream stream;

    public void Connect(string host, int port)
    {
        try
        {
            client = new TcpClient(host, port);
            stream = client.GetStream();
        }
        catch (Exception e)
        {
            Console.WriteLine($"서버 연결 실패: {e.Message}");
            return;
        }


        Console.WriteLine("베틀넷에 연결되었습니다.");
        Console.WriteLine("사용할 이름을 입력하세요: ");
        string playerName = Console.ReadLine();

        SendMessage(playerName);

        // [수정] 메시지 수신 스레드 시작
        Thread receiveThread = new Thread(ReceiveMessage);
        receiveThread.Start();
    }

    private void ReceiveMessage()
    {
        try
        {
            while (true)
            {
                byte[] buffer = new byte[256];
                int byteRead = stream.Read(buffer, 0, buffer.Length);

                if (byteRead == 0)
                {
                    Console.WriteLine("서버와의 연결이 끊어졌습니다. 퇴장되었습니다.");
                    break;
                }
                string message = Encoding.UTF8.GetString(buffer, 0, byteRead);
                Console.WriteLine(message);
            }
        }
        catch
        {
            Console.WriteLine("서버와의 연결이 끊어졌습니다. 퇴장되었습니다");
        }
        finally
        {
            Disconnect();
        }
    }

    private void Disconnect()
    {
        if (client != null)
        {
            client.Close();
            client = null;
            Console.WriteLine("연결이 종료되었습니다. (엔터키를 눌러 종료)");
        }
    }

    // [수정] Main에서 호출할 수 있도록 public으로 변경
    public void SendMessage(string message)
    {
        if (client == null || !client.Connected) return;

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
        catch (Exception e)
        {
            Console.WriteLine($"메시지 전송 실패: {e.Message}");
            Disconnect();
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Client client = new Client();
        
        try
        {
            // [수정] IP 주소 오타 수정 및 연결
            client.Connect("127.0.0.1", 9000);

            // [수정] 사용자 입력을 받기 위한 전송 루프 추가
            while (true)
            {
                string message = Console.ReadLine();
                if (string.IsNullOrEmpty(message)) continue;
                
                client.SendMessage(message);

                if (message == "/exit")
                {
                    break; // /exit 입력 시 전송 루프 종료
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"오류 발생: {e.Message}");
        }
        
        Console.WriteLine("클라이언트를 종료합니다.");
    }
}