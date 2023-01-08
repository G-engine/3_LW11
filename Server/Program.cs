using System.Net;
using System.Net.Sockets;
using System.Text;

IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 8888);
using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
socket.Bind(ipPoint);
Console.WriteLine(socket.LocalEndPoint);

socket.Listen();
Console.WriteLine("Сервер запущен. Ожидание подключений...");

using Socket client = await socket.AcceptAsync();
Console.WriteLine($"Адрес подключенного клиента: {client.RemoteEndPoint}");

byte[] messageData = new byte[512];
StringBuilder message = new StringBuilder();
await client.ReceiveAsync(messageData, SocketFlags.None);
message.Append(Encoding.UTF8.GetString(messageData));
int i = -1;
foreach (var s in message.ToString())
{
    ++i;
    if (s == '\0')
    {
        message.Remove(i, message.Length - i);
        break;
    }
}
Console.WriteLine($"Сообщение принято: {message}");

string response = "";
using (var db = new TickerdbContext())
{
    foreach (var ticker in db.Tickers)
    {
        if (ticker.Ticker1 == message.ToString())
        {
            response = $"id = {ticker.Id}, {ticker.Ticker1}: {db.Prices.OrderBy(x => x.Date).Last(p => p.TickerId == ticker.Id).Price1}$ ({db.Prices.OrderBy(x => x.Date).Last(p => p.TickerId == ticker.Id).Date})";
            break;
        }
               
    }
}
await client.SendAsync(Encoding.UTF8.GetBytes(response), SocketFlags.None);
Console.WriteLine("Ответ отправлен");


