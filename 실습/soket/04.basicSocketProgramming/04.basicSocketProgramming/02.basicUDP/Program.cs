using System; // 기본 시스템 기능을 사용하기 위한 네임스페이스
using System.Net; // 네트워크 관련 클래스 사용
using System.Net.Sockets; // 소켓 관련 클래스 사용
using System.Text; // 텍스트 인코딩 관련 클래스 사용
using System.Threading; // 스레드 관련 클래스 사용

namespace BasicUDP // BasicUDP라는 네임스페이스 정의
{
    class Program // Program 클래스 정의
    {
        static void Main(string[] args) // 프로그램의 진입점
        {
            // 서버 소켓이 동작하는 스레드 생성
            Thread serverThread = new Thread(serverFunc); // 서버 함수 실행할 스레드 생성
            serverThread.IsBackground = true; // 백그라운드 스레드로 설정(Main 종료시 같이 종료됨)
            serverThread.Start(); // 서버 스레드 시작
            
            Thread.Sleep(500); // 서버 소켓용 스레드가 실행될 시간을 주기 위함
            
            Thread clientThread = new Thread(clientFunc); // 클라이언트 함수 실행할 스레드 생성
            clientThread.IsBackground = true; // 백그라운드 스레드로 설정
            clientThread.Start(); // 클라이언트 스레드 시작
            
            Console.WriteLine("종료하려면 아무 키나 누르세요...."); // 종료 안내 메시지 출력
            Console.ReadLine(); // 사용자 입력 대기
        }

        private static void serverFunc(object obj) // 서버 스레드에서 실행될 함수
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); // UDP 소켓 생성
            
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 10200); // 모든 IP, 10200 포트로 엔드포인트 생성

            socket.Bind(endPoint); // 소켓을 엔드포인트에 바인딩

            byte[] recvBytes = new byte[1024]; // 수신 데이터 버퍼 생성
            EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0); // 클라이언트 엔드포인트 생성

            try
            {
                while (true) // 무한 루프
                {
                    int nRecv = socket.ReceiveFrom(recvBytes, ref clientEP); // 클라이언트로부터 데이터 수신
                    string txt = Encoding.UTF8.GetString(recvBytes, 0, nRecv); // 수신 데이터 문자열로 변환: recvBytes의 인덱스 번호 0에서부터 nRecv바이트를 UTF-8로 디코딩하여 새 C# 문자열(UTF-16)을 생성
                    byte[] sendBytes = Encoding.UTF8.GetBytes("Hello:" + txt + " from." + clientEP); // 응답 메시지 생성
                    socket.SendTo(sendBytes, clientEP); // 클라이언트에게 응답 전송
                }
            }
            catch (SocketException ex) // 소켓 예외 처리
            {
                Console.WriteLine("Server socket exception: " + ex.Message); // 예외 메시지 출력
            }
            finally
            {
                socket.Close(); // 소켓 닫기
                Console.WriteLine("UDP Server socket: Closed"); // 소켓 종료 메시지 출력
            }
        }

        private static void clientFunc(object obj) // 클라이언트 스레드에서 실행될 함수
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); // UDP 소켓 생성
            
            EndPoint serverEP = new IPEndPoint(IPAddress.Loopback, 10200); // 서버 엔드포인트(로컬호스트, 10200포트) 생성
            EndPoint senderEP = new IPEndPoint(IPAddress.Any, 0); // 송신자 엔드포인트 생성, port: 0 → 동적 포트. 커널이 사용 가능한 임의의 포트를 자동 배정

            int times = 5; // 5번 반복

            while (times-- > 0) // 5번 반복 루프
            {
                byte[] buf = Encoding.UTF8.GetBytes(DateTime.Now.ToString()); // 현재 시간 문자열을 바이트 배열로 변환
                socket.SendTo(buf, serverEP); // 서버로 데이터 전송

                byte[] recvBytes = new byte[1024]; // 수신 데이터 버퍼 생성
                int nRecv = socket.ReceiveFrom(recvBytes, ref senderEP); // 서버로부터 데이터 수신

                string txt = Encoding.UTF8.GetString(recvBytes, 0, nRecv); // 수신 데이터 문자열로 변환
                
                Console.WriteLine(txt); // 수신 메시지 출력
                Thread.Sleep(1000); // 1초 대기
            }

            socket.Close(); // 소켓 닫기
            Console.WriteLine("UDP Client socket: Closed"); // 소켓 종료 메시지 출력
        }
        
    }
}