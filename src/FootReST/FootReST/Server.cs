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
        public bool Start()
        {
            server = new TcpListener(IPAddress, DefaultPort);
            server.Start();

            Thread thread = new Thread(new ThreadStart(ProcessConnections));
            thread.Start();

            return true;
        }

        private void ProcessConnections()
        {
            TcpClient client = server.AcceptTcpClient();
            NetworkStream ns = client.GetStream();

            string request = ReadStreamIntoString(ns);

            if (request.StartsWith("GET / HTTP/1.1"))
            {
                string response = "{\"footrest\":\"Welcome\",\"version\":\"0.1.0\"}";
                WriteOKResponse(ns);
                WriteStringToStream(ns, "Content-Length: " + response.Length);
                WriteStringToStream(ns, "");
                WriteStringToStream(ns, response);
            }

            WriteStringToStream(ns, "");

            ns.Close();
        }

        private void WriteOKResponse(NetworkStream ns)
        {
            WriteStringToStream(ns, "HTTP/1.1 200 OK");
            WriteStringToStream(ns, "Server: FootReST/0.1.0");
            WriteStringToStream(ns, "Date: Mon, 05 Apr 2010 11:36:24 GMT");
            WriteStringToStream(ns, "Content-Type: text/plain;charset=utf-8");
        }

        private void WriteStringToStream(NetworkStream ns, string data)
        {
            string dataToSend = data + "\r\n";
            ns.Write(Encoding.ASCII.GetBytes(dataToSend), 0, dataToSend.Length);
            ns.Flush();
        }

        private string ReadStreamIntoString(NetworkStream ns)
        {
            byte[] buffer = new byte[1024];
            ns.Read(buffer, 0, buffer.Length);
            while (ns.DataAvailable)
            {
                ns.Read(buffer, 0, buffer.Length);
            }
            ASCIIEncoding enc = new ASCIIEncoding();
            return enc.GetString(buffer);
        }
    }
}
