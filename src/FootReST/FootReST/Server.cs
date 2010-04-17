using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace FootReST
{
    public class Server
    {
        private int DefaultPort = 5984;
        private IPAddress IPAddress = IPAddress.Loopback;

        TcpListener _server;
        RequestHandler _handler;

        public bool Start()
        {
            _server = new TcpListener(IPAddress, DefaultPort);
            _server.Start();

            _handler = new RequestHandler(_server);
            Thread thread = new Thread(_handler.ProcessConnections);
            thread.Start();
            
            return true;
        }

        public void Close()
        {
            _server.Stop();
        }

        public void DefineCustomResponse(string endpoint, string response)
        {
            _handler.DefineCustomResponse(endpoint, response);
        }
    }
}
