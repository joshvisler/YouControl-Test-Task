using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SockNet.ServerSocket
{
    internal class TcpListenerAdapter : ITcpServer
    {
        private TcpListener _tcpListener;

        public void CreateTcpListener(IPAddress ip, int port)
        {
            _tcpListener = new TcpListener(ip, port);           
        }

        public void Start()
        {
            _tcpListener.Start();
        }

        public async Task<TcpClient> AcceptTcpClientAsync()
        {
            var res = await _tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
            return res;
        }

        public void Stop()
        {
            _tcpListener.Stop();
        }
    
    }
}
