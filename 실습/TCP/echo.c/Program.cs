//echo tcp client

using System;
using System.Net.Sockets;
using System.Text;

class TcpEchoClient
{
    static void Main(string[] args)
    {
        TcpClient client = new TcpClient("127.0.0.1", 9000);
        //The client creates a NetworkStream to exchange data with the server.
        //The client and server communicate through this stream.
        NetworkStream stream = client.GetStream();

        string message = "Hello, Server!"; //The client writes a message to send to the server.
        byte[] data = Encoding.ASCII.GetBytes(message);//Convert string to byte array

        // Send the message
        stream.Write(data, 0, data.Length);

        // Receive the echoed message
        //The client calls stream.Read()
        //  to read the data echoed from the server.
        //Store the data sent from the server in the buffer array
        //  and return the number of bytes read.
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);

        string receivedMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        Console.WriteLine("Received from server: " + receivedMessage);

        client.Close();
    }
}
