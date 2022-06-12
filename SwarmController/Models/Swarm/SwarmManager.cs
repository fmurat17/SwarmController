using Haberlesme;
using SwarmController.Models.Log;
using SwarmController.Models.Plan;
using SwarmController.ViewModels;
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
        public int availableNumberOfDrones = 0;

        // dict<port, missionID> -1 = available, others = assigned to a mission
        //public Dictionary<int, int> allDrones = new Dictionary<int, int>();
        public List<Drone> allDrones = new List<Drone>();

        LogManager lM = LogManager.getLogManager();
        public DroneNumbersViewModel droneNumbersViewModel = new DroneNumbersViewModel(3, 0);

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

        public MissionBase getMissionByPort(int port)
        {
            PlanController pC = PlanController.getPlanController();
            foreach(MissionBase mb in pC.allMissions)
            {
                foreach(Drone drone in mb.drones)
                {
                    if(drone.port == port)
                    {
                        return mb;
                    }
                }
            }
            return new MissionBase();
        }

        public void createDrones()
        {
            for(int i = 0; i < totalNumberOfDrones; i++)
            {
                int port = 5760 + i * 10 + 2;
                Drone drone = new Drone(port, -1);
                //drone.tcpClient = new TcpClient();
                allDrones.Add(drone);
                //allDrones.Add(port, -1);
            }
            lM.addLog($"{totalNumberOfDrones} drones are created");
        }

        public void InitListenAllDrones()
        {
            Thread thread = new Thread(() => ListenDrone());
            thread.Start();
        }

        public void ListenDrone()
        {
            foreach(Drone drone in allDrones)
            {
                // GLOBAL_POSITION_INT - lat lng alt
                SendPacket.send_mavlink_command_long_t_tcp(drone.tcpClient,
                                                           33,
                                                           20000,
                                                           0, 0, 0, 0, 0,
                                                           MAVLink.MAV_CMD.SET_MESSAGE_INTERVAL
                                                           );

                // ATTITUDE - roll yaw pitch
                SendPacket.send_mavlink_command_long_t_tcp(drone.tcpClient,
                                                           30,
                                                           20000,
                                                           0, 0, 0, 0, 0,
                                                           MAVLink.MAV_CMD.SET_MESSAGE_INTERVAL
                                                           );

                // HEARTBEAT
                //SendPacket.send_mavlink_command_long_t_tcp(tcpClient,
                //                                           0,
                //                                           50000,
                //                                           0, 0, 0, 0, 0,
                //                                           MAVLink.MAV_CMD.SET_MESSAGE_INTERVAL
                //                                           );
            }

            lM.addLog("Drones are listening");
            while (true)
            {
                foreach (Drone drone in allDrones)
                {
                    if (drone.tcpClient.Connected)
                    {
                        var packet = new MAVLink.MAVLinkMessage();
                        try
                        {
                            packet = ReceivePacket.ReceieveTCPPackets(drone.tcpClient);
                        }
                        catch
                        {

                        }

                        try
                        {
                            if (packet != null)
                            {
                                //Debug.WriteLine($"{drone.port} -> {packet}");

                                switch (packet.msgtypename)
                                {
                                    case "HEARTBEAT":
                                        //hbCounter++;
                                        break;
                                    case "ATTITUDE":
                                        MAVLink.mavlink_attitude_t attitude = (MAVLink.mavlink_attitude_t)packet.data;
                                        drone.roll  = Math.Round(attitude.roll  * 180 / Math.PI, 3);
                                        drone.yaw   = Math.Round(attitude.yaw   * 180 / Math.PI, 3);
                                        drone.pitch = Math.Round(attitude.pitch * 180 / Math.PI, 3);
                                        break;
                                    case "GLOBAL_POSITION_INT":
                                        MAVLink.mavlink_global_position_int_t pos = (MAVLink.mavlink_global_position_int_t)packet.data;
                                        drone.lat = Math.Round(pos.lat / 1e7, 6);
                                        drone.lng = Math.Round(pos.lon / 1e7, 6);
                                        drone.alt = pos.relative_alt / 1000.0;
                                        //Debug.WriteLine($"[{drone.port}] {pos.lat / 1e7} - {pos.lon / 1e7}");
                                        break;
                                }
                            }
                        }
                        catch
                        {
                            Debug.WriteLine("ListenDrone -> error");
                        }
                    }
                    else
                    {
                        if (!drone.isClosedForever)
                        {
                            Debug.WriteLine($"Connection lost with {drone.port}");
                            lM.addLog($"Connection lost with {drone.port}");
                            drone.isClosedForever = true;
                            drone.availability = false;
                        }
                    }
                }
            }
        }

        public void InitStartMission(string missionName)
        {
            PlanController pc = PlanController.getPlanController();
            currentMission = pc.getMissionByName(missionName);

            Thread start_mission_thread = new Thread(() => StartMission());
            start_mission_thread.Start();
        }
        public void StartMission()
        {
            assignDrones();

            bool readyFlag = true;
            // Check if all drones are connected and ready
            foreach (Drone drone in currentMission.drones)
            {
                if (!drone.availability)
                {
                    Debug.WriteLine($"Drone {drone.droneID} is not ready!");
                    readyFlag = false;
                }
            }

            if (!readyFlag) return;

            UploadMission();
            InitFlyDrones();
        }

        public void assignDrones()
        {
            int numberOfDronesInMission = currentMission.numberOfDronesInMission;
            int missionID = currentMission.missionID;

            for (int i = 0, p = 0; i < numberOfDronesInMission ; )
            {
                int port = 5760 + (p * 10) + 2;
                if (allDrones[p].missionID == -1)
                {
                    allDrones[p].missionID = missionID;
                    allDrones[p].droneIdInMission = currentMission.drones.Count;
                    currentMission.drones.Add(allDrones[p]);
                    currentMission.assignedDronePorts.Add(port);

                    droneNumbersViewModel.availableNumberOfDrones--;
                    droneNumbersViewModel.inMissionNumberOfDrones++;

                    i++;
                }
                p++;
            }
        }

        public void UploadMission()
        {
            for(int i = 0; i < currentMission.drones.Count; i++) // currentMission
            {
                UploadMissionToOneDrone(currentMission.drones[i]);
            }
            Debug.WriteLine("All missions are uploaded to drones!");
        }
        public void UploadMissionToOneDrone(Drone drone)
        {
            PlanController pC = PlanController.getPlanController();

            int port = drone.port;
            TcpClient tcpClient = drone.tcpClient;

            // first clean the mission in drone
            SendPacket.send_mission_clear_all_tcp(tcpClient,
                                                  MAVLink.MAV_MISSION_TYPE.MISSION);

            //wait a bit
            Thread.Sleep(200);

            Route route = pC.allMissions[drone.missionID].routes[drone.droneIdInMission];
            int numberOfMissionItems = route.gMapRoute.Points.Count;
            Debug.WriteLine($"mission_item: {numberOfMissionItems}, {port}");
            // send number of items in mission
            SendPacket.send_mavlink_mission_count_t_tcp(tcpClient,
                                                        numberOfMissionItems,
                                                        MAVLink.MAV_MISSION_TYPE.MISSION);

            for (int j = 0; j < numberOfMissionItems; j++)
            {
                Debug.WriteLine($"{j}, {port}");
                try
                {
                    //if (ReceivePacket.ReceiveTCPPacket(tcpClient, "MISSION_REQUEST"))
                    {
                        int lat = (int)(route.missionItems[j].koordinat.Lat * 1e7);
                        int lng = (int)(route.missionItems[j].koordinat.Lng * 1e7);
                        float alt = (float)route.missionItems[j].altitude;

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
            lM.addLog($"Mission is uploaded for {drone.port}");
            //currentMission.tcpClients[i].Close();
            //Thread.Sleep(1000);
        }

        public void InitFlyDrones()
        {
            Thread fly_drones_thread = new Thread(() => flyDrones());
            fly_drones_thread.Start();
        }
        public void flyDrones()
        {
            lM.addLog($"Initializing drones to fly");
            for (int i = 0; i < currentMission.drones.Count; i++)
            {
                flyDrone(currentMission.drones[i]);
            }
        }
        public void flyDrone(Drone drone)
        {
            TcpClient tcpClient = drone.tcpClient;

            // guided'a al
            SendPacket.send_mavlink_command_long_t_tcp(tcpClient,
                                                       89,
                                                       4,
                                                       0, 0, 0, 0, 0,
                                                       MAVLink.MAV_CMD.DO_SET_MODE);

            Thread.Sleep(500);

            // arm
            SendPacket.send_mavlink_command_long_t_tcp(tcpClient,
                                                       1,
                                                       1,
                                                       0, 0, 0, 0, 0,
                                                       MAVLink.MAV_CMD.COMPONENT_ARM_DISARM);

            Thread.Sleep(500);

            // takeoff
            SendPacket.send_mavlink_command_long_t_tcp(tcpClient,
                                                       0, 1, 0, 0, 0, 0,
                                                       10,
                                                       MAVLink.MAV_CMD.TAKEOFF);
            lM.addLog($"{drone.port} -> Takeoff");
            Thread.Sleep(3000); // 5 sec

            // auto'ya al
            SendPacket.send_mavlink_command_long_t_tcp(tcpClient,
                                                       89,
                                                       3,
                                                       0, 0, 0, 0, 0,
                                                       MAVLink.MAV_CMD.DO_SET_MODE);
            lM.addLog($"{drone.port} -> AUTO");
        }

        public Drone ReAssignDrone(Drone closedDrone)
        {
            //SwarmManager sM = SwarmManager.getSwarmManager();
            PlanController pC = PlanController.getPlanController();

            Drone newDrone = null;

            // find available drone
            foreach (Drone drone in allDrones)
            {
                if (drone.availability && drone.missionID == -1)
                {
                    newDrone = drone;
                    break;
                }
            }

            if (newDrone == null) return null;

            MissionBase mission = pC.allMissions[closedDrone.missionID];

            newDrone.missionID = closedDrone.missionID;
            newDrone.droneIdInMission = mission.drones.Count;
            mission.routes.Add(mission.routes[closedDrone.droneIdInMission]);
            mission.drones.Add(newDrone);
            mission.assignedDronePorts.Add(newDrone.port);

            droneNumbersViewModel.availableNumberOfDrones--;
            droneNumbersViewModel.inMissionNumberOfDrones++;

            return newDrone;
            //mission.numberOfDronesInMission++; // bi şeye etki etmeyebilir şimdilik
        }
        public void startMissionForOneDrone(Drone drone)
        {
            Drone newDrone = ReAssignDrone(drone); // closed drone as parameter
            if(newDrone == null)
            {
                Debug.WriteLine("There is not enough available drone to re-assign mission!");
                lM.addLog("Not enough drone to reassign mission");
                return;
            }
            lM.addLog($"{drone.port} is assigned for {newDrone.port}");
            UploadMissionToOneDrone(newDrone); // new drone as parameter
            flyDrone(newDrone); // new drone as parameter
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

//while (drone.availability)
//{
//    var packet = ReceivePacket.ReceieveTCPPackets(tcpClient);
//    //if (timeCounter == 24 && hbCounter == 0)
//    //{
//    //    Debug.WriteLine($"port {drone.port} is closed!");
//    //    return;
//    //}
//    //else
//    //{
//    //    timeCounter = 0;
//    //}

//    //if (timeCounter == 4)
//    //{
//    //    hbCounter = 0;
//    //}

//    try
//    {