using System;
using System.Collections.Generic;
using System.Linq; // string.Join을 위해 추가
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class MUDServer
{
    private const int RoomSize = 20;
    private TcpListener listener;
    private Dictionary<int, Player> players = new Dictionary<int, Player>();
    private List<Item> items = new List<Item>(); // Holds items in the room
    private Random random = new Random();

    public MUDServer(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine("Server started...");
        PlaceItems();
        StartAcceptingClients();
    }

    private void StartAcceptingClients()
    {
        listener.BeginAcceptTcpClient(new AsyncCallback(AcceptClientCallback), null);
    }

    private void AcceptClientCallback(IAsyncResult ar)
    {
        if (players.Count >= 2) // 2명 초과 방지 (지시문 3번 힌트)
        {
            Console.WriteLine("Max players reached. Cannot accept more connections.");
            StartAcceptingClients(); // 다시 수신 대기
            return; 
        }

        TcpClient tcpClient = listener.EndAcceptTcpClient(ar);
        int clientId = tcpClient.Client.RemoteEndPoint.GetHashCode();
        Player player = new Player(clientId, tcpClient, GetRandomPosition());
        players[clientId] = player;

        Console.WriteLine($"Player {clientId} connected at position {player.Position}. Awaiting username...");

        StartAcceptingClients();
        StartReceivingData(tcpClient, clientId);
    }

    private void StartReceivingData(TcpClient tcpClient, int clientId)
    {
        NetworkStream stream = tcpClient.GetStream();
        byte[] buffer = new byte[256];
        try
        {
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback((ar) => {
                try
                {
                    int bytesRead = stream.EndRead(ar);
                    if (bytesRead > 0)
                    {
                        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                        // 플레이어가 존재할 때만 명령어 처리
                        if (players.ContainsKey(clientId))
                        {
                            HandleCommand(data, clientId);
                            StartReceivingData(tcpClient, clientId); // 다음 데이터 수신
                        }
                    }
                    else // 0 바이트 수신 = 클라이언트 연결 종료
                    {
                        if (players.ContainsKey(clientId))
                        {
                            Console.WriteLine($"Player {players[clientId].Username} disconnected.");
                            players.Remove(clientId);
                        }
                    }
                }
                catch (Exception) // 스트림 읽기 오류 (강제 종료 등)
                {
                    if (players.ContainsKey(clientId))
                    {
                        Console.WriteLine($"Player {players[clientId].Username} connection lost.");
                        players.Remove(clientId);
                    }
                }
            }), null);
        }
        catch (Exception ex)
        {
             Console.WriteLine($"StartReceivingData Error: {ex.Message}");
             if (players.ContainsKey(clientId))
             {
                 players.Remove(clientId);
             }
        }
    }

    private void HandleCommand(string data, int clientId)
    {
        Player player = players[clientId];
        string[] parts = data.Split(' ');
        string command = parts[0].ToLower();

        switch (command)
        {
            case "username":
                if (parts.Length > 1) player.Username = parts[1];
                Console.WriteLine($"Player {clientId} set username to {player.Username}");
                SendMessage(player, $"Welcome, {player.Username}!");

                if (players.Count == 2)
                {
                    BroadcastMessage("Both players are present. The battle begins!");
                    BroadcastMessage("Use 'help' to see available commands.");
                }
                else
                {
                    BroadcastMessage("Waiting for another player to join...");
                }
                                
                break;

            case "move":
                if (parts.Length > 1)
                    MovePlayer(player, parts[1]);
                break;

            case "loc":
                SendMessage(player, $"Your location: {player.Position}");
                break;

            case "pickup":
                PickupItem(player);
                break;

            case "fart":
                AttackOpponent(player);
                break;

            case "status":
                ShowStatus(player);
                break;

            case "show":
                ShowRoomMap(player);
                break;

            case "help":
                SendHelp(player);
                break;
 
            case "say":
                SayToOpponent(player, parts);
                break;

            case "scan":
                ScanForOpponent(player);
                break;

            default:
                SendMessage(player, "Unknown command. Type 'help' for available commands.");
                break;
        }

        CheckNearby(player);
        CheckGameOver(); // 이동 또는 행동 후에 항상 게임 오버 체크
    }


    private void PlaceItems()
    {
        items.Clear();
        for (int i = 0; i < 5; i++)
        {
            items.Add(new Item("Medicine", GetRandomPosition()));
            items.Add(new Item("Fart Bag", GetRandomPosition()));
            // --- 지시문 4 ---
            items.Add(new Item("Trap", GetRandomPosition()));
            items.Add(new Item("Health Fountain", GetRandomPosition()));
        }
    }

    private void MovePlayer(Player player, string direction)
    {
        (int x, int y) = player.Position;
        switch (direction)
        {
            case "up": y = Math.Max(0, y - 1); break;
            case "down": y = Math.Min(RoomSize - 1, y + 1); break;
            case "left": x = Math.Max(0, x - 1); break;
            case "right": x = Math.Min(RoomSize - 1, y + 1); break;
            default:
                SendMessage(player, "Invalid direction. Use up, down, left, or right.");
                return;
        }
        player.Position = (x, y);
        Console.WriteLine($"{player.Username} moved to {player.Position}");

        CheckForSpecialItems(player);
    }


    private void CheckForSpecialItems(Player player)
    {
        Item item = items.Find(i => i.Position == player.Position);
        if (item != null)
        {
            switch (item.Name)
            {
                case "Trap":
                    player.Health -= 100;
                    SendMessage(player, "You stepped on a Trap! HP decreased by 100.");
                    if (player.Health <= 0) player.Health = 0; // 체력이 음수가 되지 않도록
                    break;
                case "Health Fountain":
                    if (player.Health < 100)
                    {
                        player.Health = Math.Min(100, player.Health + 15);
                        SendMessage(player, "You found a Health Fountain! HP restored by 15.");
                    }
                    else
                    {
                        SendMessage(player, "You found a Health Fountain, but your HP is already full.");
                    }
                    break;
            }
            // Medicine, Fart Bag은 'pickup' 명령어로만 획득
        }
    }


    private void PickupItem(Player player)
    {
        Item item = items.Find(i => i.Position == player.Position);
        if (item != null)
        {
            if (item.Name == "Medicine" || item.Name == "Fart Bag")
            {
                items.Remove(item);
                player.Inventory.Add(item.Name); // Add the item to the player's inventory
                if (item.Name == "Medicine")
                {
                    player.Health = Math.Min(100, player.Health + 20);
                    SendMessage(player, "You picked up a Medicine! HP restored by 20.");
                }
                else if (item.Name == "Fart Bag")
                {
                    player.HasFartBag = true; // HasFartBag은 인벤토리 카운트 대신 단순 플래그로 사용됨
                    SendMessage(player, "You picked up a Fart Bag! Use 'fart' to attack.");
                }
            }
            else if (item.Name == "Trap" || item.Name == "Health Fountain")
            {
                SendMessage(player, $"You cannot pick up a {item.Name}.");
            }
            else
            {
                 SendMessage(player, "No item here to pick up.");
            }
        }
        else
        {
            SendMessage(player, "No item here to pick up.");
        }
    }

    private void AttackOpponent(Player player)
    {
        // HasFartBag 대신 인벤토리의 Fart Bag 개수를 세도록 수정
        int fartBagCount = player.Inventory.Count(item => item == "Fart Bag");

        if (fartBagCount == 0)
        {
            SendMessage(player, "You don't have a Fart Bag. Find one first!");
            return;
        }

        Player opponent = GetOpponent(player);
        if (opponent != null && GetDistance(player.Position, opponent.Position) <= 2)
        {
            opponent.Health -= 10;
            // Fart Bag 1개 사용 (인벤토리에서 제거)
            player.Inventory.Remove("Fart Bag");
            
            SendMessage(player, "Fart attack successful! Opponent damaged.");
            SendMessage(opponent, "You were hit by a fart attack! HP decreased by 10.");
        }
        else
        {
            SendMessage(player, "Opponent not in range for fart attack.");
        }
    }

    private void ShowStatus(Player player)
    {
        // Count the number of fart bags in the player's inventory
        int fartBagCount = player.Inventory.Count(item => item == "Fart Bag");

        // Build the status message
        StringBuilder statusMessage = new StringBuilder();
        statusMessage.AppendLine("Your Status:");
        statusMessage.AppendLine($"- Username: {player.Username}");
        statusMessage.AppendLine($"- HP: {player.Health}");
        statusMessage.AppendLine($"- Fart Bags: {fartBagCount}");
        statusMessage.AppendLine($"- Position: {player.Position}");

        // Send the status message to the player
        SendMessage(player, statusMessage.ToString());
    }


    private void SayToOpponent(Player player, string[] parts)
    {
        if (parts.Length < 2)
        {
            SendMessage(player, "Usage: say [message]");
            return;
        }
        Player opponent = GetOpponent(player);
        if (opponent != null)
        {
            // "say" 명령어 뒤의 모든 텍스트를 메시지로 재구성
            string message = string.Join(" ", parts, 1, parts.Length - 1);
            SendMessage(opponent, $"{player.Username}: {message}");
        }
        else
        {
            SendMessage(player, "There is no one here to talk to.");
        }
    }

    private void ScanForOpponent(Player player)
    {
        Player opponent = GetOpponent(player);
        if (opponent != null)
        {
            int distance = GetDistance(player.Position, opponent.Position);
            if (distance <= 3)
            {
                SendMessage(player, $"Opponent {opponent.Username} detected at {opponent.Position}. Distance: {distance}");
            }
            else
            {
                SendMessage(player, "Opponent not detected (range > 3).");
            }
        }
        else
        {
            SendMessage(player, "No opponent to scan for.");
        }
    }


    private void ShowRoomMap(Player player)
    {
        char[,] roomMap = new char[RoomSize, RoomSize];

        // Initialize map with empty spaces
        for (int y = 0; y < RoomSize; y++)
        {
            for (int x = 0; x < RoomSize; x++)
            {
                roomMap[x, y] = '.'; // Empty space
            }
        }
        foreach (var item in items)
        {
            (int ix, int iy) = item.Position;
            if (item.Name == "Medicine")
                roomMap[ix, iy] = 'M';
            else if (item.Name == "Fart Bag")
                roomMap[ix, iy] = 'F';
            else if (item.Name == "Trap") // 지시문 4
                roomMap[ix, iy] = 'T';
            else if (item.Name == "Health Fountain") // 지시문 4
                roomMap[ix, iy] = 'H';
        }

        // 플레이어 배치 (아이템 위에 겹쳐서 표시됨)
        int playerNumber = 1;
        foreach (var p in players.Values)
        {
            (int px, int py) = p.Position;
            roomMap[px, py] = (char)('0' + playerNumber++);
        }
        // --- 끝 ---


        // Build the map display with consistent padding for each cell
        StringBuilder mapDisplay = new StringBuilder("Room Map:\n");
        for (int y = 0; y < RoomSize; y++)
        {
            for (int x = 0; x < RoomSize; x++)
            {
                mapDisplay.Append($"{roomMap[x, y],-2}"); // Use -2 to make each cell two characters wide
            }
            mapDisplay.AppendLine(); // New line after each row to keep rows consistent
        }

        // Send the entire map as a single message to the client
        SendMessage(player, mapDisplay.ToString());
    }

    private void CheckNearby(Player player)
    {
        Player opponent = GetOpponent(player);
        if (opponent != null && GetDistance(player.Position, opponent.Position) <= 2)
        {
            SendMessage(player, "Your opponent is nearby!");
        }
    }

    private void CheckGameOver()
    {
        List<Player> losers = new List<Player>();
        List<Player> remainingPlayers = new List<Player>();

        foreach (var player in players.Values)
        {
            if (player.Health <= 0)
            {
                losers.Add(player);
            }
            else
            {
                remainingPlayers.Add(player);
            }
        }

        // 패배자가 없으면 아무것도 안 함
        if (losers.Count == 0) return;

        // --- CASE 1: 무승부 (2명 게임에서 2명 다 짐) ---
        if (remainingPlayers.Count == 0 && losers.Count > 0)
        {
            BroadcastMessage("It's a draw! Both players are defeated. Game over.");
            ResetGame(); // 둘 다 내보내고 게임 초기화
        }
        else if (losers.Count == 1 && remainingPlayers.Count == 1 && players.Count == 2)
        {
            Player loser = losers[0];
            Player winner = remainingPlayers[0];
            
            BroadcastMessage($"{loser.Username} has been defeated. Game over.");
            
            // 승자에게 알림
            SendMessage(winner, "You are victorious! Waiting for a new challenger...");
            
            // 패자 접속 종료 및 제거
            SendMessage(loser, "You have been defeated. You are disconnected.");
            loser.TcpClient.Close(); // 소켓 닫기
            players.Remove(loser.ClientId); // 딕셔너리에서 제거
            
            // 승자를 위해 아이템 재배치
            PlaceItems();
        }
        else if (losers.Count > 0 && remainingPlayers.Count == 0)
        {
            BroadcastMessage("All players have been defeated. Game over.");
            ResetGame();
        }
    }


    private void ResetGame()
    {
        // 모든 플레이어 연결 종료
        foreach (var player in players.Values)
        {
            player.TcpClient.Close();
        }
        players.Clear();
        PlaceItems();
        BroadcastMessage("Game has been reset. Waiting for players...");
    }

    private void SendHelp(Player player)
    {
        string helpText = "Available commands:\n" +
                          "- move [up/down/left/right]: Move in the room\n" +
                          "- loc: Show your current coordinates\n" +
                          "- pickup: Pick up an item at your location\n" +
                          "- fart: Attack opponent if within range\n" +
                          "- show: Display map\n" +
                          "- status: Show your current status (HP, Position, etc.) \n" +
                          "- say [message]: Chat with your opponent \n" + 
                          "- scan: Scan for opponent within 3 units \n" + 
                          "- help: Show this help message";
        SendMessage(player, helpText);
    }

    private void BroadcastMessage(string message)
    {
        foreach (var player in players.Values)
        {
            SendMessage(player, message);
        }
    }

    private void SendMessage(Player player, string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n"); // 메시지 끝에 개행 추가
            player.TcpClient.GetStream().Write(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message to {player.Username}: {ex.Message}");
            // 메시지 전송 실패 시 해당 플레이어 제거 (연결 끊김)
             if (players.ContainsKey(player.ClientId))
             {
                 players.Remove(player.ClientId);
                 Console.WriteLine($"Player {player.Username} removed due to send error.");
             }
        }
    }

    private Player GetOpponent(Player player)
    {
        foreach (var p in players.Values)
        {
            if (p.ClientId != player.ClientId) return p;
        }
        return null;
    }

    private int GetDistance((int x, int y) pos1, (int x, int y) pos2)
    {
        return Math.Abs(pos1.x - pos2.x) + Math.Abs(pos1.y - pos2.y);
    }

    private (int x, int y) GetRandomPosition()
    {
        return (random.Next(RoomSize), random.Next(RoomSize));
    }
}

// Player and Item classes

class Player
{
    public int ClientId { get; }
    public string Username { get; set; }
    public (int x, int y) Position { get; set; }
    public List<string> Inventory { get; private set; }
    public int Health { get; set; } = 100;
    public bool HasFartBag { get; set; } = false; 
    public TcpClient TcpClient { get; }

    public Player(int clientId, TcpClient tcpClient, (int x, int y) position)
    {
        ClientId = clientId;
        TcpClient = tcpClient;
        Position = position;
        Inventory = new List<string>(); // Initialize the inventory as an empty list
    }
}

class Item
{
    public string Name { get; }
    public (int x, int y) Position { get; }

    public Item(string name, (int x, int y) position)
    {
        Name = name;
        Position = position;
    }
}

class Program
{
    static void Main(string[] args)
    {
        int port = 12345;  // Specify the port for the server
        MUDServer server = new MUDServer(port);
        Console.WriteLine($"Server is running on port {port}. Waiting for players to connect...");

        // Prevent the main thread from exiting
        while (true)
        {
            // Keep the server running
            Thread.Sleep(1000);
        }
    }
}

