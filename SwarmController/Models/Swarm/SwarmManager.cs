using Haberlesme;
using SwarmController.Models.Plan;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SwarmController.Models.Swarm
{
    public class SwarmManager
    {
        private static SwarmManager swarmManager = null;
        public static SwarmManager getSwarmManager()
        {
            if (swarmManager == null)
            {
                swarmManager = new SwarmManager();
            }

            return swarmManager;
        }

        public string localhost = "127.0.0.1";
        public MissionBase currentMission = null;
        public int totalNumberOfDrones = 3;

        // dict<port, missionID> -1 = available, others = assigned to a mission
        public Dictionary<int, int> allDrones = new Dictionary<int, int>();

        public void createDrones()
        {
            for(int i = 0; i < totalNumberOfDrones; i++)
            {
                int port = 5760 + i * 10 + 3;
                allDrones.Add(port, -1);
            }
        }

        public void startMission()
        {
            assignDrones();
            uploadMissionToDrones();
            flyDrones();
        }

        public void assignDrones()
        {
            int numberOfDronesInMission = currentMission.numberOfDronesInMission;
            int missionID = currentMission.missionID;
            

            for (int i = 0, p = 0; i < numberOfDronesInMission;)
            {
                int port = 5760 + (p * 10) + 3;
                p++;
                if (allDrones[port] == -1)
                {
                    allDrones[port] = missionID;
                    currentMission.tcpClients.Add(new TcpClient());
                    currentMission.assignedDrones.Add(port);
                    i++;
                }
            }
        }

        public void uploadMissionToDrones()
        {
            //(currentMission as MissionSurvelliance).uploadMissionToDrones();

            //for(int i = 0; i < currentMission.assignedDrones.Count; i++)
            for(int i = 0; i < 3; i++)
            {
                int k = i; // böyle yapmazsak i pass by reference olmuş oluyor ve içeride değeri değişiyor
                Thread uploadMissionThread = new Thread(() => uploadMission(k));
                uploadMissionThread.Start();
            }
        }

        public void uploadMission(int i)
        {
            TcpClient tcpClient = currentMission.tcpClients[i];
            int port = currentMission.assignedDrones[i];

            currentMission.tcpClients[i].Connect(localhost, port);

            int numberOfMissionItems = (currentMission as MissionSurvelliance).routes[i].gMapRoute.Points.Count;
            Debug.WriteLine($"mission_item: {numberOfMissionItems}, {port}");
            SendPacket.send_mavlink_mission_count_t_tcp(tcpClient,
                                                        numberOfMissionItems,
                                                        MAVLink.MAV_MISSION_TYPE.MISSION);

            for (int j = 0; j < numberOfMissionItems; j++)
            {
                Debug.WriteLine($"{j}, {port}");
                if (ReceivePacket.ReceiveTCPPacket(tcpClient, "MISSION_REQUEST"))
                {
                    int lat = (int)((currentMission as MissionSurvelliance).routes[i].missionItems[j].koordinat.Lat * 1e7);
                    int lng = (int)((currentMission as MissionSurvelliance).routes[i].missionItems[j].koordinat.Lng * 1e7);
                    float alt = (float)(currentMission as MissionSurvelliance).routes[i].missionItems[j].altitude;

                    SendPacket.send_mavlink_mission_item_int_t_tcp(tcpClient,
                                                                   0, 0, 0, 0,
                                                                   lat, lng, alt,
                                                                   (ushort)j,
                                                                   MAVLink.MAV_CMD.WAYPOINT,
                                                                   1, 1,
                                                                   MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT_INT,
                                                                   MAVLink.MAV_MISSION_TYPE.MISSION);
                }
            }

            //
        }

        public void flyDrones()
        {

        }

    }
}