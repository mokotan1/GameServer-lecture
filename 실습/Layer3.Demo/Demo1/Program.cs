using System;
using System.Net;

class NetworkLayerExample
{
    static void Main(string[] args)
    {
        string ipString = "192.168.1.10";
        string subnetMask = "255.255.255.0";

        // Convert the IP string to an IPAddress object
        IPAddress ip = IPAddress.Parse(ipString);
        IPAddress mask = IPAddress.Parse(subnetMask);

        Console.WriteLine($"IP Address: {ip}");
        Console.WriteLine($"Subnet Mask: {mask}");

        // Convert IP to binary format
        Console.WriteLine($"Binary IP: {ConvertToBinary(ip)}");
        Console.WriteLine($"Binary Mask: {ConvertToBinary(mask)}");

        // Demonstrate subnet calculation (network address)
        IPAddress networkAddress = CalculateNetworkAddress(ip, mask);
        Console.WriteLine($"Network Address: {networkAddress}");
    }

    static string ConvertToBinary(IPAddress ip)
    {
        byte[] bytes = ip.GetAddressBytes();
        string binaryString = string.Join(".", Array.ConvertAll(bytes, b => Convert.ToString(b, 2).PadLeft(8, '0')));
        return binaryString;
    }

    static IPAddress CalculateNetworkAddress(IPAddress ip, IPAddress mask)
    {
        byte[] ipBytes = ip.GetAddressBytes();
        byte[] maskBytes = mask.GetAddressBytes();
        byte[] networkBytes = new byte[ipBytes.Length];

        for (int i = 0; i < ipBytes.Length; i++)
        {
            networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
        }

        return new IPAddress(networkBytes);
    }
}