﻿using SwarmController.Models.Swarm;
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

        public string missionName { get; set; }
        public int missionID { get; set; }
        public int droneIdInMissionCounter { get; set; }
        public int numberOfDronesInMission { get; set; } // kaldırılabilir
        public List<int> assignedDronePorts { get; set; }

        //public List<TcpClient> tcpClients;
        public List<Drone> drones { get; set; }
        public List<Route> routes { get; set; }

        public MissionBase()
        {
            missionName = "NaN";
            missionID = -1;
        }

        public override string ToString()
        {
            return $"{missionID} - {missionName}";
        }
    }
}
