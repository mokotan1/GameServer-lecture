using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;


class MUDClient
{
    private TcpClient client;
    private NetworkStream stream;
    private string username;
    private bool connected = false;

    public MUDClient(string serverAddress, int port)
    {
        try
        {
            client = new TcpClient(serverAddress, port);
            stream = client.GetStream();
            connected = true;
            Console.WriteLine("Connected to server.");

            StartReceivingData();
        }
        catch (SocketException)
        {
            Console.WriteLine("Failed to connect to server. Is the server running?");
            connected = false;
        }
    }

    public void StartGame()
    {
        if (!connected) return;

        Console.Write("Enter your username: ");
        username = Console.ReadLine();
        SendMessage($"username {username}");
        Console.WriteLine("Type 'help' to see available commands.");

        while (connected)
        {
            string input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                SendMessage(input);
            }
        }
    }

    private void StartReceivingData()
    {
        Thread thread = new Thread(() =>
        {
            while (connected)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine(message);
                    }
                    else
                    {
                        // 0 바이트 수신은 서버가 연결을 정상적으로 닫았음을 의미
                        Console.WriteLine("Server closed the connection.");
                        connected = false;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Disconnected from server.");
                    connected = false;
                }
            }
        });
        thread.IsBackground = true;
        thread.Start();
    }

    private void SendMessage(string message)
    {
        if (!connected) return;
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
        catch (Exception)
        {
            Console.WriteLine("Error sending message. Disconnected from server.");
            connected = false;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        string serverAddress = "127.0.0.1";
        int port = 12345;
        MUDClient mUDClient = new MUDClient(serverAddress, port);

        mUDClient.StartGame();
    }
}
