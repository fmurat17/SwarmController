using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;


namespace Haberlesme
{
    public class ReceivePacket
    {
        private MAVLink.MavlinkParse mavparse = new MAVLink.MavlinkParse();

        public MAVLink.MAVLinkMessage ReceieveTCPPacket(TcpClient tcpClient)
        {
            Stream stream = tcpClient.GetStream();
            return mavparse.ReadPacket(stream);
        }

    }
}


