using System; // 기본 시스템 기능을 위한 네임스페이스
using System.Net; // 네트워크 관련 클래스
using System.Net.Sockets; // 소켓 관련 클래스
using System.Text; // 텍스트 인코딩 관련 클래스

namespace BasicSocketClient // BasicSocketClient 네임스페이스 정의
{
    class Program // Program 클래스 정의
    {
        static void Main(string[] args) // 프로그램의 진입점
        {
            Console.WriteLine("Basic TCP Client!"); // 클라이언트 시작 메시지 출력
            StartClient(); // 클라이언트 동작 함수 호출
        }

        public static void StartClient() // 클라이언트 동작 함수
        {
            // 수신 데이터 버퍼 생성
            byte[] bytes = new byte[1024];
            
            try
            {
                // 현재 호스트의 IP 정보 가져오기
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                // IPv4 주소 중 하나 선택
                IPAddress ipAddress = Array.Find(ipHostInfo.AddressList, a => a.AddressFamily == AddressFamily.InterNetwork);
                if (ipAddress == null) // IPv4 주소가 없으면 예외 발생
                {
                    throw new Exception("No IPv4 address found for the host.");
                }
                // 서버의 IP와 포트로 엔드포인트 생성
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11002);

                // 1. TCP 소켓 객체 생성
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    // 2. 서버에 보낼 메시지 준비
                    byte[] msg = Encoding.ASCII.GetBytes("This is a test...<EOF>");

                    // 서버에 연결 시도
                    sender.Connect(remoteEP);
                    Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint); // 연결 성공 메시지 출력

                    // 3. 서버로 데이터 전송
                    int bytesSent = sender.Send(msg);

                    // 4. 서버에서 데이터 수신
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec)); // 받은 데이터 출력
                }
                catch (ArgumentNullException ane) // 인자가 null일 때 예외 처리
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se) // 소켓 예외 처리
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e) // 기타 예외 처리
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
                finally
                {
                    // 소켓이 연결되어 있으면 종료 처리
                    if (sender.Connected)
                    {
                        sender.Shutdown(SocketShutdown.Both); // 송수신 종료
                    }
                    sender.Close(); // 소켓 닫기
                }
            }
            catch (Exception e) // 전체 예외 처리
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}