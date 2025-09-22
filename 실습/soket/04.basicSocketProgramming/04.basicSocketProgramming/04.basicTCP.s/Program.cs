using System; // 기본 시스템 기능을 위한 네임스페이스
using System.Text; // 텍스트 인코딩 관련 클래스
using System.Net; // 네트워크 관련 클래스
using System.Net.Sockets; // 소켓 관련 클래스

namespace BasicSocketServer // BasicSocketServer 네임스페이스 정의
{
    class Program // Program 클래스 정의
    {
        static void Main(string[] args) // 프로그램의 진입점
        {
            // 1. 서버 소켓 초기화
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 9999); // 모든 IP, 9999 포트로 엔드포인트 생성
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // TCP 서버 소켓 생성

            server.Bind(ipep); // 소켓을 엔드포인트에 바인딩
            server.Listen(20); // 최대 20개의 클라이언트 연결 요청을 대기열에 저장

            Console.WriteLine("Server Start....Listen port 9999..."); // 서버 시작 메시지 출력

            // 2. 클라이언트가 접속 할 때까지 대기
            Socket client = server.Accept(); // 클라이언트 접속 대기 및 연결 수락
            
            // 3. 새로운 클라이언트 접속
            IPEndPoint ip = (IPEndPoint) client.RemoteEndPoint; // 접속한 클라이언트의 IP 엔드포인트 정보 가져오기
            
            Console.WriteLine("주소 {0} 에서 접속", ip.Address); // 클라이언트 주소 출력

            // 4. 클라이언트에 데이터를 보낸다
            String buf = "Woori 서버에 오신 걸 환영합니다."; // 클라이언트에게 보낼 환영 메시지 작성
            Byte[] data = Encoding.Default.GetBytes(buf); // 메시지를 바이트 배열로 변환 (오타: _buf → buf)
            client.Send(data); // 클라이언트에게 데이터 전송

            // 5. 클라이언트로부터 데이터를 받는다
            data = new Byte[1024]; // 수신 데이터 버퍼 생성
            int receivedBytes = client.Receive(data); // 클라이언트로부터 데이터 수신
            buf = Encoding.UTF8.GetString(data); // 수신 데이터를 문자열로 변환 (오타: _data → data)

            Console.WriteLine(buf); // 받은 데이터 출력

            // 클라이언트/서버 소켓 종료
            client.Close(); // 클라이언트 소켓 닫기
            server.Close(); // 서버 소켓 닫기
        }
    }
}