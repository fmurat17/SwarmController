using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SwarmController.Models.Swarm
{
    public class Drone
    {
        public int port { get; set; }
        public TcpClient tcpClient { get; set; }
        public int missionID { get; set; }

        public double lat;
        public double lng;
        public double alt;

        public double roll;
        public double yaw;
        public double pitch;

        public Drone(int port, int missionID)
        {
            this.port = port;
            this.missionID = missionID;
        }
    }

}
