using System; // C#의 기본 클래스 및 기본 형식을 정의하는 System 네임스페이스를 사용합니다. Console.WriteLine 같은 기능을 쓰기 위해 필요합니다.
using System.Net; // IP 주소, 엔드포인트 등 네트워크 관련 클래스를 사용하기 위해 필요합니다.
using System.Net.Sockets; // Socket, TCP, UDP 등 소켓 프로그래밍을 위한 핵심 클래스를 사용하기 위해 필요합니다.
using System.Text; // 문자열을 바이트 배열로 변환하거나 그 반대로 변환하는 인코딩 클래스를 사용하기 위해 필요합니다.

// SocketExample이라는 이름의 클래스를 정의합니다. 이 클래스는 프로그램의 전체 로직을 담고 있습니다.
class SocketExample
{
    // C# 프로그램의 시작점인 Main 메서드입니다. 프로그램이 실행되면 가장 먼저 이 메서드가 호출됩니다.
    static void Main(string[] args)
    {
        // 사용자에게 서버로 실행할지 클라이언트로 실행할지 묻는 메시지를 콘솔에 출력합니다.
        Console.WriteLine("Server or Client? (S/C)");
        // 사용자의 입력을 한 줄 읽어옵니다. ToUpper()로 대문자로 바꾸고, [0]으로 첫 글자만 가져옵니다.
        char role = Console.ReadLine().ToUpper()[0];

        // 만약 사용자가 'S'를 입력했다면,
        if (role == 'S')
        {
            // 서버를 시작하는 StartServer 메서드를 호출합니다.
            StartServer();
        }
        // 'S'가 아니라면 (예: 'C'를 입력했다면),
        else
        {
            // 클라이언트를 시작하는 StartClient 메서드를 호출합니다.
            StartClient();
        }
    }

    // 서버 역할을 수행하는 메서드를 정의합니다.
    static void StartServer()
    {
        // IP 주소와 포트 번호를 설정합니다. IPAddress.Any는 모든 네트워크 인터페이스(랜카드)로부터의 연결을 허용하겠다는 의미이며, 포트는 8080을 사용합니다.
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 8080);
        // 소켓을 생성합니다. AddressFamily.InterNetwork는 IPv4 주소 체계를 사용하겠다는 의미입니다.
        // SocketType.Stream은 연결 지향형(TCP) 소켓을, ProtocolType.Tcp는 TCP 프로토콜을 사용하겠다는 것을 명시합니다.
        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // 생성된 소켓(listener)을 위에서 설정한 IP 주소와 포트 번호(localEndPoint)에 할당(바인딩)합니다.
        listener.Bind(localEndPoint);
        // 클라이언트의 연결 요청을 기다리기 시작합니다. 숫자 10은 동시에 대기할 수 있는 최대 클라이언트 수를 의미합니다. (연결 백로그)
        listener.Listen(10);

        // 서버가 클라이언트의 접속을 기다리고 있음을 콘솔에 출력합니다.
        Console.WriteLine("Server listening...");

        // 클라이언트의 연결 요청이 올 때까지 여기서 대기합니다. 연결이 수락되면, 클라이언트와 통신할 새로운 소켓(clientSocket)이 생성됩니다.
        Socket clientSocket = listener.Accept();
        // 클라이언트가 성공적으로 연결되었음을 콘솔에 출력합니다.
        Console.WriteLine("Client connected.");

        // 클라이언트로부터 데이터를 수신할 바이트 배열을 생성합니다. 크기는 1024바이트로 설정합니다.
        byte[] bytes = new byte[1024];
        // 클라이언트 소켓으로부터 데이터를 수신하고, 수신한 데이터의 길이를 numBytes 변수에 저장합니다.
        int numBytes = clientSocket.Receive(bytes);
        // 수신한 바이트 배열(bytes)을 ASCII 인코딩을 사용해 문자열로 변환하여 콘솔에 출력합니다. 0부터 numBytes 길이만큼만 변환합니다.
        Console.WriteLine("Received: " + Encoding.ASCII.GetString(bytes, 0, numBytes));

        // 클라이언트와의 통신이 끝났으므로, 클라이언트 소켓을 닫습니다.
        clientSocket.Close();
        // 서버의 역할을 마쳤으므로, 연결 요청을 받던 리스너 소켓도 닫습니다.
        listener.Close();
    }

    // 클라이언트 역할을 수행하는 메서드를 정의합니다.
    static void StartClient()
    {
        // 접속할 서버의 IP 주소와 포트 번호를 설정합니다. "127.0.0.1"은 자기 자신(localhost)을 의미합니다.
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
        // 서버와 통신할 소켓을 생성합니다. 서버와 마찬가지로 IPv4, TCP 설정을 사용합니다.
        Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // try-catch 블록을 사용하여 네트워크 연결 중 발생할 수 있는 예외를 처리합니다.
        try
        {
            // 위에서 설정한 서버의 IP 주소와 포트(remoteEndPoint)로 연결을 시도합니다.
            sender.Connect(remoteEndPoint);
            // 서버에 성공적으로 연결되었음을 콘솔에 출력합니다.
            Console.WriteLine("Connected to server.");

            // 서버로 전송할 메시지("Hello from Client")를 ASCII 인코딩을 사용해 바이트 배열로 변환합니다.
            byte[] msg = Encoding.ASCII.GetBytes("Hello from Client");
            // 생성된 소켓(sender)을 통해 서버로 메시지(바이트 배열)를 전송합니다.
            sender.Send(msg);

            // 메시지 전송이 끝났으므로 소켓을 닫습니다.
            sender.Close();
        }
        // 만약 try 블록 안에서 예외(에러)가 발생하면,
        catch (Exception e)
        {
            // 발생한 예외의 정보를 콘솔에 출력합니다.
            Console.WriteLine(e.ToString());
        }
    }
}
