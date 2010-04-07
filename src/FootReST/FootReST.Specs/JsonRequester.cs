using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace FootReST.Specs
{
    public class JsonRequester
    {
        public JObject Get(string host, string port, string endpoint)
        {
            Stream responseStream = MakeRequest(host, port, endpoint);

            string json;
            using (var reader = new StreamReader(responseStream))
            {
                json = reader.ReadToEnd();
            }

            return JObject.Parse(json);
        }

        public Stream MakeRequest(string host, string port, string endpoint)
        {
            var request = (HttpWebRequest)WebRequest.Create(string.Format("http://{0}:{1}/{2}", host, port, endpoint));
            var response = (HttpWebResponse)request.GetResponse();
            return response.GetResponseStream();
        }
    }

}
