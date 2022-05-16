using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;


namespace Haberlesme
{
    public static class ReceivePacket
    {
        //private MAVLink.MavlinkParse mavparse = new MAVLink.MavlinkParse();

        public static MAVLink.MAVLinkMessage ReceieveTCPPackets(TcpClient tcpClient)
        {
            MAVLink.MavlinkParse mavparse = new MAVLink.MavlinkParse();
            Stream stream = tcpClient.GetStream();
            return mavparse.ReadPacket(stream);
        }

        public static bool ReceiveTCPPacket(TcpClient tcpClient, string msgtypename, int timeout=500)
        {
            MAVLink.MavlinkParse mavparse = new MAVLink.MavlinkParse();
            NetworkStream stm = tcpClient.GetStream();

            while (true)
            {
                var packet = mavparse.ReadPacket(stm);
                //Console.WriteLine("Received 1: {0}", packet);
                if (packet.msgtypename == msgtypename)
                {
                    Console.WriteLine("GOTCHA!");
                    Console.WriteLine(packet);
                    return true;
                }
            }
        }

    }
}


