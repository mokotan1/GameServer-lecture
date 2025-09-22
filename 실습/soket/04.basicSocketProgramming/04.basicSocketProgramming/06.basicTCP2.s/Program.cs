using System;                         // 기본 입출력(Console) 등 핵심 BCL
using System.Text;                    // 인코딩(Encoding.ASCII/UTF8 등)
using System.Net;                     // Dns, IPHostEntry, IPAddress
using System.Net.Sockets;             // Socket, IPEndPoint 등 소켓 API

namespace BasicSocketServer           // 네임스페이스 선언
{
    class Program                     // 엔트리 클래스
    {
        static int Main(string[] args)    // 애플리케이션 시작점(Main)
        {
            Console.WriteLine("Basic TCP Server"); // 서버 시작 안내 메시지 출력
            StartListening();                     // 리스닝(수신 대기) 로직 진입
            return 0;                             // 프로세스 종료 코드(성공)
        }

        // Incoming data from the client
        public static string data = null;         // 클라이언트로부터 누적 수신한 텍스트를 담을 변수(초기 null)
                                                  // null에 += 하면 첫 결합 시 빈 문자열로 간주되어 정상 동작한다.

        public static void StartListening()       // 서버 소켓을 열고 요청을 처리하는 메서드
        {
            // Data buffer for incoming data
            byte[] bytes = new Byte[1024];        // 수신 버퍼(1KB). Receive가 채운 실제 길이만 사용해야 한다.

            // Dns.Resolve(...) is obsoleted for this type
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName()); // 로컬 호스트 정보 조회(IPv4/IPv6 주소 목록 포함)
            IPAddress ipAddress = Array.Find(ipHostInfo.AddressList, a => a.AddressFamily == AddressFamily.InterNetwork); //a: 람다 매개변수 (IPAddress) / => : 람다 연산자, 왼쪽의 매개변수들을 받아서, 오른쪽 식(또는 블록)을 실행
                                                  // 주소 목록에서 IPv4(InterNetwork) 하나 선택
            if (ipAddress == null)                // IPv4 주소를 찾지 못했다면
            {
                throw new Exception("No IPv4 address found for this host."); // 예외 발생
            }
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11002); // 바인드할 로컬 끝점(선택한 IPv4, 포트 11000)  // (팁) 모든 NIC에서 받으려면 IPAddress.Any 사용 가능

            // 1, 소켓 객체 생성 (TCP 소켓)
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                                                  // TCP 스트림 소켓 생성(IPv4 패밀리)

            try
            {
                // 2. 포트에 Bind
                listener.Bind(localEndPoint);     // 로컬 끝점에 소켓 바인드(해당 IP/포트 점유)

                // 3. 포트 Listening 시작
                listener.Listen(10);              // 연결 대기 시작, 백로그 큐 크기=10

                while (true)                      // 메인 Accept 루프(연결을 연속 처리)
                {
                    Console.WriteLine("Waiting for a connection... Listening at {0}", localEndPoint); // 접속 대기 안내

					// 4. 연결을 받아들여 새 소켓 생성
                    Socket handler = listener.Accept(); // 클라이언트 연결 수락(블로킹), 통신 전용 소켓 반환
                    data = null;                        // 누적 버퍼 초기화(새 연결마다 리셋)

                    while (true)                        // 수신 루프(현재 연결에서 데이터 끝까지 받기)
                    {
                        // 5. 소켓 수신
                        int bytesRec = handler.Receive(bytes);         // 블로킹 수신, 읽은 바이트 수 반환
                                                                       // (주의) bytesRec == 0이면 원격이 정상 종료(FIN)한 것
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                                                                       // 받은 구간만 ASCII로 디코딩하여 누적
                                                                       // (주의) 한글/유니코드면 UTF8 권장: Encoding.UTF8.GetString(...)
                        if (data.IndexOf("<EOF>") > -1)                // 프로토콜 종료 토큰("<EOF>")을 받았는지 검사
                        {
                            break;                                     // 종료 토큰 수신 → 루프 탈출
                        }
                    }

                    Console.WriteLine("Text received : {0}", data);    // 수신 문자열 콘솔 출력

                    byte[] msg = Encoding.ASCII.GetBytes(data);        // 에코할 응답을 ASCII로 인코딩(원본 그대로 반환)
                                                                       // (주의) 유니코드 대응 필요 시 UTF8 사용

                    // 6. 소켓 송신
                    handler.Send(msg);                                 // 클라이언트로 데이터 전송

                    // 7. 소켓 닫기
                    handler.Shutdown(SocketShutdown.Both);             // 송·수신 모두 종료(half-close 처리)
                    handler.Close();                                   // 소켓 리소스 해제(Dispose)
                }
            }
            catch (Exception e)                                        // 예외 처리
            {
                Console.WriteLine(e.ToString());                       // 예외 정보 출력
            }

            Console.WriteLine("\nPress ENTER to continue...");          // 종료 대기 메시지
            Console.Read();                                             // ENTER 입력 대기(콘솔 유지)
        }
    }
}
