using System.Net.Sockets;
using System.Text;

using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
try
{
    await socket.ConnectAsync("192.168.43.232", 8888);
    Console.WriteLine($"Подключение к {socket.RemoteEndPoint} установлено");
    
    string message = Console.ReadLine();
    await socket.SendAsync(Encoding.UTF8.GetBytes(message), SocketFlags.None); 
    Console.WriteLine("Сообщение отправлено");
    
    
    byte[] responseData = new byte[512];
    StringBuilder response = new StringBuilder();
    await socket.ReceiveAsync(responseData, SocketFlags.None);
    response.Append(Encoding.UTF8.GetString(responseData));
    int i = -1;
    foreach (var s in response.ToString())
    {
        ++i;
        if (s == '\0')
        {
            response.Remove(i, response.Length - i);
            break;
        }
    }
    Console.WriteLine(response);
}
catch (SocketException)
{
    Console.WriteLine($"Не удалось установить подключение с {socket.RemoteEndPoint}");
}