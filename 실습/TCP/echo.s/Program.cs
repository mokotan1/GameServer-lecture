//echo tcp server

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class TcpEchoServer
{
    static void Main(string[] args)
    {
        //TcpListener is the role of waiting for a connection request
        //  from a client on the server side.
        //IPAddress.Any: The server connects from any IP address regardless
        //  of the network interface.
        //9000: TCP port number.
        TcpListener listener = new TcpListener(IPAddress.Any, 9000);
        listener.Start();
        Console.WriteLine("Server started...");

        while (true)
        {
            // The server waits indefinitely until the client requests a connection
            //AcceptTcpClient() accepts the client's connection request,
            //and returns a TcpClient object for communication with the client.
            TcpClient client = listener.AcceptTcpClient();

            //NetworkStream is a stream for sending and receiving data
            //  with a client via a TCP connection.
            //Data transmission and reception with the client is done through
            //  this stream.
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            //bytesRead: The size of the data actually read
            //  from the client (in bytes).
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            //Encoding.ASCII.GetString():
            //  Method to convert a byte array to a string
            string receivedMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Received: " + receivedMessage);

            // Echo back the message
            //Send bytesRead amount of data from
            //  the beginning of the buffer array to the client.
            stream.Write(buffer, 0, bytesRead);
            client.Close();
        }
    }
}
