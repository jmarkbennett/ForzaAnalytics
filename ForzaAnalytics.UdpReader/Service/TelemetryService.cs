using System.Net.Sockets;
using System.Net;
using ForzaAnalytics.UdpReader.Model;
namespace ForzaAnalytics.UdpReader.Service
{
    public class TelemetryService
    {
        private readonly IPAddress address;
        private UdpClient client;
        private IPEndPoint endpoint;

        public TelemetryService(string address, int port)
        {
            this.address = IPAddress.Parse(address);
            this.client = new UdpClient(port);
            this.endpoint = new IPEndPoint(this.address, port);
        }

        public Telemetry ReceiveEvents()
        {
            byte[] bytes = client.Receive(ref endpoint);

            return new Telemetry(bytes);
        }
    }

}
