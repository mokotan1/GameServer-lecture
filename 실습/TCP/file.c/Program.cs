//file client

using System;
using System.IO;
using System.Net.Sockets;

class FileSender
{
    static void Main(string[] args)
    {
        TcpClient client = new TcpClient("127.0.0.1", 9000);
        // The client creates a NetworkStream object to exchange data with the server.
        NetworkStream stream = client.GetStream();

        //Opens the file to be transferred in read mode. The file can only be opened if it exists.
        using (FileStream fileStream = new FileStream("file_to_send.txt", FileMode.Open))
        {
            byte[] buffer = new byte[1024];//Create a buffer to store file data
            int bytesRead;
            //Read data from a file into a buffer
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                //Sending read file data to the server
                stream.Write(buffer, 0, bytesRead);
            }//Repeat until the entire file is transferred to the server.
        }

        Console.WriteLine("File sent.");
        client.Close();
    }
}
