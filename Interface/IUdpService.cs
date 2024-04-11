namespace lasertech_backend.Interface;

public interface IUdpService
{
    Task StartListening(CancellationToken token);
    Task StartBroadcastEquipment(CancellationToken token);
}