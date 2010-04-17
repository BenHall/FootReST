using System;
using System.Text;
using System.Net.Sockets;

namespace FootReST
{
    public class NetworkStreamHandler
    {
        public static void WriteOkResponse(NetworkStream ns)
        {
            WriteStringToStream(ns, "HTTP/1.1 200 OK");
            WriteStringToStream(ns, "Server: FootReST/0.1.0");
            WriteStringToStream(ns, "Date: " + DateTime.Now);
            WriteStringToStream(ns, "Content-Type: text/plain;charset=utf-8");
        }

        public static void WriteStringToStream(NetworkStream ns, string data)
        {
            string dataToSend = data + "\r\n";
            ns.Write(Encoding.ASCII.GetBytes(dataToSend), 0, dataToSend.Length);
            ns.Flush();
        }

        public static string ReadStreamIntoString(NetworkStream ns)
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
