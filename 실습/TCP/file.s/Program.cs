//file server

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

class FileReceiver
{
    static void Main(string[] args)
    {
        TcpListener listener = new TcpListener(IPAddress.Any, 9000);
        listener.Start();
        Console.WriteLine("Server is ready to receive file...");

        while (true)
        {
            //listener.AcceptTcpClient():
            //   When a client requests a connection,
            //   the server accepts the connection through this method, and 
            //   creates a TcpClient object that can communicate with the client.
            using (TcpClient client = listener.AcceptTcpClient())
            //The using statement here ensures that the TcpClient connection is properly closed when the code exits this block.
            //  This automatically releases the network resources associated with the client.

            //client.GetStream(): exchanges data with the client.
            using (NetworkStream stream = client.GetStream())
            //new FileStream("received_file.txt", FileMode.Create):
            //   server creates a file stream to store the file it will receive
            //   from the client.
            //   FileMode.Create: Create a new file if it doesn't exist, or overwrite it if it does exist.
            using (FileStream fileStream = new FileStream("received.txt", FileMode.Create))
            {
                //Create a buffer for data transfer. Process data 1024 bytes at a time.
                byte[] buffer = new byte[1024];
                int bytesRead;
                //stream.Read: Stores data sent by the client in the buffer, and 
                //   returns the size (bytes) of the data read.
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    //Write data sent by client to file
                    fileStream.Write(buffer, 0, bytesRead);
                }
                //Continue reading data sent by the client through a loop
                //   until all data is written to the file
            }
            Console.WriteLine("File received.");
        }
    }
}
