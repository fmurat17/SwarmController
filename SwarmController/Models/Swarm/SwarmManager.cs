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
        //public Dictionary<int, int> allDrones = new Dictionary<int, int>();
        public List<Drone> allDrones = new List<Drone>();

        public Drone getDroneByMissionIdAndPort(int missionID, int port)
        {
            foreach(Drone drone in allDrones)
            {
                if(drone.missionID == missionID && drone.port == port)
                {
                    return drone;
                }
            }

            return new Drone(-10,-10);
        }

        public void createDrones()
        {
            for(int i = 0; i < totalNumberOfDrones; i++)
            {
                int port = 5760 + i * 10 + 3;
                Drone drone = new Drone(port, -1);
                //drone.tcpClient = new TcpClient();
                allDrones.Add(drone);
                //allDrones.Add(port, -1);
            }
        }

        public void InitListenAllDrones()
        {
            for(int i = 0; i < this.allDrones.Count; i++)
            {
                int k = i;
                Thread thread = new Thread(() => ListenDrone(allDrones[k]));
                thread.Start();
            }
        }

        public void InitListenAllDronesOfMission()
        {

        }

        public void ListenDrone(Drone drone)
        {
            //drone.tcpClient.Connect(localhost, drone.port);
            //TcpClient tcpClient = new TcpClient(localhost, drone.port);

            TcpClient tcpClient = drone.tcpClient;

            // GLOBAL_POSITION_INT - lat lng alt
            SendPacket.send_mavlink_command_long_t_tcp(tcpClient,
                                                       33,
                                                       20000,
                                                       0, 0, 0, 0, 0,
                                                       MAVLink.MAV_CMD.SET_MESSAGE_INTERVAL
                                                       );

            // ATTITUDE - roll yaw pitch
            SendPacket.send_mavlink_command_long_t_tcp(tcpClient,
                                                       30,
                                                       20000,
                                                       0, 0, 0, 0, 0,
                                                       MAVLink.MAV_CMD.SET_MESSAGE_INTERVAL
                                                       );

            // HEARTBEAT
            SendPacket.send_mavlink_command_long_t_tcp(tcpClient,
                                                       0,
                                                       50000,
                                                       0, 0, 0, 0, 0,
                                                       MAVLink.MAV_CMD.SET_MESSAGE_INTERVAL
                                                       );


            while (true)
            {
                var packet = ReceivePacket.ReceieveTCPPackets(tcpClient);

                try
                {
                    switch (packet.msgtypename)
                    {
                        case "HEARTBEAT":
                            break;
                        case "ATTITUDE":
                            MAVLink.mavlink_attitude_t attitude = (MAVLink.mavlink_attitude_t)packet.data;
                            drone.roll = attitude.roll;
                            drone.yaw = attitude.yaw;
                            drone.pitch = attitude.pitch;
                            break;
                        case "GLOBAL_POSITION_INT":
                            MAVLink.mavlink_global_position_int_t pos = (MAVLink.mavlink_global_position_int_t)packet.data;
                            drone.lat = pos.lat / 1e7;
                            drone.lng = pos.lon / 1e7;
                            break;
                    }

                    Debug.WriteLine($"{drone.port} -> {packet}");
                }
                catch
                {
                    Debug.WriteLine("ListenDrone -> error");
                }

                
            }
        }

        //public void ListenDrone(Drone drone)
        //{
        //    int current = 0;
        //    int previous = 0;

        //    while (true)
        //    {
        //        try
        //        {
        //            current++;
        //            if (previous + 1 != current)
        //            {
        //                drone.tcpClient.Connect(localhost, drone.port);
        //                break;
        //            }
        //        }
        //        catch
        //        {
        //            Debug.WriteLine("Ports has not been opened yet!");
        //            previous++;
        //        }
        //    }

        //    while (true)
        //    {
        //        try
        //        {
        //            var packet = ReceivePacket.ReceieveTCPPackets(drone.tcpClient);
        //            if(packet.msgtypename == "GPS_GLOBAL_ORIGIN")
        //            {
        //                Debug.WriteLine(packet);
        //            }
        //        }
        //        catch
        //        {
        //            Debug.WriteLine("error!!");
        //        }
        //    }
        //}

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
            

            for (int i = 0, p = 0; i < numberOfDronesInMission ; )
            {
                int port = 5760 + (p * 10) + 3;
                if (allDrones[p].missionID == -1)
                {
                    allDrones[p].missionID = missionID;
                    currentMission.assignedDronePorts.Add(port);
                    //TcpClient tcpClient = new TcpClient();
                    //currentMission.tcpClients.Add(tcpClient);
                    ////currentMission.assignedDronePorts.Add(port);
                    //Drone newDrone = new Drone(port, missionID);
                    //newDrone.tcpClient = tcpClient;
                    //currentMission.drones.Add(newDrone);
                    i++;
                }
                p++;
            }
        }

        public void uploadMissionToDrones()
        {
            //(currentMission as MissionSurvelliance).uploadMissionToDrones();

            //for(int i = 0; i < currentMission.assignedDronePorts.Count; i++)
            for(int i = 0; i < 3; i++)
            {
                int k = i; // böyle yapmazsak i pass by reference olmuş oluyor ve içeride değeri değişiyor
                Thread uploadMissionThread = new Thread(() => uploadMission(k));
                uploadMissionThread.Start();
            }
        }

        public void uploadMission(int i)
        {
            //TcpClient tcpClient = currentMission.tcpClients[i];
            ////int port = currentMission.assignedDronePorts[i];
            //int port = currentMission.drones[i].port;

            //currentMission.tcpClients[i].Connect(localhost, port);

            int missionId = currentMission.missionID;
            int port = currentMission.assignedDronePorts[i];
            Drone drone = getDroneByMissionIdAndPort(missionId, port);
            TcpClient tcpClient = drone.tcpClient;

            int numberOfMissionItems = (currentMission as MissionSurvelliance).routes[i].gMapRoute.Points.Count;
            Debug.WriteLine($"mission_item: {numberOfMissionItems}, {port}");
            SendPacket.send_mavlink_mission_count_t_tcp(tcpClient,
                                                        numberOfMissionItems,
                                                        MAVLink.MAV_MISSION_TYPE.MISSION);

            for (int j = 0; j < numberOfMissionItems; j++)
            {
                Debug.WriteLine($"{j}, {port}");
                try
                {
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
                catch
                {

                }
            }
            //currentMission.tcpClients[i].Close();
            Thread.Sleep(1000);
            
            //
        }

        public void flyDrones()
        {
            for (int i = 0; i < currentMission.numberOfDronesInMission; i++)
            {
                int k = i;
                Thread thread = new Thread(() => flyDrone(allDrones[k]));
                thread.Start();
            }
        }

        public void flyDrone(Drone drone)
        {
            //TcpClient tcpClient = new TcpClient(localhost, drone.port);

            //TcpClient tcpClient = currentMission.tcpClients[drone.droneID];

            TcpClient tcpClient = drone.tcpClient;

            // guided'a al
            SendPacket.send_mavlink_command_long_t_tcp(tcpClient,
                                                       89,
                                                       4,
                                                       0, 0, 0, 0, 0,
                                                       MAVLink.MAV_CMD.DO_SET_MODE);
            Thread.Sleep(100);

            // arm
            SendPacket.send_mavlink_command_long_t_tcp(tcpClient,
                                                       1,
                                                       1,
                                                       0, 0, 0, 0, 0,
                                                       MAVLink.MAV_CMD.COMPONENT_ARM_DISARM);
            Thread.Sleep(100);

            Debug.WriteLine("GAPATHH!!");
            // takeoff
            SendPacket.send_mavlink_command_long_t_tcp(tcpClient,
                                                       0, 1, 0, 0, 0, 0,
                                                       10,
                                                       MAVLink.MAV_CMD.TAKEOFF);
            Thread.Sleep(10000); // 10 sec

            // auto
            // guided'a al
            SendPacket.send_mavlink_command_long_t_tcp(tcpClient,
                                                       89,
                                                       3,
                                                       0, 0, 0, 0, 0,
                                                       MAVLink.MAV_CMD.DO_SET_MODE);

        }

        

    }
}