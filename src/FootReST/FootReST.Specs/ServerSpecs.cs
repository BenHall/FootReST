using System;
using FootReST.Specs.Infrastructure;
using Xunit;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;

namespace FootReST.Specs
{
    public class Server_specs_start : IDisposable
    {
        Server _server;

        public void Dispose()
        {
            _server.Close();
        }
        [Fact]
        public void Can_Start_Listening_On_Default_Port_5984()
        {
            _server = new Server();
            bool started = _server.Start();
            Assert.True(started);

            Assert_Port_Open("127.0.0.1", 5984);
        }

        private void Assert_Port_Open(string ip, int port)
        {
 	        using(var c = new TcpClient())
            {
                c.Connect(ip, port);
                Assert.True(c.Connected);
            }
        }
    }

    public class Server_specs_version : IDisposable
    {
        Server server;
        public Server_specs_version()
        {
            server = new Server();
            server.Start();
        }

        public void Dispose()
        {
            server.Close();
        }

        [Fact]
        public void Can_return_version_in_json_format()
        {
            JsonRequester request = new JsonRequester();
            JObject json = request.Get("127.0.0.1", "5984", "");
            Assert.Equal("0.1.0", json.Value<string>("version"));
        }

        [Fact]
        public void Returns_welcome_message()
        {
            JsonRequester request = new JsonRequester();
            JObject json = request.Get("127.0.0.1", "5984", "");
            Assert.Equal("Welcome", json.Value<string>("footrest"));
        }

        [Fact]
        public void Can_override_version_to_return_couchdb_style()
        {
            string returnMessage = "{\"couchdb\":\"Welcome\",\"version\":\"0.11.0\"}";
            server.DefineCustomResponse("GET", "/", returnMessage);

            JsonRequester request = new JsonRequester();
            JObject json = request.Get("127.0.0.1", "5984", "");
            Assert.Equal("Welcome", json.Value<string>("couchdb"));
            Assert.Equal("0.11.0", json.Value<string>("version"));
        }
    }
}
