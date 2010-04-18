using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace FootReST
{
    public class RequestHandler
    {
        readonly Dictionary<string, Dictionary<string, string>> _verbResponses;
        private readonly TcpListener _server;

        public RequestHandler(TcpListener server)
        {
            _server = server;
            _verbResponses = new Dictionary<string, Dictionary<string, string>>();
            _verbResponses.Add("GET", new Dictionary<string, string>());
            _verbResponses["GET"].Add("/", "{\"footrest\":\"Welcome\",\"version\":\"0.1.0\"}");
        }

        public void DefineCustomResponse(string verb, string endpoint, string response)
        {
            EnsureVerbExists(verb);

            if (_verbResponses[verb].Keys.Contains(endpoint))
                _verbResponses[verb].Remove(endpoint);

            _verbResponses[verb].Add(endpoint, response);
        }

        private void EnsureVerbExists(string verb)
        {
            if (!_verbResponses.ContainsKey(verb))
                _verbResponses.Add(verb, new Dictionary<string, string>());
        }

        public void ProcessConnections()
        {
            TcpClient client = _server.AcceptTcpClient();
            NetworkStream ns = client.GetStream();

            string request = NetworkStreamHandler.ReadStreamIntoString(ns);

            HandleRequest(ns, request);

            NetworkStreamHandler.WriteStringToStream(ns, "");

            ns.Close();
        }

        private void HandleRequest(NetworkStream ns, string request)
        {
            string verb = GetRequestVerb(request);
            string endpoint = GetRequestedEndpoint(verb, request);
            string response = GetResponseForEndpoint(verb, endpoint);

            WriteResponse(ns, response);
        }

        internal string GetRequestVerb(string request)
        {
            int indexOfSpace = request.IndexOf(" ");
            return request.Substring(0, indexOfSpace);
        }

        private void WriteResponse(NetworkStream ns, string response)
        {
            NetworkStreamHandler.WriteOkResponse(ns);
            NetworkStreamHandler.WriteStringToStream(ns, "Content-Length: " + response.Length);
            NetworkStreamHandler.WriteStringToStream(ns, "");
            NetworkStreamHandler.WriteStringToStream(ns, response);
        }

        internal string GetResponseForEndpoint(string verb, string endpoint)
        {
            if(_verbResponses.ContainsKey(verb) && _verbResponses[verb].ContainsKey(endpoint))
                return _verbResponses[verb][endpoint];

            return GetWildcardResponse(verb, endpoint);
        }

        internal string GetWildcardResponse(string verb, string endpoint)
        {
            if (_verbResponses.ContainsKey(verb))
            {
                foreach (var possibleResponse in _verbResponses[verb])
                {
                    if (Regex.IsMatch(endpoint, possibleResponse.Key))
                        return possibleResponse.Value;
                }
            }

            return string.Empty;
        }

        internal string GetRequestedEndpoint(string verb, string request)
        {
            string endpointWithoutVerb = request.Substring(verb.Length + 1);
            int endOfRequest = endpointWithoutVerb.IndexOf(" ");

            return endpointWithoutVerb.Substring(0, endOfRequest);
        }
    }
}
