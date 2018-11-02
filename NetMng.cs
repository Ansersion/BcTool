using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BcTool
{
    class NetMng
    {
        public static int BC_SERVER_PORT = 8025;
        private Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private string address;
        private int port;

        public bool connect()
        {
            bool ret = true;
            IPAddress ip = IPAddress.Parse(address);
            IPEndPoint point = new IPEndPoint(ip, port);

            try
            {
                client.Connect(point);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ret = false;
            }

            return ret;
        }

        public bool write(byte[] msg)
        {
            bool ret = true;
            try
            {
                if(client.Send(msg) <= 0)
                {
                    ret = false;
                }
            } 
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                ret = false;
            }
            return ret;
        }

        public void setAddress(String address)
        {
            this.address = address;
        }

        public void setPort(int port)
        {
            this.port = port;
        }

    }
}
