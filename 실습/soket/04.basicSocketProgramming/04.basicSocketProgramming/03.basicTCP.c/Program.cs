using System; // 기본 시스템 기능을 위한 네임스페이스
using System.Text; // 텍스트 인코딩 관련 클래스
using System.Net; // 네트워크 관련 클래스
using System.Net.Sockets; // 소켓 관련 클래스

namespace BasicSocketClient // BasicSocketClient 네임스페이스 정의
{
    class Program // Program 클래스 정의
    {
        static void Main(string[] args) // 프로그램의 진입점
        {
            Connect(); // Connect 함수 호출
        }

        static void Connect() // 서버에 접속하고 데이터 송수신하는 함수
        {
            // 1. 서버에 접속
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999); // 서버 IP와 포트 지정
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // TCP 소켓 생성
            client.Connect(ipep); // 서버에 연결
            
            Console.WriteLine("Socket connect"); // 연결 성공 메시지 출력

            // 2. 서버로부터 데이터 받기
            Byte[] _data = new Byte[1024]; // 수신 데이터 버퍼 생성
            int received = client.Receive(_data); // 서버로부터 데이터 수신
            
            String _buf = Encoding.UTF8.GetString(_data); // 수신 데이터를 문자열로 변환
            Console.WriteLine(_buf); // 받은 데이터 출력
            
            // 3. 서버에 데이터 보내기
            _buf = "소켓 접속 확인 됐습니다"; // 보낼 메시지 작성
            _data = Encoding.UTF8.GetBytes(_buf); // 메시지를 바이트 배열로 변환
            client.Send(_data); // 서버로 데이터 전송
            
            // 4. 서버와 접속 끊기
            client.Close(); // 소켓 닫기
            
            Console.WriteLine("Press any key..."); // 종료 안내 메시지 출력
            Console.ReadKey(); // 키 입력 대기
        }
    }
}