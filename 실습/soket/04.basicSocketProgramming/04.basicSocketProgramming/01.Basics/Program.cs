//Basic Concepts - Networking
//1. IP Address
//2. DNS
//3. EndPoint

using System;
//네트워크 관련한 클래스들이 들어있음 
using System.Net;
internal class Program
{
    private static void Main(string[] args)
    {
        ///////////////////////////////////////////////////////
        //I. IP 주소 (System.Net.IPAddress)
        //-IP 주소는 인터넷에 연결된 컴퓨터들의 주소
        //-.NET에서 IP 주소를 사용하기 위해서는 System.Net의 IPAddress클래스를 사용한다.
        ///////////////////////////////////////////////////////

        //1. IPAddress 객체는 IP 주소를 표현하는 클래스이다.
        Console.WriteLine("I.1 IPAddress 객체 생성");
        //하나 영역에 들어가는 10진수 == 0~256 
        //ip주소는 32비트
        IPAddress ip1 = IPAddress.Parse("192.169.1.13");
        Console.WriteLine(ip1);

        // Parse()
        // 숫자로 변환할 문자열을 넘기면 숫자로 변환
        int a = int.Parse("12345");
        float b = float.Parse("123.45");
        Console.WriteLine(a); // 12345
        Console.WriteLine(b); // 123.45


        //2. 혹은, 바이트 배열 혹은 정수를 직접 IPAddress 생성자에 넣어 IPAddress 객체를 만들 수도 있다!
        //IPAddress 객체로부터 ToString() 메서드를 호출하면 "192.168.1.13"과 같은 표현으로 IP가 출력된다.
        Console.WriteLine("I.2 바이트 배열 혹은 정수로 IPAddress 객체 생성");
        IPAddress ip2 = new IPAddress(new byte[] { 192, 169, 1, 14 });
        Console.WriteLine(ip2.ToString()); // "192.169.1.14" 출력

        // ToString()
        // 숫자 데이터 형식을 문자열로 변환
        // 정수 계열 데이터 형식이나 부동 소수점 데이터 형식은 object로부터 물려받은 ToString 메서드를 오버라이드 즉, 자신이 갖고 있는 숫자를 문자열로 변환하도록 재정의
        int c = 12345;
        string d = c.ToString();

        float e = 123.45f;
        string f = e.ToString();
        Console.WriteLine(d); // "12345" 출력
        Console.WriteLine(f); // "123.45" 출력

        //3. 유용한 IPAddress 메서드
        Console.WriteLine("I.3 유용한 IPAddress 메서드");
        IPAddress ip = IPAddress.Parse("216.58.216.174");
        byte[] ipbytes = ip.GetAddressBytes(); // IP를 바이트 배열로
        IPAddress ipv6 = ip.MapToIPv6(); // IPv4를 IPv6로 매핑
        Console.WriteLine(ip); // 216.58.216.174
        Console.WriteLine(ipbytes[0]); // 216
        Console.WriteLine(ipbytes[1]); // 58
        Console.WriteLine(ipbytes[2]); // 216
        Console.WriteLine(ipbytes[3]); // 174
        Console.WriteLine(ipbytes.Length); // 4
        //128비트 
        Console.WriteLine(ipv6); // IPv6 주소 출력

        ///////////////////////////////////////////////////////
        //II. DNS (System.Net.Dns)
        //-DNS(Domain Name System)는 도메인 이름과 IP 주소를 서로 변환하는 시스템
        //-예를 들어, www.naver.com이라는 도메인 이름은 223.130.195.200이라는 IP 주소로 변환된다.
        ///////////////////////////////////////////////////////

        //1. localhost
        Console.WriteLine("II.1 localhost 조회");
        string host = Dns.GetHostName();
        Console.WriteLine(host); // 현재 호스트(computer) 이름 출력

          //ip 하나에 도메인 여러 개가잇을 수 잇기 떄문에 배열로 받아옴 
        IPAddress[] addresses = Dns.GetHostAddresses(host);
        foreach (IPAddress address in addresses)
        {
            Console.WriteLine(address); // 호스트의 IP 주소 출력
        }

        //2. 특정 도메인 이름의 IP 주소 조회
        Console.WriteLine("II.2 특정 도메인 이름의 IP 주소 조회");
        IPAddress[] naverAddresses = Dns.GetHostAddresses("www.naver.com");
        foreach (IPAddress address in naverAddresses)
        {
            Console.WriteLine(address); // www.naver.com의 IP 주소 출력

        }
        //도메인 이름을 사용할 때의 단 한가지 단점이라면, DNS로부터 IP 주소를 조회해야 하기 때문에 그만큼 속도가 저하된다는 것이다.
        //이 때문에 윈도우 운영체제에서는 내부적으로 한번 조회된 적이 있는 도메인명과 IP주소는 일정 시간 동안 저장해 두는 기능이 있다. 그래서 다음 번에 동일한 DNS 조회 요청이 오면 서버와의 통신 없이 미리 저장해 둔 IP 주소를 곧바로 반환함으로써 속도를 향상시킨다.
        //1개의 도메인명에 N개의 IP가 묶인 경우 일종의 부하 분산(load balance)역할을 하기도 한다.

        ///////////////////////////////////////////////////////
        //III. EndPoint (System.Net.IPEndPoint)
        //-EndPoint는 IP 주소와 포트 번호를 함께 표현하는 클래스. TCP나 UDP는 IP 주소와 함께 포트번호를 사용한다. 이러한 종단점(EndPoint)을 표현하기 위해 IPEndPoint 클래스를 사용한다.
        //-포트 번호는 0부터 65535까지의 숫자 중에서 지정할 수 있다.
        //-포트 번호는 네트워크 상에서 특정한 서비스를 구분하는 데 사용된다.
        // 예를 들어, 웹 서버는 일반적으로 포트 번호 80을 사용하고, FTP 서버는 포트 번호 21을 사용한다.
        //-IPEndPoint는 IP주소와 포트번호의 조합으로, ToString() 메서드를 호출하면 "IP주소:포트번호" 형식으로 문자열을 리턴한다.
        //-주로 네트워크 통신에서 특정한 호스트와 포트를 지정할 때 사용된다.
        ///////////////////////////////////////////////////////

        IPAddress ip3 = IPAddress.Parse("74.125.28.99");
        IPEndPoint ep = new IPEndPoint(ip3, 80);
        //III.1 IPEndPoint 객체 생성
        Console.WriteLine("III.1 IPEndPoint 객체 생성");
        Console.WriteLine(ep.Address); // "74.125.28.99"    출력
        Console.WriteLine(ep.Port);    // "80"              출력
        Console.WriteLine(ep.ToString()); // "74.125.28.99:80" 출력
        //III.2 IPEndPoint 객체 비교
        Console.WriteLine("III.2 IPEndPoint 객체 비교");
        IPEndPoint ep2 = new IPEndPoint(IPAddress.Parse("74.125.28.99"), 80);
        Console.WriteLine(ep.Equals(ep2)); // True 출력 (주소와 포트가 동일하므로)
        IPEndPoint ep3 = new IPEndPoint(IPAddress.Parse("74.125.28.99"), 8080);
        Console.WriteLine(ep.Equals(ep3)); // False 출력 (포트가 다르므로)

        //End of File!
    }//Main
}//Program class