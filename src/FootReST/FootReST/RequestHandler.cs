using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace FootReST
{
    public class RequestHandler
    {
        Dictionary<string, string> _responses;
        private TcpListener _server;

        public RequestHandler(TcpListener server)
        {
            _server = server;
            _responses = new Dictionary<string, string>();
            _responses.Add("/", "{\"footrest\":\"Welcome\",\"version\":\"0.1.0\"}");
        }

        public void DefineCustomResponse(string endpoint, string response)
        {
            if (_responses.Keys.Contains(endpoint))
                _responses.Remove(endpoint);

            _responses.Add(endpoint, response);
        }

        public void ProcessConnections()
        {
            TcpClient client = _server.AcceptTcpClient();
            NetworkStream ns = client.GetStream();

            string request = NetworkStreamHandler.ReadStreamIntoString(ns);

            if (request.StartsWith("GET "))
            {
                string endpoint = GetEndpoint("GET", request);
                string response = _responses[endpoint];

                NetworkStreamHandler.WriteOkResponse(ns);
                NetworkStreamHandler.WriteStringToStream(ns, "Content-Length: " + response.Length);
                NetworkStreamHandler.WriteStringToStream(ns, "");
                NetworkStreamHandler.WriteStringToStream(ns, response);
            }

            NetworkStreamHandler.WriteStringToStream(ns, "");

            ns.Close();
        }

        private static string GetEndpoint(string verb, string request)
        {
            string endpointWithoutVerb = request.Substring(verb.Length + 1);
            int endOfRequest = endpointWithoutVerb.IndexOf(" ");

            return endpointWithoutVerb.Substring(0, endOfRequest);
        }
    }
}
