using SwarmController.Models.Swarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SwarmController.Models.Plan
{
    public class MissionBase
    {
        public int missionID { get; set; }
        public int numberOfDronesInMission { get; set; }
        public List<int> assignedDronePorts { get; set; }
        //public List<TcpClient> tcpClients;
        public List<Drone> drones { get; set; }
        public List<Route> routes { get; set; }
    }
}
