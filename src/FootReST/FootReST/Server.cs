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

        Dictionary<string, string> Responses;

        public bool Start()
        {
            Responses = new Dictionary<string, string>();
            Responses.Add("/", "{\"footrest\":\"Welcome\",\"version\":\"0.1.0\"}");

            server = new TcpListener(IPAddress, DefaultPort);
            server.Start();

            Thread thread = new Thread(new ThreadStart(ProcessConnections));
            thread.Start();
            
            return true;
        }

        public void Close()
        {
            server.Stop();
        }

        public void DefineRequest(string endpoint, string response)
        {
            if (Responses.Keys.Contains(endpoint))
                Responses.Remove(endpoint);

            Responses.Add(endpoint, response);
        }

        private void ProcessConnections()
        {
            TcpClient client = server.AcceptTcpClient();
            NetworkStream ns = client.GetStream();

            string request = ReadStreamIntoString(ns);

            if (request.StartsWith("GET "))
            {
                string endpoint = GetEndpoint("GET", request);
                string response = Responses[endpoint];

                WriteOKResponse(ns);
                WriteStringToStream(ns, "Content-Length: " + response.Length);
                WriteStringToStream(ns, "");
                WriteStringToStream(ns, response);
            }

            WriteStringToStream(ns, "");

            ns.Close();
        }
        
        private static string GetEndpoint(string verb, string request)
        {
            string endpointWithoutVerb = request.Substring(verb.Length + 1);
            int endOfRequest = endpointWithoutVerb.IndexOf(" ");

            return endpointWithoutVerb.Substring(0, endOfRequest);
        }

        private void WriteOKResponse(NetworkStream ns)
        {
            WriteStringToStream(ns, "HTTP/1.1 200 OK");
            WriteStringToStream(ns, "Server: FootReST/0.1.0");
            WriteStringToStream(ns, "Date: " + DateTime.Now.ToString());
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
