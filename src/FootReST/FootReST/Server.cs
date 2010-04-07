using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace FootReST
{
    public class Server
    {
        private int DefaultPort = 5984;
        private IPAddress IPAddress = IPAddress.Loopback;

        TcpListener server;
        RequestHandler handler;

        public bool Start()
        {
            server = new TcpListener(IPAddress, DefaultPort);
            server.Start();

            handler = new RequestHandler(server);
            Thread thread = new Thread(new ThreadStart(handler.ProcessConnections));
            thread.Start();
            
            return true;
        }

        public void Close()
        {
            server.Stop();
        }

        public void DefineCustomResponse(string endpoint, string response)
        {
            handler.DefineCustomResponse(endpoint, response);
        }
    }
}
