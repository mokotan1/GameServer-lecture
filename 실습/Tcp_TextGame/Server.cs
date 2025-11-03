using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    private TcpListener server;
    private List<TcpClient> clients = new List<TcpClient>();
    private Dictionary<TcpClient, Player> players = new Dictionary<TcpClient, Player>();
    
    // 스레드 동기화를 위한 잠금 객체
    private readonly object _lock = new object();

    public void Start()
    {
        server = new TcpListener(IPAddress.Any, 9000);
        server.Start();
        Console.WriteLine("=====================");
        Console.WriteLine("=      베 틀 넷      =");
        Console.WriteLine("= 서버가 시작되었습니다=");
        Console.WriteLine("=====================");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            
            lock (_lock)
            {
                clients.Add(client);
            }

            Thread clientThread = new Thread(HandleClient);
            clientThread.Start(client);
        }
    }

    private void HandleClient(object clientObj)
    {
        TcpClient client = (TcpClient)clientObj;
        NetworkStream stream = client.GetStream();

        byte[] buffer = new byte[256];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string playerName = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

        Player player = new Player(playerName, 100, 10);
        
        lock (_lock)
        {
            players[client] = player;
        }

        Console.WriteLine($"새로운 키보드 워리어 접속 : {player.Name}");

        BroadcastMessage($"{player.Name}키보드 워리어 접속");

        while (true)
        {
            try
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break; // 클라이언트가 정상 종료 (예: /exit)

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim(); // Trim 추가

                Console.WriteLine($"{player.Name}: {message}");

                ProcessMessage(client, player, message); // player 객체 전달
            }
            catch
            {
                Console.WriteLine($"{player.Name} 워리어 연결 종료");
                
                lock (_lock)
                {
                    clients.Remove(client);
                    players.Remove(client);
                }

                BroadcastMessage($"{player.Name} 워리어 퇴장");
                break;
            }
        }
    }

    private void ProcessMessage(TcpClient client, Player player, string message)
    {

        if (message == "/exit")
        {
            client.Close(); // 클라이언트를 닫으면 HandleClient의 catch 블록이 실행되어 정리됨
            return;
        }
        
   
        if (message == "/list")
        {
            StringBuilder playerList = new StringBuilder();
            playerList.AppendLine("--- 접속중인 워리어 ---");
            
            lock (_lock)
            {
                foreach (Player p in players.Values)
                {
                    playerList.AppendLine($"{p.Name} (HP: {p.HP})");
                }
            }
            
            playerList.AppendLine("-----------------------");
            SendMessage(client, playerList.ToString());
        }
   
        else if (message.StartsWith("/Battle") || message.StartsWith("/UseGun") || message.StartsWith("/Headshot"))
        {
            var parts = message.Split(' ');

            if (parts.Length < 2)
            {
                SendMessage(client, "전투대상 없음");
                return;
            }

            string command = parts[0];
            string targetName = parts[1];
            var target = FindPlayerByName(targetName);

            if (target != null)
            {
                if (target == player)
                {
                    SendMessage(client, "스스로를 공격할 수 없습니다.");
                    return;
                }

                // [1번] 공격 유형에 따라 다른 데미지와 메시지 전달
                if (command == "/Battle")
                {
                    ProcessBattle(player, target, player.AttackPower, "기본 공격");
                }
                else if (command == "/UseGun")
                {
                    ProcessBattle(player, target, 10, "총쏘기");
                }
                else if (command == "/Headshot")
                {
                    ProcessBattle(player, target, target.HP, "헤드샷"); 
                }
            }
            else
            {
                SendMessage(client, $"{targetName}키보드 워리어 부재!");
            }
        }
        else
        {
            BroadcastMessage($"{player.Name} : {message}");
        }
    }

    private Player FindPlayerByName(string name)
    {
        lock (_lock)
        {
            foreach (Player player in players.Values)
            {
                if (player.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) return player;
            }
        }
        return null;
    }

    private void ProcessBattle(Player attacker, Player defender, int damage, string attackName)
    {
        defender.HP -= damage;
        
        string battleMessage = $"{attacker.Name}이(가) {defender.Name}에게 {attackName} 시전! 데미지 {damage} 가격. {defender.Name}의 남은 HP : {defender.HP}";

        TcpClient attackerClient = GetClientByPlayer(attacker);
        TcpClient defenderClient = GetClientByPlayer(defender);

        if (attackerClient != null)
        {
            SendMessage(attackerClient, battleMessage);
        }
        if (defenderClient != null)
        {
            SendMessage(defenderClient, battleMessage);
        }


        if (defender.HP <= 0)
        {
            // 사망 메시지는 모두에게 브로드캐스트
            BroadcastMessage($"{defender.Name}키보드 워리어 사망!");
            
            if (defenderClient != null)
            {
                defenderClient.Close();
            }
        }
    }


    private TcpClient GetClientByPlayer(Player player)
    {
        lock (_lock)
        {
            foreach (var entry in players)
            {
                if (entry.Value == player)
                {
                    return entry.Key;
                }
            }
        }
        return null;
    }

    private void BroadcastMessage(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        List<TcpClient> clientsCopy;

        lock (_lock)
        {
            clientsCopy = new List<TcpClient>(clients);
        }

        foreach (var client in clientsCopy)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
            }
            catch
            {
                // 전송 실패 (해당 클라이언트가 방금 연결 종료되었을 수 있음)
            }
        }
    }

    private void SendMessage(TcpClient client, string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }
        catch
        {
            // 메시지 전송 실패
        }
    }
}

class Player
{
    public int HP { get; set; }
    public string Name { get; set; }
    public int AttackPower { get; set; }

    public Player(string name, int hp, int attackPower)
    {
        Name = name;
        HP = hp;
        AttackPower = attackPower;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Server server = new Server();
        server.Start();
    }
}