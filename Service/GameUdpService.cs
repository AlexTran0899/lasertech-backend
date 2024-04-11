using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using lasertech_backend.Enum;
using lasertech_backend.Interface;

public class GameUdpService : IUdpService
{
    private readonly UdpClient udpClient;
    private readonly int listenPort;
    private readonly int transmitPort;
    private List<string> equipmentIDs;
    
    public GameUdpService(int listenPort, int transmitPort)
    {
        this.listenPort = listenPort;
        this.transmitPort = transmitPort;
        this.udpClient = new UdpClient(listenPort);
        this.equipmentIDs = new List<string>();
    }

    public async Task StartListening(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            var receivedResult = await udpClient.ReceiveAsync();
            string receivedMessage = Encoding.ASCII.GetString(receivedResult.Buffer); // need to optimize this later
            Console.WriteLine($"Received: {receivedMessage}");
            
            byte[] bytes = Encoding.ASCII.GetBytes(receivedResult.ToString());
            await udpClient.SendAsync(bytes, bytes.Length, "127.0.0.1", transmitPort);  
            
        }
    }

    public async Task broadcastGameStatus(GameStatus gameStatus)
    {
        int statusCode = gameStatus == GameStatus.Start ? 202 : 221;
        byte[] bytes = Encoding.ASCII.GetBytes(statusCode.ToString());
        if (gameStatus == GameStatus.Start)
        {
            await udpClient.SendAsync(bytes, bytes.Length, "127.0.0.1", transmitPort);   
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                await Task.Delay(500);
                await udpClient.SendAsync(bytes, bytes.Length, "127.0.0.1", transmitPort);   
            }
        }
    }

    public async Task StartBroadcastEquipment(CancellationToken token)
    {
        Console.WriteLine("Broadcast started...");
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(2000);
            if (equipmentIDs.Count < 1) continue;
            Console.WriteLine("Broadcasting equpmenent ID...");
            foreach (var equipmentID in this.equipmentIDs)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(equipmentID);
                await udpClient.SendAsync(bytes, bytes.Length, "127.0.0.1", transmitPort);
            }
        }
    }

    public void AddEquipmentID(int equipmentID)
    {
        this.equipmentIDs.Add(equipmentID.ToString());
    }
}