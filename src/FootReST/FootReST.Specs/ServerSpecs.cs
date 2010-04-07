using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;

namespace FootReST.Specs
{
    public class ServerSpecs_Start
    {
        [Fact]
        public void Can_Start_Listening_On_Default_Port_5984()
        {
            Server server = new Server();
            bool started = server.Start();
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

    public class ServerSpecs_Version
    {
        public ServerSpecs_Version()
        {
            Server server = new Server();
            server.Start();
        }
        
        [Fact]
        public void Can_return_version_in_json_format()
        {
            JsonRequester request = new JsonRequester();
            JObject json = request.Get("127.0.0.1", "5984", "");
            Assert.Equal("0.1.0", json.Value<string>("version"));
        }

        [Fact]
        public void Can_override_version_to_return_coucdb_style()
        {
            Assert.True(false);
        }
    }



}
